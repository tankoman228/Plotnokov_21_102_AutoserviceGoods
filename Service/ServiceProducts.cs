using Plotnokov_21_102_AutoserviceGoods.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotnokov_21_102_AutoserviceGoods.Service
{
    internal class ServiceProducts
    {
        public IEnumerable<Product> GetProducts(Func<Product, bool> search)
        {
            using (var db = new DB.DB())
            {
                var p = db.Product.ToList();
                return p.Where(x => search(x)).ToList();
            }
        }

        public void Upsert(Product product)
        {
            using (var db = new DB.DB())
            {
                db.Product.AddOrUpdate(product);
                db.SaveChanges();
            }
        }

        public void Delete(Product product)
        {
            using (var db = new DB.DB())
            {
                db.Product.Remove(db.Product.Find(product.ProductArticleNumber));    
                db.SaveChanges();
            }
        }
    }
}
