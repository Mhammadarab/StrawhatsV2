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

    // Explicitly update each item's CrossDockingStatus
    foreach (var item in shipment.Items)
    {
      item.CrossDockingStatus = "In Transit";
    }

    // Update the shipment status
    shipment.Shipment_Status = "In Transit";

    // Persist the changes
    _shipmentService.Update(shipment);

    return "Shipment received and items marked as 'In Transit'.";
  }

  public List<object> MatchItems(int? shipmentId, int pageNumber, int pageSize)
  {
    var shipments = _shipmentService.GetAll();

    // Filter by shipmentId if provided
    if (shipmentId.HasValue)
    {
      shipments = shipments.Where(s => s.Id == shipmentId.Value).ToList();
    }

    // Apply pagination
    shipments = shipments.Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

    var orders = _orderService.GetAll();
    var matches = new List<object>();

    foreach (var shipment in shipments)
    {
      foreach (var item in shipment.Items)
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

    foreach (var item in shipment.Items)
    {
      item.CrossDockingStatus = "Shipped";
    }

    if (shipment.Items.All(i => i.CrossDockingStatus == "Shipped"))
    {
      shipment.Shipment_Status = "Completed";
    }

    _shipmentService.Update(shipment);

    foreach (var item in shipment.Items)
    {
      Console.WriteLine($"Updating Item {item.Item_Id}: Setting CrossDockingStatus to 'In Transit'");
      item.CrossDockingStatus = "In Transit";
    }

    Console.WriteLine($"Updating Shipment {shipment.Id}: Setting Shipment_Status to 'In Transit'");


    return "Items marked as 'Shipped'.";
  }
}
