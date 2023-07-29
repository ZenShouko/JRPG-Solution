using JRPG_Project.ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace JRPG_Project
{
    /// <summary>
    /// Interaction logic for ChangeEquipmentWindow.xaml
    /// </summary>
    public partial class ChangeEquipmentWindow : Window
    {
        public ChangeEquipmentWindow(string type, int charIndex)
        {
            InitializeComponent();

            //Set type and charIndex
            this.type = type;
            this.charIndex = charIndex;

            //Load items
            LoadItems();
        }

        string type = "";
        int charIndex = 0;

        private Brush GetBrush(string rarity)
        {
            switch (rarity.ToUpper())
            {
                case "COMMON":
                    return Brushes.Black;
                case "SPECIAL":
                    return Brushes.SteelBlue;
                case "CURSED":
                    return Brushes.Orchid;
                case "LEGENDARY":
                    return Brushes.Goldenrod;
                default:
                    return Brushes.WhiteSmoke;
            }
        }

        private void LoadItems()
        {
            //Load items based on type
            if (type == "WEAPON")
            {
                foreach (Weapon weapon in Inventory.Weapons)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Tag = weapon.UniqueID;
                    item.Foreground = GetBrush(weapon.Rarity);
                    item.MouseDoubleClick += (sender, e) => ListboxItemClick(sender, e, type);

                    //If weapon is equipped, add a star and the owner
                    string owner = ItemData.GetOwnersName(weapon);
                    if (owner == "/")
                    {
                        item.Content = $"[{weapon.Level}] " +  weapon.Name;
                    }
                    else
                    {
                        item.Content = "★ " + $"[{weapon.Level}] " + weapon.Name + $"    -->    {owner}";
                    }

                    ListEquipments.Items.Add(item);
                }
            }
            else if (type == "ARMOUR")
            {
                foreach (Armour armour in Inventory.Armours)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = armour.Name;
                    item.Tag = armour.UniqueID;
                    item.Foreground = GetBrush(armour.Rarity);
                    item.MouseDoubleClick += (sender, e) => ListboxItemClick(sender, e, type);

                    //If item is equipped, add a star and the owner
                    string owner = ItemData.GetOwnersName(armour);
                    if (owner == "/")
                    {
                        item.Content = $"[{armour.Level}] " + armour.Name;
                    }
                    else
                    {
                        item.Content = "★ " + $"[{armour.Level}] " + armour.Name + $"    -->    {owner}";
                    }

                    ListEquipments.Items.Add(item);
                }
            }
            else if (type == "AMULET")
            {
                foreach (Amulet amu in Inventory.Amulets)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = amu.Name;
                    item.Tag = amu.UniqueID;
                    item.Foreground = GetBrush(amu.Rarity);
                    item.MouseDoubleClick += (sender, e) => ListboxItemClick(sender, e, type);

                    //If item is equipped, add a star and the owner
                    string owner = ItemData.GetOwnersName(amu);
                    if (owner == "/")
                    {
                        item.Content = $"[{amu.Level}] " + amu.Name;
                    }
                    else
                    {
                        item.Content = "★ " + $"[{amu.Level}] " + amu.Name + $"    -->    {owner}";
                    }

                    ListEquipments.Items.Add(item);
                }
            }
        }

        private void ListboxItemClick(object sender, MouseButtonEventArgs e, string type)
        {
            //Get sender as ListBoxItem
            ListBoxItem item = (ListBoxItem)sender;

            //Open Statswindow
            StatsWindow statsWindow = new StatsWindow((string)item.Tag, false);
            statsWindow.ShowDialog();
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void EquipItem(object sender, RoutedEventArgs e)
        {
            //If nothing selected, close window
            if (ListEquipments.SelectedIndex == -1)
            {
                Close();
                return;
            }

            //Get UniqueID of selected item
            ListBoxItem item = (ListBoxItem)ListEquipments.SelectedItem;
            string uniqueId = (string)item.Tag;

            if (type == "WEAPON")
            {
                Weapon wpn = Inventory.Weapons.FirstOrDefault(x => x.UniqueID == uniqueId);
                CharacterData.EquipItem(wpn, charIndex);
            }
            else if (type == "ARMOUR")
            {
                Armour arm = Inventory.Armours.FirstOrDefault(x => x.UniqueID == uniqueId);
                CharacterData.EquipItem(arm, charIndex);
            }
            else if (type == "AMULET")
            {
                Amulet amu = Inventory.Amulets.FirstOrDefault(x => x.UniqueID == uniqueId);
                CharacterData.EquipItem(amu, charIndex);
            }

            Close();
        }

        private void RemoveEquipment(object sender, RoutedEventArgs e)
        {
            CharacterData.ClearEquipment(charIndex, type.ToUpper());
            Close();
        }
    }
}