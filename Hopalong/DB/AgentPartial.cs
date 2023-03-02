using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopalong.DB
{
    public partial class Agent
    {
        public int Discount
        {
            get
            {
                var sum = ProductSales.Sum(x => x.ProductCount * x.Product.MinCostForAgent);

                return sum < 10_000 ? 0 : (sum < 50_000 ? 5 : (sum < 150_000 ? 10 : (sum < 500_000 ? 20 : 25)));
            }
        }

        public int YearSalesCount => ProductSales.Count(x => x.SaleDate > DateTime.Now.AddYears(-1));

        public string Color => Discount >= 10 ? "#90EA83" : "Transparent";
    }
}
