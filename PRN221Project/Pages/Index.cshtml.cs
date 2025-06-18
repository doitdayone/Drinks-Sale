using BusinessObject;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PRN221Project.Pages.Customer;
using PRN221Project.Pagination;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace PRN221Project.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ShopDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, ShopDbContext context)
        {
            _logger = logger;
            _context = context;
            PageSize = 6;
        }
        [BindProperty]
        public LoginDAO LoginDAO { get; set; }
        [BindProperty]
        public SignInDAO SignInDAO { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public int Category { get; set; } = 0;
        
        [BindProperty]
        public string Open { get; set; }
        public PaginatedList<Product> Products { get; set; }

        public async Task OnGetAsync()
        {
            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            IQueryable<Product> productIQ = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Discounts)
                .Include(p => p.Inventory);

            switch (SortOrder)
            {
                case "LowToHigh":
                    productIQ = productIQ.OrderBy(p => p.Price);
                    break;

                case "HighToLow":
                    productIQ = productIQ.OrderByDescending(p => p.Price);
                    break;

                case "Sale":
                    productIQ = productIQ.Where(p => p.Discounts.Any(d => d.EndAt >= DateTime.Now));
                    break;

                case "Litte":
                    productIQ = productIQ.Where(p => p.Inventory.QuantityInStock <= 30);
                    break;

                default:
                    break;
            }

            if (SearchString != null)
            {
                productIQ = productIQ.Where(p => p.ProductName.Contains(SearchString));
            }

            if (Category != 0)
            {
                productIQ = productIQ.Where(p => p.Category.CategoryID.Equals(Category));
            }

            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            this.Products = await PaginatedList<Product>.CreateAsync(productIQ, PageIndex, PageSize);
        }


        public async Task<IActionResult> OnPostLoginAsync()
        {

            if (LoginDAO.Password == null || LoginDAO.Username == null)
            {
                await LoadProductsAsync();
                Open = "login";
                return Page();
            }

            var user = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserName == LoginDAO.Username && c.Password == LoginDAO.Password);


            if (user == null)
            {
                TempData["LoginError"] = "Invalid login attempt.";
                Open = "login";
                await LoadProductsAsync();
                return Page();
            }
            else
            {
                if (user.Role.Equals("Admin"))
                {
                    HttpContext.Session.SetObjectsession("User", user);
                    HttpContext.Session.SetString("FullName", user.FullName);
                    return RedirectToPage("/Admin/Product/Index");
                }
                else
                {
                    HttpContext.Session.SetObjectsession("User", user);
                    HttpContext.Session.SetString("FullName", user.FullName);
                    return RedirectToPage("/Index");
                }
            }

        }

        public async Task<IActionResult> OnPostSignInAsync()
        {

            if (SignInDAO.Password == null || SignInDAO.Username == null || SignInDAO.FullName == null)
            {
                await LoadProductsAsync();
                Open = "signIn";
                return Page();
            }
            if (_context.Customers.Any(c => c.UserName == SignInDAO.Username))
            {
                TempData["SignUpError"] = "Username already exists.";
                Open = "signIn";
                await LoadProductsAsync();
                return Page();
            }

            var newCustomer = new BusinessObject.Customer
            {
                UserName = SignInDAO.Username,
                Password = SignInDAO.Password,
                FullName = SignInDAO.FullName,
                Role = "Customer"
            };

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = SignInDAO.FullName + " Sign Up Successfully";
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Index"); 
        }

        public async Task<IActionResult> OnPostBuyNowAsync(int productId)
        {
            var user = HttpContext.Session.GetObjectsession<BusinessObject.Customer>("User");
            if (user == null)
            {
                TempData["ErrorMessage"] = "You Need Login to Buy Product";
                await LoadProductsAsync();
                return Page();
            }

            var product = _context.Products.FirstOrDefault(p => p.ProductID.Equals(productId));
            List<CartItem> cartItems = new List<CartItem>();
            cartItems.Add(new CartItem()
            {
                ProductId = productId,
                Quantity = 1,
                ProductName = product.ProductName,
                Price = CountDiscountPrice(product.Price, (List<Discount>) _context.Discounts.Where(d => d.ProductID==productId).ToList())

            });
            TempData["CartItems"] = JsonConvert.SerializeObject(cartItems);
            return RedirectToPage("/Customer/Order");
        }

        public static decimal CountDiscountPrice(decimal price, List<Discount> discounts)
        {
            if(discounts==null)
                return price;
            foreach (Discount discount in discounts)
            {
                if(discount.EndAt > DateTime.Now)
                    price = price * (100 - discount.DiscountPercent) / 100;
            }
            return price;
        }
    }

    public class LoginDAO
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }

    public class SignInDAO
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }
    }
}
