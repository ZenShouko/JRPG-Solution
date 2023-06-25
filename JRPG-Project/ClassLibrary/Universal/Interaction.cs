using JRPG_Project.ClassLibrary.Universal;
using JRPG_Project.Tabs;
using System.Windows;
using System.Windows.Controls;

namespace JRPG_ClassLibrary
{
    public static class Interaction
    {
        public static Grid Grid { get; set; }
        public static MainTab MainTab;
        public static PlaygroundTab PlaygroundTab;
        public static InventoryTab InventoryTab;

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

            switch (tab)
            {
                case "MainTab":
                    {
                        MainTab = new MainTab();
                        Grid.Children.Add(MainTab);
                        break;
                    }
                case "BtnDispatch":
                    {
                        PlaygroundTab = new PlaygroundTab();
                        Grid.Children.Add(PlaygroundTab);
                        break;
                    }
                case "BtnInventory":
                    {
                        InventoryTab = new InventoryTab();
                        Grid.Children.Add(InventoryTab);
                        break;
                    }
                default:
                    {
                        MessageBox.Show("tab not recognized."); break;
                    }
            }
        }
    }
}
