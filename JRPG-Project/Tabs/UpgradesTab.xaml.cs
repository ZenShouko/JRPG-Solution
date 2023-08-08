using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JRPG_ClassLibrary.Entities;

namespace JRPG_Project.Tabs
{
    /// <summary>
    /// Interaction logic for UpgradesTab.xaml
    /// </summary>
    public partial class UpgradesTab : UserControl
    {
        public UpgradesTab(BaseItem item, bool refinement)
        {
            InitializeComponent();
            Item = item;
            if (!refinement)
                PrepareGuiForUpgrade();
        }
        /// <summary>
        /// Item in question, used for reference.
        /// </summary>
        BaseItem Item { get; set; }
        /// <summary>
        /// Item after upgrades.
        /// </summary>
        BaseItem UpgradePreview { get; set; }

        Dictionary<string, int> MaterialsToUse = new Dictionary<string, int>(); //ID, amount
        Dictionary<string, TextBox> TextboxCorrelatedToMat = new Dictionary<string, TextBox>();

        private void PrepareGuiForUpgrade()
        {
            //Add all material ID's to dictionary
            foreach (string id in Inventory.Materials.Keys)
            {
                MaterialsToUse.Add(id, 0);
            }

            //Set item image
            ItemImage.Source = Item.ItemImage.Source;

            //Display Stats
            RefreshStats();

            //Add materials to MaterialsPanel
            LoadMaterials();
        }

