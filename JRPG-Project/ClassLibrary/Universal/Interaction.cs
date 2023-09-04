using JRPG_Project;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Universal;
using JRPG_Project.Tabs;
using System.Security.Cryptography;
using System;
using System.Windows;
using System.Windows.Controls;
using JRPG_Project.ClassLibrary.Player;
using JRPG_Project.ClassLibrary;
using System.Collections.Generic;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_ClassLibrary.Entities;
using System.Threading.Tasks;
using System.Linq;

namespace JRPG_ClassLibrary
{
    public static class Interaction
    {
        public static Grid Grid { get; set; }

        private static UserControl previousTab { get; set; }

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
                case "BTNBATTLE": //[Battle Sim] button on MainTab
                    {
                        //Get current tab in Grid and save it
                        previousTab = (UserControl)Grid.Children[0];

                        //Add team to currentstage (for test's sake)
                        Stages.CurrentStage = new Stage();
                        foreach (Character c in Inventory.Team)
                        {
                            Stages.CurrentStage.Team.Add(c);
                        }

                        BattleTab battleTab = new BattleTab(FoeData.GetGenericFoeTeam());
                        Grid.Children.Clear();
                        Grid.Children.Add(battleTab);
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

        public static void OpenUpgradeTab(BaseItem Item)
        {
            //Save current tab
            previousTab = (UserControl)Grid.Children[0];

            UpgradesTab upgradeTab = new UpgradesTab(Item);
            Grid.Children.Clear();
            Grid.Children.Add(upgradeTab);
        }

        public static void ReturnToPreviousTab()
        {
            Grid.Children.Clear();
            Grid.Children.Add(previousTab);
            previousTab = null;
        }

        public async static void OpenBattletab(MapFoe mapFoe)
        {
            BattleTransitionTab battleTransitionTab = new BattleTransitionTab();
            Grid.Children.Add(battleTransitionTab);

            await Task.Delay(3000);

            //remove transition tab
            Grid.Children.Remove(battleTransitionTab);
            
            //Get current tab in Grid and save it
            previousTab = (UserControl)Grid.Children[0];

            BattleTab battleTab = new BattleTab(mapFoe.FoeTeam);
            Grid.Children.Clear();
            Grid.Children.Add(battleTab);

            //MessageBox.Show(Grid.Children.Count.ToString() + " Tabs in MainGrid.");
        }

        public static void CloseBattleTab(bool win)
        {
            Grid.Children.Clear();

            if (win)
            {
                //Remove mapfoe from tile
                Stages.CurrentStage.TileList.Find(x => x.Foe == Stages.CurrentStage.BattlingWith).Foe = null;

                //Remove mapfoe from platform
                Stages.CurrentStage.FoeList.Remove(Stages.CurrentStage.BattlingWith);

                //Refresh platform
                Stages.UpdateVisiblePlatform();

                //Let dispatchtab know that the battle is over
                Stages.CurrentStage.IsBattle = false;
                Stages.CurrentStage.BattlingWith = null;

                //Add previous tab
                Grid.Children.Add(previousTab);
            }
            else
            {
                //Return back to main tab
                OpenTab("MainTab");
            }
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
