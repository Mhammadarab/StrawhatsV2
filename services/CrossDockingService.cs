using Cargohub.interfaces;
using Cargohub.models;
using Newtonsoft.Json;
using StrawhatsV2.models;

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

    private void LogCrossDockingOperation(string operation, string performedBy, Dictionary<string, object> details)
    {
        var logEntry = new CrossDockingLogEntry
        {
            Timestamp = DateTime.UtcNow,
            PerformedBy = performedBy,
            Operation = operation,
            Details = details
        };

        var logFilePath = Path.Combine("logs", "cross_docking_logs.log");
        Directory.CreateDirectory("logs");

        var logLine = $"Timestamp={logEntry.Timestamp:O} | PerformedBy={logEntry.PerformedBy} | Operation={logEntry.Operation} | Details={JsonConvert.SerializeObject(logEntry.Details)}";

        File.AppendAllText(logFilePath, logLine + Environment.NewLine);
    }

    public string ReceiveShipment(int shipmentId, string apiKey)
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

        foreach (var item in shipment.Items)
        {
            item.CrossDockingStatus = "Transit";
        }

        shipment.Shipment_Status = "Transit";
        _shipmentService.Update(shipment);

        var details = new Dictionary<string, object>
    {
        { "ShipmentId", shipmentId },
        { "Status", shipment.Shipment_Status }
    };

        LogCrossDockingOperation("ReceiveShipment", apiKey, details);

        return $"Shipment with ID {shipmentId} has been received and marked as 'Transit'.";
    }

    public string ShipItems(int shipmentId, string apiKey)
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

        var orders = _orderService.GetAll();
        var matchingOrder = orders.FirstOrDefault(o => o.Shipment_Id.Contains(shipmentId));
        if (matchingOrder == null)
        {
            throw new KeyNotFoundException($"No order found linked to shipment ID {shipmentId}.");
        }

        foreach (var shipmentItem in shipment.Items)
        {
            var orderItem = matchingOrder.Items.FirstOrDefault(o => o.Item_Id == shipmentItem.Item_Id);
            if (orderItem != null)
            {
                int fulfilledAmount = Math.Min(shipmentItem.Amount, orderItem.Amount);
                orderItem.Amount -= fulfilledAmount;
                shipmentItem.Amount -= fulfilledAmount;
            }
        }

        shipment.Shipment_Status = "Delivered";
        _shipmentService.Update(shipment);
        _orderService.Update(matchingOrder);

        var details = new Dictionary<string, object>
    {
        { "ShipmentId", shipmentId },
        { "OrderStatus", matchingOrder.Order_Status }
    };

        LogCrossDockingOperation("ShipItems", apiKey, details);

        return $"Shipment with ID {shipmentId} has been shipped and marked as 'Delivered'.";
    }

    public List<object> MatchItems(int? shipmentId = null, int? pageNumber = null, int? pageSize = null)
    {
        var shipments = _shipmentService.GetAll();
        var orders = _orderService.GetAll();

        var matches = new List<object>();
        var pendingItems = new List<object>();

        foreach (var shipment in shipments.Where(s => shipmentId == null || s.Id == shipmentId))
        {
            var matchingOrder = orders.FirstOrDefault(o => o.Shipment_Id.Contains(shipment.Id));
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
}