        private void RefreshStats()
        {
            //Calculate Upgrade Stats
            CalculateStatsAfterUpgrade();

            //Vars
            IStatsHolder baseObj = Item as IStatsHolder;
            IStatsHolder upgradeObj = UpgradePreview as IStatsHolder;

            //Display Current Stats
            TxtHpPreview.Text = baseObj.Stats.HP == upgradeObj.Stats.HP ? baseObj.Stats.HP.ToString() : baseObj.Stats.HP + " -> " + upgradeObj.Stats.HP;
            TxtDefPreview.Text = baseObj.Stats.DEF == upgradeObj.Stats.DEF ? baseObj.Stats.DEF.ToString() : baseObj.Stats.DEF + " -> " + upgradeObj.Stats.DEF;
            TxtDmgPreview.Text = baseObj.Stats.DMG == upgradeObj.Stats.DMG ? baseObj.Stats.DMG.ToString() : baseObj.Stats.DMG + " -> " + upgradeObj.Stats.DMG;
            TxtSpdPreview.Text = baseObj.Stats.SPD == upgradeObj.Stats.SPD ? baseObj.Stats.SPD.ToString() : baseObj.Stats.SPD + " -> " + upgradeObj.Stats.SPD;
            TxtStaPreview.Text = baseObj.Stats.STA == upgradeObj.Stats.STA ? baseObj.Stats.STA.ToString() : baseObj.Stats.STA + " -> " + upgradeObj.Stats.STA;
            TxtStrPreview.Text = baseObj.Stats.STR == upgradeObj.Stats.STR ? baseObj.Stats.STR.ToString() : baseObj.Stats.STR + " -> " + upgradeObj.Stats.STR;
            TxtCrcPreview.Text = baseObj.Stats.CRC == upgradeObj.Stats.CRC ? baseObj.Stats.CRC.ToString() : baseObj.Stats.CRC + " -> " + upgradeObj.Stats.CRC;
            TxtCrdPreview.Text = baseObj.Stats.CRD == upgradeObj.Stats.CRD ? baseObj.Stats.CRD.ToString() : baseObj.Stats.CRD + " -> " + upgradeObj.Stats.CRD;

            //Highlight changed stats by adding fontweight bold
            TxtHpPreview.FontWeight = baseObj.Stats.HP == upgradeObj.Stats.HP ? FontWeights.Normal : FontWeights.Bold;
            TxtDefPreview.FontWeight = baseObj.Stats.DEF == upgradeObj.Stats.DEF ? FontWeights.Normal : FontWeights.Bold;
            TxtDmgPreview.FontWeight = baseObj.Stats.DMG == upgradeObj.Stats.DMG ? FontWeights.Normal : FontWeights.Bold;
            TxtSpdPreview.FontWeight = baseObj.Stats.SPD == upgradeObj.Stats.SPD ? FontWeights.Normal : FontWeights.Bold;
            TxtStaPreview.FontWeight = baseObj.Stats.STA == upgradeObj.Stats.STA ? FontWeights.Normal : FontWeights.Bold;
            TxtStrPreview.FontWeight = baseObj.Stats.STR == upgradeObj.Stats.STR ? FontWeights.Normal : FontWeights.Bold;
            TxtCrcPreview.FontWeight = baseObj.Stats.CRC == upgradeObj.Stats.CRC ? FontWeights.Normal : FontWeights.Bold;
            TxtCrdPreview.FontWeight = baseObj.Stats.CRD == upgradeObj.Stats.CRD ? FontWeights.Normal : FontWeights.Bold;

            //XP related
            TxtCurrentXp.Text = upgradeObj.Stats.GetXP() + "xp";
            XpBar.Value = int.Parse(upgradeObj.Stats.GetXP());

            //Get max XP
            int maxXp = 0;
            if (Item is Weapon)
                maxXp = LevelData.WeaponXPTable[Item.Level + 1].Item1;
            else if (Item is Armour)
                maxXp = LevelData.ArmourXPTable[Item.Level + 1].Item1;
            else if (Item is Amulet)
                maxXp = LevelData.AmuletXPTable[Item.Level + 1].Item1;

            TxtMaxXp.Text = maxXp + "xp";
            XpBar.Maximum = maxXp;

            //Highlight xp if changed
            TxtCurrentXp.FontWeight = baseObj.Stats.XP == upgradeObj.Stats.XP ? FontWeights.Normal : FontWeights.Bold;
            TxtMaxXp.FontWeight = TxtCurrentXp.FontWeight;

            //Display level
            TxtLevelPreview.Text = Item.Level == UpgradePreview.Level ? Item.Level.ToString() : Item.Level + " -> " + UpgradePreview.Level;
            TxtLevelPreview.FontWeight = Item.Level == UpgradePreview.Level ? FontWeights.Normal : FontWeights.Bold;

            //Display value
            TxtValuePreview.Text = Item.Value == UpgradePreview.Value ? Item.Value.ToString() : Item.Value + " -> " + UpgradePreview.Value;
            TxtValuePreview.FontWeight = Item.Value == UpgradePreview.Value ? FontWeights.Normal : FontWeights.Bold;
        }

        private void CalculateStatsAfterUpgrade()
        {
            //Refresh upgrade object
            //Copy item to upgrade preview
            if (Item is Weapon)
                UpgradePreview = new Weapon();
            else if (Item is Armour)
                UpgradePreview = new Armour();
            else if (Item is Amulet)
                UpgradePreview = new Amulet();

            UpgradePreview.CopyFrom(Item);

            //Vars
            IStatsHolder baseObj = Item as IStatsHolder;
            IStatsHolder upgradeObj = UpgradePreview as IStatsHolder;

            //Get total xp from materials
            int totalXp = 0;
            foreach (var item in MaterialsToUse)
            {
                if (item.Value != 0)
                {
                    ClassLibrary.Items.Material mat = ItemData.ListMaterials.Find(x => x.ID == item.Key);
                    totalXp += mat.Stats.XP * item.Value;
                }
            }

            //Add xp to Stats
            upgradeObj.Stats.XP += totalXp;

            //Can item level up?
            if (Item is Weapon)
            {
                //Check if item xp is enough to level up and if item is not max level
                while (LevelData.WeaponXPTable.ContainsKey(Item.Level + 1) && upgradeObj.Stats.XP >= LevelData.WeaponXPTable[Item.Level + 1].Item1) //oui
                {
                    //Get level up stats
                    Stats levelUpStats = LevelData.WeaponXPTable[Item.Level + 1].Item2;

                    //Add level up stats
                    upgradeObj.Stats.HP += levelUpStats.HP;
                    upgradeObj.Stats.DEF += levelUpStats.DEF;
                    upgradeObj.Stats.DMG += levelUpStats.DMG;
                    upgradeObj.Stats.SPD += levelUpStats.SPD;
                    upgradeObj.Stats.STA += levelUpStats.STA;
                    upgradeObj.Stats.STR += levelUpStats.STR;
                    upgradeObj.Stats.CRC += levelUpStats.CRC;
                    upgradeObj.Stats.CRD += levelUpStats.CRD;

                    //Deduct xp required for level up
                    upgradeObj.Stats.XP -= LevelData.WeaponXPTable[Item.Level + 1].Item1;

                    //Increase level
                    UpgradePreview.Level++;

                    //Increase value
                    UpgradePreview.Value = ItemData.GetValue(UpgradePreview);
                }
            }
        }

