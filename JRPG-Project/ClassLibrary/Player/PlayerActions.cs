using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JRPG_Project.ClassLibrary.Player
{
    public static class PlayerActions
    {
        public static void CollectTileItem(Tile tile)
        {
            //Return if no lootbox is present
            if (string.IsNullOrEmpty(tile.TypeLootbox)) { return; }

            //Get the lootbox
            LootboxWindow window = new LootboxWindow(tile.TypeLootbox);
            window.ShowDialog();

            //Remove the lootbox from the tile
            tile.TypeLootbox = null;
            tile.TileElement.Child = null;
        }

        public static void AddToInventory(object item)
        {
            if (Inventory.Capacity == Inventory.PlayerInventory.Count)
            {
                MessageBox.Show("Sorry, but you don't have enough room in your inventory.");
                return;
            }

            Inventory.PlayerInventory.Add(item);
        }
    }
}
