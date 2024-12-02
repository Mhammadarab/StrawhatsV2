namespace Cargohub.models
{
  public class ItemDetail
  {
    public string Item_Id { get; set; }
    public int Amount { get; set; }
    public string? CrossDockingStatus { get; set; } // New: Tracks "In Transit", "Matched", "Shipped"
  }
}
