using System.Linq;

namespace ProductsShared
{
    public class Seller
    {
        public static string[] FanaticsStores =
            {"Kitbag", "Man United Direct", "Fanatics", "Man City Store", "NBA Store"};

        public Seller(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; }
        public decimal Price { get; }

        public bool IsVip => FanaticsStores.Contains(Name);
    }
}