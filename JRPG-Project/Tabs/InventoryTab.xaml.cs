using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using System;
using System.Linq;
using System.Threading.Tasks;
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
        public InventoryTab()
        {
            InitializeComponent();
            LoadListboxs();

            //Focus on the first button after UI is loaded
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Button btn = ButtonPanel.Children.OfType<Button>().FirstOrDefault();
                btn.Focus();
            }));

            //Load all items
            LoadAllitems();
        }

        private void LoadAllitems()
        {
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
                    return Brushes.SteelBlue;
                case "CURSED":
                    return Brushes.Orchid;
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
                            Txtlevel.Text = collectable.Level.ToString();
                            TxtValue.Text = collectable.Value.ToString();
                            TxtRarity.Text = collectable.Rarity;
                            TxtRarity.Foreground = collectable.Rarity == "COMMON" ? Brushes.White : GetBrush(collectable.Rarity);
                            ImgItem.Source = new BitmapImage(new Uri(@"/Resources/Assets/Collectables/" + collectable.ImageName, UriKind.Relative));
                            HideItemStats();
                            break;
                        }
                    case "LstWeapons":
                        {
                            //Get selected item as weapon
                            Weapon weapon = ItemData.ListWeapons.FirstOrDefault(i => i.ID == (string)listItem.Tag);
                            TxtItemName.Text = weapon.Name;
                            TxtItemDescription.Text = weapon.Description;
                            Txtlevel.Text = weapon.Level.ToString();
                            TxtValue.Text = weapon.Value.ToString();
                            TxtRarity.Text = weapon.Rarity;
                            TxtRarity.Foreground = weapon.Rarity == "COMMON" ? Brushes.White : GetBrush(weapon.Rarity);
                            ImgItem.Source = new BitmapImage(new Uri(@"/Resources/Assets/Weapons/" + weapon.ImageName, UriKind.Relative));
                            DisplayItemStats($"{weapon.Stats}");

                            TxtXp.Text = weapon.Stats.GetXP() + "xp";
                            TxtMaxXp.Text = LevelData.GetMaxXpAsString(weapon) + "xp";
                            ProgressbarXP.Maximum = Convert.ToInt32(TxtMaxXp.Text.Replace("xp", ""));
                            ProgressbarXP.Value = weapon.Stats.XP;
                            break;
                        }
                    case "LstArmours":
                        {
                            //Get selected item as armour
                            Armour armour = ItemData.ListArmours.FirstOrDefault(i => i.ID == (string)listItem.Tag);
                            TxtItemName.Text = armour.Name;
                            TxtItemDescription.Text = armour.Description;
                            Txtlevel.Text = armour.Level.ToString();
                            TxtValue.Text = armour.Value.ToString();
                            TxtRarity.Text = armour.Rarity;
                            TxtRarity.Foreground = armour.Rarity == "COMMON" ? Brushes.White : GetBrush(armour.Rarity);
                            ImgItem.Source = new BitmapImage(new Uri(@"/Resources/Assets/Armours/" + armour.ImageName, UriKind.Relative));
                            DisplayItemStats(armour.Stats.ToString());

                            TxtXp.Text = armour.Stats.GetXP() + "xp";
                            TxtMaxXp.Text = LevelData.GetMaxXpAsString(armour) + "xp";
                            ProgressbarXP.Maximum = Convert.ToInt32(TxtMaxXp.Text.Replace("xp", ""));
                            ProgressbarXP.Value = armour.Stats.XP;
                            break;
                        }
                    case "LstAmulets":
                        {
                            //Get selected item as amulet
                            Amulet amulet = ItemData.ListAmulets.FirstOrDefault(i => i.ID == (string)listItem.Tag);
                            TxtItemName.Text = amulet.Name;
                            TxtItemDescription.Text = amulet.Description;
                            Txtlevel.Text = amulet.Level.ToString();
                            TxtValue.Text = amulet.Value.ToString();
                            TxtRarity.Text = amulet.Rarity;
                            TxtRarity.Foreground = amulet.Rarity == "COMMON" ? Brushes.White : GetBrush(amulet.Rarity);
                            ImgItem.Source = new BitmapImage(new Uri(@"/Resources/Assets/Amulets/" + amulet.ImageName, UriKind.Relative));
                            DisplayItemStats(amulet.Stats.ToString());

                            TxtXp.Text = amulet.Stats.GetXP() + "xp";
                            TxtMaxXp.Text = LevelData.GetMaxXpAsString(amulet) + "xp";
                            ProgressbarXP.Maximum = Convert.ToInt32(TxtMaxXp.Text.Replace("xp", ""));
                            ProgressbarXP.Value = amulet.Stats.XP;
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Add keydown event to window
            //var window = Window.GetWindow(this);
            //window.KeyDown += Window_KeyDown;
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //Remove keydown event from window
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //Scraped
        }
    }
}
