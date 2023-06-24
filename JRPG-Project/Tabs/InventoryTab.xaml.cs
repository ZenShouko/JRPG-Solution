using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            LoadListboxs();

            //Remove later
            //Display all collectable items
            foreach (Collectable collectable in ItemData.ListCollectables)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = collectable.ToString();
                item.Tag = collectable.ID;
                item.Foreground = GetBrush(collectable.Rarity);

                LstCollectables.Items.Add(item);
            }

            //Display All weapons
            foreach (Weapon weapon in ItemData.ListWeapons)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = weapon.ToString();
                item.Tag = weapon.ID;
                item.Foreground = GetBrush(weapon.Rarity);

                LstWeapons.Items.Add(item);
            }

            //Display all armours
            foreach (Armour armour in ItemData.ListArmours)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = armour.ToString();
                item.Tag = armour.ID;
                item.Foreground = GetBrush(armour.Rarity);

                LstArmours.Items.Add(item);
            }

            //Display all amulets
            foreach (Amulet amulet in ItemData.ListAmulets)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = amulet.ToString();
                item.Tag = amulet.ID;
                item.Foreground = GetBrush(amulet.Rarity);

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
                    return Brushes.LightSkyBlue;
                case "CURSED":
                    return Brushes.DarkOrchid;
                case "LEGENDARY":
                    return Brushes.Goldenrod;
                default:
                    return Brushes.WhiteSmoke;
            }
        }

        private void LoadListboxs()
        {
            //Load all items into listbox
            foreach (Collectable item in Player.Inventory.Collectables)
            {
                LstCollectables.Items.Add(item);
            }

            foreach (Weapon item in Player.Inventory.Weapons)
            {
                LstWeapons.Items.Add(item.ToString());
            }

            foreach (Armour item in Player.Inventory.Armours)
            {
                LstArmours.Items.Add(item.ToString());
            }

            foreach (Amulet item in Player.Inventory.Amulets)
            {
                LstAmulets.Items.Add(item.ToString());
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

            //Get selected item & display
            try
            {
                switch (listbox.Name)
                {
                    case "LstCollectables":
                        {
                            //Get selected item as collectable
                            Collectable collectable = ItemData.ListCollectables.FirstOrDefault(i => i.ID == (string)listItem.Tag);
                            TxtItemName.Text = collectable.Name;
                            TxtItemDescription.Text = collectable.Description;
                            TxtValue.Text = collectable.Value.ToString();
                            TxtRarity.Text = collectable.Rarity;
                            TxtRarity.Foreground = collectable.Rarity == "COMMON" ? Brushes.White : GetBrush(collectable.Rarity);
                            ImgItem.Source = new BitmapImage(new Uri(@"/Resources/Assets/Collectables/" + collectable.ImageName, UriKind.Relative));
                            break;
                        }
                    case "LstWeapons":
                        {
                            //Get selected item as weapon
                            Weapon weapon = ItemData.ListWeapons.FirstOrDefault(i => i.ID == (string)listItem.Tag);
                            TxtItemName.Text = weapon.Name;
                            TxtItemDescription.Text = weapon.Description;
                            TxtValue.Text = weapon.Value.ToString();
                            TxtRarity.Text = weapon.Rarity;
                            TxtRarity.Foreground = weapon.Rarity == "COMMON" ? Brushes.White : GetBrush(weapon.Rarity);
                            ImgItem.Source = new BitmapImage(new Uri(@"/Resources/Assets/Weapons/" + weapon.ImageName, UriKind.Relative));
                            break;
                        }
                    case "LstArmours":
                        {
                            //Get selected item as armour
                            Armour armour = ItemData.ListArmours.FirstOrDefault(i => i.ID == (string)listItem.Tag);
                            TxtItemName.Text = armour.Name;
                            TxtItemDescription.Text = armour.Description;
                            TxtValue.Text = armour.Value.ToString();
                            TxtRarity.Text = armour.Rarity;
                            TxtRarity.Foreground = armour.Rarity == "COMMON" ? Brushes.White : GetBrush(armour.Rarity);
                            ImgItem.Source = new BitmapImage(new Uri(@"/Resources/Assets/Armours/" + armour.ImageName, UriKind.Relative));
                            break;
                        }
                    case "LstAmulets":
                        {
                            //Get selected item as amulet
                            Amulet amulet = ItemData.ListAmulets.FirstOrDefault(i => i.ID == (string)listItem.Tag);
                            TxtItemName.Text = amulet.Name;
                            TxtItemDescription.Text = amulet.Description;
                            TxtValue.Text = amulet.Value.ToString();
                            TxtRarity.Text = amulet.Rarity;
                            TxtRarity.Foreground = amulet.Rarity == "COMMON" ? Brushes.White : GetBrush(amulet.Rarity);
                            ImgItem.Source = new BitmapImage(new Uri(@"/Resources/Assets/Amulets/" + amulet.ImageName, UriKind.Relative));
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Interaction.OpenTab("MainTab");
        }
    }
}
