using Plotnokov_21_102_AutoserviceGoods.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotnokov_21_102_AutoserviceGoods.ViewModel
{
    internal class OrderModel
    {
        public Order Order { get; set; }

        public decimal SumPrice { get {
            
            decimal sum = 0;

            foreach (var item in Order.OrderProduct)
                {
                    sum += item.Count * item.Product.ProductCost * (decimal)item.Product.ProductDiscountAmount / (decimal)100;
                }

            return sum;      
        }
        }
    }
}
