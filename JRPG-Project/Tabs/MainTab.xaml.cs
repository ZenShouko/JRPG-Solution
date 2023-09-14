using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Player;
using JRPG_Project.ClassLibrary.Universal;
using System;
using System.Collections.Generic;
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

            if (Interaction.LastProgression.Count == 0)
                return;

            //Get % of progression foreach category
            float foes = (float)Interaction.LastProgression["Foes"].Item1 / (float)Interaction.LastProgression["Foes"].Item2 * 100;
            foes = (float)Math.Round(foes, 2);
            float lootboxes = (float)Interaction.LastProgression["Lootboxes"].Item1 / (float)Interaction.LastProgression["Lootboxes"].Item2 * 100;
            lootboxes = (float)Math.Round(lootboxes, 2);

            //Update progression
            TxtProgress.Text = $"Got {Interaction.LastProgression["Lootboxes"].Item1}/{Interaction.LastProgression["Lootboxes"].Item2} lootboxes ({lootboxes}%) " +
                $"and defeated {Interaction.LastProgression["Foes"].Item1}/{Interaction.LastProgression["Foes"].Item2} foes ({foes}%).\n";
            TxtProgress.Text += $"You've received {Stages.GetProgressionRewards()} coins as completion reward!";
        }

        private void OpenTab(object sender, RoutedEventArgs e)
        {
            //Play sound
            SoundManager.PlaySound("click-medium.wav");

            //Open tab
            Button btn = sender as Button;
            Interaction.OpenTab(btn.Name);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.PlaySound("click-short.wav");

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
