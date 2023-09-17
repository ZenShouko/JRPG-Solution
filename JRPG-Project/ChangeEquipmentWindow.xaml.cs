using JRPG_Project.ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using JRPG_Project.ClassLibrary.Universal;
using System.Collections.Generic;
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
            //List to use
            List<BaseItem> items = new List<BaseItem>();

            //Fill list based on type
            if (type == "WEAPON")
                foreach (Weapon wpn in Inventory.Weapons)
                    items.Add(wpn);
            else if (type == "ARMOUR")
                foreach (Armour arm in Inventory.Armours)
                    items.Add(arm);
            else if (type == "AMULET")
                foreach (Amulet amu in Inventory.Amulets)
                    items.Add(amu);

            //Fill listbox based on items
            foreach (BaseItem item in items)
            {
                ListBoxItem listItem = new ListBoxItem();
                listItem.Content = item.Name;
                listItem.Tag = item.UniqueID;
                listItem.Foreground = GetBrush(item.Rarity);
                listItem.MouseDoubleClick += (sender, e) => ListboxItemClick(sender, e, type);

                //If item is equipped, add a star and the owner
                string owner = ItemData.GetOwnersName(item);
                if (owner == "/")
                    listItem.Content = $"[{item.Level}] " + item.Name;
                else
                    listItem.Content = $"[{item.Level}] " + "★ " + item.Name + $" x{owner}";

                ListEquipments.Items.Add(listItem);
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
            SoundManager.PlaySound("click-short.wav");
            Close();
        }

        private void EquipItem(object sender, RoutedEventArgs e)
        {
            //If nothing selected, close window
            if (ListEquipments.SelectedIndex == -1)
            {
                SoundManager.PlaySound("click-short.wav");
                Close();
                return;
            }

            //Play sound
            SoundManager.PlaySound("equip.wav");

            //Get UniqueID of selected item
            ListBoxItem item = (ListBoxItem)ListEquipments.SelectedItem;
            string uniqueId = (string)item.Tag;
            BaseItem thisItem = null;

            if (type == "WEAPON")
                thisItem = Inventory.Weapons.FirstOrDefault(x => x.UniqueID == uniqueId);
            else if (type == "ARMOUR")
                thisItem = Inventory.Armours.FirstOrDefault(x => x.UniqueID == uniqueId);
            else if (type == "AMULET")
                thisItem = Inventory.Amulets.FirstOrDefault(x => x.UniqueID == uniqueId);

            //Equip item
            CharacterData.EquipItem(thisItem, charIndex);

            Close();
        }

        private void RemoveEquipment(object sender, RoutedEventArgs e)
        {
            //playsound
            SoundManager.PlaySound("unequip.wav");

            CharacterData.ClearEquipment(charIndex, type.ToUpper());
            Close();
        }
    }
}