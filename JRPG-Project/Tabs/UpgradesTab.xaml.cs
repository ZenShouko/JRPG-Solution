using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using JRPG_Project.ClassLibrary.Universal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JRPG_Project.Tabs
{
    /// <summary>
    /// Interaction logic for UpgradesTab.xaml
    /// </summary>
    public partial class UpgradesTab : UserControl
    {
        public UpgradesTab(BaseItem item)
        {
            //TEST
            Inventory.Materials["M2"] += 100;
            Inventory.Materials["M7"] += 20;
            InitializeComponent();
            Item = item;
            PrepareGuiForUpgrade();
        }

        /// <summary>
        /// Item in question, used for reference.
        /// </summary>
        BaseItem Item { get; set; }
        IStatsHolder ItemStatObj { get; set; }
        /// <summary>
        /// Item after upgrades.
        /// </summary>
        BaseItem ItemPreview { get; set; }
        IStatsHolder ItemPreviewStatObj { get; set; }

        Dictionary<int, int> XpPerLevel = new Dictionary<int, int>(); //Level, XP
        Dictionary<string, Border> ScrollContainers = new Dictionary<string, Border>();
        Dictionary<string, TextBlock> ScrollTextblocks = new Dictionary<string, TextBlock>();

        List<TextBlock> AniListTextblocks = new List<TextBlock>();


        #region Prep
        private void PrepareGuiForUpgrade()
        {
            //Add all animated elements to a list
            AddAllElementsToAniList();

            //Initialize ItemPreview
            if (Item is Weapon)
                ItemPreview = new Weapon();
            else if (Item is Armour)
                ItemPreview = new Armour();
            else if (Item is Amulet)
                ItemPreview = new Amulet();

            //Copy item to upgrade preview
            ItemPreview.CopyFrom(Item);

            //Initialize Stat objects
            ItemStatObj = Item as IStatsHolder;
            ItemPreviewStatObj = ItemPreview as IStatsHolder;

            //Fill dictionary with xp required per level
            if (Item is Weapon)
                for (int i = LevelData.WeaponXPTable.Keys.FirstOrDefault(); i <= LevelData.WeaponXPTable.Keys.LastOrDefault(); i++)
                    XpPerLevel.Add(i, LevelData.WeaponXPTable[i].Item1);
            else if (Item is Armour)
                for (int i = LevelData.ArmourXPTable.Keys.FirstOrDefault(); i <= LevelData.ArmourXPTable.Keys.LastOrDefault(); i++)
                    XpPerLevel.Add(i, LevelData.ArmourXPTable[i].Item1);
            else if (Item is Amulet)
                for (int i = LevelData.AmuletXPTable.Keys.FirstOrDefault(); i <= LevelData.AmuletXPTable.Keys.LastOrDefault(); i++)
                    XpPerLevel.Add(i, LevelData.AmuletXPTable[i].Item1);

            //Set item image
            ItemImage.Source = Item.ItemImage.Source;

            //Item name and rarity
            TxtName.Text = Item.Name;
            TxtRarity.Text = Item.Rarity.ToString();

            //Display Stats
            RefreshStats();

            //Add scrolls to MaterialsPanel
            LoadScrolls();

            //Check if upgrade is possible
            IsUpgradePossible();

            //Display essence mat
            TxtOrbs.Text = Inventory.Materials["M2"].ToString();
        }

        private void LoadScrolls()
        {
            //Itterate through every distinct material
            foreach (var item in Inventory.Materials)
            {
                if (item.Value != 0 && item.Key != "M1" && item.Key != "M2")
                {
                    ClassLibrary.Items.Material scroll = ItemData.ListMaterials.Find(x => x.ID == item.Key);
                    AddScrollsToContainer(scroll);
                }
            }

            //If empty, display message
            HandleEmptyScrollContainer();
        }

        private void AddScrollsToContainer(ClassLibrary.Items.Material scroll)
        {
            //#Border element
            Border border = new Border();
            border.SetValue(DockPanel.DockProperty, Dock.Top);
            border.BorderThickness = new Thickness(1, 1, 2, 3);
            border.CornerRadius = new CornerRadius(2, 4, 4, 2);
            border.BorderBrush = Brushes.FloralWhite;
            border.Padding = new Thickness(8, 6, 8, 6);
            border.Margin = new Thickness(0, 0, 0, 8);
            DockPanel.SetDock(border, Dock.Top);

            SolidColorBrush backgroundBrush = new SolidColorBrush(Color.FromArgb(13, 255, 250, 240));
            border.Background = backgroundBrush;

            //#Grid element => (border child)
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(32) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80) });

            //#Image element
            Image image = new Image();
            image.Source = scroll.ItemImage.Source;
            image.Stretch = Stretch.Uniform;
            image.Height = 32;
            image.Width = 32;
            Grid.SetColumn(image, 0);

            //#Name element
            TextBlock textBlock = new TextBlock();
            textBlock.Text = scroll.Name;
            textBlock.FontSize = 16;
            textBlock.Foreground = Brushes.GhostWhite;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.Margin = new Thickness(5, 0, 0, 0);
            Grid.SetColumn(textBlock, 1);

            //#Apply Button
            Button applyButton = new Button();
            applyButton.Content = "Apply";
            applyButton.FontSize = 13;
            applyButton.Width = 80;
            applyButton.VerticalAlignment = VerticalAlignment.Center;
            applyButton.HorizontalAlignment = HorizontalAlignment.Right;
            applyButton.Margin = new Thickness(5, 0, 5, 0);
            applyButton.Click += (s, e) => ApplyScroll(scroll);
            applyButton.Style = (Style)FindResource("menu-button");
            Grid.SetColumn(applyButton, 2);

            //#Total Quantity
            TextBlock quantityTextBlock = new TextBlock();
            quantityTextBlock.Text = $"x{Inventory.Materials[scroll.ID]} left";
            quantityTextBlock.FontSize = 14;
            quantityTextBlock.Foreground = Brushes.WhiteSmoke;
            quantityTextBlock.VerticalAlignment = VerticalAlignment.Center;
            quantityTextBlock.FontStyle = FontStyles.Italic;
            quantityTextBlock.TextAlignment = TextAlignment.Right;
            Grid.SetColumn(quantityTextBlock, 3);

            grid.Children.Add(image);
            grid.Children.Add(textBlock);
            grid.Children.Add(applyButton);
            grid.Children.Add(quantityTextBlock);

            border.Child = grid;

            // x|=> Add to container
            MaterialsContainer.Children.Add(border);

            //Add to dictionaries
            ScrollContainers.Add(scroll.ID, border);
            ScrollTextblocks.Add(scroll.ID, quantityTextBlock);
        }

        private void AddAllElementsToAniList()
        {
            AniListTextblocks.Add(TxtHpPreview);
            AniListTextblocks.Add(TxtDefPreview);
            AniListTextblocks.Add(TxtDmgPreview);
            AniListTextblocks.Add(TxtSpdPreview);
            AniListTextblocks.Add(TxtStaPreview);
            AniListTextblocks.Add(TxtStrPreview);
            AniListTextblocks.Add(TxtCrcPreview);
            AniListTextblocks.Add(TxtCrdPreview);
            AniListTextblocks.Add(TxtLevelPreview);
            AniListTextblocks.Add(TxtValuePreview);
            AniListTextblocks.Add(TxtCurrentXp);
            AniListTextblocks.Add(TxtMaxXp);
            AniListTextblocks.Add(TxtOrbs);
        }

        #endregion

        #region Buttons
        private void ApplyScroll(Material mat)
        {
            //Play sound
            SoundManager.PlaySound("magical-power-up.wav");

            //Add all material stats to item
            ItemStatObj.Stats.HP += mat.Stats.HP;
            ItemStatObj.Stats.DEF += mat.Stats.DEF;
            ItemStatObj.Stats.DMG += mat.Stats.DMG;
            ItemStatObj.Stats.SPD += mat.Stats.SPD;
            ItemStatObj.Stats.STA += mat.Stats.STA;
            ItemStatObj.Stats.STR += mat.Stats.STR;
            ItemStatObj.Stats.CRC += mat.Stats.CRC;
            ItemStatObj.Stats.CRD += mat.Stats.CRD;

            //Add XP
            LevelData.AddXP(Item, mat.Stats.XP);

            //Remove 1 scroll from inventory
            Inventory.Materials[mat.ID]--;
            ScrollTextblocks[mat.ID].Text = $"x{Inventory.Materials[mat.ID]} left";

            //Remove container if no more scrolls left
            if (Inventory.Materials[mat.ID] == 0)
            {
                MaterialsContainer.Children.Remove(ScrollContainers[mat.ID]);
                ScrollContainers.Remove(mat.ID);
                ScrollTextblocks.Remove(mat.ID);
            }

            //Recalculate value
            ItemData.SetValue(Item);

            //Refresh stats
            RefreshStats();
        }

        private void ReturnButton(object sender, RoutedEventArgs e)
        {
            Interaction.ReturnToPreviousTab();
        }

        private void BtnUpgrade_Click(object sender, RoutedEventArgs e)
        {
            //Animate upgrade
            AnimateTexts();

            //Play sound
            SoundManager.PlaySound("power-up.wav");

            //Get required orbs
            int requiredOrbs = GetRequiredOrbs(GetRequiredXp());

            //[!] Check if we have enough orbs
            if (Inventory.Materials["M2"] < requiredOrbs)
                return;

            //Apply changes
            int essenceXp = requiredOrbs * ItemData.ListMaterials.FirstOrDefault(x => x.ID == "M2").Stats.XP;
            LevelData.AddXP(Item, essenceXp);

            //Remove materials
            Inventory.Materials["M2"] -= requiredOrbs;

            //Recalculate value
            ItemData.SetValue(Item);

            //Update UI
            RefreshStats();
            IsUpgradePossible();
            TxtOrbs.Text = Inventory.Materials["M2"].ToString();

            //Is scroll container empty?
            HandleEmptyScrollContainer();
        }

        private void HandleEmptyScrollContainer()
        {
            //If there are no scrolls, add a message
            if (MaterialsContainer.Children.Count < 2)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = "You don't have any scrolls 😵‍💫";
                textBlock.FontSize = 16;
                textBlock.Foreground = Brushes.GhostWhite;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;

                MaterialsContainer.Children.Add(textBlock);
            }
        }

        #endregion

        private void IsUpgradePossible()
        {
            //Is item max level?
            if (IsMaxLevel())
            {
                TxtUpgradeStatus.Text = "Max level reached";
                TxtUpgradeStatus.Foreground = Brushes.FloralWhite;
                BtnUpgrade.IsEnabled = false;
                BtnUpgrade.Background = Brushes.Gray;
                return;
            }

            //Check if we have enough essence for an upgrade
            int requiredOrbs = GetRequiredOrbs(GetRequiredXp());

            if (requiredOrbs < Inventory.Materials["M2"])
            {
                TxtUpgradeStatus.Text = requiredOrbs > 1 ? $"Upgrade Available for {GetRequiredOrbs(GetRequiredXp())} orbs!" :
                    $"Upgrade Available for {GetRequiredOrbs(GetRequiredXp())} orb!";
                TxtUpgradeStatus.Foreground = Brushes.LimeGreen;
                BtnUpgrade.IsEnabled = true;
                BtnUpgrade.Background = Brushes.GhostWhite;
            }
            else
            {
                TxtUpgradeStatus.Text = requiredOrbs > 1 ? $"Need {GetRequiredOrbs(GetRequiredXp())} orbs to upgrade." :
                    $"Need {GetRequiredOrbs(GetRequiredXp())} orb to upgrade.";
                TxtUpgradeStatus.Foreground = Brushes.Crimson;
                BtnUpgrade.IsEnabled = false;
                BtnUpgrade.Background = Brushes.Gray;
            }
        }

        private bool IsMaxLevel()
        {
            //Check if item is max level
            return Item.Level == XpPerLevel.Keys.LastOrDefault();
        }

        private void RefreshStats()
        {
            //Display Current Stats
            TxtHpPreview.Text = ItemStatObj.Stats.HP.ToString();
            TxtDefPreview.Text = ItemStatObj.Stats.DEF.ToString();
            TxtDmgPreview.Text = ItemStatObj.Stats.DMG.ToString();
            TxtSpdPreview.Text = ItemStatObj.Stats.SPD.ToString();
            TxtStaPreview.Text = ItemStatObj.Stats.STA.ToString();
            TxtStrPreview.Text = ItemStatObj.Stats.STR.ToString();
            TxtCrcPreview.Text = ItemStatObj.Stats.CRC.ToString();
            TxtCrdPreview.Text = ItemStatObj.Stats.CRD.ToString();

            //Highlight changed stats by adding fontweight bold
            TxtHpPreview.FontWeight = ItemStatObj.Stats.HP == ItemPreviewStatObj.Stats.HP ? FontWeights.Normal : FontWeights.Bold;
            TxtDefPreview.FontWeight = ItemStatObj.Stats.DEF == ItemPreviewStatObj.Stats.DEF ? FontWeights.Normal : FontWeights.Bold;
            TxtDmgPreview.FontWeight = ItemStatObj.Stats.DMG == ItemPreviewStatObj.Stats.DMG ? FontWeights.Normal : FontWeights.Bold;
            TxtSpdPreview.FontWeight = ItemStatObj.Stats.SPD == ItemPreviewStatObj.Stats.SPD ? FontWeights.Normal : FontWeights.Bold;
            TxtStaPreview.FontWeight = ItemStatObj.Stats.STA == ItemPreviewStatObj.Stats.STA ? FontWeights.Normal : FontWeights.Bold;
            TxtStrPreview.FontWeight = ItemStatObj.Stats.STR == ItemPreviewStatObj.Stats.STR ? FontWeights.Normal : FontWeights.Bold;
            TxtCrcPreview.FontWeight = ItemStatObj.Stats.CRC == ItemPreviewStatObj.Stats.CRC ? FontWeights.Normal : FontWeights.Bold;
            TxtCrdPreview.FontWeight = ItemStatObj.Stats.CRD == ItemPreviewStatObj.Stats.CRD ? FontWeights.Normal : FontWeights.Bold;

            //XP related (display max if level is max. Set progressbar to 100 both max value as current value)
            TxtCurrentXp.Text = Item.Level == XpPerLevel.Keys.LastOrDefault() ? "max" : ItemStatObj.Stats.GetXP() + "xp";
            XpBar.Value = Item.Level == XpPerLevel.Keys.LastOrDefault() ? 100 : int.Parse(ItemStatObj.Stats.GetXP());

            //Get max XP
            int maxXp = 0;
            try
            {
                maxXp = XpPerLevel[Item.Level + 1];
            }
            catch (Exception e)
            {
                maxXp = 0;
            }

            TxtMaxXp.Text = maxXp == 0 ? "MAX" : maxXp + "xp";
            XpBar.Maximum = maxXp == 0 ? 100 : maxXp;

            //Highlight xp if changed
            TxtCurrentXp.FontWeight = ItemStatObj.Stats.XP == ItemPreviewStatObj.Stats.XP ? FontWeights.Normal : FontWeights.Bold;
            TxtMaxXp.FontWeight = TxtCurrentXp.FontWeight;

            //Display level
            TxtLevelPreview.Text = Item.Level.ToString();
            TxtLevelPreview.FontWeight = Item.Level == ItemPreview.Level ? FontWeights.Normal : FontWeights.Bold;

            //Display value
            TxtValuePreview.Text = Item.Value.ToString();
            TxtValuePreview.FontWeight = Item.Value == ItemPreview.Value ? FontWeights.Normal : FontWeights.Bold;
        }

        private int GetRequiredXp()
        {
            //Check if key exists
            if (!XpPerLevel.ContainsKey(Item.Level + 1))
                return 0;

            return XpPerLevel[Item.Level + 1] - ItemStatObj.Stats.XP;
        }

        private int GetRequiredOrbs(int xp)
        {
            //Put xp into double to allow decimals in the calculation
            double orbXp = ItemData.ListMaterials.FirstOrDefault(x => x.Name.Contains("Orb")).Stats.XP;
            double requiredXp = xp;

            //Calculate required orbs
            double orbs = requiredXp / orbXp;
            orbs = Math.Ceiling(orbs);
            return (int)orbs;
        }

        private async void AnimateTexts()
        {
            //#Make a popping effect for all textblocks

            //[1] Make all textblocks bigger
            int ticks = 0;

            while (ticks < 2)
            {
                foreach (TextBlock element in AniListTextblocks)
                {
                    element.FontSize += 2;
                }

                ItemImage.Height += 8;

                ticks++;
                await Task.Delay(10);
            }

            //[2] Make all textblocks smaller
            ticks = 0;

            while (ticks < 8)
            {
                foreach (TextBlock element in AniListTextblocks)
                {
                    element.FontSize -= 0.5;
                }

                ItemImage.Height -= 2;

                ticks++;
                await Task.Delay(20);
            }
        }
    }
}
