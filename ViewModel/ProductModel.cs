using Plotnokov_21_102_AutoserviceGoods.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Plotnokov_21_102_AutoserviceGoods.ViewModel
{
    internal class ProductModel
    {
        public string basePath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public Product Product { get; set; }

        public ImageSource ImagePath { 

            get { 

                if (Product.ProductPhoto != null)
                {
                    try
                    {
                        return new BitmapImage(new Uri(basePath + "\\Resources\\" + Product.ProductPhoto));
                    }
                    catch {
                        return new BitmapImage(new Uri(basePath + "\\Resources\\picture.png"));
                    }
                }
                return new BitmapImage(new Uri(basePath + "\\Resources\\picture.png"));
            } 
        }

        public string CostNoDiscount => Product.ProductDiscountAmount == 0 ? "" : Product.ProductCost.ToString("F2");

        public string Cost => (Product.ProductCost * (decimal)(100 - Product.ProductDiscountAmount) / (decimal)100).ToString("F2");

        public string Discount =>  $"Скидка: {Product.ProductDiscountAmount}%";

        public System.Windows.Media.Brush BG => Product.ProductDiscountAmount > 15 ? (Brush)(new BrushConverter().ConvertFrom("#7fff00")) : Brushes.White;
    }
}
