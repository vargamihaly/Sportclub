using System.Text;
using Rendelesek.Dal;
using Rendelesek.Models;

namespace Rendelesek.Services
{
    internal interface IProductService
    {
         bool IsThereEnoughProductOnStock(string? productId, int quantity);
         bool ReduceStock(string? productId, int quantity);
         IEnumerable<string> CreateSupplyReport();
    }

    internal class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public ProductService(IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public bool ReduceStock(string? productId, int quantity)
        {
            if (!IsThereEnoughProductOnStock(productId, quantity)) return false;
            
            var product = _productRepository.Get(productId);

            product.Quantity = quantity;

            return true;

        }

        public bool IsThereEnoughProductOnStock(string? productId, int quantity)
        {
            var product = _productRepository.Get(productId);

            return product.Quantity >= quantity;
        }

        public IEnumerable<string> CreateSupplyReport()
        {
            var supplyReportBuilder = new StringBuilder();

            var productsToOrder = GetProductsToOrder().OrderBy(x => x.Key);

            foreach (var productToOrder in productsToOrder)
            {
                supplyReportBuilder.AppendLine($"{productToOrder.Key};{productToOrder.Value}");
            }

            return supplyReportBuilder.ToString().Split(' ').ToArray();
        }

        private Dictionary<string, int> GetProductsToOrder()
        {
            var productsToOrder = new Dictionary<string, int>();

            IEnumerable<IGrouping<string?, KeyValuePair<string, int>>> unFullFilledItems = _orderRepository.List().Where(x => x.CurrentState == OrderState.Pending)
                .SelectMany(x => x.RequestedItems).GroupBy(x => x.Key);

            foreach (var product in unFullFilledItems)
            {
                var sumOfProduct = product.Sum(x => x.Value);

                if (IsThereEnoughProductOnStock(product.Key, sumOfProduct)) continue;

                var inStock = _productRepository.Get(product.Key).Quantity;

                if (product.Key != null) productsToOrder.Add(product.Key, sumOfProduct - inStock);
            }

            return productsToOrder;
        }
    }
}
