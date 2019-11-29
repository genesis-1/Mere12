using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mere12.Models;
using Mere12.Data;
using Microsoft.EntityFrameworkCore;
using Mere12.Extensions;

namespace Mere12.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var productList = await _context.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).ToListAsync();
            return View(productList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).Where(m => m.Id == id).FirstOrDefaultAsync();

            return View(product);
        }

        [HttpPost,ActionName("Details")]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsPost(int id)
        {
             List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShopingCart");
            if(listShoppingCart == null)
            {
                listShoppingCart = new List<int>();
            }
            listShoppingCart.Add(id);
            HttpContext.Session.Set("ssShopingCart", listShoppingCart);
            //redirect to Action index,inside home controller and Customer area
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        public IActionResult Remove(int id)
        {
            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShopingCart");
            if (listShoppingCart.Count > 0)
            {
                if(listShoppingCart.Contains(id))
                {
                    listShoppingCart.Remove(id);
                }
            }
            HttpContext.Session.Set("ssShopingCart", listShoppingCart);

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
