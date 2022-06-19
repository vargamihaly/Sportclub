using System.Text;

namespace Rendelesek.Models
{
    internal record Product
    {
        public string? Id { get; init; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Id: {Id}");
            sb.AppendLine($"Name: {Name}");
            sb.AppendLine($"Price: {Price}");
            sb.AppendLine($"Quantity: {Quantity}");
            sb.AppendLine($"-------------------------");

            return sb.ToString();
        }
    }
}
