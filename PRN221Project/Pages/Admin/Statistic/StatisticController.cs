using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PRN221Project.Pages.Admin.Statistic
{
    [ApiController]
    [Route("Admin/Statistic")]
    public class StatisticController : Controller
    {
       private readonly ShopDbContext _context;
        public StatisticController(ShopDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetSalesData")]
        public IActionResult GetSalesData()
        {
            var salesData = _context.Orders
                .Where(o => o.CreateAt.Year == 2024)
                .GroupBy(o => o.CreateAt.Month)
                .Select(g => new { Month = g.Key, TotalSales = g.Sum(o => o.Amount) })
                .ToList();

            return new JsonResult(new
            {
                labels = salesData.Select(d => $"Tháng {d.Month}").ToList(),
                data = salesData.Select(d => d.TotalSales).ToList()
            });
        }
    }
}
