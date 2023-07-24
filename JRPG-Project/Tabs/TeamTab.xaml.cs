using JRPG_ClassLibrary;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JRPG_Project.Tabs
{
    /// <summary>
    /// Interaction logic for TeamTab.xaml
    /// </summary>
    public partial class TeamTab : UserControl
    {
        public TeamTab()
        {
            InitializeComponent();
            LoadCharacterImages();
            FocusOnCharacter(0);
        }

        private void LoadCharacterImages()
        {
            CharacterSlot1.Source = Inventory.Team[0].CharImage.Source;
            //CharacterSlot2.Source = Inventory.Team[1].CharImage.Source;
            //CharacterSlot3.Source = Inventory.Team[2].CharImage.Source;
        }

        private void FocusOnCharacter(int charIndex)
        {
            //Highlight selected character's border
            CharacterSlotBorder1.BorderBrush = charIndex == 0 ? Brushes.GreenYellow : Brushes.Black;
            CharacterSlotBorder2.BorderBrush = charIndex == 1 ? Brushes.GreenYellow : Brushes.Black;
            CharacterSlotBorder3.BorderBrush = charIndex == 2 ? Brushes.GreenYellow : Brushes.Black;

            //Display Info
            TxtCharacterName.Text = Inventory.Team[charIndex].Name;
            TxtCharacterLevel.Text = "Level " + Inventory.Team[charIndex].Level.ToString();
            TxtCharacterDescription.Text = Inventory.Team[charIndex].Description;
            CharacterImage.Source = Inventory.Team[charIndex].CharImage.Source;

            //Display Equipment
            TxtWeaponName.Text = Inventory.Team[charIndex].Weapon.Name;
            TxtWeaponLevel.Text = "Level " + Inventory.Team[charIndex].Weapon.Level.ToString();
            CharacterWeaponImage.Source = Inventory.Team[charIndex].Weapon.ItemImage.Source;

            TxtArmourName.Text = Inventory.Team[charIndex].Armour.Name;
            TxtArmourLevel.Text = "Level " + Inventory.Team[charIndex].Armour.Level.ToString();
            CharacterArmourImage.Source = Inventory.Team[charIndex].Armour.ItemImage.Source;

            TxtAmuletName.Text = Inventory.Team[charIndex].Amulet.Name;
            TxtAmuletLevel.Text = "Level " + Inventory.Team[charIndex].Amulet.Level.ToString();
            CharacterAmuletImage.Source = Inventory.Team[charIndex].Amulet.ItemImage.Source;
        }

        private void DisplayStats(string item)
        {
            if (item.ToUpper() == "WEAPON")
            {
                //TODO
            }
        }

        private void OpenMainTab(object sender, RoutedEventArgs e)
        {
            Interaction.OpenTab("MainTab");
        }
    }
}
