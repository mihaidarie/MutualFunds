using System.Text;
using System.Linq;
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

        public IngPriceRetriever()
        {
            urls.Add("NN (L) Euro Credit", "https://api.nnip.com/FundsApi/v2/funds/LU0546918409");
            urls.Add("NN (L) Euro Fixed Income", "https://api.nnip.com/FundsApi/v2/funds/LU0546918151");
            urls.Add("NN (L) Euro High Dividend", "https://api.nnip.com/FundsApi/v2/funds/LU0127786860");
            urls.Add("NN (L) European Sustainable Equity", "https://api.nnip.com/FundsApi/v2/funds/LU1542714578");
            urls.Add("NN (L) Global Equity Impact Opportunities", "https://api.nnip.com/FundsApi/v2/funds/LU0250170304");
            urls.Add("NN (L) Global Sustainable Equity", "https://api.nnip.com/FundsApi/v2/funds/LU0121204431");
            urls.Add("NN (L) Global High Yield", "https://api.nnip.com/FundsApi/v2/funds/LU0548664886");
            urls.Add("NN (L) Global High Dividend", "https://api.nnip.com/FundsApi/v2/funds/LU0146259923");
            urls.Add("NN (L) Health & Well-being", "https://api.nnip.com/FundsApi/v2/funds/LU0121202492");
            urls.Add("NN (L) Patrimonial Balanced", "https://api.nnip.com/FundsApi/v2/funds/LU0121216955");
            urls.Add("NN (L) European Real Estate", "https://api.nnip.com/FundsApi/v2/funds/LU0121177280");
            urls.Add("NN (L) International ING Conservator", "https://api.nnip.com/FundsApi/v2/funds/LU1505915899");
            urls.Add("NN (L) International ING Moderat", "https://api.nnip.com/FundsApi/v2/funds/LU1505916194");
            urls.Add("NN (L) International ING Dinamic", "https://api.nnip.com/FundsApi/v2/funds/LU1505916350");
            urls.Add("NN (L) Emerging Europe Equity", "https://api.nnip.com/FundsApi/v2/funds/LU0113311731");
            urls.Add("NN (L) Emerging Markets Debt (Hard Currency)", "https://api.nnip.com/FundsApi/v2/funds/LU0546915215");
            urls.Add("NN (L) Emerging Markets High Dividend", "https://api.nnip.com/FundsApi/v2/funds/LU0300634226");
            urls.Add("NN (L) First Class Multi Asset", "https://api.nnip.com/FundsApi/v2/funds/LU0809674384");
        }

        public IDictionary<string, IEnumerable<Day>> ExtractPrices()
        {
            var client = new Client();
            var result = new Dictionary<string, IEnumerable<Day>>();

            foreach (var url in urls)
            {
                var priceHistory = client.GetPriceHistory<PriceHistory>(url.Value);
                result.Add(priceHistory.Result.SubFundName, priceHistory.Result.NavList);

                // using (var sw = File.CreateText($"D:\\MihaiRepo\\MutualFunds\\MutualFunds\\{priceHistory.Result.SubFundName}_price_history.csv"))
                // {
                //     using (var csv = new CsvWriter(sw, CultureInfo.InvariantCulture))
                //     {
                //         csv.WriteRecords(priceHistory.Result.NavList);
                //     }
                // }
            }

            return result;
        }

        public void ComputeAnualEvolution(IDictionary<string, IEnumerable<Day>> priceHistory)
        {
            using (var sw = File.CreateText($"D:\\MihaiRepo\\MutualFunds\\MutualFunds\\price_evolution_by_year.csv"))
            {
                using (var csv = new CsvWriter(sw, CultureInfo.InvariantCulture))
                {
                    foreach (var fund in priceHistory)
                    {
                        var yearsReported = fund.Value
                        .Select(d => new DateTime(d.NavDate.Year, 1, 1))
                        .Distinct()
                        .ToList();

                        var evolution = new List<Evolution>();

                        foreach (var reportedYear in yearsReported)
                        {
                            var firstYearDayValue = fund.Value
                                .Where(w => w.NavDate.Year == reportedYear.Year)
                                .OrderBy(d => d.NavDate).First().NavVal;
                            var lastYearDayValue = fund.Value
                                .Where(w => w.NavDate.Year == reportedYear.Year)
                                .OrderBy(d => d.NavDate).Last().NavVal;
                            var difference = firstYearDayValue - lastYearDayValue;
                            var evolutionPercentage = difference * 100 / firstYearDayValue;
                            var isNegative = difference < 0;
                            var percentageToShow = isNegative ? -evolutionPercentage : evolutionPercentage;

                            evolution.Add(new Evolution
                            {
                                FundName = fund.Key,
                                Year = reportedYear.Year,
                                FirstYearDayValue = firstYearDayValue,
                                LastYearDayValue = lastYearDayValue,
                                Percentage = percentageToShow
                            });
                        }

                        csv.WriteRecords(evolution);
                    }
                }
            }
        }

        public void ComputeAnualEvolutionSideBySide(IDictionary<string, IEnumerable<Day>> priceHistory)
        {
            var yearsReported = priceHistory.Values
                .SelectMany(day => day.Select(d => new DateTime(d.NavDate.Year, 1, 1)).Distinct())
                .Distinct();

            using (var sw = 
            File.CreateText($"D:\\MihaiRepo\\MutualFunds\\MutualFunds\\price_evolution_by_year_side_by_side.csv"))
            {
                foreach (var fund in priceHistory)
                {
                    var fundReport = new StringBuilder($"{fund.Key}");

                    foreach (var reportedYear in yearsReported)
                    {
                        if(fund.Value
                            .Any(w => w.NavDate.Year == reportedYear.Year))
                        {
                            var firstYearDayValue = fund.Value
                                .Where(w => w.NavDate.Year == reportedYear.Year)
                                .OrderBy(d => d.NavDate).First().NavVal;
                            var lastYearDayValue = fund.Value
                                .Where(w => w.NavDate.Year == reportedYear.Year)
                                .OrderBy(d => d.NavDate).Last().NavVal;
                            var difference = lastYearDayValue - firstYearDayValue;
                            var evolutionPercentage = difference * 100 / firstYearDayValue;
                            var isNegative = difference < 0;
                            var percentageToShow = Math.Round(evolutionPercentage, 2);

                            fundReport.AppendJoin(',', $",{reportedYear.ToShortDateString()}", firstYearDayValue, lastYearDayValue, percentageToShow);
                        }
                    }

                    sw.WriteLine(fundReport);
                }
            }
        }
    }
}