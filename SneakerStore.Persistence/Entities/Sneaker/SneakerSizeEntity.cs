namespace SneakerStore.Persistence.Entities.Sneaker;

public class SneakerSizeEntity
{
    public Guid Id { get; set; }
    public decimal Size { get; set; }
    public int RemainedInStock { get; set; }
    
    public Guid SneakerId { get; set; }
    public SneakerEntity? Sneaker { get; set; }
}