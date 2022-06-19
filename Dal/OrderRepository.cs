using Rendelesek.Models;

namespace Rendelesek.Dal
{
    internal interface IOrderRepository 
    {
        public void Add(Order o);
        IEnumerable<Order> List();
        Order Get(int id);
        void Update(Order o);
        void Delete(Order o);
    }

    internal class OrderRepository : IOrderRepository
    {
        private readonly List<Order> _orders;

        public OrderRepository()
        {
            _orders = new List<Order>();
        }
        
        public void Add(Order o) => _orders.Add(o);

        public void Delete(Order o) => _orders.Remove(o);

        public Order Get(int id) => _orders[id];

        public IEnumerable<Order> List() => _orders;

        public void Update(Order o)
        {
            var order = Get(o.Id);

            order.OrderDate = o.OrderDate;
            order.RequestedItems = o.RequestedItems;
            order.Orderer = o.Orderer;
            order.CurrentState = o.CurrentState;
        }
    }
}
