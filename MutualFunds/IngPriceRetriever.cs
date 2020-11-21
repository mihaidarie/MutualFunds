using System.Collections.Generic;
using System;
using System.IO;
using CsvHelper;
using System.Globalization;

namespace MutualFunds
{
    public class IngPriceRetriever
    {
        private readonly IDictionary<string, string> urls = new Dictionary<string, string>();

        public IngPriceRetriever() {
            urls.Add("NN (L) Euro Credit", "https://api.nnip.com/FundsApi/v2/funds/LU0546918409");
            urls.Add("NN (L) Euro Fixed Income", "https://api.nnip.com/FundsApi/v2/funds/LU0546918151");
            urls.Add("NN (L) Euro High Dividend", "https://api.nnip.com/FundsApi/v2/funds/LU0127786860");
            urls.Add("NN (L) European Sustainable Equity","https://api.nnip.com/FundsApi/v2/funds/LU1542714578");
            urls.Add("NN (L) Global Equity Impact Opportunities","https://api.nnip.com/FundsApi/v2/funds/LU0250170304");
            urls.Add("NN (L) Global Sustainable Equity","https://api.nnip.com/FundsApi/v2/funds/LU0121204431");
            urls.Add("NN (L) Global High Yield","https://api.nnip.com/FundsApi/v2/funds/LU0548664886");
            urls.Add("NN (L) Global High Dividend","https://api.nnip.com/FundsApi/v2/funds/LU0146259923");
            urls.Add("NN (L) Health & Well-being","https://api.nnip.com/FundsApi/v2/funds/LU0121202492");
            urls.Add("NN (L) Patrimonial Balanced","https://api.nnip.com/FundsApi/v2/funds/LU0121216955");
            urls.Add("NN (L) European Real Estate","https://api.nnip.com/FundsApi/v2/funds/LU0121177280");
            urls.Add("NN (L) International ING Conservator","https://api.nnip.com/FundsApi/v2/funds/LU1505915899");
            urls.Add("NN (L) International ING Moderat","https://api.nnip.com/FundsApi/v2/funds/LU1505916194");
            urls.Add("NN (L) International ING Dinamic","https://api.nnip.com/FundsApi/v2/funds/LU1505916350");
            urls.Add("NN (L) Emerging Europe Equity","https://api.nnip.com/FundsApi/v2/funds/LU0113311731");
            urls.Add("NN (L) Emerging Markets Debt (Hard Currency)","https://api.nnip.com/FundsApi/v2/funds/LU0546915215");
            urls.Add("NN (L) Emerging Markets High Dividend","https://api.nnip.com/FundsApi/v2/funds/LU0300634226");
            urls.Add("NN (L) First Class Multi Asset","https://api.nnip.com/FundsApi/v2/funds/LU0809674384");
        }

        public void ExtractPrices()
        {
            var client = new Client();

            foreach (var url in urls)
            {
                var priceHistory = client.GetPriceHistory<PriceHistory>(url.Value);
                
                using(var sw = File.CreateText($"D:\\MihaiRepo\\MutualFunds\\MutualFunds\\{priceHistory.Result.SubFundName}.csv"))
                { 
                    using (var csv = new CsvWriter(sw, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(priceHistory.Result.NavList);
                    }
                }
            }
        }
    }
}