        private void LoadMaterials()
        {
            //Itterate through every distinct material
            foreach (var item in Inventory.Materials)
            {
                if (item.Value != 0)
                {
                    ClassLibrary.Items.Material mat = ItemData.ListMaterials.Find(x => x.ID == item.Key);
                    AddMaterialToContainer(mat);
                }
            }
        }

        private void AddMaterialToContainer(ClassLibrary.Items.Material mat)
        {
            //#Border element
            Border border = new Border();
            border.SetValue(DockPanel.DockProperty, Dock.Top);
            border.BorderThickness = new Thickness(1, 1, 2, 3);
            border.CornerRadius = new CornerRadius(2, 4, 4, 2);
            border.BorderBrush = Brushes.FloralWhite;
            border.Padding = new Thickness (8, 6, 8, 6);
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
            image.Source = mat.ItemImage.Source;
            image.Stretch = Stretch.Uniform;
            image.Height = 32;
            image.Width = 32;
            Grid.SetColumn(image, 0);

            //#Name element
            TextBlock textBlock = new TextBlock();
            textBlock.Text = mat.Name;
            textBlock.FontSize = 16;
            textBlock.Foreground = Brushes.GhostWhite;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.Margin = new Thickness(5, 0, 0, 0);
            Grid.SetColumn(textBlock, 1);

            //#Stackpanel for quantity adjustment
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Margin = new Thickness(25, 0, 0, 0);
            Grid.SetColumn(stackPanel, 2);

            //#Quantity adjustment buttons (Decresing)
            Button btnGreatlyDecrease = new Button();
            btnGreatlyDecrease.Content = "<<";
            btnGreatlyDecrease.Height = 24;
            btnGreatlyDecrease.Width = 24;
            btnGreatlyDecrease.Style = (Style)Application.Current.Resources["menu-button"];
            btnGreatlyDecrease.Click += (s, e) => AdjustMaterialQuantity(mat.ID, -5);
            stackPanel.Children.Add(btnGreatlyDecrease);

            Button btnDecrease = new Button();
            btnDecrease.Content = "<";
            btnDecrease.Height = 24;
            btnDecrease.Width = 24;
            btnDecrease.Style = (Style)Application.Current.Resources["menu-button"];
            btnDecrease.Click += (s, e) => AdjustMaterialQuantity(mat.ID, -1);
            stackPanel.Children.Add(btnDecrease);

            //#Total Quantity Textbox
            TextBox quantityTextbox = new TextBox();
            quantityTextbox.Text = "0";
            quantityTextbox.Width = 40;
            quantityTextbox.MaxLength = 3;
            quantityTextbox.VerticalAlignment = VerticalAlignment.Center;
            quantityTextbox.TextAlignment = TextAlignment.Center;
            quantityTextbox.FontSize = 16;
            quantityTextbox.FontWeight = FontWeights.Bold;
            quantityTextbox.Padding = new Thickness(2);
            quantityTextbox.Margin = new Thickness(10, 0, 10, 0);
            quantityTextbox.Style = (Style)Application.Current.Resources["RoundedCornerTextBox"];
            quantityTextbox.TextChanged += (s, e) => QuantityTextbox_TextChanged(s, e, mat.ID);
            stackPanel.Children.Add(quantityTextbox);
            //Add to dictionary
            TextboxCorrelatedToMat.Add(mat.ID, quantityTextbox);

            //#Quantity adjustment buttons (Increasing)
            Button btnIncrease = new Button();
            btnIncrease.Content = ">";
            btnIncrease.Height = 24;
            btnIncrease.Width = 24;
            btnIncrease.Style = (Style)Application.Current.Resources["menu-button"];
            btnIncrease.Click += (s, e) => AdjustMaterialQuantity(mat.ID, 1);
            stackPanel.Children.Add(btnIncrease);

            Button btnGreatlyIncrease = new Button();
            btnGreatlyIncrease.Content = ">>";
            btnGreatlyIncrease.Height = 24;
            btnGreatlyIncrease.Width = 24;
            btnGreatlyIncrease.Style = (Style)Application.Current.Resources["menu-button"];
            btnGreatlyIncrease.Click += (s, e) => AdjustMaterialQuantity(mat.ID, 5);
            stackPanel.Children.Add(btnGreatlyIncrease);

            //#Total Quantity
            TextBlock quantityTextBlock = new TextBlock();
            quantityTextBlock.Text = $"x{Inventory.Materials[mat.ID]} left";
            quantityTextBlock.FontSize = 14;
            quantityTextBlock.Foreground = Brushes.WhiteSmoke;
            quantityTextBlock.VerticalAlignment = VerticalAlignment.Center;
            quantityTextBlock.FontStyle = FontStyles.Italic;
            quantityTextBlock.TextAlignment = TextAlignment.Right;
            Grid.SetColumn(quantityTextBlock, 3);

            grid.Children.Add(image);
            grid.Children.Add(textBlock);
            grid.Children.Add(stackPanel);
            grid.Children.Add(quantityTextBlock);

            border.Child = grid;

            // x|=> Add to container
            MaterialsContainer.Children.Add(border);
        }

