using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
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

namespace JRPG_Project.Tabs
{
    /// <summary>
    /// Interaction logic for MainTab.xaml
    /// </summary>
    public partial class MainTab : UserControl
    {
        public MainTab()
        {
            InitializeComponent();
        }

        private void OpenTab(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Interaction.OpenTab(btn.Name);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            //Check if user has unsaved changes
            if (GameData.HasUnsavedChanges())
            {
                MessageBoxResult result = MessageBox.Show("Save before closing?", "Unsaved changes", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    GameData.Save();
                }
            }

            Window.GetWindow(this).Close();
        }
    }
}
