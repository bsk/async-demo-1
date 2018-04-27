using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using ProductsShared;

namespace ProductsSync
{
    internal class Program
    {
        private static void Main()
        {
            var watch = Stopwatch.StartNew();

            var queries = new[]
            {
                "manchester united t shirt",
                "manchester city t shirt",
                "miami dolphins jersey",
                "chicago bulls jersey",
                "argentina shirt",
                "bolton shirt",
                "liverpool shirt",
                "yankees jersey",
                "barcelona t shirt",
                "real madrid t shirt"
            };

            var client = new WebClient();

            queries.SelectMany(query => RunQuery(query, client)).ToArray();

            watch.Stop();

            Console.WriteLine($"Duration: {watch.Elapsed.TotalSeconds} seconds");
        }

        private static IEnumerable<string> RunQuery(string query, WebClient client)
        {
            var url = @"https://www.google.co.uk/search?tbm=shop&q=" + query;
            var content = client.DownloadString(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            var linkedProducts = doc.DocumentNode.Descendants("a")
                .Select(a => a.GetAttributeValue("href", null))
                .Where(href => href.StartsWith(@"/shopping/product/"))
                .Select(href => href.Substring(0, 50))
                .Distinct();

            var products = linkedProducts.Select(linkedProduct =>
            {
                var productUrl = @"https://www.google.co.uk" + linkedProduct;
                var productPage = client.DownloadString(productUrl);

                var productDoc = new HtmlDocument();
                productDoc.LoadHtml(productPage);

                var productName = productDoc.DocumentNode.Descendants("title").FirstOrDefault()?.InnerText;

                var sellersNodes = productDoc.DocumentNode.Descendants("tr")
                    .Where(tr => tr.GetAttributeValue("class", null) == "os-row");

                var sellers = sellersNodes
                    .Select(seller => new Seller(seller.Descendants("a").First().InnerText, decimal.Parse(seller.Descendants("td").Last().InnerText.Substring(2))))
                    .ToArray();

                return sellers.Any() ? new Product(sellers, productName) : null;
            });

            return products.Select(product =>
            {
                if (product == null) return "";

                var result =
                    $"{product.Name}{Environment.NewLine}Best price: {product.BestPrice()}, from {product.BestPriceSellers()}"
                    + $", Average price: {product.AveragePrice()}";

                result = product.FanaticsSellers().Aggregate(result,
                    (current, fanaticsSeller) => current + $", {fanaticsSeller.Name}: {fanaticsSeller.Price}");

                var queryResult = result + Environment.NewLine;
                Console.WriteLine(queryResult);
                return queryResult;
            });
        }
    }
}