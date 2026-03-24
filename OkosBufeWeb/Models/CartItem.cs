namespace OkosBufeWeb.Models;

public class CartItem
{
    public int ProductId {get; set;}
    public string ProductName {get; set;} = string.Empty;

    public int Price {get; set;}

    public int Quantity {get; set;}

    public int TotalPrice => Quantity * Price;
}