using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace Store.Pages.OrderMaster
{
    public class UserListDetailModel : PageModel
    {
        private readonly ShopDbContext _context;

        public UserListDetailModel(ShopDbContext context)
        {
            _context = context;
        }

        public Order Order { get; set; }
        public IList<OrderDetail> OrderDetails { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.OrderID == id);

            if (Order == null)
            {
                return NotFound();
            }

            OrderDetails = Order.OrderDetails.ToList();

            return Page();
        }
    }
}
