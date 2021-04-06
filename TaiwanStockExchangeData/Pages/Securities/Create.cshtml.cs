using System;
using System.Collections.Generic;
using System.Globalization;
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
                    var fileName = file.FileName.Split('_');
                    fileName = fileName[3].Split('.');
                    var result = string.Empty;
                    int counter = 0;
                    using (var reader = new StreamReader(file.OpenReadStream(), Encoding.GetEncoding("Big5"), true))
                    {
                        while ((result = reader.ReadLine()) != null)
                        {
                            var targetData = result.Split(',');
                            if (counter >= 2 && targetData.Length == 8)
                            {
                                var securities = from s in _context.Security select s;
                                securities = securities.Where(s => s.CodeName.Contains(targetData[0].Replace("\"", "")));
                                securities = securities.Where(s => s.Date == DateTime.ParseExact(fileName[0], "yyyyMMdd", null));
                                if (!securities.Any())
                                {
                                    if(targetData[2].Replace("\"", "") == "-")
                                    {
                                        targetData[2] = "0";
                                    }
                                    if (targetData[3].Replace("\"", "") == "-")
                                    {
                                        targetData[3] = "0";
                                    }
                                    if (targetData[4].Replace("\"", "") == "-")
                                    {
                                        targetData[4] = "0";
                                    }
                                    if (targetData[5].Replace("\"", "") == "-")
                                    {
                                        targetData[5] = "0";
                                    }
                                    _context.Security.AddRange(
                                        new Security
                                        {
                                            CodeName = targetData[0].Replace("\"", ""),
                                            Name = targetData[1].Replace("\"", ""),
                                            DividendYield = Convert.ToDouble(targetData[2].Replace("\"", "")),
                                            DividendYear = Convert.ToUInt32(targetData[3].Replace("\"", "")),
                                            PriceToEarningRatio = Convert.ToDouble(targetData[4].Replace("\"", "")),
                                            PriceToBookRatio = Convert.ToDouble(targetData[5].Replace("\"", "")),
                                            FinancialStatements = targetData[6].Replace("\"", ""),
                                            Date = DateTime.ParseExact(fileName[0], "yyyyMMdd", null)
                                        }
                                    );
                                    _context.SaveChanges();
                                }
                            }
                            counter++;
                            /*if(counter == 15)
                            {
                                break;
                            }*/
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
