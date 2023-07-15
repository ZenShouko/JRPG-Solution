using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
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

            //(Visible)Remove the lootbox from the tile
            tile.TileElement.Child = null;

            //Get the lootbox
            LootboxWindow window = new LootboxWindow(tile.TypeLootbox);
            window.ShowDialog();

            //Remove the lootbox from the tile
            tile.TypeLootbox = null;
        }

        public static void AddWeapon(Weapon weapon)
        {
            //Add the weapon to the inventory
            Inventory.Weapons.Add(weapon);
        }

        public static void AddArmour(Armour armour)
        {
            //Add the weapon to the inventory
            Inventory.Armours.Add(armour);
        }

        public static void AddAmulet(Amulet amulet)
        {
            //Add the weapon to the inventory
            Inventory.Amulets.Add(amulet);
        }

        public static void AddCollectable(Collectable collectable)
        {
            //Add the weapon to the inventory
            Inventory.Collectables.Add(collectable);
        }

        public static bool IsInventoryFull()
        {
            return Inventory.Capacity == (Inventory.Weapons.Count + Inventory.Armours.Count + Inventory.Amulets.Count);
        }
    }
}
