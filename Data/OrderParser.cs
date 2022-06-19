using Rendelesek.Dal;
using Rendelesek.Models;
using Serilog;

namespace Rendelesek.Data
{
    internal class OrderParser
    {
        private readonly IOrderRepository _orderRepository;

        public OrderParser(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public void ParseOrdersFromFile(string source)
        {
            Log.Information($"Parsing orders from: {source}");

            var lines = File.ReadAllLines(source);
            var orders = new List<Order>();

            foreach (var line in lines)
            {
                string?[] properties = line.Split(';');
                switch (properties[0])
                {
                    case "M":
                    {
                        ProcessBasicOrderValues();
                        break;
                    }

                    case "T":
                    {
                        ProcessOrderedProducts();

                        break;
                    }
                    default:
                    {
                        ThrowArgumentException();
                        break;
                    }
                }

                void ProcessOrderedProducts()
                {
                    var id = Convert.ToInt16(properties[1]);
                    if (orders.Any(x => x.Id == id))
                    {
                        AddRequestedItemsToOrder(id);
                    }

                    void AddRequestedItemsToOrder(short orderId)
                    {
                        var order = orders.First(x => x.Id == orderId);
                        var requestedItems = order.RequestedItems;

                        if (requestedItems.TryGetValue(
                                properties[2] ??
                                throw new InvalidOperationException("The given file's has wrong format."),
                                out var numberOfItemsToBuy))
                        {
                            numberOfItemsToBuy += Convert.ToInt16(properties[3]);
                        }
                        else
                        {
                            requestedItems.Add(
                                properties[2] ??
                                throw new InvalidOperationException("The given file's has wrong format."),
                                Convert.ToInt16(properties[3]));
                        }
                    }
                }

                void ProcessBasicOrderValues()
                {
                    var id = Convert.ToInt16(properties[2]);

                    var newOrder = new Order()
                    {
                        Id = id,
                        OrderDate = Convert.ToDateTime(properties[1]),
                        Orderer = properties[3]
                    };

                    orders.Add(newOrder);
                }

                void ThrowArgumentException()
                {
                    throw new ArgumentException("The given file's has wrong format.");
                }
            }

            Log.Information("Orders added:");

            foreach (var order in orders)
            {
                _orderRepository.Add(order);
                
                Log.Information($"{order}");
            }
        }
    }
}