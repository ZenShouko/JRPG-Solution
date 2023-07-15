using JRPG_ClassLibrary;
using JRPG_ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace JRPG_Project.Tabs
{
    /// <summary>
    /// Interaction logic for PlaygroundTab.xaml
    /// </summary>
    public partial class DispatchTab : UserControl
    {
        int loadTimeMS = 0;
        public DispatchTab(string stageName)
        {
            InitializeComponent();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //Security check
            //GameData.InitializeDatabase();
            
            Stages.CreateStage(MainGrid, stageName);
            CreateMenu();

            watch.Stop();
            loadTimeMS = (int)watch.ElapsedMilliseconds;
        }

        StackPanel menu = new StackPanel();

        private void CreateMenu()
        {
            //Panel
            menu.Orientation = Orientation.Horizontal;
            menu.HorizontalAlignment = HorizontalAlignment.Center;
            menu.VerticalAlignment = VerticalAlignment.Top;
            menu.Margin = new Thickness(10);
            menu.Background = Brushes.LightGray;
            menu.Height = 60;
            Grid.SetColumn(menu, 0);
            Grid.SetColumnSpan(menu, MainGrid.ColumnDefinitions.Count);
            Grid.SetRowSpan(menu, MainGrid.RowDefinitions.Count);
            menu.Visibility = Visibility.Collapsed;
            MainGrid.Children.Add(menu);

            //Buttons
            Button BtnLeave = new Button();
            BtnLeave.Content = "Leave";
            BtnLeave.Click += BtnLeave_Click;
            BtnLeave.Margin = new Thickness(10);
            BtnLeave.Width = 100;
            BtnLeave.Height = 35;
            BtnLeave.Style = (Style)FindResource("menu-button");
            menu.Children.Add(BtnLeave);
        }

        private void BtnLeave_Click(object sender, RoutedEventArgs e)
        {
            //Return to main screen
            Interaction.OpenTab("MainTab");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Attach keydown event to window
            Window.GetWindow(this).KeyDown += DispatchTab_KeyDown;
        }

        private void DispatchTab_KeyDown(object sender, KeyEventArgs e)
        {
            //Open main menu?
            if (e.Key == Key.Escape)
            {
                menu.Visibility = menu.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                MainGrid.Opacity = menu.Visibility == Visibility.Visible ? 0.9 : 1;
                return;
            }

            //Cancel if menu is open
            if (menu.Visibility == Visibility.Visible)
            {
                return;
            }

            //Send directional keys to player controls
            if (PlayerControls.DirectionalKeys.Contains(e.Key))
            {
                PlayerControls.HandleInput(e.Key);
            }
        }
    }
}
