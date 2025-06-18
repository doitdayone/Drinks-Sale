using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System.IO;
using System.Threading.Tasks;

namespace Store.Pages.ProductMaster
{
    public class EditModel : PageModel
    {
        private readonly ShopDbContext _context;

        public EditModel(ShopDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public Inventory Inventory { get; set; }

        [BindProperty]
        public IFormFile Image { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (Product == null)
            {
                return NotFound();
            }

            Inventory = Product.Inventory;

            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", Product.CategoryID);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", Product.CategoryID);
                return Page();
            }

            var productToUpdate = await _context.Products.Include(p => p.Inventory).FirstOrDefaultAsync(p => p.ProductID == Product.ProductID);

            if (productToUpdate == null)
            {
                return NotFound();
            }

            productToUpdate.ProductName = Product.ProductName;
            productToUpdate.ProductDescription = Product.ProductDescription;
            productToUpdate.CategoryID = Product.CategoryID;
            productToUpdate.Price = Product.Price;

            if (Image != null)
            {
                // Handle the image file upload (you can add logic to save the file and set the Product.ProductImage property)
                var filePath = Path.Combine("wwwroot/images", Image.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }
                productToUpdate.ProductImage = Image.FileName;
            }

            productToUpdate.Inventory.QuantityInStock = Inventory.QuantityInStock;
            productToUpdate.Inventory.Sales = Inventory.Sales;

            _context.Attach(productToUpdate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.ProductID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            TempData["SuccessMessage"] = "Product updated successfully!";
            return RedirectToPage("./Index");
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
    }
}
