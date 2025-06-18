using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using BusinessObject;

namespace Store.Pages.DiscountMaster
{
    public class CreateModel : PageModel
    {
        private readonly ShopDbContext _context;

           [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public CreateModel(ShopDbContext context)
        {
            _context = context;
        }

        public IList<Product> Products { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Products != null)
            {
                Products = await _context.Products.ToListAsync();
                
            }
           
        }

        [BindProperty]
        public Discount Discount { get; set; }

        public async Task<IActionResult> OnPostAsync(List<int> selectedProducts)
        {
            if (ModelState.IsValid)
            {
                return Page();
            }


            try
            {
                foreach (var productId in selectedProducts)
                {
                    var productDiscount = new Discount
                    {
                        ProductID = productId,
                        DiscountName = Discount.DiscountName,
                        DiscountPercent = Discount.DiscountPercent,
                        CreatedAt = Discount.CreatedAt,
                        EndAt = Discount.EndAt
                    };
                    _context.Discounts.Add(productDiscount);
                }

                await _context.SaveChangesAsync();
                SuccessMessage = "Discount created successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating discount: {ex.Message}";
                return Page();
            }


            return RedirectToPage("");
        }
    }
}
