using Microsoft.Win32;
using Plotnokov_21_102_AutoserviceGoods.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
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
        private bool createNew;
        string basePath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public UpsertProduct(Product product_)
        {
            InitializeComponent();

            product = product_;
            createNew = product_ == null;
            if (createNew)
            {
                product = new Product();
            }

            btnSave.Click += BtnSave_Click;
            btnSelectPhoto.Click += BtnSelectPhoto_Click;

            using (var db = new DB.DB())
            {
                foreach (var p in db.Product)
                {
                    if (!tbCategory.Items.Contains(p.ProductCategory))
                    {
                        tbCategory.Items.Add(p.ProductCategory);
                    }
                }
            }

            img.Source = new BitmapImage(new Uri(basePath + "\\Resources\\picture.png"));
            if (!createNew)
            {
                tbArticle.Text = product.ProductArticleNumber.ToString();
                tbArticle.IsEnabled = false;

                tbCategory.Text = product.ProductCategory; 
                tbDescription.Text = product.ProductDescription;
                tbInStock.Text = product.ProductQuantityInStock.ToString();
                tbManufact.Text = product.ProductManufacturer;
                tbMaxDiscount.Text = product.ProductMaxDiscount.ToString();
                tbProductCost.Text = product.ProductCost.ToString();
                tbProductDiscount.Text = product.ProductDiscountAmount.ToString();
                tbProductName.Text = product.ProductName;
                tbSupplier.Text = product.ProductSupplier;
                cbInPack.IsChecked = product.ProductInPack;

                try
                {
                    img.Source = new BitmapImage(new Uri(basePath + "\\Resources\\" + product.ProductPhoto));
                }
                catch { 
                    
                }
            }
        }

        private void BtnSelectPhoto_Click(object sender, RoutedEventArgs eeeeee)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();        
            fileDialog.ShowDialog();

           
            if (fileDialog.FileName != null)
            {
                try
                {
                    product.ProductPhoto = fileDialog.FileName.Replace("\\", "").Replace(":", "");
                    img.Source = new BitmapImage(new Uri(basePath + "\\Resources\\" + product.ProductPhoto));

                }
                catch (Exception e)
                {
                    MessageBox.Show("Error: you are a teapot :(\n" + e.Message + "\n" + e.StackTrace);
                }
            }       
        }

        private void BtnSave_Click(object sender, RoutedEventArgs eee)
        {
            try
            {
                product.ProductCategory = tbCategory.Text;
                product.ProductDescription = tbDescription.Text;
                product.ProductQuantityInStock = int.Parse(tbInStock.Text);
                product.ProductManufacturer = tbManufact.Text;
                product.ProductMaxDiscount = int.Parse(tbMaxDiscount.Text);
                product.ProductArticleNumber = tbArticle.Text;
                product.ProductCost = decimal.Parse(tbProductCost.Text);
                product.ProductDiscountAmount = byte.Parse(tbProductDiscount.Text);
                product.ProductName = tbProductName.Text;
                product.ProductSupplier = tbSupplier.Text;
                product.ProductInPack = cbInPack.IsChecked == true;

                if (product.ProductMaxDiscount > 100)
                    throw new Exception("Макс. скидка не может быть больше 100%");

                if (product.ProductMaxDiscount < 0)
                    throw new Exception("Скидка не может быть меньше нуля");

                if (product.ProductDiscountAmount > product.ProductMaxDiscount)
                    throw new Exception("Скидка больше максимальной");

                if (product.ProductQuantityInStock < 0)
                    throw new Exception("Меньше нуля кол-во на складе");

                if (product.ProductCost <= 0)
                    throw new Exception("Цена должна быть больше нуля");


                using (var db = new DB.DB())
                {
                    db.Product.AddOrUpdate(product);
                    db.SaveChanges();
                    Close();
                }
            }
            catch (Exception e) 
            {
                if (e is FormatException)
                    MessageBox.Show("Неверно введено некоторое число: " + e.Message, "Ошыбка", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show(e.Message, "Ошыбка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
