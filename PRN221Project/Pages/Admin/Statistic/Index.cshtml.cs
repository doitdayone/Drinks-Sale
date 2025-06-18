using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace PRN221Project.Pages.Admin.Statistic
{
    public class IndexModel : PageModel
    {
        private readonly ShopDbContext _context;

        public IndexModel(ShopDbContext context)
        {
            _context = context;
        }
        public string SalesDataJson { get; set; }
        public int SelectedYear { get; set; }

        public async Task OnGetAsync(int? year)
        {
            SelectedYear = year ?? DateTime.Now.Year;

            var salesData = _context.Orders
                .Where(o => o.CreateAt.Year == SelectedYear)
                .GroupBy(o => o.CreateAt.Month)
                .Select(g => new { Month = g.Key, TotalSales = g.Sum(o => o.Amount) })
                .ToList();

            SalesDataJson = JsonConvert.SerializeObject(salesData);

         
        }
        
    }
    public class SalesData
    {
        public int Month { get; set; }
        public decimal TotalSales { get; set; }
    }
}
