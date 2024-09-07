using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAOnlineMart.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SAOnlineMart.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MartContext _context;

        public HomeController(ILogger<HomeController> logger, MartContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users
                                     .FirstOrDefaultAsync(u => u.email == email && u.password == password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("userID", user.userID);
                HttpContext.Session.SetString("userName", $"{user.firstName} {user.lastName}");

                return RedirectToAction("Home", "Home");
            }

            ViewBag.ErrorMessage = "Invalid credentials";
            return View("Index");
        }

        public IActionResult Home()
        {
            if (HttpContext.Session.GetInt32("userID") == null)
            {
                return RedirectToAction("Index");
            }

            var saleProducts = _context.Products
                               .Where(p => p.productStatus == "Sale")
                               .ToList();

            return View(saleProducts);
        }

        public IActionResult Privacy()
        {
            if (HttpContext.Session.GetInt32("userID") == null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }

        public async Task<IActionResult> Checkout(string phoneNumber, string address, string suburb, string city, string province)
        {
            var userID = HttpContext.Session.GetInt32("userID");
            if (userID == null)
            {
                return RedirectToAction("Index");
            }

            var cartItems = await _context.Cart.ToListAsync();
            double totalAmount = (double)cartItems.Sum(item => item.productPrice * item.productQuantity);

            string fullAddress = $"{address}, {suburb}, {city}, {province}";

            var newOrder = new Orders
            {
                userID = userID.Value,
                orderTotal = totalAmount,
                orderAddress = fullAddress
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            return RedirectToAction("Payment");
        }

        public IActionResult Payment()
        {
            if (HttpContext.Session.GetInt32("userID") == null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Payment(string cardNumber, string expiryDate, string cvv)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) ||
                string.IsNullOrWhiteSpace(expiryDate) ||
                string.IsNullOrWhiteSpace(cvv))
            {
                ViewBag.ErrorMessage = "All fields are required.";
                return View();
            }

            return RedirectToAction("Home");
        }


        [HttpPost]
        public async Task<IActionResult> AddToCart(int productID, int quantity)
        {
            var product = await _context.Products.FindAsync(productID);
            if (product == null)
            {
                return NotFound();
            }

            var existingCartItem = await _context.Cart.FirstOrDefaultAsync(c => c.productName == product.productName);

            if (existingCartItem != null)
            {
                existingCartItem.productQuantity += quantity; 
            }
            else
            {
                var cartItem = new Cart
                {
                    productName = product.productName,
                    productPrice = (decimal)(double)product.productPrice,
                    productQuantity = quantity
                };
                _context.Cart.Add(cartItem); 
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Cart");
        }

        public async Task<IActionResult> Cart()
        {
            var cartItems = await _context.Cart.ToListAsync();
            ViewBag.CartItems = cartItems;

            double totalAmount = (double)cartItems.Sum(item => item.productPrice * item.productQuantity);
            ViewBag.TotalAmount = totalAmount;

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var cartItems = await _context.Cart.ToListAsync();

            _context.Cart.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("Cart");
        }

        public IActionResult ProceedToCheckout()
        {
            return RedirectToAction("Checkout");
        }

        public IActionResult Admin()
        {
            var userID = HttpContext.Session.GetInt32("userID");
            if (userID == null)
            {
                return RedirectToAction("Index");
            }

            var isAdmin = _context.Admin.Any(a => a.userID == userID);
            if (!isAdmin)
            {
                return RedirectToAction("Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(string productName, string productDescription, decimal productPrice, string productStatus, string productImage, string productCreator)
        {
            var newProduct = new Products
            {
                productName = productName,
                productDescription = productDescription,
                productPrice = productPrice,
                productStatus = productStatus,
                productImage = productImage,
                productCreator = productCreator
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(int productID, string productName, string productDescription, decimal productPrice, string productStatus, string productImage, string productCreator)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.productID == productID);
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.productName = productName;
            existingProduct.productDescription = productDescription;
            existingProduct.productPrice = productPrice;
            existingProduct.productStatus = productStatus;
            existingProduct.productImage = productImage;
            existingProduct.productCreator = productCreator;

            await _context.SaveChangesAsync();

            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int productID)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.productID == productID);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Admin");
        }

        public IActionResult Catalogue()
        {
            var saleProducts = _context.Products
                               .Where(p => p.productStatus == "Sale")
                               .ToList();
            var regularProducts = _context.Products
                                          .Where(p => p.productStatus == "Regular")
                                          .ToList();

            ViewBag.SaleProducts = saleProducts;
            ViewBag.RegularProducts = regularProducts;

            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string firstName, string lastName, string email, string password)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.email == email);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Email already in use";
                return View("Register");
            }

            var newUser = new Users
            {
                firstName = firstName,
                lastName = lastName,
                email = email,
                password = password
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("userID", newUser.userID);
            HttpContext.Session.SetString("userName", $"{newUser.firstName} {newUser.lastName}");

            return RedirectToAction("Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


