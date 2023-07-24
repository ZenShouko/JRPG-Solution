using JRPG_Project;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Universal;
using JRPG_Project.Tabs;
using System.Windows;
using System.Windows.Controls;

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
    }
}
