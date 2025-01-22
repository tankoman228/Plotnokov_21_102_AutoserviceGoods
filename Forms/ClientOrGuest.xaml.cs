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
    /// Логика взаимодействия для ClientOrGuest.xaml
    /// </summary>
    public partial class ClientOrGuest : Window
    {
        ServiceProducts ServiceProducts;

        public ClientOrGuest()
        {
            InitializeComponent();
            ServiceProducts = new ServiceProducts();

            tbSearch.TextChanged += (s, e) => updateProducts();
            cbSortNoExpCheap.SelectionChanged += (s, e) => updateProducts();
            cbDiscount09910149915andmore.SelectionChanged += (s, e) => updateProducts();

            updateProducts();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var item = ((e.Source as Button).Tag as ProductModel).Product;
            MessageBox.Show(item.ProductName);
        }
    }
}
