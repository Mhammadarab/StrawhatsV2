using Cargohub.interfaces;
using Cargohub.models;

public class CrossDockingService
{
    private readonly ICrudService<Shipment, int> _shipmentService;
    private readonly ICrudService<Order, int> _orderService;

    public CrossDockingService(
        ICrudService<Shipment, int> shipmentService,
        ICrudService<Order, int> orderService)
    {
        _shipmentService = shipmentService;
        _orderService = orderService;
    }

    public string ReceiveShipment(int shipmentId)
  {
    var shipment = _shipmentService.GetById(shipmentId);
    if (shipment == null)
    {
        throw new KeyNotFoundException($"Shipment with ID {shipmentId} not found.");
    }

    if (shipment.Shipment_Status == "Delivered")
    {
        throw new InvalidOperationException($"Shipment with ID {shipmentId} has already been delivered and cannot be updated.");
    }

    if (shipment.Shipment_Status == "Transit")
    {
        // Items might still need to be marked as "Transit" if not already set
        foreach (var item in shipment.Items)
        {
            if (item.CrossDockingStatus != "Transit")
            {
                item.CrossDockingStatus = "Transit";
            }
        }
        _shipmentService.Update(shipment);
        return $"Shipment with ID {shipmentId} is already in transit. Items were updated if needed.";
    }

    // Set shipment and item statuses to "Transit" for "Pending" shipments
    foreach (var item in shipment.Items)
    {
        item.CrossDockingStatus = "Transit";
    }
    shipment.Shipment_Status = "Transit";
    _shipmentService.Update(shipment);

    return $"Shipment with ID {shipmentId} has been received and marked as 'Transit'.";
  }


    public List<object> MatchItems(int? shipmentId = null, int? pageNumber = null, int? pageSize = null)
    {
        var shipments = _shipmentService.GetAll();

        // Filter shipments by ID and ensure they are in 'Transit'
        if (shipmentId.HasValue)
        {
            shipments = shipments.Where(s => s.Id == shipmentId.Value && s.Shipment_Status == "Transit").ToList();
        }
        else
        {
            shipments = shipments.Where(s => s.Shipment_Status == "Transit").ToList();
        }

        // Apply pagination to shipments if pageNumber and pageSize are provided
        if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
        {
            shipments = shipments.Skip((pageNumber.Value - 1) * pageSize.Value)
                                .Take(pageSize.Value)
                                .ToList();
        }

        var orders = _orderService.GetAll();
        var matches = new List<object>();

        foreach (var shipment in shipments)
        {
            // Find the order with the same shipment ID
            var matchingOrder = orders.FirstOrDefault(o => o.Shipment_Id == shipment.Id);

            if (matchingOrder != null)
            {
                foreach (var shipmentItem in shipment.Items)
                {
                    // Check if the item exists in the order and match quantities
                    var orderItem = matchingOrder.Items.FirstOrDefault(o => o.Item_Id == shipmentItem.Item_Id);

                    if (orderItem != null)
                    {
                        matches.Add(new
                        {
                            ItemId = shipmentItem.Item_Id,
                            ShipmentId = shipment.Id,
                            OrderId = matchingOrder.Id,
                            Amount = Math.Min(shipmentItem.Amount, orderItem.Amount),
                            Status = "Matched"
                        });

                        shipmentItem.CrossDockingStatus = "Matched";
                    }
                }
            }
        }

        // Apply pagination to matches if pageNumber and pageSize are provided
        if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
        {
            matches = matches.Skip((pageNumber.Value - 1) * pageSize.Value)
                            .Take(pageSize.Value)
                            .ToList();
        }

        return matches;
    }




    public string ShipItems(int shipmentId)
{
    var shipment = _shipmentService.GetById(shipmentId);
    if (shipment == null)
    {
        throw new KeyNotFoundException($"Shipment with ID {shipmentId} not found.");
    }

    if (shipment.Shipment_Status == "Pending")
    {
        throw new InvalidOperationException($"Shipment with ID {shipmentId} must be in transit before it can be shipped.");
    }

    if (shipment.Shipment_Status == "Delivered")
    {
        throw new InvalidOperationException($"Shipment with ID {shipmentId} has already been delivered and cannot be updated.");
    }

    foreach (var item in shipment.Items)
    {
        item.CrossDockingStatus = "Shipped";
    }

    shipment.Shipment_Status = "Delivered";
    _shipmentService.Update(shipment);

    return $"Shipment with ID {shipmentId} has been shipped and marked as 'Delivered'.";
}

}
