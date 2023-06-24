using JRPG_Project.ClassLibrary.Player;
using JRPG_Project.ClassLibrary.Universal;
using JRPG_Project.Tabs;
using System.Windows;
using System.Windows.Controls;

namespace JRPG_ClassLibrary
{
    public static class Interaction
    {
        public static Grid Grid { get; set; }
        public static MainTab MainTab = new MainTab();
        public static PlaygroundTab PlaygroundTab = new PlaygroundTab();
        public static InventoryTab InventoryTab = new InventoryTab();

        private static object _key = null;
        public static void SetKey(object key)
        {
            //Prioritize reset
            if (key is null)
            {
                _key = null;
            }

            //Set key if not already set
            if (_key is null)
            {
                _key = key;
            }
        }

        public static object GetKey()
        {
            return _key;
        }

        public static void OpenTab(string tab)
        {
            Grid.Children.Clear();

            switch (tab)
            {
                case "MainTab":
                    {
                        Grid.Children.Add(MainTab);
                        break;
                    }
                case "BtnDispatch":
                    {
                        Grid.Children.Add(PlaygroundTab);
                        break;
                    }
                case "BtnInventory":
                    {
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
