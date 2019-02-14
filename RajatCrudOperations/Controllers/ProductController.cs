using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RajatCrudOperations.Models;

namespace RajatCrudOperations.Controllers
{
    [Route("product")]
    public class ProductController : Controller
    {
        private IMemoryCache _cache;

        public ProductController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [Route("")]
        [Route("index")]
        [Route("~/")]
        // GET: Home
        public ActionResult Index()
        {
            DataContext db = new DataContext();
            if (_cache.Get("product") == null)
            {
                var products = db.Product.ToList();
                _cache.Set<List<Product>>("product", products);
            }
            return View(_cache.Get("product"));
        }

        [HttpPost]
        public JsonResult InsertProduct(Product product)
        {
            using (DataContext db = new DataContext())
            {
                db.Product.Add(product);
                db.SaveChanges();
                _cache.Set<List<Product>>("product", null);
            }

            return Json(product);
        }

        [HttpPost]
        public ActionResult UpdateProduct(Product product)
        {
            using (DataContext db = new DataContext())
            {
                Product updatedProduct = (from c in db.Product
                                            where c.Id == product.Id
                                            select c).FirstOrDefault();
                updatedProduct.Name = product.Name;
                db.SaveChanges();
                _cache.Set<List<Product>>("product", null);
            }

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult DeleteCustomer(int productId)
        {
            using (DataContext db = new DataContext())
            {
                Product product = (from c in db.Product
                                     where c.Id == productId
                                   select c).FirstOrDefault();
                db.Product.Remove(product);
                db.SaveChanges();
                _cache.Set<List<Product>>("product", null);
            }

            return new EmptyResult();
        }
    }
}