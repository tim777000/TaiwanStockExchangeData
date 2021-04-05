using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaiwanStockExchangeData.Data;
using TaiwanStockExchangeData.Models;
using System.ComponentModel.DataAnnotations;

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
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        public SelectList Genres { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SecurityGenre { get; set; }

        [BindProperty(SupportsGet = true)]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        [BindProperty(SupportsGet = true)]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public async Task OnGetAsync()
        {
            var securities = from s in _context.Security select s;
            if(!string.IsNullOrEmpty(SearchString))
            {
                securities = securities.Where(s => s.CodeName.Contains(SearchString));
            }
            if(StartDate.HasValue && EndDate.HasValue)
            {
                securities = securities.Where(s => s.Date >= StartDate && s.Date <= EndDate);
            }
            if(StartDate.HasValue && !EndDate.HasValue)
            {
                securities = securities.Where(s => s.Date == StartDate);
            }
            Security = await securities.ToListAsync();
        }
    }
}
