using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JRPG_Project
{
    /// <summary>
    /// Interaction logic for EssenceExtractionWindow.xaml
    /// </summary>
    public partial class EssenceExtractionWindow : Window
    {
        public EssenceExtractionWindow(BaseItem item)
        {
            InitializeComponent();
            Item = item;
            PrepareGUI();
        }

        public EssenceExtractionWindow(List<BaseItem> itemList)
        {
            InitializeComponent();
            Items = itemList;
            PrepareGUI();
        }

        BaseItem Item { get; set; }
        List<BaseItem> Items { get; set; } = new List<BaseItem>();
        private Dictionary<string, double> RarityMultiplier = new Dictionary<string, double>()
        {
            {"COMMON", 1.2 }, {"SPECIAL", 1.5 }, {"CURSED", 2 }, {"LEGENDARY", 2.5 }
        };

        private Dictionary<string, int> ScrollChance = new Dictionary<string, int>()
        {
            {"COMMON", 8 }, {"SPECIAL", 16 }, {"CURSED", 32 }, {"LEGENDARY", 40}
        };

        private void PrepareGUI()
        {
            if (Item != null)
            {
                ImgItem.Source = Item.ItemImage.Source;
                TxtName.Text = Item.Name;
                TxtValue.Text = Item.Value.ToString();
                int orbs = CalculateEssenceGain();
                TxtOrbCount.Text = orbs.ToString();
            }
            else
            {
                ImgItem.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Assets/GUI/alligator.png", UriKind.RelativeOrAbsolute));
                TxtName.Text = $"Bulk Extract {Items.Count} items";
                TxtValue.Text = Items.Sum(x => x.Value).ToString();
                int orbs = CalculateEssenceGain();
                TxtOrbCount.Text = orbs.ToString();
            }
        }

        private int CalculateEssenceGain()
        {
            //Vars
            int orbValue = ItemData.ListMaterials.Find(orb => orb.Name.Contains("Orb")).Stats.XP;
            double essenceGain = 0;

            //Calculate total essence gain
            essenceGain = Item == null ? Math.Round((Items.Sum(x => x.Value) / 5) * RarityMultiplier[Items[0].Rarity]) :
                Math.Round((Item.Value / 5) * RarityMultiplier[Item.Rarity]);

            //Convert to int
            essenceGain = Convert.ToInt32(essenceGain);

            //#Formula: (Value / 5) * (Rarity)
            //Calculate orbs
            int orbs = (int)essenceGain / orbValue;

            return orbs;
        }

        private Material GetScroll(string rarity)
        {
            //Odds of geting a scroll: (COMMON: 0%), SPECIAL: 10%, CURSED: 20%, LEGENDARY: 30%
            int roll = Interaction.GetRandomNumber(1, 100);

            if (roll <= ScrollChance[rarity])
            {
                //Get scroll
                List<Material> listScrolls = new List<Material>();
                foreach (Material mat in ItemData.ListMaterials)
                {
                    if (mat.Name.Contains("Scroll"))
                    {
                        listScrolls.Add(mat);
                    }
                }

                //Return random scroll
                Material newScroll = new Material();
                newScroll.CopyFrom(listScrolls[Interaction.GetRandomNumber(0, listScrolls.Count - 1)]);
                return newScroll;
            }
            else
            {
                return null;
            }
        }

        private void ImgItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Item is null) return;

            if (Item.UniqueID is null || Item.UniqueID.Contains("collectable"))
            {
                StatsWindow window = new StatsWindow(Item);
                window.ShowDialog();
            }
            else
            {
                StatsWindow window = new StatsWindow(Item.UniqueID, false);
                window.ShowDialog();
            }
        }

        private void MaterialMouseUp(object sender, MouseButtonEventArgs e)
        {
            //Get sender
            Image img = (Image)sender;

            if (img.Source.ToString().Contains("orb"))
            {
                Material mat = ItemData.ListMaterials.Find(x => x.Name.Contains("Orb"));
                StatsWindow window = new StatsWindow(mat);
                window.ShowDialog();
            }
            else if (img.Source.ToString().Contains("scroll"))
            {
                if (Item != null)
                {
                    Material mat = ItemData.ListMaterials.Find(x => x.Name.Contains(TxtScroll.Text));
                    StatsWindow window = new StatsWindow(mat);
                    window.ShowDialog();
                }
            }
        }


        private void CancelButton(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RemoveItem()
        {
            if (Item is null)
            {
                foreach (BaseItem item in Items)
                {
                    Inventory.Collectables.RemoveAll(x => x.UniqueID == item.UniqueID);
                    Inventory.Weapons.RemoveAll(x => x.UniqueID == item.UniqueID);
                    Inventory.Armours.RemoveAll(x => x.UniqueID == item.UniqueID);
                    Inventory.Amulets.RemoveAll(x => x.UniqueID == item.UniqueID);
                }
            }
            else
            {
                Inventory.Collectables.RemoveAll(x => x.UniqueID == Item.UniqueID);
                Inventory.Weapons.RemoveAll(x => x.UniqueID == Item.UniqueID);
                Inventory.Armours.RemoveAll(x => x.UniqueID == Item.UniqueID);
                Inventory.Amulets.RemoveAll(x => x.UniqueID == Item.UniqueID);
            }
        }

        private void ExtractButtonAsync(object sender, RoutedEventArgs e)
        {
            //IDs [Bottle of Essence: M1, Orb of Essence: M2]
            //Remove item from inventory
            RemoveItem();

            //Give essence
            int orbs = CalculateEssenceGain();
            Inventory.Materials["M2"] += orbs;

            //Buttons
            BtnClose.Foreground = Brushes.Black;
            BtnExtract.Visibility = Visibility.Collapsed;

            //Handle scrolls
            HandleScrolls();
        }

        private async void HandleScrolls()
        {
            if (Item != null)
            {
                //Get scroll
                Material scroll = GetScroll(Item.Rarity);
                if (scroll != null)
                {
                    //Display scroll
                    TxtScroll.Text = scroll.Name.Replace("Scroll of", "");

                    //Add scroll to inventory
                    Inventory.Materials[scroll.ID] += 1;
                }

                //#Extract animation
                await PlayExtractAnimation(scroll != null);

                //#Idle animation
                //IdleAnimation(BottleContainer);
                //await Task.Delay(300);
                IdleAnimation(OrbContainer);

                if (scroll is null)
                    return;

                await Task.Delay(300);
                IdleAnimation(ScrollContainer);
            }
            else
            {
                List<Material> scrolls = new List<Material>();
                foreach (var item in Items)
                {
                    Material mat = GetScroll(item.Rarity);
                    if (mat != null)
                    {
                        scrolls.Add(mat);
                    }
                }

                //Assign
                if (scrolls.Count > 0)
                {
                    //Display
                    TxtScroll.Text = $"{scrolls.Count}x Scrolls";

                    //Add to inventory
                    foreach (Material scroll in scrolls)
                    {
                        Inventory.Materials[scroll.ID] += 1;
                    }
                }

                //#Extract animation
                await PlayExtractAnimation(scrolls.Count > 0);

                //#Idle animation
                //IdleAnimation(BottleContainer);
                //await Task.Delay(300);
                IdleAnimation(OrbContainer);

                if (scrolls.Count == 0)
                    return;

                await Task.Delay(300);
                IdleAnimation(ScrollContainer);
            }
        }

        private async Task PlayExtractAnimation(bool withScroll)
        {
            //hide items
            BorderValue.Visibility = Visibility.Hidden;
            ConvertionSymbol.Visibility = Visibility.Hidden;

            //Prep
            int moveDistance = 52;
            if (withScroll)
            {
                moveDistance += 44;
            }

            //Move essence to center
            int margin = 0;
            while (margin < moveDistance)
            {
                await Task.Delay(20);
                margin += 4;
                //BottleContainer.Margin = new Thickness(-margin, 0, margin, 0);
                //FirstPlusSymbol.Margin = new Thickness(-margin + 8, 0, margin + 8, 0);
                OrbContainer.Margin = new Thickness(-margin, 0, margin, 0);
            }

            //Wait a bit
            await Task.Delay(200);

            //Collapse & reset
            BorderValue.Visibility = Visibility.Collapsed;
            ConvertionSymbol.Visibility = Visibility.Collapsed;
            //BottleContainer.Margin = new Thickness(0, 0, 0, 0);
            //FirstPlusSymbol.Margin = new Thickness(8, 0, 8, 0);
            OrbContainer.Margin = new Thickness(0, 0, 0, 0);

            if (withScroll)
            {
                await PlayScrollAnimation();
            }
        }

        private async Task PlayScrollAnimation()
        {
            ScrollContainer.Visibility = Visibility.Hidden;
            FinalPlusSymbol.Visibility = Visibility.Visible;
            ScrollContainer.Opacity = 0.5;
            FinalPlusSymbol.Opacity = 0.8;

            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(200);
                ScrollContainer.Visibility = ScrollContainer.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }

            ScrollContainer.Opacity = 1;
            FinalPlusSymbol.Opacity = 1;
        }


        private async void IdleAnimation(DockPanel panel)
        {
            while (true)
            {
                //Move up
                await IdleMoveUp(panel);

                //Move down
                await IdleMoveDown(panel);
            }
        }

        private async Task IdleMoveUp(DockPanel panel)
        {
            double margin = panel.Margin.Bottom;

            while (margin < 2)
            {
                //Move up
                panel.Margin = new Thickness(0, -margin, 0, margin);
                margin += 1;
                await Task.Delay(150);
            }
        }

        private async Task IdleMoveDown(DockPanel panel)
        {
            double margin = panel.Margin.Bottom;

            while (margin > -2)
            {
                //Move up
                panel.Margin = new Thickness(0, -margin, 0, margin);
                margin -= 1;
                await Task.Delay(150);
            }
        }
    }
}
