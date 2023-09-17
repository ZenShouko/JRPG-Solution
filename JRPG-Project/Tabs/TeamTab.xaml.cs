using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using JRPG_Project.ClassLibrary.Universal;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            DisplayStats(Inventory.Team[CharacterIndex]);
        }
        //#Vars
        int CharacterIndex = 0;
        Brush ItemHighlightBrush = Brushes.Yellow;

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
            TxtThreatScore.Text = CharacterData.GetThreatScore(Inventory.Team[CharacterIndex]).ToString();

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

            //Display XP
            if (Inventory.Team[CharacterIndex].Level < LevelData.CharacterXPTable.Keys.LastOrDefault())
            {
                AnimateXpBar(Inventory.Team[CharacterIndex].Stats.XP, LevelData.CharacterXPTable[Inventory.Team[CharacterIndex].Level + 1].Item1);
            }
            else
            {
                AnimateXpBar(LevelData.CharacterXPTable.Values.Last().Item1, LevelData.CharacterXPTable.Values.Last().Item1);
            }
        }


        bool xpAnimate = false;
        private async void AnimateXpBar(int xp, int max)
        {
            //[!] Stop animation
            xpAnimate = false;

            //[X] Reset everything
            XpBar.Value = 0;
            XpBar.Maximum = max;
            TxtXp.Text = "Calculating ...";

            //Display empty if max is 1
            if (max == 1)
            {
                TxtXp.Text = "";
                return;
            }

            //Prep
            int increment = xp / 4;

            //[>] Start animation
            xpAnimate = true;

            //[>>] Animate
            while (xpAnimate)
            {
                XpBar.Value += increment;
                TxtXp.Text = $"{XpBar.Value}/{max} XP";
                await Task.Delay(1);

                if (XpBar.Value >= xp)
                {
                    XpBar.Value = xp;
                    xpAnimate = false;
                }
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

        private void DisplayStats(BaseItem item)
        {
            if (item is null)
            {
                HighlightItemBorder("NONE");
                TxtHp.Text = "0";
                TxtDmg.Text = "0";
                TxtDef.Text = "0";
                TxtSpd.Text = "0";
                TxtSta.Text = "0";
                TxtStr.Text = "0";
                TxtCrc.Text = "0";
                TxtCrd.Text = "0";

                //Display XP
                AnimateXpBar(0, 1);
                return;
            }

            //Get stats object
            IStatsHolder statsObj = item as IStatsHolder;

            TxtHp.Text = statsObj.Stats.HP.ToString();
            TxtDmg.Text = statsObj.Stats.DMG.ToString();
            TxtDef.Text = statsObj.Stats.DEF.ToString();
            TxtSpd.Text = statsObj.Stats.SPD.ToString();
            TxtSta.Text = statsObj.Stats.STA.ToString();
            TxtStr.Text = statsObj.Stats.STR.ToString();
            TxtCrc.Text = statsObj.Stats.CRC.ToString();
            TxtCrd.Text = statsObj.Stats.CRD.ToString();

            //Highlight item border
            HighlightItemBorder(item.GetType().Name.ToUpper());

            //Display XP
            if (item.Level < LevelData.WeaponXPTable.Keys.LastOrDefault())
            {
                AnimateXpBar(statsObj.Stats.XP, Convert.ToInt16(LevelData.WeaponXPTable[item.Level + 1].Item1 * ItemData.RarityMultipliers[item.Rarity.ToUpper()]));
            }
            else
            {
                AnimateXpBar(statsObj.Stats.XP, statsObj.Stats.XP);
            }
        }

        private void DisplayStats(Character c)
        {
            TxtHp.Text = c.GetAccumulatedStats().HP.ToString();
            TxtDmg.Text = c.GetAccumulatedStats().DMG.ToString();
            TxtDef.Text = c.GetAccumulatedStats().DEF.ToString();
            TxtSpd.Text = c.GetAccumulatedStats().SPD.ToString();
            TxtSta.Text = c.GetAccumulatedStats().STA.ToString();
            TxtStr.Text = c.GetAccumulatedStats().STR.ToString();
            TxtCrc.Text = c.GetAccumulatedStats().CRC.ToString();
            TxtCrd.Text = c.GetAccumulatedStats().CRD.ToString();

            //Highlight item border
            HighlightItemBorder("None");
            FocusOnCharacter();

            //Display XP
            if (c.Level < LevelData.CharacterXPTable.Keys.LastOrDefault())
            {
                AnimateXpBar(c.Stats.XP, LevelData.CharacterXPTable[c.Level + 1].Item1);
            }
            else
            {
                AnimateXpBar(LevelData.CharacterXPTable.Values.Last().Item1, LevelData.CharacterXPTable.Values.Last().Item1);
            }
        }

        private void OpenMainTab(object sender, RoutedEventArgs e)
        {
            SoundManager.PlaySound("click-short.wav");
            Interaction.OpenTab("MainTab");
        }

        private void PickCharacter(object sender, MouseButtonEventArgs e)
        {
            //play sound
            SoundManager.PlaySound("switch.wav");

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
            DisplayStats(Inventory.Team[CharacterIndex]);
        }

        private void Item_Click(object sender, MouseButtonEventArgs e)
        {
            //Which item was clicked?
            Border border = (Border)sender;

            //Display Stat
            if (border.Name.Contains("Weapon"))
            {
                DisplayStats(Inventory.Team[CharacterIndex].Weapon);
            }
            else if (border.Name.Contains("Armour"))
            {
                DisplayStats(Inventory.Team[CharacterIndex].Armour);
            }
            else if (border.Name.Contains("Amulet"))
            {
                DisplayStats(Inventory.Team[CharacterIndex].Amulet);
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

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("This is the threat score. The game looks at the stats and makes a rough estimate of how strong the character is." +
                " Threat scores are not 100% accurate! Someone with very high stats but very low damage will have a high threat score. " +
                "The game has lots of randomnesses implemented paired with crit-chances which makes it harder to make an accurate estimate. " +
                "Pair it with the power of friendship ('cause together we're strong!), it becomes even more challenging to calculate." +
                "\nAnd don't forget about cursed items that make things 100x worse 😵‍💫"
                , "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
