using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace PRN221Project.Pages.Customer
{
    public class CartModel : PageModel
    {
        [BindProperty]
        public List<CartItem> CartItems { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            CartItems = HttpContext.Session.GetObjectsession<List<CartItem>>("Cart");
            if (CartItems == null)
                CartItems = new List<CartItem>();

            var cartItemJson = TempData["CartItem"] as string;
             if (string.IsNullOrEmpty(cartItemJson))
                return Page();

             CartItem cartItem = JsonConvert.DeserializeObject<CartItem>(cartItemJson);

            var existItem = CartItems.FirstOrDefault(c => c.ProductId == cartItem.ProductId);
            if (existItem != null)
            {
                cartItem.Price += cartItem.Price / cartItem.Quantity;
                cartItem.Quantity += existItem.Quantity;
                cartItem.Price = cartItem.Price * cartItem.Quantity;
                CartItems.Remove(existItem);
            }

            CartItems.Add(cartItem);
            HttpContext.Session.SetObjectsession("Cart", CartItems);
            return Page();
        }   

        public IActionResult OnPost()
        {

            var user = HttpContext.Session.GetObjectsession<BusinessObject.Customer>("User");
            if (user == null)
            {
                TempData["ErrorMessage"] = "You Need Login to Buy Product";
                return RedirectToPage("/Index");
            }

            if(user.Address == null || user.Phone == null)
            {
                TempData["ErrorMessage"] = "You Need Update Personal Information to Order";
                return RedirectToPage("/Customer/Profile");
            }

            TempData["CartItems"] = JsonConvert.SerializeObject(CartItems);
            return RedirectToPage("/Customer/Order");
        }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
