using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using System;
using System.CodeDom;
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

            //(Visible)Remove the lootbox from the tile
            tile.TileElement.Child = null;

            //Get the lootbox
            LootboxWindow window = new LootboxWindow(tile.TypeLootbox);
            window.ShowDialog();

            //Remove the lootbox from the tile
            tile.TypeLootbox = null;
        }

        public static void AddItem(BaseItem referenceItem)
        {
            //Create a new item based on the reference item
            BaseItem item;

            if (referenceItem is Weapon)
            {
                item = new Weapon();
                item.CopyFrom(referenceItem);
                Inventory.Weapons.Add(item as Weapon);
            }
            else if (referenceItem is Armour)
            {
                item = new Armour();
                item.CopyFrom(referenceItem);
                Inventory.Armours.Add(item as Armour);
            }
            else if (referenceItem is Amulet)
            {
                item = new Amulet();
                item.CopyFrom(referenceItem);
                Inventory.Amulets.Add(item as Amulet);
            }
            else if (referenceItem is Collectable)
            {
                item = new Collectable();
                item.CopyFrom(referenceItem);
                Inventory.Collectables.Add(item as Collectable);
            }
            else
            {
                throw new Exception("Item type not supported.");
            }
        }

        public static bool IsInventoryFull()
        {
            return Inventory.Capacity == (Inventory.Weapons.Count + Inventory.Armours.Count + Inventory.Amulets.Count);
        }
    }
}
