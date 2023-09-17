using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Entities.Serialization;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace JRPG_Project.ClassLibrary.Data
{
    public static class GameData
    {
        public static void InitializeData()
        {
            //Initialize Tile Table and add to database
            TileData.InitializeList();

            //Initialize XP dictionaries
            LevelData.InitializeDictionaries();

            //Initialize Item List
            ItemData.InitializeLists();

            //Initialize Character list
            CharacterData.InitializeList();

            //Initialize Foe list
            FoeData.InitializeList();

            //Initialize Lootbox list
            LootboxData.InitializeList();

            //Is this the first time the game is launched?
            if (!IsThereASaveFile())
            {
                InitializeFirstTime();
            }

            //Load Player Data
            Load();
        }

        private static void InitializeFirstTime()
        {
            //@Generate default team
            Inventory.Team.Add(CharacterData.CharacterList.Find(x => x.ID == "CH1"));
            Inventory.Team.Add(CharacterData.CharacterList.Find(x => x.ID == "CH2"));
            Inventory.Team.Add(CharacterData.CharacterList.Find(x => x.ID == "CH3"));

            //@Generate default inventory
            for (int i = 0; i < 3; i++)
            {
                PlayerActions.AddItem(ItemData.ListWeapons.Find(x => x.ID == "W1"));
            }
            PlayerActions.AddItem(ItemData.ListAmulets.Find(x => x.ID == "AM1"));

            //@Add All Materials
            foreach (Material material in ItemData.ListMaterials)
            {
                Inventory.Materials.Add(material.ID, 0);
            }
        }

        public static void Save()
        {
            //Serialize PlayerData
            string json = JsonConvert.SerializeObject(GetCurrentData());

            //Save to file
            File.WriteAllText(@"../../Resources/Data/PlayerData.json", json);
        }

        public static void Load()
        {
            //Check if file exists
            if (!IsThereASaveFile())
            {
                return;
            }

            //Read file
            string json = File.ReadAllText(@"../../Resources/Data/PlayerData.json");

            //Deserialize PlayerData
            PlayerData data = JsonConvert.DeserializeObject<PlayerData>(json);

            //#Copy PlayerData to Inventory
            Inventory.Capacity = data.Capacity;
            Inventory.Coins = data.Coins;
            Inventory.LastSaveTime = data.LastSaveTime;

            //#Copy items to inventory
            Inventory.Collectables = data.Collectables;
            Inventory.Weapons = data.Weapons;
            Inventory.Armours = data.Armours;
            Inventory.Amulets = data.Amulets;
            Inventory.Materials = data.Materials;

            //Generate image for each item
            foreach (Collectable item in Inventory.Collectables)
            {
                item.ItemImage = ItemData.GetItemImage(item);
            }
            foreach (Weapon item in Inventory.Weapons)
            {
                item.ItemImage = ItemData.GetItemImage(item);
            }
            foreach (Armour item in Inventory.Armours)
            {
                item.ItemImage = ItemData.GetItemImage(item);
            }
            foreach (Amulet item in Inventory.Amulets)
            {
                item.ItemImage = ItemData.GetItemImage(item);
            }

            //#Load Market
            Inventory.MarketRefresh = data.MarketRefresh;
            Inventory.MarketRequests = data.MarketRequests;

            //#Load team
            Inventory.Team = data.Team;
            for (int i = 0; i < 3; i++)
            {
                //Load image
                Inventory.Team[i].CharImage = CharacterData.GetCharacterImage(Inventory.Team[i].ImageName);

                //[!] Re-equip items
                if (Inventory.Team[i].Weapon != null)
                    Inventory.Team[i].Weapon = Inventory.Weapons.FirstOrDefault(x => x.UniqueID == Inventory.Team[i].Weapon.UniqueID);
                if (Inventory.Team[i].Armour != null)
                    Inventory.Team[i].Armour = Inventory.Armours.FirstOrDefault(x => x.UniqueID == Inventory.Team[i].Armour.UniqueID);
                if (Inventory.Team[i].Amulet != null)
                    Inventory.Team[i].Amulet = Inventory.Amulets.FirstOrDefault(x => x.UniqueID == Inventory.Team[i].Amulet.UniqueID);
            }
        }

        public static void Reset()
        {
            //Remove playerdata file
            try
            {
                File.Delete(@"../../Resources/Data/PlayerData.json");
            }
            catch { }
        }

        public static bool IsThereASaveFile()
        {
            //Checks if "PlayerData.json" exists in ../Resources/Data/
            return File.Exists(@"../../Resources/Data/PlayerData.json");
        }

        public static PlayerData GetCurrentData()
        {
            //Create PlayerData object
            PlayerData data = new PlayerData();

            //Copy Data To PlayerData
            data.Capacity = Inventory.Capacity;

            //#Items
            data.Weapons = Inventory.Weapons;
            data.Armours = Inventory.Armours;
            data.Amulets = Inventory.Amulets;
            data.Collectables = Inventory.Collectables;
            data.Materials = Inventory.Materials;

            //#Team
            data.Team = Inventory.Team;

            //#Economy
            data.Coins = Inventory.Coins;
            //data.ShoppingBag = Inventory.ShoppingBag;

            //#Save Time
            data.LastSaveTime = DateTime.Now;

            //#Market
            data.MarketRefresh = Inventory.MarketRefresh;
            data.MarketRequests = Inventory.MarketRequests;

            return data;
        }
    }
}
