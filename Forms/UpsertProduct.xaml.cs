using Plotnokov_21_102_AutoserviceGoods.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Plotnokov_21_102_AutoserviceGoods.Forms
{
    /// <summary>
    /// Логика взаимодействия для UpsertProduct.xaml
    /// </summary>
    public partial class UpsertProduct : Window
    {
        private Product product;
        string basePath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public UpsertProduct(Product product_)
        {
            InitializeComponent();

            product = product_;
            btnSave.Click += BtnSave_Click;
            btnSelectPhoto.Click += BtnSelectPhoto_Click;

            img.Source = new BitmapImage(new Uri(basePath + "\\Resources\\picture.png"));
            if (product != null )
            {
                try
                {
                    img.Source = new BitmapImage(new Uri(basePath + "\\Resources\\" + product.ProductPhoto));
                }
                catch { 
                    
                }
            }
        }

        private void BtnSelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
