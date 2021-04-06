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

            if(!string.IsNullOrEmpty(SearchString) && !StartDate.HasValue && !EndDate.HasValue)
            {
                securities = securities.Where(s => s.CodeName.Contains(SearchString));
                securities = securities.OrderByDescending(s => s.Date);
            }

            if(string.IsNullOrEmpty(SearchString) && StartDate.HasValue && !EndDate.HasValue)
            {
                securities = securities.Where(s => s.Date == StartDate);
                securities = securities.OrderByDescending(s => s.PriceToEarningRatio);
            }

            if(!string.IsNullOrEmpty(SearchString) && StartDate.HasValue && EndDate.HasValue)
            {
                securities = securities.Where(s => s.CodeName.Contains(SearchString));
                securities = securities.Where(s => s.Date >= StartDate && s.Date <= EndDate);
                securities = securities.OrderBy(s => s.Date);
                var sda = StartDate;
                var sd = StartDate;
                var ed = EndDate;
                double dy = -1.0;
                int counter = 0;
                int max = 0;
                Security pre = null;
                foreach (Security s in securities)
                {
                    if(counter == 0)
                    {
                        pre = s;
                    }
                    if (s.DividendYield > dy)
                    {
                        dy = s.DividendYield;
                        counter++;
                    }
                    else
                    {
                        dy = s.DividendYield;
                        if(counter >= max)
                        {
                            max = counter;
                            sda = sd;
                            ed = pre.Date;
                            sd = s.Date;
                            counter = 1;
                        }
                    }
                    pre = s;
                }
                if (counter >= max)
                {
                    sda = sd;
                    ed = pre.Date;
                }
                securities = securities.Where(s => s.Date >= sda && s.Date <= ed);
                securities = securities.OrderBy(s => s.Date);
            }
            
            Security = await securities.ToListAsync();
        }
    }
}
