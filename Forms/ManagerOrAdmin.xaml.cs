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

            tbUserName.Text = $"{ServiceLogin.CurrentUser.UserName} {ServiceLogin.CurrentUser.UserSurname} {ServiceLogin.CurrentUser.UserPatronymic}";

            btnAddP.Click += BtnAddP_Click;
            btnDeleteP.Click += BtnDeleteP_Click;
            btnEditP.Click += BtnEditP_Click;

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

        private void BtnEditP_Click(object sender, RoutedEventArgs e)
        {
            if (lbProducts.SelectedItem == null)
                return;

            new UpsertProduct((lbProducts.SelectedItem as ProductModel).Product).ShowDialog();
            updateProducts();
        }

        private void BtnDeleteP_Click(object sender, RoutedEventArgs e)
        {
            if (lbProducts.SelectedItem == null)
                return;

            try
            {
                ServiceProducts.Delete((lbProducts.SelectedItem as ProductModel).Product);
                updateProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при удалении", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddP_Click(object sender, RoutedEventArgs e)
        {
            new UpsertProduct(null).ShowDialog();
            updateProducts();
        }
    }
}
