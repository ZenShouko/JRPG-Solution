using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        bool AddedKeydownEvent = false;
        public InventoryTab()
        {
            InitializeComponent();
            PrepareGUI();
            DisplayBlank();


            //[?] Items get loaded when sort combobox' selection changes. Hence why we don't need to call LoadItems() here.

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

        bool BulkExtract = false;

        private void PrepareGUI()
        {
            foreach (string option in SortOptions.Values)
            {
                CboxSort.Items.Add(option);
            }

            CboxSort.SelectedIndex = 0;

            //If player comes from a platform, disable extra functionality
            if (Stages.CurrentStage != null)
            {
                BtnExtractEssence.IsEnabled = false;
                BtnExtractEssence.Background = Brushes.Gray;
                BtnBulkExtract.IsEnabled = false;
                BtnBulkExtract.Background = Brushes.Gray;
                BtnUpgrade.IsEnabled = false;
                BtnUpgrade.Background = Brushes.Gray;
            }
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

        private void LoadMaterials()
        {
            //@Materials do not follow regular sorting rules
            //Clear list
            LstMaterials.Items.Clear();

            List<Material> materials = new List<Material>();

            //Add all materials to list
            foreach (var item in Inventory.Materials)
            {
                Material mat = ItemData.ListMaterials.FirstOrDefault(x => x.ID == item.Key);

                //Skip if player has no materials of this type
                if (Inventory.Materials[item.Key] == 0)
                    continue;

                materials.Add(mat);
            }

            //Sort list based on xp descending
            materials = materials.OrderByDescending(x => x.Stats.XP).ToList();

            //Display all materials
            foreach (Material mat in materials)
            {
                ListBoxItem boxItem = new ListBoxItem();
                boxItem.Content = $"[x{Inventory.Materials[mat.ID]}] {mat.Name}";
                boxItem.Tag = mat.ID;
                LstMaterials.Items.Add(boxItem);
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

            //Load materials (Materials do not follow sorting options)
            LoadMaterials();
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
                    listItem.Content = "★ " + item.ToString() + $" x{owner}";
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
                    listItem.Content = "★ " + item.ToString() + $" x{owner}";
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
                    listItem.Content = "★ " + item.ToString() + $" x{owner}";
                }
                listItem.Tag = item.UniqueID;
                listItem.Foreground = GetBrush(item.Rarity);
                LstAmulets.Items.Add(listItem);
            }
        }

        private void Listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectItem(sender);
        }

        private void SelectItem(object sender)
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
                    case "LstMaterials":
                        {
                            //Get selected item as collectable
                            Material mat = ItemData.ListMaterials.FirstOrDefault(i => i.ID == (string)listItem.Tag);
                            TxtItemName.Text = mat.Name;
                            TxtItemDescription.Text = mat.Description;
                            Txtlevel.Text = mat.Level.ToString();
                            TxtValue.Text = mat.Value.ToString();
                            TxtRarity.Text = mat.Rarity;
                            TxtRarity.Foreground = mat.Rarity == "COMMON" ? Brushes.White : GetBrush(mat.Rarity);
                            ImgItem.Source = mat.ItemImage.Source;
                            DisplayItemStats(mat.Stats.ToString());

                            TxtXp.Text = mat.Stats.GetXP() + "xp";
                            TxtMaxXp.Text = "MAX";
                            ProgressbarXP.Maximum = 250;
                            ProgressbarXP.Value = mat.Stats.XP;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //Unselect item in other listboxes
            if (!BulkExtract)
            {
                foreach (TabItem item in tabControl.Items)
                {
                    if (item.Content is ListBox box && box.Name != listbox.Name)
                    {
                        box.SelectedIndex = -1;
                    }
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

        private void DisplayBlank()
        {
            //Count mat amount
            int matCount = 0;
            foreach (var count in Inventory.Materials.Values)
            {
                matCount += count;
            }

            //Get selected item as amulet //What did I mean by this??
            TxtItemName.Text = "My Inventory";
            TxtItemDescription.Text = $"You've got: {Inventory.Weapons.Count} weapons, {Inventory.Armours.Count} armours, {Inventory.Amulets.Count} amulets, {Inventory.Collectables.Count} collectables and {matCount} materials.";
            Txtlevel.Text = "1";
            TxtValue.Text = "0";
            TxtRarity.Text = "COMMON";
            TxtRarity.Foreground = Brushes.White;
            ImgItem.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Assets/GUI/alligator.png", UriKind.RelativeOrAbsolute));
            DisplayItemStats("0;0;0;0;0;0;0;0");

            TxtXp.Text = (Inventory.Weapons.Count + Inventory.Armours.Count + Inventory.Amulets.Count).ToString();
            TxtMaxXp.Text = Inventory.Capacity.ToString();
            ProgressbarXP.Maximum = Convert.ToInt32(TxtMaxXp.Text);
            ProgressbarXP.Value = Convert.ToInt16(TxtXp.Text);
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            //Unsubscribe from keydown event
            if (Window.GetWindow(this) != null && AddedKeydownEvent)
            {
                Window.GetWindow(this).KeyDown -= DispatchTab_KeyDown;
                AddedKeydownEvent = false;
            }

            SoundManager.PlaySound("click-short.wav");
            if (Stages.CurrentStage == null)
                Interaction.OpenTab("MainTab");
            else
                Interaction.ReturnToPreviousTab();
        }

        private void CboxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadItems(SortOptions[CboxSort.SelectedIndex]);
        }

        private void ExtractEssence_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.PlaySound("click-medium.wav");
            if (!BulkExtract)
            {
                //Get selected item as base item
                BaseItem item;

                if (LstCollectables.SelectedIndex != -1)
                {
                    ListBoxItem listItem = (ListBoxItem)LstCollectables.SelectedItem;
                    item = Inventory.Collectables.FirstOrDefault(i => i.UniqueID == (string)listItem.Tag);
                }
                else if (LstWeapons.SelectedIndex != -1)
                {
                    ListBoxItem listItem = (ListBoxItem)LstWeapons.SelectedItem;
                    item = Inventory.Weapons.FirstOrDefault(i => i.UniqueID == (string)listItem.Tag);
                }
                else if (LstArmours.SelectedIndex != -1)
                {
                    ListBoxItem listItem = (ListBoxItem)LstArmours.SelectedItem;
                    item = Inventory.Armours.FirstOrDefault(i => i.UniqueID == (string)listItem.Tag);
                }
                else if (LstAmulets.SelectedIndex != -1)
                {
                    ListBoxItem listItem = (ListBoxItem)LstAmulets.SelectedItem;
                    item = Inventory.Amulets.FirstOrDefault(i => i.UniqueID == (string)listItem.Tag);
                }
                else
                {
                    return;
                }

                //Open extract essence window
                EssenceExtractionWindow window = new EssenceExtractionWindow(item);
                window.ShowDialog();

                //Refresh inventory
                LoadItems(SortOptions[CboxSort.SelectedIndex]);
                DisplayBlank();
            }
            else
            {
                //Get the selected items in a list
                List<BaseItem> itemList = new List<BaseItem>();

                //Itterate through all listboxes and add selected items to list
                foreach (TabItem item in tabControl.Items)
                {
                    if (item.Content is ListBox box)
                    {
                        if (box.SelectedIndex != -1)
                        {
                            foreach (ListBoxItem boxItem in box.SelectedItems)
                            {
                                BaseItem obj = null;

                                obj = boxItem.Tag.ToString().Contains("collectable") ?
                                    Inventory.Collectables.FirstOrDefault(i => i.UniqueID == (string)boxItem.Tag) : obj;
                                obj = boxItem.Tag.ToString().Contains("weapon") ?
                                    Inventory.Weapons.FirstOrDefault(i => i.UniqueID == (string)boxItem.Tag) : obj;
                                obj = boxItem.Tag.ToString().Contains("armour") ?
                                    Inventory.Armours.FirstOrDefault(i => i.UniqueID == (string)boxItem.Tag) : obj;
                                obj = boxItem.Tag.ToString().Contains("amulet") ?
                                    Inventory.Amulets.FirstOrDefault(i => i.UniqueID == (string)boxItem.Tag) : obj;

                                itemList.Add(obj);
                            }
                        }
                    }
                }

                //If no items are selected, return
                if (itemList.Count == 0)
                {
                    return;
                }

                //Open extract essence window
                EssenceExtractionWindow window = new EssenceExtractionWindow(itemList);
                window.ShowDialog();

                //Refresh inventory
                LoadItems(SortOptions[CboxSort.SelectedIndex]);
                DisplayBlank();

                //Reset bulk extract
                ButtonBulkExtract_Click(sender, e);
            }
        }

        private void ButtonBulkExtract_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.PlaySound("click-short.wav");
            BulkExtract = !BulkExtract;
            BtnBulkExtract.Content = BulkExtract ? "Cancel" : "Bulk Extract";
            BtnBulkExtract.Foreground = BulkExtract ? Brushes.Crimson : Brushes.Black;
            UnselectedEverything();
            SwitchMultipleSelection(BulkExtract);
            DisplayBlank();
            CboxSort.IsEnabled = !BulkExtract;

            if (BulkExtract)
                TxtItemDescription.Text += "(Tip: double click headers to quick bulk select based on rarity.)";
        }

        private void UnselectedEverything()
        {
            foreach (TabItem item in tabControl.Items)
            {
                if (item.Content is ListBox box)
                {
                    box.SelectedIndex = -1;
                }
            }
        }

        private void SwitchMultipleSelection(bool on)
        {
            LstCollectables.SelectionMode = on ? SelectionMode.Multiple : SelectionMode.Single;
            LstWeapons.SelectionMode = on ? SelectionMode.Multiple : SelectionMode.Single;
            LstArmours.SelectionMode = on ? SelectionMode.Multiple : SelectionMode.Single;
            LstAmulets.SelectionMode = on ? SelectionMode.Multiple : SelectionMode.Single;
            LstMaterials.IsEnabled = !on;
        }

        private void BtnUpgrade_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.PlaySound("click-medium.wav");
            //Get selected item as base item
            BaseItem item;

            if (LstWeapons.SelectedIndex != -1)
            {
                ListBoxItem listItem = (ListBoxItem)LstWeapons.SelectedItem;
                item = Inventory.Weapons.FirstOrDefault(i => i.UniqueID == (string)listItem.Tag);
            }
            else if (LstArmours.SelectedIndex != -1)
            {
                ListBoxItem listItem = (ListBoxItem)LstArmours.SelectedItem;
                item = Inventory.Armours.FirstOrDefault(i => i.UniqueID == (string)listItem.Tag);
            }
            else if (LstAmulets.SelectedIndex != -1)
            {
                ListBoxItem listItem = (ListBoxItem)LstAmulets.SelectedItem;
                item = Inventory.Amulets.FirstOrDefault(i => i.UniqueID == (string)listItem.Tag);
            }
            else
            {
                return;
            }

            //Open upgrade tab
            Interaction.OpenUpgradeTab(item);
        }


        Dictionary<string, (bool, bool, bool, bool, bool)> BulkPreference = new Dictionary<string, (bool, bool, bool, bool, bool)>()
        {
            {"LstCollectables", (false, false, false, false, false) },
            {"LstWeapons", (false, false, false, false, false) },
            {"LstArmours", (false, false, false, false, false) },
            {"LstAmulets", (false, false, false, false, false) }
        };
        private void QuickItemSelector(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Return if bulk extract is not enabled
            if (!BulkExtract)
                return;

            //Vars
            TabItem currentTab = (TabItem)sender;

            //Get listbox from tab
            ListBox currentListbox = (ListBox)currentTab.Content;

            bool common = BulkPreference[currentListbox.Name].Item1;
            bool special = BulkPreference[currentListbox.Name].Item2;
            bool cursed = BulkPreference[currentListbox.Name].Item3;
            bool legendary = BulkPreference[currentListbox.Name].Item4;
            bool enhancedItems = BulkPreference[currentListbox.Name].Item5;

            //Open rarity selector window
            RaritySelectorWindow window = new RaritySelectorWindow(BulkPreference[currentListbox.Name].Item1, BulkPreference[currentListbox.Name].Item2,
                BulkPreference[currentListbox.Name].Item3, BulkPreference[currentListbox.Name].Item4, BulkPreference[currentListbox.Name].Item5);
            if (window.ShowDialog() == true)
            {
                common = window.Common;
                special = window.Special;
                cursed = window.Cursed;
                legendary = window.Legendary;
                enhancedItems = window.EnhancedItems;

                //Save preferences
                BulkPreference[currentListbox.Name] = (common, special, cursed, legendary, enhancedItems);
            }

            //Select items
            foreach (ListBoxItem listItem in currentListbox.Items)
            {
                //Get item
                BaseItem obj;

                if (currentListbox.Name.Contains("Collectables"))
                    obj = Inventory.Collectables.FirstOrDefault(i => i.UniqueID == (string)listItem.Tag);
                else if (currentListbox.Name.Contains("Weapons"))
                    obj = Inventory.Weapons.FirstOrDefault(i => i.UniqueID == (string)listItem.Tag);
                else if (currentListbox.Name.Contains("Armours"))
                    obj = Inventory.Armours.FirstOrDefault(i => i.UniqueID == (string)listItem.Tag);
                else if (currentListbox.Name.Contains("Amulets"))
                    obj = Inventory.Amulets.FirstOrDefault(i => i.UniqueID == (string)listItem.Tag);
                else
                    return;

                //Unselect if rarities don't match
                if (!common && obj.Rarity == "COMMON")
                {
                    listItem.IsSelected = false;
                    continue;
                }
                else if (!special && obj.Rarity == "SPECIAL")
                {
                    listItem.IsSelected = false;
                    continue;
                }
                else if (!cursed && obj.Rarity == "CURSED")
                {
                    listItem.IsSelected = false;
                    continue;
                }
                else if (!legendary && obj.Rarity == "LEGENDARY")
                {
                    listItem.IsSelected = false;
                    continue;
                }

                //Unselect if enhanced items don't match
                if (!enhancedItems && IsItemEnhanced(obj))
                {
                    listItem.IsSelected = false;
                    continue;
                }

                //Select item
                listItem.IsSelected = true;
            }
        }

        private bool IsItemEnhanced(BaseItem item)
        {
            //Collectables are always false
            if (item is Collectable)
                return false;

            //Get stats holder
            IStatsHolder statObj = item as IStatsHolder;

            //Check if item is above level 1 and has more than 1 xp.
            if (item.Level > 1 && statObj.Stats.XP > 0)
                return true;
            else
                return false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Add keydown event
            if (!AddedKeydownEvent)
            {
                Window.GetWindow(this).KeyDown += DispatchTab_KeyDown;
                AddedKeydownEvent = true;
            }

            //#Did we come back from upgrade tab?
            if (string.IsNullOrEmpty(SelectedItemUniqueID))
                return;

            //Refresh listboxes
            LoadItems(SortOptions[CboxSort.SelectedIndex]);
            LoadMaterials();

            //Select item
            TabItem targetTab = new TabItem();
            if (SelectedItemUniqueID.Contains("weapon"))
            {
                //open weapons tab
                targetTab = tabControl.Items.Cast<TabItem>().FirstOrDefault(i => i.Name == "Weapons");
                tabControl.SelectedItem = targetTab;
            }
            else if (SelectedItemUniqueID.Contains("armour"))
            {
                targetTab = tabControl.Items.Cast<TabItem>().FirstOrDefault(i => i.Name == "Armours");
                tabControl.SelectedItem = targetTab;
            }
            else if (SelectedItemUniqueID.Contains("amulet"))
            {
                targetTab = tabControl.Items.Cast<TabItem>().FirstOrDefault(i => i.Name == "Amulets");
                tabControl.SelectedItem = targetTab;
            }
            else
            {
                return;
            }

            //Get listbox from target tab
            ListBox targetListbox = (ListBox)targetTab.Content;

            //Select item, listboxitem.tag == item.uniqueid
            foreach (ListBoxItem item in targetListbox.Items)
            {
                if ((string)item.Tag == SelectedItemUniqueID)
                {
                    item.IsSelected = true;
                    break;
                }
            }

            //Display item details
            SelectItem(targetListbox);
        }

        private void DispatchTab_KeyDown(object sender, KeyEventArgs e)
        {
            //Return
            ReturnButton_Click(sender, e);
        }
    }
}
