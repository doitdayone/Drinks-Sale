
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObject;

namespace Store.Pages.ProductMaster
{
    public class CreateModel : PageModel
    {

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }
        [BindProperty]
        public IFormFile Image { set; get; }
        private readonly ShopDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment;

        public CreateModel(ShopDbContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _context = context;
            this.hostingEnvironment = environment;
        }

        public IActionResult OnGet()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
           
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        [BindProperty]
        public Inventory Inventory { get; set; } = new Inventory();
        public async Task<IActionResult> OnPostAsync()
        {
            if (_context.Products == null || Product == null)
            {
                return Page();
            }
            try
            {
                if (this.Image != null)
                {
                    var fileName = GetUniqueName(this.Image.FileName);

                    var uploads = Path.Combine(this.hostingEnvironment.WebRootPath, "images");
                    var filePath = Path.Combine(uploads, fileName);
                    this.Image.CopyTo(new FileStream(filePath, FileMode.Create));
                    Product.ProductImage = fileName;
                }

                _context.Products.Add(Product);
                await _context.SaveChangesAsync();
                Inventory.ProductID = Product.ProductID;
                Inventory.Sales = 0;
                _context.Inventories.Add(Inventory);
                await _context.SaveChangesAsync();
                SuccessMessage = "Product created successfully.";
            }
            catch(Exception ex)
            {
                ErrorMessage = "Product created failed.";
            }
            return RedirectToPage("");
        }
        private string GetUniqueName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                   + "_" + Guid.NewGuid().ToString().Substring(0, 4)
                   + Path.GetExtension(fileName);
        }
    }

}


