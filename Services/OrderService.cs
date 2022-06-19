using System.ComponentModel;
using Rendelesek.Dal;
using Rendelesek.Models;

namespace Rendelesek.Services
{
    internal interface IOrderService
    {
        void ProcessOrders();
        public IEnumerable<string> CreateSummarizedReportFromOrders();
    }

    internal class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        private readonly IProductService _productService;

        public OrderService(IProductRepository productRepository, IOrderRepository orderRepository, IProductService productService)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _productService = productService;
        }

        private IEnumerable<Order> GetApprovedOrders()
        {
            return _orderRepository.List().Where(x => x.CurrentState == OrderState.Approved);
        }

        public IEnumerable<string> CreateSummarizedReportFromOrders()
        {
            var report = new List<string>();
            var orders = _orderRepository.List().OrderBy(x => x.Id);
            var costs = CalculateToTalCostForOrders();

            foreach (var order in orders)
            {
                switch (order.CurrentState)
                {
                    case OrderState.Approved:
                    {
                        var cost = costs.First(x => x.Key == order.Id).Value;
                        report.Add($"{order.Orderer}; A rendelését két napon belül szállítjuk. A rendelés értéke: {cost} Ft.");
                        break;
                    }
                    case OrderState.Pending:
                        report.Add($"{order.Orderer} A rendelése függő állapotba került.");
                        break;

                    case OrderState.Unprocessed:
                        break;
                    
                    default:
                        throw new InvalidEnumArgumentException($"The {order.Id} order has an invalid state.");
                }
            }

            return report.ToArray();
        }
        
        private Dictionary<int, double> CalculateToTalCostForOrders()
        {
            var approvedOrders = GetApprovedOrders();

            var result = new Dictionary<int, double>();

            foreach (var order in approvedOrders)
            {
                var totalCostForOrder = (order.RequestedItems
                    .Select(item => new {item, product = _productRepository.Get(item.Key)})
                    .Select(@t => @t.product.Price * @t.item.Value)).Sum();

                result.Add(order.Id, totalCostForOrder);
            }

            return result;
        }

        public void ProcessOrders()
        {
            var orders = _orderRepository.List();

            foreach (var order in orders)
            {
                if (CanOrderBeFulfilled(order))
                {
                    FulFillOrder(order);
                    order.CurrentState = OrderState.Approved;
                }
                else
                {
                    order.CurrentState = OrderState.Pending;
                }
            }
        }
        
        private bool CanOrderBeFulfilled(Order order)
        {
            var products = _productRepository.List();
            var requestedItems = order.RequestedItems;

            return requestedItems.All(item => _productService.IsThereEnoughProductOnStock(item.Key, item.Value));
        }

        private void FulFillOrder(Order order)
        {
            foreach (var requestedItem in order.RequestedItems)
            {
                _productService.ReduceStock(requestedItem.Key, requestedItem.Value);
            }   
        }
    }
}
