using JRPG_Project;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Universal;
using JRPG_Project.Tabs;
using System.Security.Cryptography;
using System;
using System.Windows;
using System.Windows.Controls;
using JRPG_Project.ClassLibrary.Player;

namespace JRPG_ClassLibrary
{
    public static class Interaction
    {
        public static Grid Grid { get; set; }

        public static void OpenTab(string tab)
        {
            switch (tab.ToUpper())
            {
                case "MAINTAB":
                    {
                        MainTab MainTab = new MainTab();
                        Grid.Children.Clear();
                        Grid.Children.Add(MainTab);
                        break;
                    }
                case "BTNDISPATCH":
                    {
                        PlatformFinder platformFinder = new PlatformFinder();
                        platformFinder.ShowDialog();
                        break;
                    }
                case "BTNINVENTORY":
                    {
                        InventoryTab InventoryTab = new InventoryTab();
                        Grid.Children.Clear();
                        Grid.Children.Add(InventoryTab);
                        break;
                    }
                case "BTNTEAM":
                    {
                        TeamTab teamTab = new TeamTab();
                        Grid.Children.Clear();
                        Grid.Children.Add(teamTab);
                        break;
                    }
                case "BTNSAVE":
                    {
                        SaveWindow saveWindow = new SaveWindow();
                        saveWindow.ShowDialog();
                        OpenTab("MainTab");
                        break;
                    }
                    case "BTNMARKET":
                    {
                        MarketTab shopTab = new MarketTab();
                        Grid.Children.Clear();
                        Grid.Children.Add(shopTab);
                        break;
                    }
                case "BTNUPGRADES":
                    {
                        UpgradesTab upgradeTab = new UpgradesTab(Inventory.Weapons[0], false);
                        Grid.Children.Clear();
                        Grid.Children.Add(upgradeTab);
                        break;
                    }
                default:
                    {
                        MessageBox.Show("tab not recognized.");
                        Grid.Children.Clear();
                        OpenTab("MainTab");
                        break;
                    }
            }
        }

        public static void OpenDispatchTab(string stageName)
        {
            Grid.Children.Clear();
            DispatchTab dispatchTab = new DispatchTab(stageName);
            Grid.Children.Add(dispatchTab);
        }

        /// <summary>
        /// Generates a number between min and max. Min and Max are included in the range.
        /// </summary>
        public static int GetRandomNumber(int min, int max)
        {
            //Make max inclusive
            max++;

            //Generate random number
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] randomNumber = new byte[4]; // 4 bytes for a 32-bit integer

                rng.GetBytes(randomNumber);

                // Convert the random bytes to a 32-bit signed integer
                int generatedNumber = Math.Abs(BitConverter.ToInt32(randomNumber, 0));

                // Scale the number to be within the desired range
                try
                {
                    return min + (generatedNumber % (max - min));
                }
                catch
                {
                    return 0;
                }
            }
        }
    }
}
