using System.Collections.Generic;

namespace MutualFunds
{
    public class PriceHistory
    {
        public string SubFundName { get; set; }
        public IEnumerable<Day> NavList { get; set; }
    }
}