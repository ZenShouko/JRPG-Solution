using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Player;
using System.Windows;
using System.Windows.Controls;

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
            UpdateGUI();
        }

        private void UpdateGUI()
        {
            TxtCoins.Text = Inventory.Coins.ToString();
        }

        private void OpenTab(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Interaction.OpenTab(btn.Name);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            //Check if user has unsaved changes
            MessageBoxResult result = MessageBox.Show("Save before closing?", "Unsaved changes", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                GameData.Save();
            }
            else if (result == MessageBoxResult.Cancel)
            {
                return;
            }

            Window.GetWindow(this).Close();
        }
    }
}
