using Plotnokov_21_102_AutoserviceGoods.Service;
using Plotnokov_21_102_AutoserviceGoods.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для ManagerOrAdmin.xaml
    /// </summary>
    public partial class ManagerOrAdmin : Window
    {
        ServiceProducts ServiceProducts;

        public ManagerOrAdmin()
        {
            InitializeComponent();
            ServiceProducts = new ServiceProducts();

            if (ServiceLogin.CurrentUser.UserRole == 3)
                spProductsCrud.Visibility = Visibility.Visible;

            updateProducts();
        }

        void updateProducts()
        {
            var products = ServiceProducts.GetProducts(x => true);
            lbProducts.Items.Clear();

            foreach (var product in products)
            {
                lbProducts.Items.Add(new ProductModel { Product = product });
            }
        }
    }
}
