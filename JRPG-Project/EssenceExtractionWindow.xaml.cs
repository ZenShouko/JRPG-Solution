using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        BaseItem Item { get; set; }
        private Dictionary<string, double> RarityMultiplier = new Dictionary<string, double>()
        {
            {"COMMON", 1.2 }, {"SPECIAL", 1.5 }, {"CURSED", 2 }, {"LEGENDARY", 2.5 }
        };

        private Dictionary<string, int> ScrollChance = new Dictionary<string, int>()
        {
            {"COMMON", 0 }, {"SPECIAL", 100 }, {"CURSED", 100 }, {"LEGENDARY", 100}
        };

        private void PrepareGUI()
        {
            ImgItem.Source = Item.ItemImage.Source;
            TxtName.Text = Item.Name;
            TxtValue.Text = Item.Value.ToString();
            (int bottles, int orbs) = CalculateEssenceGain();

            TxtBottleCount.Text = bottles.ToString();
            TxtOrbCount.Text = orbs.ToString();
        }

        private (int bottles, int orbs) CalculateEssenceGain()
        {
            //Vars
            int bottleValue = ItemData.ListMaterials[0].Stats.XP;
            int orbValue = ItemData.ListMaterials[1].Stats.XP;

            //Formula: (Value / 10) * (Rarity)
            var essenceGain = Math.Round((Item.Value / 5) * RarityMultiplier[Item.Rarity]);
            essenceGain = Convert.ToInt32(essenceGain);

            //Calculate orbs
            int orbs = (int)essenceGain / orbValue;

            //Calculate bottles with remainder
            essenceGain -= orbs * orbValue;
            int bottles = (int)essenceGain / bottleValue;

            return (bottles, orbs);
        }

        private Material GetScroll()
        {
            //Odds of geting a scroll: (COMMON: 0%), SPECIAL: 10%, CURSED: 20%, LEGENDARY: 30%
            int roll = Interaction.GetRandomNumber(1, 100);

            if (roll <= ScrollChance[Item.Rarity])
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

            if (img.Source.ToString().Contains("bottle"))
            {
                Material mat = ItemData.ListMaterials[0];
                StatsWindow window = new StatsWindow(mat);
                window.ShowDialog();
            }
            else if (img.Source.ToString().Contains("orb"))
            {
                Material mat = ItemData.ListMaterials[1];
                StatsWindow window = new StatsWindow(mat);
                window.ShowDialog();
            }
            else if (img.Source.ToString().Contains("scroll"))
            {
                Material mat = ItemData.ListMaterials.Find(x => x.Name.Contains(TxtScroll.Text));
                StatsWindow window = new StatsWindow(mat);
                window.ShowDialog();
            }
        }


        private void CancelButton(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void ExtractButtonAsync(object sender, RoutedEventArgs e)
        {
            //IDs [Bottle of Essence: M1, Orb of Essence: M2]
            //Remove item from inventory
            if (Item is Weapon wpn)
            {
                Inventory.Weapons.Remove(wpn);
            }
            else if (Item is Armour arm)
            {
                Inventory.Armours.Remove(arm);
            }
            else if (Item is Amulet amu)
            {
                Inventory.Amulets.Remove(amu);
            }
            else if (Item is Collectable col)
            {
                Inventory.Collectables.Remove(col);
            }

            //Give essence
            (int bottles, int orbs) = CalculateEssenceGain();
            Inventory.Materials["M1"] += bottles;
            Inventory.Materials["M2"] += orbs;

            //Buttons
            BtnClose.Foreground = Brushes.Black;
            BtnExtract.Visibility = Visibility.Collapsed;

            //Get scroll
            Material scroll = GetScroll();
            if (scroll != null)
            {
                //Display scroll
                TxtScroll.Text =  scroll.Name.Replace("Scroll of", "");

                //Add scroll to inventory
                Inventory.Materials[scroll.ID] += 1;
            }

            //#Extract animation
            await PlayExtractAnimation(scroll != null);

            //#Idle animation
            IdleAnimation(BottleContainer);
            await Task.Delay(300);
            IdleAnimation(OrbContainer);

            if (scroll is null) 
                return;
            await Task.Delay(300);
            IdleAnimation(ScrollContainer);
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
                BottleContainer.Margin = new Thickness(-margin, 0, margin, 0);
                FirstPlusSymbol.Margin = new Thickness(-margin + 8, 0, margin + 8, 0);
                OrbContainer.Margin = new Thickness(-margin, 0, margin, 0);
            }

            //Wait a bit
            await Task.Delay(200);

            //Collapse & reset
            BorderValue.Visibility = Visibility.Collapsed;
            ConvertionSymbol.Visibility = Visibility.Collapsed;
            BottleContainer.Margin = new Thickness(0, 0, 0, 0);
            FirstPlusSymbol.Margin = new Thickness(8, 0, 8, 0);
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
