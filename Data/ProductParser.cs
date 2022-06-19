using System.Text;
using Rendelesek.Dal;
using Rendelesek.Models;
using Serilog;


namespace Rendelesek.Data
{
    internal class ProductParser
    {
        private readonly IProductRepository _productRepository;
        
        public ProductParser(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void ParseProductsFromFile(string source)
        {
            Log.Information($"Parsing products from: {source.ToString()}");

            var lines = File.ReadAllLines(source, Encoding.Latin1);

            var products = lines.Select(line => line.Split(';')).Select(properties => new Product()
            {
                Id = properties[0],
                Name = properties[1],
                Price = Convert.ToDouble(properties[2]),
                Quantity = Convert.ToUInt16(properties[3])

            }).ToList();

            Log.Information($"Added products:");

            foreach (var product in products)
            {
                _productRepository.Add(product);
                
                Log.Information($"{product}");

            }
        }
    }
}
