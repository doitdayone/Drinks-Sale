using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN221Project;


namespace Store.Pages.OrderMaster
{
    public class UserListModel : PageModel
    {
        private readonly ShopDbContext _context;

        public UserListModel(ShopDbContext context)
        {
            _context = context;
        }

        public IList<Order> Order { get; set; }

        public async Task OnGetAsync()
        {
            
            var user = HttpContext.Session.GetObjectsession<Customer>("User");
            
            if (user != null)
            {
                Order = await _context.Orders
                    .Include(o => o.Customer)
                    .Where(o => o.CustomerID == user.CustomerID)
                    .ToListAsync();
            }
            else
            {
               
                Order = new List<Order>();
            }
        }
    }
}
