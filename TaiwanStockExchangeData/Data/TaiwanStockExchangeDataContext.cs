using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaiwanStockExchangeData.Models;

namespace TaiwanStockExchangeData.Data
{
    public class TaiwanStockExchangeDataContext : DbContext
    {
        public TaiwanStockExchangeDataContext (DbContextOptions<TaiwanStockExchangeDataContext> options)
            : base(options)
        {
        }

        public DbSet<TaiwanStockExchangeData.Models.Security> Security { get; set; }
    }
}
