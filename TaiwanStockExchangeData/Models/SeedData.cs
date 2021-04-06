using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaiwanStockExchangeData.Data;
using System;
using System.Linq;
using System.IO;
using System.Text;

namespace TaiwanStockExchangeData.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new TaiwanStockExchangeDataContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<TaiwanStockExchangeDataContext>>()))
            {
                // Look for any securities.
                if (context.Security.Any())
                {
                    return; // DB has been seeded
                }
                context.Security.AddRange(
                    new Security
                    {
                        CodeName = "Test",
                        Name = "Test",
                        DividendYield = 71.22M,
                        DividendYear = 7122,
                        PriceToEarningRatio =71.22M,
                        PriceToBookRatio = 71.22M,
                        FinancialStatements = "Test"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
