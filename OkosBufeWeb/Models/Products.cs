using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace OkosBufeWeb.Models;

public class Product
{
    [Key]
    public int Id {get; set;}

    [Required(ErrorMessage = "A termék nevének megadása kötelező")]
    [MaxLength(100)]
    public string Name {get;set;} = string.Empty;

    [Required]
    public int Price {get;set;}

    [MaxLength(255)]
    public string? Description {get; set;} 

    public bool IsAvailable {get; set;} = true;
    public string Category {get; set;} = string.Empty;
}