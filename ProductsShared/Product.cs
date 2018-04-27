using System.Collections.Generic;
using System.Linq;

namespace ProductsShared
{
    public class Product
    {
        public Product(IEnumerable<Seller> sellers, string name)
        {
            Sellers = sellers;
            Name = name;
        }

        public IEnumerable<Seller> Sellers { get; }

        public string Name { get; }


        public decimal BestPrice()
        {
            return Sellers.OrderBy(seller => seller.Price).First().Price;
        }

        public string BestPriceSellers()
        {
            return string.Join(", ",
                Sellers.Where(seller => seller.Price == BestPrice()).Select(seller => seller.Name));
        }

        public string AveragePrice()
        {
            return Sellers.Average(seller => seller.Price).ToString("#.##");
        }

        public IEnumerable<Seller> FanaticsSellers()
        {
            return Sellers.Where(seller => seller.IsVip).OrderBy(seller => seller.Price);
        }
    }
}