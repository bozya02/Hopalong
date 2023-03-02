using Hopalong.DB;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Hopalong.Windows
{
    /// <summary>
    /// Interaction logic for AgentWindow.xaml
    /// </summary>
    public partial class AgentWindow : Window
    {
        public Agent Agent { get; set; }
        public List<AgentType> AgentTypes { get; set; }
        public List<Product> Products { get; set; }

        public AgentWindow(Agent agent, bool isNew = false)
        {
            InitializeComponent();

            Agent = agent;
            AgentTypes = DataAccess.GetAgentTypes();
            Products = DataAccess.GetProducts();

            if (isNew)
            {
                Title = $"Новый {Title}";
                btnDelete.Visibility = Visibility.Hidden;
            }
            else
                Title = $"{Title} №{Agent.ID}";

            this.DataContext = this;

            if (agent.ProductSales != null)
                btnDelete.Visibility = Visibility.Hidden;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (DataAccess.IsINNnKPPExist(Agent))
            {
                MessageBox.Show("ИНН и/или КПП не уникален", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                DataAccess.SaveAgent(Agent);
            }
            catch
            {
                MessageBox.Show("Введены некорректные значения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.Close();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы точно хотите удалить данного агента?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
                return;
            DataAccess.DeleteAgent(Agent);
            this.Close();
        }

        private void btnAddImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "*.png|*.png|*.jpeg|*.jpeg|*.jpg|*.jpg"
            };

            if (fileDialog.ShowDialog().Value)
            {
                var image = File.ReadAllBytes(fileDialog.FileName);
                Agent.Logo = image;

                imgLogo.Source = new BitmapImage(new Uri(fileDialog.FileName));
            }
        }

        private void lvProductSales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var productSale = lvProductSales.SelectedItem as ProductSale;
                if (productSale == null)
                    return;
                var result = MessageBox.Show("Вы точно хотите удалить данную продажу?", "Предупреждение", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
                    return;
                Agent.ProductSales.Remove(productSale);
                DataAccess.DeleteProductSale(productSale);

                lvProductSales.ItemsSource = Agent.ProductSales;
                lvProductSales.Items.Refresh();
            }
            catch { }
        }

        private void cbProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var product = cbProducts.SelectedItem as Product;
            if (Agent.ProductSales.Any(x => x.Product.ID == product.ID))
                return;

            Agent.ProductSales.Add(new ProductSale
            {
                Agent = Agent,
                Product = product,
                SaleDate = DateTime.Now
            });

            lvProductSales.ItemsSource = Agent.ProductSales;
            lvProductSales.Items.Refresh();
        }
    }
}
