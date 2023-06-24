using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
                    return Brushes.Purple;
                case "LEGENDARY":
                    return Brushes.DarkOrchid;
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
                            ImgItem.Source = new BitmapImage(new Uri(@"/Resources/Assets/Collectables/" + collectable.ImageName + ".png", UriKind.Relative));
                            break;
                        }
                    case "LstWeapons":
                        {
                            Weapon weapon = Player.Inventory.Weapons[listbox.SelectedIndex];
                            TxtItemName.Text = weapon.Name;
                            TxtItemDescription.Text = weapon.Description;
                            TxtRarity.Text = weapon.Rarity;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //Change rarity text color based on rarity
            switch (TxtRarity.Text.ToUpper())
            {
                case "COMMON":
                    TxtRarity.Foreground = Brushes.WhiteSmoke;
                    break;
                case "SPECIAL":
                    TxtRarity.Foreground = Brushes.LightSkyBlue;
                    break;
                case "CURSED":
                    TxtRarity.Foreground = Brushes.MediumPurple;
                    break;
                case "LEGENDARY":
                    TxtRarity.Foreground = Brushes.Orange;
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Interaction.OpenTab("MainTab");
        }
    }
}
