using System;

namespace MutualFunds
{
    class Program
    {
        static void Main(string[] args)
        {
            var ingPriceRetriever = new IngPriceRetriever();

            ingPriceRetriever.ExtractPrices();

            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }
    }
}
