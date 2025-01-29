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
            else
                gridOrders.Visibility = Visibility.Collapsed;

            tbUserName.Content = $"{ServiceLogin.CurrentUser.UserName} {ServiceLogin.CurrentUser.UserSurname} {ServiceLogin.CurrentUser.UserPatronymic}";

            btnAddP.Click += BtnAddP_Click;
            btnDeleteP.Click += BtnDeleteP_Click;
            btnEditP.Click += BtnEditP_Click;

            tbSearch.TextChanged += (s, e) => updateProducts();
            cbSortNoExpCheap.SelectionChanged += (s, e) => updateProducts();
            cbDiscount09910149915andmore.SelectionChanged += (s, e) => updateProducts();

            tbUserName.Click += (s, e) =>
            {
                if (MessageBox.Show("Вы уверены, что хотите выйти?", "logout", MessageBoxButton.YesNo, MessageBoxImage.Hand) == MessageBoxResult.Yes) {
                    ServiceLogin.CurrentUser = null;
                   new MainWindow().Show();
                    Close();
                }
            };

            updateProducts();
            updateOrders();
        }

        void updateOrders()
        {
            using (var db = new DB.DB())
            {
                var orders = db.Order.Include("OrderProduct").Include("OrderProduct.Product").ToList(); 
                lbOrders.Items.Clear();
                foreach (var o in orders)
                {
                    lbOrders.Items.Add(new OrderModel { Order = o});
                }               
            }
        }

        void updateProducts()
        {
            var products = ServiceProducts.GetProducts(x => true);
            var totalCount = products.Count();
            lbProducts.Items.Clear();

            switch (cbDiscount09910149915andmore.SelectedIndex)
            {
                case 0: break; // no
                case 1: products = products.Where(x => x.ProductDiscountAmount >= 0).Where(x => x.ProductDiscountAmount < 10).ToList(); break; // 0.99 - 10
                case 2: products = products.Where(x => x.ProductDiscountAmount >= 10).Where(x => x.ProductDiscountAmount < 15).ToList(); break; // 10 - 14 99
                case 3: products = products.Where(x => x.ProductDiscountAmount >= 15); break; // 15+
            }

            products = products.Where(x => x.ProductName.ToLower().Contains(tbSearch.Text.ToLower())).ToList();

            switch (cbSortNoExpCheap.SelectedIndex)
            {
                case 0: break;
                case 1: products = products.OrderByDescending(x => x.ProductCost).ToList(); break;
                case 2: products = products.OrderBy(x => x.ProductCost).ToList(); break;
            }

            foreach (var product in products)
            {
                lbProducts.Items.Add(new ProductModel { Product = product });
            }

            tbCount.Text = $"{products.Count()}/{totalCount}";
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
