using Plotnokov_21_102_AutoserviceGoods.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotnokov_21_102_AutoserviceGoods.Service
{
    internal class ServiceOrder
    {
        public void MakeOrder(OrderProduct[] products, DateTime deliveryDate, int idPickupPoint)
        {
            using (var db = new DB.DB())
            {
                var order = new Order { 
                    OrderDeliveryDate = deliveryDate,
                    OrderPickupPoint = idPickupPoint,
                    OrderStatus = "Новый",
                    OrderProduct = products
                };               
                
                foreach (var product in products)
                {
                    var p = db.Product.Find(product.ProductArticleNumber);
                    p.ProductQuantityInStock -= product.Count;
                    if (p.ProductQuantityInStock < 0)
                        throw new Exception("-1 product");
                }
                db.Order.Add(order);

                db.SaveChanges();
            }
        }

        public IEnumerable<Order> GetOrders()
        {
            using (var db = new DB.DB())
            {
                return db.Order.ToList();
            }
        }

        public void UpsertOrder(Order order)
        {
            using (var db = new DB.DB())
            {
                var dbOrder = db.Order.Find(order.OrderID);
                if (dbOrder != null)
                {
                    foreach (var product in dbOrder.OrderProduct)
                    {
                        var p = db.Product.Find(product.ProductArticleNumber);
                        p.ProductQuantityInStock += product.Count;
                    }
                }

                foreach (var product in order.OrderProduct)
                {
                    var p = db.Product.Find(product.ProductArticleNumber);
                    p.ProductQuantityInStock -= product.Count;
                    if (p.ProductQuantityInStock < 0)
                        throw new Exception("-1 product");
                }
                db.Order.AddOrUpdate(order);

                db.SaveChanges();
            }
        }
    }
}
