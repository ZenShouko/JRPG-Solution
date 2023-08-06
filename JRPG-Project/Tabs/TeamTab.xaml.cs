using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

            //Tab preparation
            LoadCharacterImages();
            FocusOnCharacter();
            DisplayStats("CHARACTER");
        }
        //#Vars
        int CharacterIndex = 0;
        Brush ItemHighlightBrush = Brushes.Yellow;
        string CurrentShownStat = "WEAPON";

        private void LoadCharacterImages()
        {
            CharacterSlot1.Source = Inventory.Team[0].CharImage.Source;
            CharacterSlot2.Source = Inventory.Team[1].CharImage.Source;
            CharacterSlot3.Source = Inventory.Team[2].CharImage.Source;
        }

        private void FocusOnCharacter()
        {
            //Highlight selected character's border
            CharacterSlotBorder1.BorderBrush = CharacterIndex == 0 ? Brushes.GreenYellow : Brushes.Black;
            CharacterSlotBorder2.BorderBrush = CharacterIndex == 1 ? Brushes.GreenYellow : Brushes.Black;
            CharacterSlotBorder3.BorderBrush = CharacterIndex == 2 ? Brushes.GreenYellow : Brushes.Black;

            //Display Info
            TxtCharacterName.Text = Inventory.Team[CharacterIndex].Name;
            TxtCharacterLevel.Text = "Level " + Inventory.Team[CharacterIndex].Level.ToString();
            TxtCharacterDescription.Text = Inventory.Team[CharacterIndex].Description;
            CharacterImage.Source = Inventory.Team[CharacterIndex].CharImage.Source;

            //Display Equipment
            if (Inventory.Team[CharacterIndex].Weapon != null)
            {
                TxtWeaponName.Text = Inventory.Team[CharacterIndex].Weapon.Name;
                TxtWeaponLevel.Text = "Level " + Inventory.Team[CharacterIndex].Weapon.Level.ToString();
                CharacterWeaponImage.Source = ItemData.GetItemImage(Inventory.Team[CharacterIndex].Weapon).Source;
            }
            else
            {
                TxtWeaponName.Text = "None";
                TxtWeaponLevel.Text = "";
                CharacterWeaponImage.Source = new BitmapImage(new Uri(@"/Resources/Assets/GUI/empty.png", UriKind.Relative));
            }

            if (Inventory.Team[CharacterIndex].Armour != null)
            {
                TxtArmourName.Text = Inventory.Team[CharacterIndex].Armour.Name;
                TxtArmourLevel.Text = "Level " + Inventory.Team[CharacterIndex].Armour.Level.ToString();
                CharacterArmourImage.Source = ItemData.GetItemImage(Inventory.Team[CharacterIndex].Armour).Source;
            }
            else
            {
                TxtArmourName.Text = "None";
                TxtArmourLevel.Text = "";
                CharacterArmourImage.Source = new BitmapImage(new Uri(@"/Resources/Assets/GUI/empty.png", UriKind.Relative));
            }

            if (Inventory.Team[CharacterIndex].Amulet != null)
            {
                TxtAmuletName.Text = Inventory.Team[CharacterIndex].Amulet.Name;
                TxtAmuletLevel.Text = "Level " + Inventory.Team[CharacterIndex].Amulet.Level.ToString();
                CharacterAmuletImage.Source = ItemData.GetItemImage(Inventory.Team[CharacterIndex].Amulet).Source;
            }
            else
            {
                TxtAmuletName.Text = "None";
                TxtAmuletLevel.Text = "";
                CharacterAmuletImage.Source = new BitmapImage(new Uri(@"/Resources/Assets/GUI/empty.png", UriKind.Relative));
            }
        }

        private void HighlightItemBorder(string item)
        {
            if (item.ToUpper() == "WEAPON")
            {
                BorderWeapon.BorderBrush = ItemHighlightBrush;
                BorderArmour.BorderBrush = Brushes.Black;
                BorderAmulet.BorderBrush = Brushes.Black;
            }
            else if (item.ToUpper() == "ARMOUR")
            {
                BorderWeapon.BorderBrush = Brushes.Black;
                BorderArmour.BorderBrush = ItemHighlightBrush;
                BorderAmulet.BorderBrush = Brushes.Black;
            }
            else if (item.ToUpper() == "AMULET")
            {
                BorderWeapon.BorderBrush = Brushes.Black;
                BorderArmour.BorderBrush = Brushes.Black;
                BorderAmulet.BorderBrush = ItemHighlightBrush;
            }
            else
            {
                //reset
                BorderWeapon.BorderBrush = Brushes.Black;
                BorderArmour.BorderBrush = Brushes.Black;
                BorderAmulet.BorderBrush = Brushes.Black;
            }
        }

        private void DisplayStats(string item)
        {
            if (item.ToUpper() == "CHARACTER")
            {
                TxtHp.Text = Inventory.Team[CharacterIndex].GetAccumelatedStats().HP.ToString();
                TxtDmg.Text = Inventory.Team[CharacterIndex].GetAccumelatedStats().DMG.ToString();
                TxtDef.Text = Inventory.Team[CharacterIndex].GetAccumelatedStats().DEF.ToString();
                TxtSpd.Text = Inventory.Team[CharacterIndex].GetAccumelatedStats().SPD.ToString();
                TxtSta.Text = Inventory.Team[CharacterIndex].GetAccumelatedStats().STA.ToString();
                TxtStr.Text = Inventory.Team[CharacterIndex].GetAccumelatedStats().STR.ToString();
                TxtCrc.Text = Inventory.Team[CharacterIndex].GetAccumelatedStats().CRC.ToString();
                TxtCrd.Text = Inventory.Team[CharacterIndex].GetAccumelatedStats().CRD.ToString();

                CurrentShownStat = "CHARACTER";
                HighlightItemBorder("NONE");
            }
            else if (item.ToUpper() == "WEAPON")
            {
                //Get item
                Weapon weapon = Inventory.Team[CharacterIndex].Weapon;

                //If no item, reset stats
                if (weapon is null)
                {
                    DisplayStats("NONE");
                    return;
                }

                //Display stats
                TxtHp.Text = weapon.Stats.HP.ToString();
                TxtDmg.Text = weapon.Stats.DMG.ToString();
                TxtDef.Text = weapon.Stats.DEF.ToString();
                TxtSpd.Text = weapon.Stats.SPD.ToString();
                TxtSta.Text = weapon.Stats.STA.ToString();
                TxtStr.Text = weapon.Stats.STR.ToString();
                TxtCrc.Text = weapon.Stats.CRC.ToString();
                TxtCrd.Text = weapon.Stats.CRD.ToString();

                //Highlight item border
                HighlightItemBorder("WEAPON");
                CurrentShownStat = "WEAPON";
            }
            else if (item.ToUpper() == "ARMOUR")
            {
                Armour armour = Inventory.Team[CharacterIndex].Armour;

                if (armour is null)
                {
                    DisplayStats("NONE");
                    return;
                }

                TxtHp.Text = armour.Stats.HP.ToString();
                TxtDmg.Text = armour.Stats.DMG.ToString();
                TxtDef.Text = armour.Stats.DEF.ToString();
                TxtSpd.Text = armour.Stats.SPD.ToString();
                TxtSta.Text = armour.Stats.STA.ToString();
                TxtStr.Text = armour.Stats.STR.ToString();
                TxtCrc.Text = armour.Stats.CRC.ToString();
                TxtCrd.Text = armour.Stats.CRD.ToString();

                HighlightItemBorder("ARMOUR");
                CurrentShownStat = "ARMOUR";
            }
            else if (item.ToUpper() == "AMULET")
            {
                Amulet amulet = Inventory.Team[CharacterIndex].Amulet;

                if (amulet is null)
                {
                    DisplayStats("NONE");
                    return;
                }

                TxtHp.Text = amulet.Stats.HP.ToString();
                TxtDmg.Text = amulet.Stats.DMG.ToString();
                TxtDef.Text = amulet.Stats.DEF.ToString();
                TxtSpd.Text = amulet.Stats.SPD.ToString();
                TxtSta.Text = amulet.Stats.STA.ToString();
                TxtStr.Text = amulet.Stats.STR.ToString();
                TxtCrc.Text = amulet.Stats.CRC.ToString();
                TxtCrd.Text = amulet.Stats.CRD.ToString();

                HighlightItemBorder("AMULET");
                CurrentShownStat = "AMULET";
            }
            else if (item.ToUpper() == "NONE")
            {
                //Reset stats
                TxtHp.Text = "0";
                TxtDmg.Text = "0";
                TxtDef.Text = "0";
                TxtSpd.Text = "0";
                TxtSta.Text = "0";
                TxtStr.Text = "0";
                TxtCrc.Text = "0";
                TxtCrd.Text = "0";

                //Reset border
                HighlightItemBorder("NONE");
                CurrentShownStat = "NONE";
            }
        }

        private void OpenMainTab(object sender, RoutedEventArgs e)
        {
            Interaction.OpenTab("MainTab");
        }

        private void PickCharacter(object sender, MouseButtonEventArgs e)
        {
            //Which character was clicked?
            Image img = (Image)sender;

            if (img.Name == "CharacterSlot1")
            {
                CharacterIndex = 0;
            }
            else if (img.Name == "CharacterSlot2")
            {
                CharacterIndex = 1;
            }
            else if (img.Name == "CharacterSlot3")
            {
                CharacterIndex = 2;
            }

            FocusOnCharacter();
            DisplayStats("CHARACTER");
        }

        private void Item_Click(object sender, MouseButtonEventArgs e)
        {
            //Which item was clicked?
            Border border = (Border)sender;

            //Display Stat
            if (border.Name.Contains("Weapon"))
            {
                DisplayStats("WEAPON");
            }
            else if (border.Name.Contains("Armour"))
            {
                DisplayStats("ARMOUR");
            }
            else if (border.Name.Contains("Amulet"))
            {
                DisplayStats("AMULET");
            }
        }

        private void ChangeItem(string item)
        {
            ChangeEquipmentWindow window = new ChangeEquipmentWindow(item, CharacterIndex);
            window.ShowDialog();

            //Reload tab
            FocusOnCharacter();
        }

        private void BorderWeapon_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Get sender
            Border border = (Border)sender;

            if (border.Name == "BorderWeapon")
            {
                ChangeItem("WEAPON");
            }
            else if (border.Name == "BorderArmour")
            {
                ChangeItem("ARMOUR");
            }
            else if (border.Name == "BorderAmulet")
            {
                ChangeItem("AMULET");
            }
        }
    }
}
