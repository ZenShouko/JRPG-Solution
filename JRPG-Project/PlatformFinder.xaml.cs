using JRPG_ClassLibrary;
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

namespace JRPG_Project.Tabs
{
    /// <summary>
    /// Interaction logic for PlatformFinder.xaml
    /// </summary>
    public partial class PlatformFinder : Window
    {
        public PlatformFinder()
        {
            InitializeComponent();
            LockOkButton();
            LoadAllPlatforms();
        }

        private void LoadAllPlatforms()
        {
            //Scan every platform in folder
            foreach (string platform in Directory.GetFiles(@"../../Stages", "*.json"))
            {
                //Remove path and extension
                string stageName = System.IO.Path.GetFileNameWithoutExtension(platform);
                ListPlatforms.Items.Add(stageName);
            }
        }

        private void LockOkButton()
        {
            //Enable/Disable OK button
            BtnOk.IsEnabled = ListPlatforms.SelectedIndex != -1;
        }

        private void ListPlatforms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LockOkButton();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Interaction.OpenTab("MainTab");
            Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Interaction.OpenDispatchTab(ListPlatforms.SelectedItem.ToString() + ".json");
            Close();
        }
    }
}
