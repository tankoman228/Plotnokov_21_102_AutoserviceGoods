using Plotnokov_21_102_AutoserviceGoods.DB;
using Plotnokov_21_102_AutoserviceGoods.Service;
using Plotnokov_21_102_AutoserviceGoods.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
        Order order = new Order { OrderProduct = new List<OrderProduct>(), OrderStatus = ":Принят:" };

        public ClientOrGuest()
        {
            InitializeComponent();
            ServiceProducts = new ServiceProducts();

            tbSearch.TextChanged += (s, e) => updateProducts();
            cbSortNoExpCheap.SelectionChanged += (s, e) => updateProducts();
            cbDiscount09910149915andmore.SelectionChanged += (s, e) => updateProducts();
            updateProducts();

            tabProducts.SelectionChanged += TabProducts_SelectionChanged;
            btnOrder.Click += BtnOrder_Click;
            using (var db = new DB.DB())
            {
                cbPickup.ItemsSource = db.PickupPoint.ToList();
            }
        }

        private void BtnOrder_Click(object sender, RoutedEventArgs e)
        {
            if (cbPickup.SelectedItem == null)
            {
                MessageBox.Show($"Укажите точку выдачи", "Не указана точка выдачи заказа", MessageBoxButton.OK, MessageBoxImage.Question);
                return;
            }

            foreach (var product in order.OrderProduct)
            {
                if (product.Count > product.Product.ProductQuantityInStock)
                {
                    MessageBox.Show($"Вы хотите заказать {product.Product.ProductName} в кол-ве {product.Count}, но на складе есть только {product.Product.ProductQuantityInStock}", "Мало на складе", MessageBoxButton.OK, MessageBoxImage.Question);
                    return;
                }
                product.Product.ProductQuantityInStock -= product.Count;
            }

            try
            {
                using (var db = new DB.DB())
                {
                    foreach (var product in order.OrderProduct)
                    {
                        db.Product.AddOrUpdate(product.Product);
                        product.Product = null;
                    }
                    order.OrderPickupPoint = (cbPickup.SelectedItem as PickupPoint).IdPickupPoint;
                    order.OrderDeliveryDate = DateTime.Today.AddDays(14);

                    db.Order.Add(order);
                    db.SaveChanges();
                    MessageBox.Show("Заказ принят, ура!", "Всё ок", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    MessageBox.Show(ex.InnerException.InnerException.Message, "Error of DB", MessageBoxButton.OK);
                }
                catch { }
            }
        }

        private void TabProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabProducts.SelectedIndex == 0)
                return;

            decimal sumCost = 0;
            foreach (var p in order.OrderProduct)
            {
                sumCost += p.Product.ProductCost * (100 - p.Product.ProductDiscountAmount) / 100 * p.Count;
            }
            tbSum.Text = $"Суммарная стоимость всго заказа: {sumCost}";

            lbOrder.Items.Clear();
            foreach (var p in order.OrderProduct)
            {
                lbOrder.Items.Add(p);
            }
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var item = ((e.Source as TextBox).Tag as OrderProduct);
            var tb = (e.Source as TextBox);
            try
            {
                item.Count = int.Parse(tb.Text);
            }
            catch { tb.Text = "1"; item.Count = 1; }
            if (tb.Text == "0")
            {
                order.OrderProduct.Remove(order.OrderProduct.Where(x => x.ProductArticleNumber == item.ProductArticleNumber).First());
                if (order.OrderProduct.Count <= 0)
                    tabBuy.IsEnabled = false;
            }

            TabProducts_SelectionChanged(null, null);
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var item = ((e.Source as Button).Tag as OrderProduct).Product;
            order.OrderProduct.Remove(order.OrderProduct.Where(x => x.ProductArticleNumber == item.ProductArticleNumber).First());

            TabProducts_SelectionChanged(null, null);
            if (order.OrderProduct.Count <= 0)
                tabBuy.IsEnabled = false;
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

        // To add to order
        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            var item = ((e.Source as Button).Tag as ProductModel).Product;

            if (order.OrderProduct.Any(x => x.Product.ProductArticleNumber == item.ProductArticleNumber))
            {
                order.OrderProduct.Where((x => x.ProductArticleNumber == item.ProductArticleNumber)).First().Count++;
            }
            else
            {
                order.OrderProduct.Add(new OrderProduct { ProductArticleNumber = item.ProductArticleNumber, Product = item, Count = 1 });
            }
            tabBuy.IsEnabled = true;
        }


    }
}
