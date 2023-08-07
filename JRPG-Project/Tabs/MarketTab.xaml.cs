using JRPG_ClassLibrary;
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
            //Has 3 min passed since last refresh?
            if (Inventory.MarketRefresh.AddMinutes(3) < DateTime.Now)
            {
                Inventory.MarketRefresh = DateTime.Now;
                RefreshMarketRequests();
                GenerateNewTicket();
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

        private void GenerateNewTicket()
        {
            //ODS: 1/4 chance to get nothing, 1/4 chance to get 1 basic ticket, 1/4 chance to get 1 special ticket, 1/4 chance to get 1 golden ticket
            int chance = Interaction.GetRandomNumber(1, 4);
            if (chance == 1)
            {
                Inventory.CurrentTicket = "Nothing";
            }
            else if (chance == 2)
            {
                Inventory.CurrentTicket = "Normal";
            }
            else if (chance == 3)
            {
                Inventory.CurrentTicket = "Special";
            }
            else if (chance == 4)
            {
                Inventory.CurrentTicket = "Golden";
            }
        }

        private void UpdateGUI()
        {
            //#Vending Machine
            VendingMachineTab tab = new VendingMachineTab();
            VendingMachinePanel.Children.Clear();
            VendingMachinePanel.Children.Add(tab);

            //#Display next refresh time
            TxtNextRefreshTime.Text = "Next refresh: " + Inventory.MarketRefresh.AddMinutes(3).ToString("HH:mm");

            //Display market requests
            if (Inventory.MarketRequests.Count < 1 || Inventory.MarketRequests.Values.ElementAt(0) == true)
            {
                FillItemSlotWithNothing(1, true);
                BtnSell1.IsEnabled = false;
                BtnStats1.IsEnabled = false;
            }
            else
            {
                try
                {
                    BaseItem item = ItemData.GetItemByUniqueID(Inventory.MarketRequests.Keys.ElementAt(0));
                    TxtItem1Name.Text = item.Name;
                    TxtItem1Reward.Text = Inventory.MarketRequests.Values.ElementAt(0) ? "Completed" : $"Reward: {item.Value} coins";
                    ImgItem1.Source = item.ItemImage.Source;
                }
                catch //If it fails, it means the item is not in the inventory anymore
                {
                    FillItemSlotWithNothing(1, false);
                    BtnSell1.IsEnabled = false;
                    BtnStats1.IsEnabled = false;
                }
            }

            if (Inventory.MarketRequests.Count < 2 || Inventory.MarketRequests.Values.ElementAt(1) == true)
            {
                FillItemSlotWithNothing(2, true);
                BtnSell2.IsEnabled = false;
                BtnStats2.IsEnabled = false;
            }
            else
            {
                try
                {
                    BaseItem item = ItemData.GetItemByUniqueID(Inventory.MarketRequests.Keys.ElementAt(1));
                    TxtItem2Name.Text = item.Name;
                    TxtItem2Reward.Text = $"Reward: {item.Value} coins";
                    ImgItem2.Source = item.ItemImage.Source;
                }
                catch
                {
                    FillItemSlotWithNothing(2, false);
                    BtnSell2.IsEnabled = false;
                    BtnStats2.IsEnabled = false;
                }
            }

            if (Inventory.MarketRequests.Count < 3 || Inventory.MarketRequests.Values.ElementAt(2) == true)
            {
                FillItemSlotWithNothing(3, true);
                BtnSell3.IsEnabled = false;
                BtnStats3.IsEnabled = false;
            }
            else
            {
                try
                {
                    BaseItem item = ItemData.GetItemByUniqueID(Inventory.MarketRequests.Keys.ElementAt(2));
                    TxtItem3Name.Text = item.Name;
                    TxtItem3Reward.Text = $"Reward: {item.Value} coins";
                    ImgItem3.Source = item.ItemImage.Source;
                }
                catch
                {
                    FillItemSlotWithNothing(3, false);
                    BtnSell3.IsEnabled = false;
                    BtnStats3.IsEnabled = false;
                }
            }
        }

        private void FillItemSlotWithNothing(int slot, bool happyEnd)
        {
            if (slot == 1)
            {
                TxtItem1Name.Text = happyEnd ? "Completed" : ">:(";
                TxtItem1Reward.Text = happyEnd ? ":)" : "Should've sold it to me!!";
                ImgItem1.Source = happyEnd ? new BitmapImage(new Uri(@"/Resources/Assets/GUI/monkey-happy.png", UriKind.RelativeOrAbsolute)) : 
                    new BitmapImage(new Uri(@"/Resources/Assets/GUI/monkey-angry.png", UriKind.RelativeOrAbsolute));
            }
            else if (slot == 2)
            {
                TxtItem2Name.Text = happyEnd ? "Completed" : ">:(";
                TxtItem2Reward.Text = happyEnd ? ":)" : "Should've sold it to me!!";
                ImgItem2.Source = happyEnd ? new BitmapImage(new Uri(@"/Resources/Assets/GUI/monkey-happy.png", UriKind.RelativeOrAbsolute)) :
                    new BitmapImage(new Uri(@"/Resources/Assets/GUI/monkey-angry.png", UriKind.RelativeOrAbsolute));
            }
            else if (slot == 3)
            {
                TxtItem3Name.Text = happyEnd ? "Completed" : ">:(";
                TxtItem3Reward.Text = happyEnd ? ":)" : "Should've sold it to me!!";
                ImgItem3.Source = happyEnd ? new BitmapImage(new Uri(@"/Resources/Assets/GUI/monkey-happy.png", UriKind.RelativeOrAbsolute)) :
                    new BitmapImage(new Uri(@"/Resources/Assets/GUI/monkey-angry.png", UriKind.RelativeOrAbsolute));
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
                if (Inventory.MarketRequests.Keys.ElementAt(0) == null || Inventory.MarketRequests.Values.ElementAt(0) == true)
                    return;

                //Display item stats
                StatsWindow window = new StatsWindow(Inventory.MarketRequests.Keys.ElementAt(0), false);
                window.ShowDialog();
            }
            else if (btn.Name.Contains("2"))
            {
                //Cancel if no request
                if (Inventory.MarketRequests.Keys.ElementAt(1) == null || Inventory.MarketRequests.Values.ElementAt(1) == true)
                    return;

                StatsWindow window = new StatsWindow(Inventory.MarketRequests.Keys.ElementAt(1), false);
                window.ShowDialog();
            }
            else if (btn.Name.Contains("3"))
            {
                //Cancel if no request
                if (Inventory.MarketRequests.Keys.ElementAt(2) == null || Inventory.MarketRequests.Values.ElementAt(2) == true)
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
                TxtItem1Reward.Text = "(Completed)";
            }
            else if (btn.Name.Contains("2"))
            {
                //Cancel if already sold
                if (Inventory.MarketRequests.Values.ElementAt(1) == true)
                    return;

                //Sell item
                Inventory.MarketRequests[Inventory.MarketRequests.Keys.ElementAt(1)] = true;
                Inventory.Coins += ItemData.GetItemByUniqueID(Inventory.MarketRequests.Keys.ElementAt(1)).Value;

                //Unequip item if equipped
                CharacterData.UnequipItem(ItemData.GetItemByUniqueID(Inventory.MarketRequests.Keys.ElementAt(1)));

                //Remove item from inventory
                PlayerActions.RemoveItem(ItemData.GetItemByUniqueID(Inventory.MarketRequests.Keys.ElementAt(1)));

                //Let them know
                TxtItem2Reward.Text = "(Completed)";
            }
            else if (btn.Name.Contains("3"))
            {
                //Cancel if already sold
                if (Inventory.MarketRequests.Values.ElementAt(2) == true)
                    return;

                //Sell item
                Inventory.MarketRequests[Inventory.MarketRequests.Keys.ElementAt(2)] = true;
                Inventory.Coins += ItemData.GetItemByUniqueID(Inventory.MarketRequests.Keys.ElementAt(2)).Value;

                //Unequip item if equipped
                CharacterData.UnequipItem(ItemData.GetItemByUniqueID(Inventory.MarketRequests.Keys.ElementAt(2)));

                //Remove item from inventory
                PlayerActions.RemoveItem(ItemData.GetItemByUniqueID(Inventory.MarketRequests.Keys.ElementAt(2)));

                //Let them know
                TxtItem3Reward.Text = "(Completed)";
            }
        }
    }
}
