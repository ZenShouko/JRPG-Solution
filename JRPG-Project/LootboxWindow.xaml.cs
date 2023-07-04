using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace JRPG_Project.ClassLibrary.Player
{
    /// <summary>
    /// Interaction logic for LootboxWindow.xaml
    /// </summary>
    public partial class LootboxWindow : Window
    {
        public LootboxWindow(string rarity)
        {
            InitializeComponent();
            this.Title = rarity;
            GetRandomItem(rarity);
        }

        string[] Equipables = new[] { "WEAPON", "ARMOUR", "AMULET" };

        private void GetRandomItem(string boxRarity)
        {
            //prep
            Lootbox box = LootboxData.LootboxList.FirstOrDefault(x => x.Rarity == boxRarity);
            Random random = new Random();
            string itemRarity = GetItemRarity(box);
            object item = null;

            //Get item based on Collectable or Equipable?
            if (random.Next(0, 101) <= box.CollectableOdd)
            {
                item = ItemData.ListCollectables.Where(x => x.Rarity == itemRarity).ElementAt(random.Next(0, ItemData.ListCollectables.Where(x => x.Rarity == itemRarity).Count() - 1));
            }
            else
            {
                switch (Equipables[random.Next(0, Equipables.Count() - 1)])
                {
                    case "WEAPON":
                        {
                            item = ItemData.ListWeapons.Where(x => x.Rarity == itemRarity).ElementAt(random.Next(0, ItemData.ListWeapons.Where(x => x.Rarity == itemRarity).Count() - 1));
                            break;
                        }
                    case "ARMOUR":
                        {
                            item = ItemData.ListArmours.Where(x => x.Rarity == itemRarity).ElementAt(random.Next(0, ItemData.ListArmours.Where(x => x.Rarity == itemRarity).Count() - 1));
                            break;
                        }
                    case "AMULET":
                        {
                            item = ItemData.ListAmulets.Where(x => x.Rarity == itemRarity).ElementAt(random.Next(0, ItemData.ListAmulets.Where(x => x.Rarity == itemRarity).Count() - 1));
                            break;
                        }
                }
            }

            //The rest
            DisplayItem(item);
            SetTitleColour(itemRarity);
            AddItemToInventory(item);
        }

        private string GetItemRarity(Lootbox box)
        {
            Random random = new Random();
            int odd = random.Next(0, 101);

            if (odd <= box.CommonOdd)
            {
                return "COMMON";
            }
            else if (odd <= box.CommonOdd + box.SpecialOdd)
            {
                return "SPECIAL";
            }
            else if (odd <= box.CommonOdd + box.SpecialOdd + box.CursedOdd)
            {
                return "CURSED";
            }
            else
            {
                return "LEGENDARY";
            }
        }

        private void DisplayItem(object item)
        {
            switch (item)
            {
                case Collectable collectable:
                    {
                        TxtName.Text = collectable.Name;
                        ImgItem.Source = collectable.ItemImage.Source;
                        break;
                    }
                case Weapon weapon:
                    {
                        TxtName.Text = weapon.Name;
                        ImgItem.Source = weapon.ItemImage.Source;
                        break;
                    }
                    case Armour armour:
                    {
                        TxtName.Text = armour.Name;
                        ImgItem.Source = armour.ItemImage.Source;
                        break;
                    }
                    case Amulet amulet:
                    {
                        TxtName.Text = amulet.Name;
                        ImgItem.Source = amulet.ItemImage.Source;
                        break;
                    }
            }
        }

        private void SetTitleColour(string rarity)
        {
            switch (rarity.ToUpper())
            {
                case "COMMON":
                    TxtName.Foreground = Brushes.Black;
                    break;
                case "SPECIAL":
                    TxtName.Foreground = Brushes.SteelBlue;
                    break;
                case "CURSED":
                    TxtName.Foreground = Brushes.Orchid;
                    break;
                case "LEGENDARY":
                    TxtName.Foreground = Brushes.Goldenrod;
                    break;
                default:
                    TxtName.Foreground = Brushes.WhiteSmoke; 
                    break;
            }
        }

        private void AddItemToInventory(object item)
        {
            PlayerActions.AddToInventory(item);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }
    }
}
