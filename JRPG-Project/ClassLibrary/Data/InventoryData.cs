using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace JRPG_Project.ClassLibrary.Data
{
    /// <summary>
    /// Scraped
    /// </summary>
    public static class InventoryData
    {
        public static Dictionary<int, string> SortOptions = new Dictionary<int, string>()
        {
            { 0, "Rarity Asc" },
            { 1, "Rarity Desc" },
            { 2, "Level Asc" },
            { 3, "Level Desc" },
            { 4, "Value Asc" },
            { 5, "Value Desc" }
        };

        private static Brush GetBrush(string rarity)
        {
            switch (rarity.ToUpper())
            {
                case "COMMON":
                    return Brushes.Black;
                case "SPECIAL":
                    return Brushes.SteelBlue;
                case "CURSED":
                    return Brushes.Orchid;
                case "LEGENDARY":
                    return Brushes.Goldenrod;
                default:
                    return Brushes.WhiteSmoke;
            }
        }
        #region GetItemsAsListboxItem   
        public static List<ListBoxItem> GetCollectablesAsListboxItem()
        {
            List<ListBoxItem> listBoxItems = new List<ListBoxItem>();

            foreach (Collectable col in Inventory.Collectables)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = col.ToString();
                item.Tag = col.UniqueID;
                item.Foreground = GetBrush(col.Rarity);

                listBoxItems.Add(item);
            }

            return listBoxItems;
        }

        public static List<ListBoxItem> GetWeaponsAsListboxItem()
        {
            List<ListBoxItem> listBoxItems = new List<ListBoxItem>();

            foreach (Weapon wpn in Inventory.Weapons)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = wpn.ToString();
                item.Tag = wpn.UniqueID;
                item.Foreground = GetBrush(wpn.Rarity);

                listBoxItems.Add(item);
            }

            return listBoxItems;
        }

        public static List<ListBoxItem> GetArmoursAsListboxItem()
        {
            List<ListBoxItem> listBoxItems = new List<ListBoxItem>();

            foreach (Armour arm in Inventory.Armours)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = arm.ToString();
                item.Tag = arm.UniqueID;
                item.Foreground = GetBrush(arm.Rarity);

                listBoxItems.Add(item);
            }

            return listBoxItems;
        }

        public static List<ListBoxItem> GetAmuletsAsListboxItem()
        {
            List<ListBoxItem> listBoxItems = new List<ListBoxItem>();

            foreach (Amulet amu in Inventory.Amulets)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = amu.ToString();
                item.Tag = amu.UniqueID;
                item.Foreground = GetBrush(amu.Rarity);

                listBoxItems.Add(item);
            }

            return listBoxItems;
        }

        #endregion

        public static List<Collectable> SortCollectables(int sort)
        {
            if (sort == 0 || sort == 1) //#Rarity
            {
                //Define a list for sorting order
                List<string> sortingOrder = new List<string>() { "COMMON", "SPECIAL", "CURSED", "LEGENDARY" };

                //Reverse if descending
                if (sort == 1)
                {
                    sortingOrder.Reverse();
                }

                //Define custom comparer for sorting
                var comparer = Comparer<string>.Create((x, y) =>
                {
                    int index1 = sortingOrder.IndexOf(x);
                    int index2 = sortingOrder.IndexOf(y);
                    return index1.CompareTo(index2);
                });

                //Apply sorting
                List<Collectable> collectables = Inventory.Collectables.OrderBy(x => x.Rarity, comparer).ThenBy(x => x.Name).ToList();
            }

            return null;
        }
    }
}
