using JRPG_Project.ClassLibrary.Universal;
using JRPG_Project.Tabs;
using System.Windows;
using System.Windows.Controls;

namespace JRPG_ClassLibrary
{
    public static class Interaction
    {
        public static Grid Grid { get; set; }

        private static object _key = null;
        public static void SetKey(object key)
        {
            //Prioritize reset
            if (key is null)
            {
                _key = null;
            }

            //Set key if not already set
            _key = key;
        }

        public static object GetKey()
        {
            //Used for in the platform
            return _key;
        }

        public static void OpenTab(string tab)
        {
            Grid.Children.Clear();

            switch (tab.ToUpper())
            {
                case "MAINTAB":
                    {
                        MainTab MainTab = new MainTab();
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
                        Grid.Children.Add(InventoryTab);
                        break;
                    }
                case "BTNTEAM":
                    {
                        TeamTab teamTab = new TeamTab();
                        Grid.Children.Add(teamTab);
                        break;
                    }
                default:
                    {
                        MessageBox.Show("tab not recognized."); 
                        MainTab mainTab = new MainTab();
                        Grid.Children.Add(mainTab);
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
