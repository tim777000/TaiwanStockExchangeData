using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaiwanStockExchangeData.Data;
using TaiwanStockExchangeData.Models;

namespace TaiwanStockExchangeData.Pages.Securities
{
    public class IndexModel : PageModel
    {
        private readonly TaiwanStockExchangeData.Data.TaiwanStockExchangeDataContext _context;

        public IndexModel(TaiwanStockExchangeData.Data.TaiwanStockExchangeDataContext context)
        {
            _context = context;
        }

        public IList<Security> Security { get;set; }

        public async Task OnGetAsync()
        {
            Security = await _context.Security.ToListAsync();
        }
    }
}
