namespace OkosBufeWeb.Models
{
    public class Order
    {
        public int Id {get; set;}
        public string UserId {get; set;} = string.Empty;
        public string CustomerName {get; set;} = string.Empty;
        public DateTime OrderTime {get; set;} 
        public bool IsCompleted {get; set;}

        public List<OrderItem> OrderItems {get; set;} = new List<OrderItem>();

        public bool isFavorite {get; set;} = false;

    }
}