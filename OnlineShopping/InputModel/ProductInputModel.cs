using System.ComponentModel.DataAnnotations;

public class ProductInputModel
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public decimal Price { get; set; }

    public string? Image { get; set; }

    public int Quantity { get; set; }
}
