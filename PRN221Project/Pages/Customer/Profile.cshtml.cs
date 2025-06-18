using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObject;
namespace PRN221Project.Pages.Customer
{
    public class ProfileModel : PageModel
    {
        private readonly ShopDbContext _context;

        public ProfileModel(ShopDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public bool IsEditing { get; set; }
        
        [BindProperty]
        public BusinessObject.Customer Customer { get; set; }

        public async Task OnGetAsync()
        {
            var user = HttpContext.Session.GetObjectsession<BusinessObject.Customer>("User");
            Customer = user;
        }

        public void OnPostEditProfile()
        {
            var user = HttpContext.Session.GetObjectsession<BusinessObject.Customer>("User");
            Customer = user;
            IsEditing = true;
        }

        public async Task<IActionResult> OnPostSaveProfileAsync()
        {
            var user = HttpContext.Session.GetObjectsession<BusinessObject.Customer>("User");
            if (Customer.FullName==null || Customer.Email == null || Customer.Address == null || Customer.Phone == null)
            {               
                Customer = user;
                IsEditing = true;
                return Page();
            }


            var customerToUpdate = await _context.Customers.FindAsync(user.CustomerID);
            if (customerToUpdate == null)
            {
                return RedirectToPage("/Customer/Profile");
            }

            customerToUpdate.FullName = Customer.FullName;
            customerToUpdate.Email = Customer.Email;
            customerToUpdate.UserName = Customer.UserName;
            customerToUpdate.Address = Customer.Address;
            customerToUpdate.Phone = Customer.Phone;

            _context.Customers.Update(customerToUpdate);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetObjectsession("User", customerToUpdate);

            IsEditing = false;
            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToPage();
        }
    }
}
