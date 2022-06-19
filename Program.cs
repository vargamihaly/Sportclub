using Rendelesek;
using Rendelesek.Data;
using Rendelesek.Dal;
using Rendelesek.Services;
using Serilog;

const string productDataSource = @"C:\Users\Mihaly\source\repos\Rendelesek\Rendelesek\Data\raktar.csv";
const string orderDataSource = @"C:\Users\Mihaly\source\repos\Rendelesek\Rendelesek\Data\rendeles.csv";

const string levelekDestinationFolder = @"C:\Users\Mihaly\source\repos\Rendelesek\Rendelesek\Data\levelek.csv";
const string beszerzesDestinationFolder = @"C:\Users\Mihaly\source\repos\Rendelesek\Rendelesek\Data\beszerzes.csv";


IProductRepository productRepository;
IOrderRepository orderRepository;

IProductService productService;
IOrderService orderService;

ProductParser productParser;
OrderParser orderParser;

Initialize();
Seed();

orderService.ProcessOrders();

CreateReports();

Console.ReadKey();

void Initialize()
{
    Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();
    
    productRepository = new ProductRepository();
    orderRepository = new OrderRepository();

    productService = new ProductService(productRepository, orderRepository);
    orderService = new OrderService(productRepository, orderRepository, productService);

    productParser = new ProductParser(productRepository);
    orderParser = new OrderParser(orderRepository);
}

void Seed()
{
    productParser.ParseProductsFromFile(productDataSource);
    orderParser.ParseOrdersFromFile(orderDataSource);
}

void CreateReports()
{
    orderService.CreateSummarizedReportFromOrders().WriteToCsv(levelekDestinationFolder);
    productService.CreateSupplyReport().WriteToCsv(beszerzesDestinationFolder);
}

