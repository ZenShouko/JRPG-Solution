using JRPG_Project.ClassLibrary;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JRPG_Project
{
    /// <summary>
    /// Interaction logic for StatsWindow.xaml
    /// </summary>
    public partial class StatsWindow : Window
    {
        public StatsWindow(string uniqueId, bool previewUpgrade)
        {
            InitializeComponent();
            InitializeGUI(uniqueId);
            AnimateImage();
        }

        public StatsWindow(BaseItem item)
        {
            InitializeComponent();
            InitializeGUI(item);
            AnimateImage();
        }

        Dictionary<string, string> ItemInfo = new Dictionary<string, string>()
        {
            { "Name", "" },
            { "Level", "" },
            { "Value", "" },
            { "Rarity", "" },
            { "ImageSource", "" }
        };

        #region Animation
        int moveSpeed = 60;
        int moveDistance = 3;
        int aniDelay = 150;
        int marginTop = 0;
        int marginBottom = 0;

        private async void AnimateImage()
        {
            ///Move image 10 pixels up and down
            ///Use margin to move image
            while (true)
            {
                //Move down
                await MoveDown();

                //Delay
                await Task.Delay(aniDelay);

                //Move up
                await MoveUp();

                //Delay
                await Task.Delay(aniDelay);
            }
        }

        private async Task MoveUp()
        {
            //Up movement
            while (marginBottom < moveDistance)
            {
                //Move up
                marginTop -= 1;
                marginBottom += 1;

                //Set margin
                BorderImage.Margin = new Thickness(0, marginTop, 0, marginBottom);

                //Delay
                await Task.Delay(moveSpeed);
            }
        }

        private async Task MoveDown()
        {
            //Up movement
            while (marginTop < moveDistance)
            {
                //Move up
                marginTop += 1;
                marginBottom -= 1;

                //Set margin
                BorderImage.Margin = new Thickness(0, marginTop, 0, marginBottom);

                //Delay
                await Task.Delay(moveSpeed);
            }
        }
        #endregion

        private void InitializeGUI(string uniqueId)
        {
            Stats stats = new Stats();

            //Get item from inventory
            Weapon wpn = Inventory.Weapons.Where(x => x.UniqueID == uniqueId).FirstOrDefault();
            Armour arm = Inventory.Armours.Where(x => x.UniqueID == uniqueId).FirstOrDefault();
            Amulet amu = Inventory.Amulets.Where(x => x.UniqueID == uniqueId).FirstOrDefault();
            Collectable col = Inventory.Collectables.Where(x => x.UniqueID == uniqueId).FirstOrDefault();

            if (wpn != null)
            {
                stats = wpn.Stats;
                ItemInfo["Name"] = wpn.Name;
                ItemInfo["Level"] = wpn.Level.ToString();
                ItemInfo["Value"] = wpn.Value.ToString();
                ItemInfo["Rarity"] = wpn.Rarity.ToString();
                ItemInfo["ImageSource"] = wpn.ItemImage.Source.ToString();
            }
            else if (arm != null)
            {
                stats = arm.Stats;
                ItemInfo["Name"] = arm.Name;
                ItemInfo["Level"] = arm.Level.ToString();
                ItemInfo["Value"] = arm.Value.ToString();
                ItemInfo["Rarity"] = arm.Rarity.ToString();
                ItemInfo["ImageSource"] = arm.ItemImage.Source.ToString();
            }
            else if (amu != null)
            {
                stats = amu.Stats;
                ItemInfo["Name"] = amu.Name;
                ItemInfo["Level"] = amu.Level.ToString();
                ItemInfo["Value"] = amu.Value.ToString();
                ItemInfo["Rarity"] = amu.Rarity.ToString();
                ItemInfo["ImageSource"] = amu.ItemImage.Source.ToString();
            }

            //Display stats
            DisplayStats(stats);

            //Display info
            DisplayInfo();
        }

        private void InitializeGUI(BaseItem item)
        {
            //Display default info
            TxtName.Text = item.Name;
            TxtRarity.Text = item.Rarity;
            TxtRarity.Foreground = GetBrush(item.Rarity);
            TxtLevel.Text = item.Level.ToString();
            ImgItem.Source = item.ItemImage.Source;
            BorderColorBrush.Color = GetColor(item.Rarity);

            //Display stats?
            if (item is IStatsHolder obj)
            {
                DisplayStats(obj.Stats);

                //Display XP for materials
                if (item.Name.Contains("Essence"))
                {
                    TxtLevel.Text = obj.Stats.XP.ToString() + "xp";
                }
            }
            else
            {
                DisplayStats(new Stats());
            }
        }

        private void DisplayStats(Stats stats)
        {
            //Display stats
            TxtHP.Text = stats.HP.ToString();
            TxtDEF.Text = stats.DEF.ToString();
            TxtDMG.Text = stats.DMG.ToString();
            TxtSPD.Text = stats.SPD.ToString();
            TxtSTA.Text = stats.STA.ToString();
            TxtSTR.Text = stats.STR.ToString();
            TxtCRC.Text = stats.CRC.ToString();
            TxtCRD.Text = stats.CRD.ToString();
        }

        private void DisplayInfo()
        {
            TxtName.Text = ItemInfo["Name"];
            TxtLevel.Text = "Level " + ItemInfo["Level"];
            TxtRarity.Text = ItemInfo["Rarity"];
            TxtRarity.Foreground = GetBrush(ItemInfo["Rarity"]);

            ImgItem.Source = new BitmapImage(new Uri(ItemInfo["ImageSource"], UriKind.RelativeOrAbsolute));

            BorderColorBrush.Color = GetColor(ItemInfo["Rarity"]);
        }

        private Color GetColor(string rarity)
        {
            switch (rarity.ToUpper())
            {
                case "COMMON":
                    return Colors.Black;
                case "SPECIAL":
                    return Colors.Aqua;
                case "CURSED":
                    return Colors.Orchid;
                case "LEGENDARY":
                    return Colors.Goldenrod;
                default:
                    return Colors.WhiteSmoke;
            }
        }

        private Brush GetBrush(string rarity)
        {
            switch (rarity.ToUpper())
            {
                case "COMMON":
                    return Brushes.White;
                case "SPECIAL":
                    return Brushes.Aqua;
                case "CURSED":
                    return Brushes.Orchid;
                case "LEGENDARY":
                    return Brushes.Goldenrod;
                default:
                    return Brushes.WhiteSmoke;
            }
        }
    }
}
