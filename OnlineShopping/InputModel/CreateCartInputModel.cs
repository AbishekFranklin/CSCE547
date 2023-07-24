using System.ComponentModel.DataAnnotations;

public class CreateCartInputModel
{
    [Required(ErrorMessage = "Cart name is required.")]
    public required string CartName { get; set; }
}
