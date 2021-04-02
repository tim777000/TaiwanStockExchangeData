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
    public class DeleteModel : PageModel
    {
        private readonly TaiwanStockExchangeData.Data.TaiwanStockExchangeDataContext _context;

        public DeleteModel(TaiwanStockExchangeData.Data.TaiwanStockExchangeDataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Security Security { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Security = await _context.Security.FirstOrDefaultAsync(m => m.ID == id);

            if (Security == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Security = await _context.Security.FindAsync(id);

            if (Security != null)
            {
                _context.Security.Remove(Security);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
