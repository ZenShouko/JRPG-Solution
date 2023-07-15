using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

            //Get item
            GetRandomItem(rarity);
        }

        string[] Equipables = new[] { "WEAPON", "ARMOUR", "AMULET" };

        private void GetRandomItem(string boxRarity)
        {
            //prep
            Lootbox box = LootboxData.LootboxList.FirstOrDefault(x => x.Rarity == boxRarity);
            Random random = new Random();
            string itemRarity = GetItemRarity(box);

            //Get item based on Collectable or Equipable?
            if (random.Next(0, 101) <= box.CollectableOdd)
            {
                Collectable item = ItemData.ListCollectables.Where(x => x.Rarity == itemRarity).ElementAt(random.Next(0, ItemData.ListCollectables.Where(x => x.Rarity == itemRarity).Count() - 1));
                DisplayItem(item.Name, item.ItemImage);
                PlayerActions.AddCollectable(item);
            }
            else
            {
                //Cancel if inventory is full
                if (PlayerActions.IsInventoryFull())
                {
                    return;
                }

                switch (Equipables[random.Next(0, Equipables.Count() - 1)])
                {
                    case "WEAPON":
                        {
                            Weapon item = ItemData.ListWeapons.Where(x => x.Rarity == itemRarity).ElementAt(random.Next(0, ItemData.ListWeapons.Where(x => x.Rarity == itemRarity).Count() - 1));
                            DisplayItem(item.Name, item.ItemImage);
                            PlayerActions.AddWeapon(item);
                            break;
                        }
                    case "ARMOUR":
                        {
                            Armour item = ItemData.ListArmours.Where(x => x.Rarity == itemRarity).ElementAt(random.Next(0, ItemData.ListArmours.Where(x => x.Rarity == itemRarity).Count() - 1));
                            DisplayItem(item.Name, item.ItemImage);
                            PlayerActions.AddArmour(item);
                            break;
                        }
                    case "AMULET":
                        {
                            Amulet item = ItemData.ListAmulets.Where(x => x.Rarity == itemRarity).ElementAt(random.Next(0, ItemData.ListAmulets.Where(x => x.Rarity == itemRarity).Count() - 1));
                            DisplayItem(item.Name, item.ItemImage);
                            PlayerActions.AddAmulet(item);
                            break;
                        }
                }
            }

            //The rest
            SetTitleColour(itemRarity);
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

        private void DisplayItem(string name, Image image)
        {
            TxtName.Text = name;
            ImgItem.Source = image.Source;
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
