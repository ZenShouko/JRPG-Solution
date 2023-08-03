﻿using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace JRPG_Project.Tabs
{
    public partial class MarketTab : UserControl
    {
        public MarketTab()
        {
            InitializeComponent();
            RefreshMarket();
            UpdateGUI();
        }

        private void RefreshMarket()
        {
            //Has 10 min passed since last refresh?
            if (Inventory.MarketRefresh.AddMinutes(10) < DateTime.Now)
            {
                Inventory.MarketRefresh = DateTime.Now;
                RefreshMarketRequests();
            }
        }

        private void RefreshMarketRequests()
        {
            ///Pick 3 random items from inventory and add them to market requests
            ///
            //Clear current requests
            Inventory.MarketRequests.Clear();

            //Add all items UniqueID to a list
            List<string> allItemsID = new List<string>();

            foreach (BaseItem item in Inventory.Weapons)
            {
                allItemsID.Add(item.UniqueID);
            }
            foreach (BaseItem item in Inventory.Armours)
            {
                allItemsID.Add(item.UniqueID);
            }
            foreach (BaseItem item in Inventory.Amulets)
            {
                allItemsID.Add(item.UniqueID);
            }

            //How many items can be requested?
            int totalRequestableCount = 0;
            if (allItemsID.Count > 3)
            {
                totalRequestableCount = 3;
            }
            else
            {
                totalRequestableCount = allItemsID.Count;
            }

            //Pick 3 random items from list and add them to market requests
            while (totalRequestableCount > Inventory.MarketRequests.Count)
            {
                //Pick a distinct random item
                string id = allItemsID[Interaction.GetRandomNumber(0, allItemsID.Count - 1)];

                if (Inventory.MarketRequests.Keys.Contains(id))
                {
                    continue;
                }
                else
                {
                    Inventory.MarketRequests.Add(id, false);
                }
            }
        }

        private void UpdateGUI()
        {
            //Display next refresh time
            TxtNextRefreshTime.Text = "Next refresh: " + Inventory.MarketRefresh.AddMinutes(10).ToString("HH:mm");

            //Display market requests
            if (Inventory.MarketRequests.Keys.ElementAt(0) == null || Inventory.MarketRequests.Values.ElementAt(0) == true)
            {
                FillItemSlotWithNothing(1);
            }
            else
            {
                BaseItem item = ItemData.GetItemByUniqueID(Inventory.MarketRequests.Keys.ElementAt(0));
                TxtItem1Name.Text = item.Name;
                TxtItem1Reward.Text = Inventory.MarketRequests.Values.ElementAt(0) ? "Completed" : $"Reward: {item.Value} coins";
                ImgItem1.Source = item.ItemImage.Source;
            }

            if (Inventory.MarketRequests.Keys.ElementAt(1) == null)
            {
                FillItemSlotWithNothing(2);
            }
            else
            {
                BaseItem item = ItemData.GetItemByUniqueID(Inventory.MarketRequests.Keys.ElementAt(1));
                TxtItem2Name.Text = item.Name;
                TxtItem2Reward.Text = $"Reward: {item.Value} coins";
                ImgItem2.Source = item.ItemImage.Source;
            }

            if (Inventory.MarketRequests.Keys.ElementAt(2) == null)
            {
                FillItemSlotWithNothing(3);
            }
            else
            {
                BaseItem item = ItemData.GetItemByUniqueID(Inventory.MarketRequests.Keys.ElementAt(2));
                TxtItem3Name.Text = item.Name;
                TxtItem3Reward.Text = $"Reward: {item.Value} coins";
                ImgItem3.Source = item.ItemImage.Source;
            }
        }

        private void FillItemSlotWithNothing(int slot)
        {
            if (slot == 1)
            {
                TxtItem1Name.Text = "Nothing";
                TxtItem1Reward.Text = "";
                ImgItem1.Source = new BitmapImage(new Uri(@"/Resources/Assets/GUI/empty.png", UriKind.RelativeOrAbsolute));
            }
            else if (slot == 2)
            {
                TxtItem2Name.Text = "Nothing";
                TxtItem2Reward.Text = "";
                ImgItem2.Source = new BitmapImage(new Uri(@"/Resources/Assets/GUI/empty.png", UriKind.RelativeOrAbsolute));
            }
            else if (slot == 3)
            {
                TxtItem3Name.Text = "Nothing";
                TxtItem3Reward.Text = "";
                ImgItem3.Source = new BitmapImage(new Uri(@"/Resources/Assets/GUI/empty.png", UriKind.RelativeOrAbsolute));
            }
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            Interaction.OpenTab("MainTab");
        }

        private void StatsButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Name.Contains("1"))
            {
                //Cancel if no request or request already completed
                if (Inventory.MarketRequests.Keys.ElementAt(0) == null)
                    return;

                //Display item stats
                StatsWindow window = new StatsWindow(Inventory.MarketRequests.Keys.ElementAt(0), false);
                window.ShowDialog();
            }
            else if (btn.Name.Contains("2"))
            {
                //Cancel if no request
                if (Inventory.MarketRequests.Keys.ElementAt(1) == null)
                    return;

                StatsWindow window = new StatsWindow(Inventory.MarketRequests.Keys.ElementAt(1), false);
                window.ShowDialog();
            }
            else if (btn.Name.Contains("3"))
            {
                //Cancel if no request
                if (Inventory.MarketRequests.Keys.ElementAt(2) == null)
                    return;

                StatsWindow window = new StatsWindow(Inventory.MarketRequests.Keys.ElementAt(2), false);
                window.ShowDialog();
            }
        }

        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Name.Contains("1"))
            {
                //Cancel if already sold
                if (Inventory.MarketRequests.Values.ElementAt(0) == true)
                    return;

                //Sell item
                Inventory.MarketRequests[Inventory.MarketRequests.Keys.ElementAt(0)] = true;
                Inventory.Coins += ItemData.GetItemByUniqueID(Inventory.MarketRequests.Keys.ElementAt(0)).Value;

                //Unequip item if equipped
                CharacterData.UnequipItem(ItemData.GetItemByUniqueID(Inventory.MarketRequests.Keys.ElementAt(0)));

                //Remove item from inventory
                PlayerActions.RemoveItem(ItemData.GetItemByUniqueID(Inventory.MarketRequests.Keys.ElementAt(0)));

                //Let them know
                MessageBox.Show("Sold :)");
                TxtItem1Reward.Text = "Completed";
            }
        }
    }
}