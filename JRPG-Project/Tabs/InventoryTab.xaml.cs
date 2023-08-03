using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JRPG_Project.ClassLibrary.Universal
{
    /// <summary>
    /// Interaction logic for InventoryTab.xaml
    /// </summary>
    public partial class InventoryTab : UserControl
    {
        public InventoryTab()
        {
            InitializeComponent();
            PrepareGUI();

            //LoadItems(SortOptions[CboxSort.SelectedIndex]);
            //Load all items
            //LoadAllitems();
        }

        string SelectedItemUniqueID = "";

        Dictionary<int, string> SortOptions = new Dictionary<int, string>()
        {
            { 0, "Rarity Asc" },
            { 1, "Rarity Desc" },
            { 2, "Level Asc" },
            { 3, "Level Desc" },
            { 4, "Value Asc" },
            { 5, "Value Desc" }
        };

        private void PrepareGUI()
        {
            foreach (string option in SortOptions.Values)
            {
                CboxSort.Items.Add(option);
            }

            CboxSort.SelectedIndex = 0;
        }

        private void LoadAllitems()
        {
            //Display all collectable items
            foreach (ListBoxItem item in InventoryData.GetCollectablesAsListboxItem())
            {
                LstCollectables.Items.Add(item);
            }

            //Display All weapons
            foreach (ListBoxItem item in InventoryData.GetWeaponsAsListboxItem())
            {
                LstWeapons.Items.Add(item);
            }

            //Display all armours
            foreach (ListBoxItem item in InventoryData.GetArmoursAsListboxItem())
            {
                LstArmours.Items.Add(item);
            }

            //Display all amulets
            foreach (ListBoxItem item in InventoryData.GetAmuletsAsListboxItem())
            {
                LstAmulets.Items.Add(item);
            }
        }

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

        private void LoadItems(string sort)
        {
            //Create lists
            List<Collectable> collectables = new List<Collectable>();
            List<Weapon> weapons = new List<Weapon>();
            List<Armour> armours = new List<Armour>();
            List<Amulet> amulets = new List<Amulet>();

            //Sort Lists
            if (sort == SortOptions[0] || sort == SortOptions[1]) //#Rarity
            {
                //Define a list for sorting order
                List<string> sortingOrder = new List<string>() { "COMMON", "SPECIAL", "CURSED", "LEGENDARY" };

                //Reverse if descending
                if (sort == SortOptions[1])
                {
                    sortingOrder.Reverse();
                }

                //Define custom comparer for sorting
                var comparer = Comparer<string>.Create((x, y) =>
                {
                    int index1 = sortingOrder.IndexOf(x);
                    int index2 = sortingOrder.IndexOf(y);
                    return index1.CompareTo(index2);
                });

                //Apply sorting
                collectables = Inventory.Collectables.OrderBy(x => x.Rarity, comparer).ThenBy(x => x.Name).ToList();
                weapons = Inventory.Weapons.OrderBy(x => x.Rarity, comparer).ThenBy(x => x.Name).ToList();
                armours = Inventory.Armours.OrderBy(x => x.Rarity, comparer).ThenBy(x => x.Name).ToList();
                amulets = Inventory.Amulets.OrderBy(x => x.Rarity, comparer).ThenBy(x => x.Name).ToList();
            }
            else if (sort == SortOptions[2])
            {
                //#Sort on level
                collectables = Inventory.Collectables.OrderBy(x => x.Level).ThenBy(x => x.Name).ToList();
                weapons = Inventory.Weapons.OrderBy(x => x.Level).ThenBy(x => x.Name).ToList();
                armours = Inventory.Armours.OrderBy(x => x.Level).ThenBy(x => x.Name).ToList();
                amulets = Inventory.Amulets.OrderBy(x => x.Level).ThenBy(x => x.Name).ToList();
            }
            else if (sort == SortOptions[3])
            {
                //#sort on level descending
                collectables = Inventory.Collectables.OrderByDescending(x => x.Level).ThenBy(x => x.Name).ToList();
                weapons = Inventory.Weapons.OrderByDescending(x => x.Level).ThenBy(x => x.Name).ToList();
                armours = Inventory.Armours.OrderByDescending(x => x.Level).ThenBy(x => x.Name).ToList();
                amulets = Inventory.Amulets.OrderByDescending(x => x.Level).ThenBy(x => x.Name).ToList();
            }
            else if (sort == SortOptions[4])
            {
                //#Sort based on value
                collectables = Inventory.Collectables.OrderBy(x => x.Value).ThenBy(x => x.Name).ToList();
                weapons = Inventory.Weapons.OrderBy(x => x.Value).ThenBy(x => x.Name).ToList();
                armours = Inventory.Armours.OrderBy(x => x.Value).ThenBy(x => x.Name).ToList();
                amulets = Inventory.Amulets.OrderBy(x => x.Value).ThenBy(x => x.Name).ToList();
            }
            else if (sort == SortOptions[5])
            {
                //#Sort based on value descending
                collectables = Inventory.Collectables.OrderByDescending(x => x.Value).ThenBy(x => x.Name).ToList();
                weapons = Inventory.Weapons.OrderByDescending(x => x.Value).ThenBy(x => x.Name).ToList();
                armours = Inventory.Armours.OrderByDescending(x => x.Value).ThenBy(x => x.Name).ToList();
                amulets = Inventory.Amulets.OrderByDescending(x => x.Value).ThenBy(x => x.Name).ToList();
            }

            //Add to listboxes
            AddToList(collectables, weapons, armours, amulets);
        }

        private void AddToList(List<Collectable> collectables, List<Weapon> weapons, List<Armour> armours, List<Amulet> amulets)
        {
            //Clear lists
            LstCollectables.Items.Clear();
            LstWeapons.Items.Clear();
            LstArmours.Items.Clear();
            LstAmulets.Items.Clear();

            //Load all items into listbox'
            foreach (Collectable item in collectables)
            {
                ListBoxItem listItem = new ListBoxItem();
                listItem.Content = item.ToString();
                listItem.Tag = item.UniqueID;
                listItem.Foreground = GetBrush(item.Rarity);
                LstCollectables.Items.Add(listItem);
            }

            foreach (Weapon item in weapons)
            {
                ListBoxItem listItem = new ListBoxItem();
                //Display who equiped the item
                string owner = ItemData.GetOwnersName(item);
                if (owner == "/")
                {
                    listItem.Content = item.ToString();
                }
                else
                {
                    listItem.Content = item.ToString() + " --> " + owner;
                }
                listItem.Tag = item.UniqueID;
                listItem.Foreground = GetBrush(item.Rarity);
                LstWeapons.Items.Add(listItem);
            }

            foreach (Armour item in armours)
            {
                ListBoxItem listItem = new ListBoxItem();
                //Display who equiped the item
                string owner = ItemData.GetOwnersName(item);
                if (owner == "/")
                {
                    listItem.Content = item.ToString();
                }
                else
                {
                    listItem.Content = item.ToString() + " --> " + owner;
                }
                listItem.Tag = item.UniqueID;
                listItem.Foreground = GetBrush(item.Rarity);
                LstArmours.Items.Add(listItem);
            }

            foreach (Amulet item in amulets)
            {
                ListBoxItem listItem = new ListBoxItem();
                //Display who equiped the item
                string owner = ItemData.GetOwnersName(item);
                if (owner == "/")
                {
                    listItem.Content = item.ToString();
                }
                else
                {
                    listItem.Content = item.ToString() + " --> " + owner;
                }
                listItem.Tag = item.UniqueID;
                listItem.Foreground = GetBrush(item.Rarity);
                LstAmulets.Items.Add(listItem);
            }
        }

        private void DisplayItemDetails(object sender, SelectionChangedEventArgs e)
        {
            //get sender
            ListBox listbox = (ListBox)sender;

            //If selected index is -1, return
            if (listbox.SelectedIndex == -1)
            {
                return;
            }

            //Get selected item
            ListBoxItem listItem = (ListBoxItem)listbox.SelectedItem;

            //Save unique id
            SelectedItemUniqueID = (string)listItem.Tag;

            //Get selected item & display
            try
            {
                switch (listbox.Name)
                {
                    case "LstCollectables":
                        {
                            //Get selected item as collectable
                            Collectable collectable = Inventory.Collectables.FirstOrDefault(i => i.UniqueID == (string)listItem.Tag);
                            TxtItemName.Text = collectable.Name;
                            TxtItemDescription.Text = collectable.Description;
                            Txtlevel.Text = collectable.Level.ToString();
                            TxtValue.Text = collectable.Value.ToString();
                            TxtRarity.Text = collectable.Rarity;
                            TxtRarity.Foreground = collectable.Rarity == "COMMON" ? Brushes.White : GetBrush(collectable.Rarity);
                            ImgItem.Source = collectable.ItemImage.Source;
                            HideItemStats();
                            break;
                        }
                    case "LstWeapons":
                        {
                            //Get selected item as weapon
                            Weapon weapon = Inventory.Weapons.FirstOrDefault(i => i.UniqueID == (string)listItem.Tag);
                            TxtItemName.Text = weapon.Name;
                            TxtItemDescription.Text = weapon.Description;
                            Txtlevel.Text = weapon.Level.ToString();
                            TxtValue.Text = weapon.Value.ToString();
                            TxtRarity.Text = weapon.Rarity;
                            TxtRarity.Foreground = weapon.Rarity == "COMMON" ? Brushes.White : GetBrush(weapon.Rarity);
                            ImgItem.Source = weapon.ItemImage.Source;
                            DisplayItemStats($"{weapon.Stats}");

                            if (weapon.Level != LevelData.WeaponXPTable.Keys.LastOrDefault())
                            {
                                TxtXp.Text = weapon.Stats.GetXP() + "xp";
                                TxtMaxXp.Text = LevelData.GetMaxXpAsString(weapon) + "xp";
                                ProgressbarXP.Maximum = Convert.ToInt32(TxtMaxXp.Text.Replace("xp", ""));
                                ProgressbarXP.Value = weapon.Stats.XP;
                            }
                            else
                            {
                                TxtXp.Text = LevelData.WeaponXPTable.Values.LastOrDefault().Item1.ToString() + "xp";
                                TxtMaxXp.Text = "MAX";
                                ProgressbarXP.Maximum = 100;
                                ProgressbarXP.Value = 100;
                            }

                            break;
                        }
                    case "LstArmours":
                        {
                            //Get selected item as armour
                            Armour armour = Inventory.Armours.FirstOrDefault(i => i.UniqueID == (string)listItem.Tag);
                            TxtItemName.Text = armour.Name;
                            TxtItemDescription.Text = armour.Description;
                            Txtlevel.Text = armour.Level.ToString();
                            TxtValue.Text = armour.Value.ToString();
                            TxtRarity.Text = armour.Rarity;
                            TxtRarity.Foreground = armour.Rarity == "COMMON" ? Brushes.White : GetBrush(armour.Rarity);
                            ImgItem.Source = armour.ItemImage.Source;
                            DisplayItemStats(armour.Stats.ToString());

                            if (armour.Level != LevelData.ArmourXPTable.Keys.LastOrDefault())
                            {
                                TxtXp.Text = armour.Stats.GetXP() + "xp";
                                TxtMaxXp.Text = LevelData.GetMaxXpAsString(armour) + "xp";
                                ProgressbarXP.Maximum = Convert.ToInt32(TxtMaxXp.Text.Replace("xp", ""));
                                ProgressbarXP.Value = armour.Stats.XP;
                            }
                            else
                            {
                                TxtXp.Text = LevelData.ArmourXPTable.Values.LastOrDefault().Item1.ToString() + "xp";
                                TxtMaxXp.Text = "MAX";
                                ProgressbarXP.Maximum = 100;
                                ProgressbarXP.Value = 100;
                            }

                            break;
                        }
                    case "LstAmulets":
                        {
                            //Get selected item as amulet
                            Amulet amulet = Inventory.Amulets.FirstOrDefault(i => i.UniqueID == (string)listItem.Tag);
                            TxtItemName.Text = amulet.Name;
                            TxtItemDescription.Text = amulet.Description;
                            Txtlevel.Text = amulet.Level.ToString();
                            TxtValue.Text = amulet.Value.ToString();
                            TxtRarity.Text = amulet.Rarity;
                            TxtRarity.Foreground = amulet.Rarity == "COMMON" ? Brushes.White : GetBrush(amulet.Rarity);
                            ImgItem.Source = amulet.ItemImage.Source;
                            DisplayItemStats(amulet.Stats.ToString());

                            if (amulet.Level != LevelData.AmuletXPTable.Keys.LastOrDefault())
                            {
                                TxtXp.Text = amulet.Stats.GetXP() + "xp";
                                TxtMaxXp.Text = LevelData.GetMaxXpAsString(amulet) + "xp";
                                ProgressbarXP.Maximum = Convert.ToInt32(TxtMaxXp.Text.Replace("xp", ""));
                                ProgressbarXP.Value = amulet.Stats.XP;
                            }
                            else
                            {
                                TxtXp.Text = LevelData.AmuletXPTable.Values.LastOrDefault().Item1.ToString() + "xp";
                                TxtMaxXp.Text = "MAX";
                                ProgressbarXP.Maximum = 100;
                                ProgressbarXP.Value = 100;
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //Unselect item in other listboxes
            foreach (TabItem item in tabControl.Items)
            {
                if (item.Content is ListBox box && box.Name != listbox.Name)
                {
                    box.SelectedIndex = -1;
                }
            }
        }

        private void DisplayItemStats(string stats)
        {
            GridStats.Visibility = Visibility.Visible;

            //Expects to receive the stats in this order: HP;DMG;DEF;SPD;STA;STR;CRC;CRD
            string[] statParts = stats.Split(';');

            TxtHp.Text = statParts[0];
            TxtDmg.Text = statParts[1];
            TxtDef.Text = statParts[2];
            TxtSpd.Text = statParts[3];
            TxtCrc.Text = statParts[4];
            TxtCrd.Text = statParts[5];
            TxtSta.Text = statParts[6];
            TxtStr.Text = statParts[7];
        }

        private void HideItemStats()
        {
            GridStats.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Interaction.OpenTab("MainTab");
        }

        private void CboxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadItems(SortOptions[CboxSort.SelectedIndex]);
        }
    }
}
