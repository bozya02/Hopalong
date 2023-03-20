using Hopalong.DB;
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
    /// Interaction logic for AgentsListWindow.xaml
    /// </summary>
    public partial class AgentsListWindow : Window
    {
        private const int ITEMSONPAGE = 10;
        private int _page = 0;
        private int _pagesCount => Agents.Count / ITEMSONPAGE + (_agents.Count % ITEMSONPAGE != 0 ? 1 : 0);

        private List<Agent> _agents;
        public List<Agent> Agents { get; set; }
        public List<AgentType> AgentTypes { get; set; }
        public Dictionary<string, Func<Agent, object>> Sortings { get; set; }

        public AgentsListWindow()
        {
            InitializeComponent();

            _agents = DataAccess.GetAgents();

            AgentTypes = DataAccess.GetAgentTypes();
            AgentTypes.Insert(0, new AgentType
            {
                Title = "Все",
                Agents = _agents
            });

            Sortings = new Dictionary<string, Func<Agent, object>>
            {
                { "Без сортировки", x => x.ID },
                { "Наименование по возрастанию", x => x.Title },
                { "Наименование по убыванию", x => x.Title },        //reverse
                { "Скидка по возрастанию", x => x.Discount },
                { "Скидка по убыванию", x => x.Discount },        //reverse
                { "Приоритет по возрастанию", x => x.Priority },
                { "Приоритет по убыванию", x => x.Priority }        //reverse
            };

            this.DataContext = this;

            DataAccess.AddNewItemEvent += DataAccess_AddNewItemEvent;
            ApplyFilters();
        }

        private void DataAccess_AddNewItemEvent()
        {
            _agents = DataAccess.GetAgents();
            AgentTypes[0].Agents = _agents;
            cbAgentType.ItemsSource = AgentTypes;
            ApplyFilters();
        }

        private void lvAgents_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var agent = lvAgents.SelectedItem as Agent;

            if (agent != null)
                new AgentWindow(agent).ShowDialog();
        }


        private void ApplyFilters(bool isStartFilter = true)
        {
            var search = tbSearch.Text.ToLower().Trim();
            var agentType = cbAgentType.SelectedItem as AgentType;
            var sorting = cbSorting.SelectedItem as string;

            if (string.IsNullOrEmpty(sorting) || agentType == null)
                return;

            if (isStartFilter)
                _page = 0;

            Agents = agentType.Agents.Where(x => x.Title.ToLower().Contains(search) ||
                                                 x.Phone.ToLower().Contains(search) ||
                                                 x.Email.ToLower().Contains(search)).ToList();

            Agents = sorting.Contains("убыванию") ?
                     Agents.OrderBy(Sortings[sorting]).ToList() :
                     Agents.OrderByDescending(Sortings[sorting]).ToList();

            if (sorting.Contains("Скидка") || sorting.Contains("Приоритет"))
                Agents.Reverse();

            lvAgents.ItemsSource = Agents.Skip(_page * ITEMSONPAGE).Take(ITEMSONPAGE);
            lvAgents.Items.Refresh();

            GeneratePages();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void cbAgentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void cbSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void btnEditPriority_Click(object sender, RoutedEventArgs e)
        {
            var agents = lvAgents.SelectedItems.Cast<Agent>().ToList();
            if (agents != null || agents.Count != 0)
                new EditPriorityWindow(agents).ShowDialog();

            lvAgents.SelectedIndex = -1;
        }

        private void btnNewAgent_Click(object sender, RoutedEventArgs e)
        {
            new AgentWindow(new Agent(), true).ShowDialog();
        }

        private void Paginator(object sender, MouseButtonEventArgs e)
        {
            var content = (sender as TextBlock).Text;

            if (content.Contains("<") && _page > 0)
                _page--;
            else if (content.Contains(">") && _page < _pagesCount - 1)
                _page++;
            else if (int.TryParse(content, out int page))
                _page = page - 1;

            ApplyFilters(false);
        }

        private void GeneratePages()
        {
            spPages.Children.Clear();

            for (int i = 0; i < _pagesCount; i++)
            {
                spPages.Children.Add(new TextBlock
                {
                    Text = (i + 1).ToString(),
                    Style = Application.Current.FindResource("Paginator") as Style
                });

                spPages.Children[i].PreviewMouseDown += Paginator;
            }

            if (_pagesCount != 0)
                (spPages.Children[_page] as TextBlock).TextDecorations = TextDecorations.Underline;
        }
    }
}
