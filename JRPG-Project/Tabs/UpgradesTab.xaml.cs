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
        BaseItem Item { get; set; }
        Stats CurrentStats { get; set; }
        Stats UpgradeStats { get; set; }

        Dictionary<string, int> MaterialsToUse = new Dictionary<string, int>(); //ID, amount

        private void PrepareGuiForUpgrade()
        {
            //Vars
            IStatsHolder ItemWStats = Item as IStatsHolder;
            CurrentStats = ItemWStats.Stats;

            //Add all ID's to dictionary
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
            //Display Current Stats
            TxtHpPreview.Text = CurrentStats.HP.ToString();
            TxtDefPreview.Text = CurrentStats.DEF.ToString();
            TxtDmgPreview.Text = CurrentStats.DMG.ToString();
            TxtSpdPreview.Text = CurrentStats.SPD.ToString();
            TxtStaPreview.Text = CurrentStats.STA.ToString();
            TxtStrPreview.Text = CurrentStats.STR.ToString();
            TxtCrcPreview.Text = CurrentStats.CRC.ToString();
            TxtCrdPreview.Text = CurrentStats.CRD.ToString();
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
            //Border element
            Border border = new Border();
            border.Tag = mat.ID;
            border.BorderThickness = new Thickness(1, 1, 2, 2);
            border.BorderBrush = Brushes.Black;
            border.Background = Brushes.GhostWhite;
            border.Padding = new Thickness(6, 4, 6, 4);
            border.Margin = new Thickness(4, 8, 4, 8);
            border.CornerRadius = new CornerRadius(2);
            border.Cursor = Cursors.Hand;
            border.MouseUp += BorderMaterial_MouseUp;
            border.MouseEnter += BorderHoverEffect;
            border.MouseLeave += BorderHoverEffectReset;

            //Dockpanel
            DockPanel dockPanel = new DockPanel();
            dockPanel.LastChildFill = false;
            border.Child = dockPanel;

            //Mat image
            Image img = new Image();
            img.Source = mat.ItemImage.Source;
            img.Height = 64;
            img.Stretch = Stretch.Uniform;
            DockPanel.SetDock(img, Dock.Top);
            dockPanel.Children.Add(img);

            //Mat name
            TextBlock txtName = new TextBlock();
            txtName.Text = mat.Name;
            txtName.Margin = new Thickness(0, 2, 0, 2);
            txtName.HorizontalAlignment = HorizontalAlignment.Center;
            DockPanel.SetDock(txtName, Dock.Top);
            dockPanel.Children.Add(txtName);

            //Mat Count
            TextBlock txtCount = new TextBlock();
            txtCount.Text = Inventory.Materials.FirstOrDefault(x => x.Key == mat.ID).Value.ToString();
            txtCount.HorizontalAlignment = HorizontalAlignment.Center;
            txtCount.Margin = new Thickness(0, 2, 0, 1);
            DockPanel.SetDock(txtCount, Dock.Bottom);
            dockPanel.Children.Add(txtCount);

            //Add dockPanel to container
            MaterialsPanel.Children.Add(border);
        }

        private void BorderHoverEffectReset(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            border.Background = Brushes.GhostWhite;
        }

        private void BorderHoverEffect(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            border.Background = Brushes.FloralWhite;
        }

        private void BorderMaterial_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //Get sender
            Border border = (Border)sender;

            //Can we add more?
            if (Inventory.Materials[(string)border.Tag] > MaterialsToUse[(string)border.Tag])
            {
                MaterialsToUse[(string)border.Tag] += 1;

                //Get last textblock in border
                DockPanel dockPanel = (DockPanel)border.Child;
                TextBlock txtCount = dockPanel.Children[dockPanel.Children.Count - 1] as TextBlock;

                //Modify the number--
                int currentCount = int.Parse(txtCount.Text);
                currentCount--;
                txtCount.Text = currentCount.ToString();

                //Refresh mats in use
                RefreshMatsInUse();
            }
        }

        private void RefreshMatsInUse()
        {
            TxtBottleCount.Text = MaterialsToUse["M1"].ToString();
            TxtOrbCount.Text = MaterialsToUse["M2"].ToString();

            TxtScrollHpCount.Text = MaterialsToUse["M4"].ToString();
            TxtScrollDefCount.Text = MaterialsToUse["M5"].ToString();
            TxtScrollDmgCount.Text = MaterialsToUse["M3"].ToString();
            TxtScrollSpdCount.Text = MaterialsToUse["M6"].ToString();
            TxtScrollStaCount.Text = MaterialsToUse["M7"].ToString();
            TxtScrollStrCount.Text = MaterialsToUse["M8"].ToString();
            TxtScrollCrcCount.Text = MaterialsToUse["M9"].ToString();
            TxtScrollCrdCount.Text = MaterialsToUse["M10"].ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Interaction.OpenTab("MainTab");
        }
    }
}