        private void QuantityTextbox_TextChanged(object sender, TextChangedEventArgs e, string id)
        {
            //If char is not a number, remove it
            TextBox textbox = (TextBox)sender;

            //Return if textbox is empty
            if (string.IsNullOrWhiteSpace(textbox.Text))
            {
                textbox.Text = "0";
                return;
            }

            //If textbox is not a number, revert to previous value
            if (!int.TryParse(textbox.Text, out int result))
            {
                textbox.Text = MaterialsToUse[id].ToString();
                return;
            }

            //If textbox value is too big, set it to max
            if (int.Parse(textbox.Text) > Inventory.Materials[id])
                textbox.Text = Inventory.Materials[id].ToString();

            //If textbox value starts with a 0, remove it
            while (textbox.Text.StartsWith("0") && textbox.Text.Length > 1)
                textbox.Text = textbox.Text.Remove(0, 1);

            //Set cursor to end
            textbox.CaretIndex = textbox.Text.Length;

            //Update dictionary
            MaterialsToUse[id] = int.Parse(textbox.Text);
        }

        private void AdjustMaterialQuantity(string id, int quantity)
        {
            //Keep amount in range
            if (MaterialsToUse[id] + quantity < 0)
                quantity = -MaterialsToUse[id];
            else if (MaterialsToUse[id] + quantity > Inventory.Materials[id])
                quantity = Inventory.Materials[id] - MaterialsToUse[id];

            //Adjust quantity
            MaterialsToUse[id] += quantity;

            //Update textbox
            TextboxCorrelatedToMat[id].Text = MaterialsToUse[id].ToString();

            //Refresh stats
            RefreshStats();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Interaction.OpenTab("MainTab");
        }
    }
}
