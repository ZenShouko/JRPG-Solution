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
        bool AddedKeydownEvent = false;
        int loadTimeMS = 0;
        public DispatchTab(string stageName)
        {
            InitializeComponent();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //Security check
            //GameData.InitializeDatabase();
            
            Stages.CreateStage(MainGrid, stageName);
            Grid.SetColumnSpan(Menu, 9);
            Grid.SetRowSpan(Menu, 9);

            watch.Stop();
            loadTimeMS = (int)watch.ElapsedMilliseconds;
        }

        private void BtnLeave_Click(object sender, RoutedEventArgs e)
        {
            //Unsubscribe from keydown event
            Window.GetWindow(this).KeyDown -= DispatchTab_KeyDown;

            //Return to main screen
            Interaction.OpenTab("MainTab");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AddedKeydownEvent)
            {
                Window.GetWindow(this).KeyDown += DispatchTab_KeyDown;
                AddedKeydownEvent = true;
            }
        }

        private void DispatchTab_KeyDown(object sender, KeyEventArgs e)
        {
            //Return if battle is active
            if (Stages.CurrentStage.IsBattle)
            {
                return;
            }

            //Open main menu?
            if (e.Key == Key.Escape)
            {
                Menu.Visibility = Menu.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                MainGrid.Opacity = Menu.Visibility == Visibility.Visible ? 0.8 : 1;
                return;
            }

            //Cancel if menu is open
            if (Menu.Visibility == Visibility.Visible)
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
