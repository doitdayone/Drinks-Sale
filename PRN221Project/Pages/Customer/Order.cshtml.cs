using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace PRN221Project.Pages.Customer
{
    public class OrderModel : PageModel
    {
        private readonly ShopDbContext _context;
        private readonly  IHubContext<SalesHub> signalRHub;
        public OrderModel(ShopDbContext shopDb, IHubContext<SalesHub> hubContext) { 
        _context = shopDb;
            signalRHub = hubContext;
        }
        public List<CartItem> CartItems { get; set; }

        public void OnGet()
        {
            var cartItemsJson = TempData["CartItems"] as string;
            if (!string.IsNullOrEmpty(cartItemsJson))
            {
                CartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartItemsJson);
            }
            else
            {
                CartItems = new List<CartItem>();
            }

            TempData["CartItems"] = cartItemsJson;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var cartItemsJson = TempData["CartItems"] as string;
            CartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartItemsJson);
            var order = new Order()
            {
                CreateAt = DateTime.Now,
                CustomerID = HttpContext.Session.GetObjectsession<BusinessObject.Customer>("User").CustomerID,
                Amount = CartItems.Sum(i => i.Price * i.Quantity),
                Tax = 10000
                
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            foreach (var item in CartItems)
            {
                var od = new OrderDetail()
                {
                    OrderID = order.OrderID,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ProductID = item.ProductId
                };
                await _context.OrderDetails.AddAsync(od);

                var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductID == item.ProductId);
                if (inventory != null)
                {
                    inventory.QuantityInStock -= item.Quantity;
                    _context.Inventories.Update(inventory);
                }
            };
            await _context.SaveChangesAsync();

            await signalRHub.Clients.All.SendAsync("ReceiveSalesDataUpdate");

            TempData["SuccessMessage"] = "Your order has been placed successfully.";
            return RedirectToPage("/Index");
        }

            
        
    }
}
