using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Store.Pages.OrderMaster;

namespace PRN221Project.Pages.Customer
{
    public class ProductDetailModel : PageModel
    {
        private readonly ShopDbContext _context;

        public ProductDetailModel(ShopDbContext context)
        {
            _context = context;
        }
        public Product Product { get; set; }
        [BindProperty]
        public CartItem CartItem { get; set; }
        public List<Product> RelatedProducts { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _context.Products
                .Include(p => p.Discounts)
                 .Include(p => p.Category)
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (Product == null)
            {
                return NotFound();
            }

            // Fetch related products
            RelatedProducts = await _context.Products.Where(p => p.CategoryID == Product.ProductID)
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Include(p => p.Discounts)
                .ToListAsync();
            RelatedProducts.Remove(Product);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            TempData["CartItem"] = JsonConvert.SerializeObject(CartItem);
            return RedirectToPage("/Customer/Cart");
        }
    }
}
