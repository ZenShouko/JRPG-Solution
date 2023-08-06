using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using System;
using System.Collections.Generic;
using System.Windows;
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
            {"COMMON", 5 }, {"SPECIAL", 15 }, {"CURSED", 25 }, {"LEGENDARY", 35}
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
            StatsWindow window = new StatsWindow(Item.UniqueID, false);
            window.ShowDialog();
        }

        private void CancelButton(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExtractButton(object sender, RoutedEventArgs e)
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

            //Display
            BorderValue.Visibility = Visibility.Collapsed;
            ConvertionSymbol.Visibility = Visibility.Collapsed;
            BtnClose.Foreground = Brushes.Black;
            BtnExtract.Visibility = Visibility.Collapsed;

            //Check if we've received a scroll
            //Get scroll
            Material scroll = GetScroll();
            if (scroll is null)
                return;

            //Display scroll
            ScrollContainer.Visibility = Visibility.Visible;
            FinalPlusSymbol.Visibility = Visibility.Visible;
            TxtScroll.Text = scroll.Name;

            //Add scroll to inventory
            Inventory.Materials[scroll.ID] += 1;
        }
    }
}
