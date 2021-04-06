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
                var securities = from s in context.Security select s;
                securities = securities.Where(s => s.CodeName.Contains("Test"));
                securities = securities.Where(s => s.Date == DateTime.ParseExact("20200101", "yyyyMMdd", null));
                if (securities.Any())
                {
                    return; // Already In DB
                }
                context.Security.AddRange(
                    new Security
                    {
                        CodeName = "Test",
                        Name = "Test",
                        DividendYield = Convert.ToDecimal("71.22"),
                        DividendYear = Convert.ToUInt32("7122"),
                        PriceToEarningRatio = Convert.ToDecimal("71.22"),
                        PriceToBookRatio = Convert.ToDecimal("71.22"),
                        FinancialStatements = "Test",
                        Date = DateTime.ParseExact("20200101", "yyyyMMdd", null)
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
