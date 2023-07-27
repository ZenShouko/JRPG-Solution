using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
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
            if (GetRandomNumber(100) <= box.CollectableOdd)
            {
                Collectable item = ItemData.ListCollectables.Where(x => x.Rarity == itemRarity).ElementAt(GetRandomNumber(ItemData.ListCollectables.Where(x => x.Rarity == itemRarity).Count() - 1));
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

                switch (Equipables[GetRandomNumber(Equipables.Count() - 1)])
                {
                    case "WEAPON":
                        {
                            Weapon item = ItemData.ListWeapons.Where(x => x.Rarity == itemRarity).ElementAt(GetRandomNumber(ItemData.ListWeapons.Where(x => x.Rarity == itemRarity).Count() - 1));
                            DisplayItem(item.Name, item.ItemImage);
                            PlayerActions.AddWeapon(item);
                            break;
                        }
                    case "ARMOUR":
                        {
                            Armour item = ItemData.ListArmours.Where(x => x.Rarity == itemRarity).ElementAt(GetRandomNumber(ItemData.ListArmours.Where(x => x.Rarity == itemRarity).Count() - 1));
                            DisplayItem(item.Name, item.ItemImage);
                            PlayerActions.AddArmour(item);
                            break;
                        }
                    case "AMULET":
                        {
                            Amulet item = ItemData.ListAmulets.Where(x => x.Rarity == itemRarity).ElementAt(GetRandomNumber(ItemData.ListAmulets.Where(x => x.Rarity == itemRarity).Count() - 1));
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

        /// <summary>
        /// Generates a number between min and max. Min and Max are included in the range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private int GetRandomNumber(int min, int max)
        {
            //Make max inclusive
            max++;

            //Generate random number
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] randomNumber = new byte[4]; // 4 bytes for a 32-bit integer

                rng.GetBytes(randomNumber);

                // Convert the random bytes to a 32-bit signed integer
                int generatedNumber = Math.Abs(BitConverter.ToInt32(randomNumber, 0));

                // Scale the number to be within the desired range
                try
                {
                    return min + (generatedNumber % (max - min));
                }
                catch (Exception e)
                {
                    MessageBox.Show($"MIN= {min}, MAX= {max}\n" + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return 0;
                }
            }
        }

        private int GetRandomNumber(int max)
        {
            //Get random number between 0 and max
            return GetRandomNumber(0, max);
        }

        private void DisplayItem(string name, Image image)
        {
            TxtName.Text = name;
            ImgItem.Source = image.Source;

            //Resize textsize if too long
            if (TxtName.Text.Length > 22)
            {
                TxtName.FontSize = 18;
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
