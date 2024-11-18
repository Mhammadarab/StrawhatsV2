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


    public List<object> MatchItems(int? shipmentId, int pageNumber, int pageSize)
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

    // Apply pagination
    shipments = shipments.Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

    var orders = _orderService.GetAll();
    var matches = new List<object>();

    foreach (var shipment in shipments)
    {
        foreach (var item in shipment.Items.Where(i => i.CrossDockingStatus != "Shipped"))
        {
            var orderItem = orders.SelectMany(o => o.Items)
                                  .FirstOrDefault(o => o.Item_Id == item.Item_Id);

            if (orderItem != null)
            {
                matches.Add(new
                {
                    ItemId = item.Item_Id,
                    ShipmentId = shipment.Id,
                    OrderId = orders.First(o => o.Items.Contains(orderItem)).Id,
                    Amount = Math.Min(item.Amount, orderItem.Amount),
                    Status = "Matched"
                });

                item.CrossDockingStatus = "Matched";
            }
        }
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
