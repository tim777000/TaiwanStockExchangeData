using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaiwanStockExchangeData.Models
{
    public class Security
    {
        public long ID { get; set; }
        public string CodeName { get; set; }
        public string Name { get; set; }
        public double DividendYield { get; set; }
        public long DividendYear { get; set; }
        public double PriceToEarningRatio { get; set; }
        public double PriceToBookRatio { get; set; }
        public string FinancialStatements { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}
