using Rendelesek.Models;

namespace Rendelesek.Dal
{
    internal interface IProductRepository
    {
        public void Add(Product p);
        IEnumerable<Product> List();
        Product Get(string? id);
        void Update(Product p);
        void Delete(Product p);
    }

    internal class ProductRepository : IProductRepository
    {
        private readonly List<Product> _products;

        public ProductRepository()
        {
            _products = new List<Product>();
        }

        public void Add(Product p) => _products.Add(p);

        public void Delete(Product p) => _products.Remove(p);

        public Product Get(string? id) => _products.Single(p => p.Id == id);
        public IEnumerable<Product> List() => _products;

        public void Update(Product p)
        {
            var product = Get(p.Id);

            product.Name = p.Name;
            product.Price = p.Price;
            product.Quantity = p.Quantity;
        }
    }
}
