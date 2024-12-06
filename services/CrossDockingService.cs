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
    var orders = _orderService.GetAll();

    var matches = new List<object>();
    var pendingItems = new List<object>();

    foreach (var shipment in shipments.Where(s => shipmentId == null || s.Id == shipmentId))
    {
        var matchingOrder = orders.FirstOrDefault(o => o.Shipment_Id == shipment.Id);
        if (matchingOrder != null)
        {
            foreach (var shipmentItem in shipment.Items)
            {
                var orderItem = matchingOrder.Items.FirstOrDefault(o => o.Item_Id == shipmentItem.Item_Id);
                if (orderItem != null)
                {
                    int matchedAmount = Math.Min(shipmentItem.Amount, orderItem.Amount);

                    matches.Add(new
                    {
                        ShipmentId = shipment.Id,
                        OrderId = matchingOrder.Id,
                        ItemId = shipmentItem.Item_Id,
                        MatchedAmount = matchedAmount,
                        RemainingOrderAmount = orderItem.Amount - matchedAmount
                    });

                    // Mark item status
                    shipmentItem.CrossDockingStatus = "Matched";
                    orderItem.Amount -= matchedAmount;
                }
                else
                {
                    pendingItems.Add(new
                    {
                        ShipmentId = shipment.Id,
                        ItemId = shipmentItem.Item_Id,
                        Amount = shipmentItem.Amount,
                        Status = "Pending"
                    });
                }
            }
        }
    }

    return matches.Concat(pendingItems).ToList();
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

    var orders = _orderService.GetAll();
    var matchingOrder = orders.FirstOrDefault(o => o.Shipment_Id == shipmentId);
    if (matchingOrder == null)
    {
        throw new KeyNotFoundException($"No order found linked to shipment ID {shipmentId}.");
    }

    foreach (var shipmentItem in shipment.Items)
    {
        // Find matching order item
        var orderItem = matchingOrder.Items.FirstOrDefault(o => o.Item_Id == shipmentItem.Item_Id);

        if (orderItem != null)
        {
            int fulfilledAmount = Math.Min(shipmentItem.Amount, orderItem.Amount);
            orderItem.Amount -= fulfilledAmount;
            shipmentItem.Amount -= fulfilledAmount;

            // Log fulfillment
            shipmentItem.CrossDockingStatus = "Shipped";

            if (orderItem.Amount == 0)
            {
                orderItem.CrossDockingStatus = "Fulfilled";
            }
            else
            {
                orderItem.CrossDockingStatus = "Partially Fulfilled";
            }
        }
    }

    // Check order fulfillment status
    if (matchingOrder.Items.All(item => item.Amount == 0))
    {
        matchingOrder.Order_Status = "Fulfilled";
    }
    else
    {
        matchingOrder.Order_Status = "Partially Fulfilled";
    }

    // Mark shipment as delivered
    shipment.Shipment_Status = "Delivered";
    _shipmentService.Update(shipment);
    _orderService.Update(matchingOrder);

    return $"Shipment with ID {shipmentId} has been shipped and marked as 'Delivered'.";
}

}
