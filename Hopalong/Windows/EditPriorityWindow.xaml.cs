using Hopalong.DB;
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

namespace Hopalong.Windows
{
    /// <summary>
    /// Interaction logic for EditPriorityWindow.xaml
    /// </summary>
    public partial class EditPriorityWindow : Window
    {
        public List<Agent> Agents { get; set; }

        public EditPriorityWindow(List<Agent> agents)
        {
            InitializeComponent();

            Agents = agents;

            tbPriority.Text = Agents.Max(x => x.Priority).ToString();

            this.DataContext = this;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var content = tbPriority.Text;
            if (int.TryParse(content, out int priority))
            {
                foreach (var agent in Agents)
                    agent.Priority += priority;

                DataAccess.SaveAgent(Agents[0]);
                this.Close();
            }
            else
            {
                MessageBox.Show("Введено не целое число!", "Ошибка", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }
        }
    }
}
