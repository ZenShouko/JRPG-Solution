using JRPG_ClassLibrary;
using JRPG_ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Player;
using JRPG_Project.ClassLibrary.Universal;
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
using System.Windows.Shapes;

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
            Stages.CreateStage(MainGrid, RadarGrid, stageName);
            Grid.SetColumnSpan(Menu, 9);
            Grid.SetRowSpan(Menu, 9);
        }

        private void BtnLeave_Click(object sender, RoutedEventArgs e)
        {
            //Unsubscribe from keydown event
            Window.GetWindow(this).KeyDown -= DispatchTab_KeyDown;

            //Save progression
            Interaction.LastProgression = Stages.CurrentStage.Progression;

            //Hand out progression reward
            ProgressionReward();

            //Return to main screen
            Interaction.OpenTab("MainTab");

            //Discard current stage
            Stages.CurrentStage = null;
        }

        private void ProgressionReward()
        {
            Inventory.Coins += Stages.GetProgressionRewards();
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

                if (Menu.Visibility == Visibility.Visible)
                {
                    Stages.UpdateFoeRadar();
                    PlayRadarAnimation();
                    UpdateProgression();
                }

                //Did player finish progression?
                if (Stages.CurrentStage.Progression["Lootboxes"].Item1 == Stages.CurrentStage.Progression["Lootboxes"].Item2 && 
                    Stages.CurrentStage.Progression["Foes"].Item1 == Stages.CurrentStage.Progression["Foes"].Item2)
                {
                    //Display finished on button
                    BtnExit.Content = "Finish";
                    BtnExit.Background = Brushes.LightSeaGreen;
                }

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

        private void BtnInventory_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.PlaySound("click-medium.wav");
            Interaction.OpenInventory();
        }

        private void UpdateProgression()
        {
            Stages.UpdateProgression();

            TxtPlatformName.Text = Stages.CurrentStage.Name;
            TxtLootCount.Text = $"Loot: {Stages.CurrentStage.Progression["Lootboxes"].Item1}/{Stages.CurrentStage.Progression["Lootboxes"].Item2}";
            TxtFoeCount.Text = $"Foes: {Stages.CurrentStage.Progression["Foes"].Item1}/{Stages.CurrentStage.Progression["Foes"].Item2}";
            TxtDimensions.Text = $"Size: {Stages.CurrentStage.TileList.Max(t => t.Position.X)}x{Stages.CurrentStage.TileList.Max(t => t.Position.Y)}";
        }

        private async void PlayRadarAnimation()
        {
            //[Start] Add a white ellipse in the center of the radar panel
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 10;
            ellipse.Height = 10;
            ellipse.Fill = Brushes.LightSlateGray;
            ellipse.HorizontalAlignment = HorizontalAlignment.Center;
            ellipse.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(ellipse, 0);
            Grid.SetRow(ellipse, 0);
            Grid.SetColumnSpan(ellipse, 3);
            Grid.SetRowSpan(ellipse, 3);
            RadarGrid.Children.Add(ellipse);

            //[Start] Animate the ellipse, grow in size
            DoubleAnimation growAnimation = new DoubleAnimation();
            growAnimation.From = 10;
            growAnimation.To = 250;
            growAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.5));
            ellipse.BeginAnimation(Ellipse.WidthProperty, growAnimation);
            ellipse.BeginAnimation(Ellipse.HeightProperty, growAnimation);

            //[Start] Animate the ellipse, fade out
            DoubleAnimation fadeAnimation = new DoubleAnimation();
            fadeAnimation.From = 1;
            fadeAnimation.To = 0;
            fadeAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            ellipse.BeginAnimation(Ellipse.OpacityProperty, fadeAnimation);

            //[End] Wait for the animation to finish
            await Task.Delay(1500);

            //[End] Remove the ellipse from the radar panel
            RadarGrid.Children.Remove(ellipse);
        }
    }
}
