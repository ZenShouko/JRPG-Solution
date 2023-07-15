using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Player;
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

namespace JRPG_Project
{
    /// <summary>
    /// Interaction logic for SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {
        public SaveWindow()
        {
            InitializeComponent();
            InitializeGUI();
        }

        private void InitializeGUI()
        {
            TxtLastSave.Text = Inventory.LastSaveTime.Year != 1 ? "Last save:\n" + Inventory.LastSaveTime.ToString("F") : "Last save:\nNever";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GameData.Save();
            Console.Beep();
            GameData.Load();
            InitializeGUI();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to reset your save?", "Reset save", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                GameData.Reset();

                //restart the game
                string currentProcessPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                System.Diagnostics.Process.Start(currentProcessPath);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
    }
}
