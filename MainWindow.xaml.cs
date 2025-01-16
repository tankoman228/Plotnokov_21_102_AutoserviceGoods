using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Plotnokov_21_102_AutoserviceGoods.Forms;
using Plotnokov_21_102_AutoserviceGoods.Service;

namespace Plotnokov_21_102_AutoserviceGoods
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // DEBUG PART ONLY
            ServiceLogin.Login("loginDEluw2018", "S3wj{I");
            new ManagerOrAdmin().Show();
            Close();

            btnEnter.Click += BtnEnter_Click;
            btnEnterGuest.Click += BtnEnterGuest_Click;
        }

        private void BtnEnterGuest_Click(object sender, RoutedEventArgs e)
        {
            new ClientOrGuest().Show();
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckCapctha())
            {
                MessageBox.Show("Капча введена неверно!", "Robot detected", MessageBoxButton.OK, MessageBoxImage.Stop);
                BlockEnter();
                return;
            }

            if (ServiceLogin.Login(tbLogin.Text, tbPassword.Password)) {

                if (ServiceLogin.CurrentUser.UserRole == 2 || ServiceLogin.CurrentUser.UserRole == 3)
                {
                    new ManagerOrAdmin().Show();
                    this.Close();
                }
                if (ServiceLogin.CurrentUser.UserRole == 1)
                {
                    new ClientOrGuest().Show();
                    this.Close();
                }
            }
            else
            {
                if (cap != null)
                {
                    BlockEnter();
                }
                MessageBox.Show($"Не получилось войти, неверный логин или пароль", "Enter refused", MessageBoxButton.OK, MessageBoxImage.Stop);
                OpenCaptcha();
            }
        }

        private string cap;
        private char[] capSymbols = {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'A', 'a', 'V', 'W', 'L', 'G', 'F', 'D', '1', '2', '3', '4', '5', '6' };
        private char[] noiseSymbols = { '-', '_', '+', '&', '?', ',', '.', '~', '@', '\a' };
        private void OpenCaptcha()
        {
            captcha.Visibility = Visibility.Visible;
            Random r = new Random();

            cap = "" + 
                capSymbols[r.Next(capSymbols.Length)] + 
                capSymbols[r.Next(capSymbols.Length)] + 
                capSymbols[r.Next(capSymbols.Length)] + 
                capSymbols[r.Next(capSymbols.Length)];

            tbCaptcha1.Text = cap[0].ToString();
            tbCaptcha2.Text = cap[1].ToString();
            tbCaptcha3.Text = cap[2].ToString();
            tbCaptcha4.Text = cap[3].ToString();

            tbCaptchaNoise1.Text = "" + 
                noiseSymbols[r.Next(noiseSymbols.Length)] + 
                noiseSymbols[r.Next(noiseSymbols.Length)] + 
                noiseSymbols[r.Next(noiseSymbols.Length)] +
                noiseSymbols[r.Next(noiseSymbols.Length)] +
                noiseSymbols[r.Next(noiseSymbols.Length)] +
                noiseSymbols[r.Next(noiseSymbols.Length)];

            tbCaptchaNoise2.Text = "" +
                noiseSymbols[r.Next(noiseSymbols.Length)] +
                noiseSymbols[r.Next(noiseSymbols.Length)] +
                noiseSymbols[r.Next(noiseSymbols.Length)] +
                noiseSymbols[r.Next(noiseSymbols.Length)] +
                noiseSymbols[r.Next(noiseSymbols.Length)] +
                noiseSymbols[r.Next(noiseSymbols.Length)];

            tbCaptcha1.FontSize = 90 + (0.5 - r.NextDouble()) * 8;
            tbCaptcha2.FontSize = 90 + (0.5 - r.NextDouble()) * 8;
            tbCaptcha3.FontSize = 90 + (0.5 - r.NextDouble()) * 8;
            tbCaptcha4.FontSize = 90 + (0.5 - r.NextDouble()) * 8;
        }
        private bool CheckCapctha()
        {
            if (cap == null) return true;
            if (cap == tbCap.Text) return true;

            OpenCaptcha();

            return false;
        }

        private void BlockEnter()
        {
            btnEnter.IsEnabled = false;

            new Thread(() =>
            {
                for (int i = 10; i > 0; i--)
                {
                    Dispatcher.Invoke(() =>
                    {
                        btnEnter.Content = "Войти (" + i + ")";
                    });
                    Thread.Sleep(1000);
                }

                Dispatcher.Invoke(() =>
                {
                    btnEnter.IsEnabled = true;
                    btnEnter.Content = "Войти";
                });

            }).Start();
        }
    }
}
