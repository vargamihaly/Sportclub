using System.Text;

namespace Rendelesek.Models
{
    public enum OrderState
    {
        Approved,
        Pending,
        Unprocessed,
    }

    internal record Order
    {
        public int Id { get; init; }
        public DateTime OrderDate { get; set; }
        public string? Orderer { get; set; }

        public Dictionary<string, int> RequestedItems = new();

        public OrderState CurrentState { get; set; } = OrderState.Unprocessed;

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Id: {this.Id}");
            sb.AppendLine($"OrderDate: {this.OrderDate.ToShortDateString()}");
            sb.AppendLine($"Orderer: {this.Orderer}");
            sb.AppendLine($"Current state: {this.CurrentState.ToString()}");
            sb.AppendLine($"Requested items:");

            foreach (var keyValuePair in this.RequestedItems)
            {
                sb.AppendLine($"Product id: {keyValuePair.Key}");
                sb.AppendLine($"Ordered quantity: {keyValuePair.Value}");
            }
            
            sb.AppendLine($"-------------------------");

            return sb.ToString();
        }
    }
}
