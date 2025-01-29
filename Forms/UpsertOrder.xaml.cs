using Plotnokov_21_102_AutoserviceGoods.DB;
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
    /// Логика взаимодействия для UpsertOrder.xaml
    /// </summary>
    public partial class UpsertOrder : Window
    {
        private Order order;
        private bool insert;

        public UpsertOrder(Order order)
        {
            InitializeComponent();

            insert = order == null;
            this.order = order;

            using (var db = new DB.DB())
            {
                cbPickUpPoint.ItemsSource = db.PickupPoint.ToList();
                cbAddToOrder.ItemsSource = db.Product.ToList();
            }

            if (!insert)
            {
                tbIdTb.Text = order.OrderID.ToString();
                tbStatus.Text = order.OrderStatus.ToString();
                dpDeliveryDate.SelectedDate = order.OrderDeliveryDate;
                updOrderList();

                foreach (PickupPoint item in cbPickUpPoint.Items)
                {
                    if (item.IdPickupPoint == order.OrderPickupPoint)
                    {
                        cbPickUpPoint.SelectedItem = item;
                    }
                }
            }
            else
            {
                order = new Order { OrderProduct = new List<OrderProduct>()};
            }

            btnAddToOrder.Click += BtnAddToOrder_Click;
            btnSave.Click += BtnSave_Click;
            btnDeleteFromOrder.Click += BtnDeleteFromOrder_Click;
        }

        private void updOrderList()
        {
            lbProductOrder.Items.Clear();
            foreach (var i in order.OrderProduct)
            {
                lbProductOrder.Items.Add(i);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbPickUpPoint.SelectedItem == null)
            {
                MessageBox.Show($"Укажите точку выдачи", "Не указана точка выдачи заказа", MessageBoxButton.OK, MessageBoxImage.Question);
                return;
            }
            order.OrderPickupPoint = (cbPickUpPoint.SelectedItem as PickupPoint).IdPickupPoint;

            if (tbStatus.Text == "")
            {
                MessageBox.Show($"Укажите статус заказа", "Error", MessageBoxButton.OK, MessageBoxImage.Question);
                return;
            }
            order.OrderStatus = tbStatus.Text;

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
                    }
                    order.OrderDeliveryDate = dpDeliveryDate.SelectedDate.Value;

                    if (!insert)
                    {
                        foreach (var p in order.OrderProduct)
                        {
                            db.OrderProduct.Remove(db.OrderProduct.Find(order.OrderID, p.ProductArticleNumber));
                        }
                    }

                    db.Order.AddOrUpdate(order);
                    db.SaveChanges();
                    MessageBox.Show("Заказ принят, ура!", "Всё ок", MessageBoxButton.OK);
                    Close();
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

        private void BtnAddToOrder_Click(object sender, RoutedEventArgs e)
        {
            if (cbAddToOrder.SelectedIndex == -1)
            {
                MessageBox.Show("Не выбран товар", "Error", MessageBoxButton.OK, MessageBoxImage.Question);
                return;
            }

            var selected = order.OrderProduct.FirstOrDefault(x => x.ProductArticleNumber == (cbAddToOrder.SelectedItem as Product).ProductArticleNumber);
            if (selected == null)
            {
                order.OrderProduct.Add(new OrderProduct { 
                    Product = cbAddToOrder.SelectedItem as Product,
                    Count = 1,
                    ProductArticleNumber = (cbAddToOrder.SelectedItem as Product).ProductArticleNumber,
                    Order = order
                });
                updOrderList();
            }
            else
            {
                selected.Count++;
                selected.Product.ProductQuantityInStock--;
                updOrderList();
            }
        }

        private void BtnDeleteFromOrder_Click(object sender, RoutedEventArgs e)
        {
            if (lbProductOrder.SelectedIndex == -1)
            {
                MessageBox.Show("Не выбран товар из состава заказа", "Error", MessageBoxButton.OK, MessageBoxImage.Question);
                return;
            }
            var selected = lbProductOrder.SelectedItem as OrderProduct;
            selected.Product.ProductQuantityInStock += selected.Count;
            order.OrderProduct.Remove(selected);
            updOrderList();
        }
    }
}
