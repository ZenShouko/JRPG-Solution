using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.Tabs;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JRPG_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            GameData.InitializeDatabase();
            InitializeComponent();
            Interaction.Grid = grid;
            Interaction.OpenTab("MainTab");
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            Interaction.SetKey(null);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Interaction.SetKey(e.Key);
        }
    }
}
