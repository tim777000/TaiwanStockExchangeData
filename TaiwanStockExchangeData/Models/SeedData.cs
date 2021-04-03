using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaiwanStockExchangeData.Data;
using System;
using System.Linq;

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
                        CodeName = 0001,
                        Name = "QwQ",
                        DividendYield = 0.1,
                        DividendYear = 100,
                        PriceToEarningRatio = 0.1,
                        PriceToBookRatio = 0.1,
                        FinancialStatements = "100/25"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
