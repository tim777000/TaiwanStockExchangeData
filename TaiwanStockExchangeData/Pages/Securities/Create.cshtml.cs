using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaiwanStockExchangeData.Data;
using TaiwanStockExchangeData.Models;

namespace TaiwanStockExchangeData.Pages.Securities
{
    public class CreateModel : PageModel
    {
        private readonly TaiwanStockExchangeData.Data.TaiwanStockExchangeDataContext _context;

        public CreateModel(TaiwanStockExchangeData.Data.TaiwanStockExchangeDataContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Security Security { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(List<IFormFile> files)
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var path = $@"CSV/{file.FileName}";
                    var result = string.Empty;
                    using (var reader = new StreamReader(file.OpenReadStream(), Encoding.GetEncoding("Big5"), true))
                    {
                        while ((result = reader.ReadLine()) != null)
                        {
                            System.Console.WriteLine(result);
                        }
                    }
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }

            if (!String.IsNullOrEmpty(Security.CodeName))
            {
                _context.Security.Add(Security);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
