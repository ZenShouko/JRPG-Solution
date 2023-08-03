using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Entities.Serialization;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace JRPG_Project.ClassLibrary.Data
{
    public static class GameData
    {
        public static DataSet DB_Game = new DataSet("DB_JRPG"); //to be removed

        public static void InitializeData()
        {
            //TODO: CHANGE (Initialize Foe Table and add to database)
            //FoeData.InitializeTable();
            //DB_Game.Tables.Add(FoeData.FoeTable);

            //Initialize Tile Table and add to database
            TileData.InitializeList();

            //Initialize XP dictionaries
            LevelData.InitializeDictionaries();

            //Initialize Item List
            ItemData.InitializeLists();

            //Initialize Character list
            CharacterData.InitializeList();

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

            //(Test) Enhance first weapon
            Inventory.Weapons[0].Stats.DMG += 100;
            Inventory.Weapons[0].Stats.SPD += 100;
            Inventory.Weapons[0].Stats.DEF -= 100;
            Inventory.Weapons[0].Rarity = "CURSED";
            Inventory.Weapons[0].Name = "Cursed " + Inventory.Weapons[0].Name;
            Inventory.Weapons[0].Description = "Cursed by yo moms baking poweder ... !! \n >:-(";
            Inventory.Weapons[0].Value = 500;
            LevelData.AddXP(Inventory.Weapons[0], 2000);

            PlayerActions.AddItem(ItemData.ListAmulets.Find(x => x.ID == "AM1"));
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

            //Copy PlayerData to Inventory
            Inventory.Capacity = data.Capacity;
            Inventory.Coins = data.Coins;
            Inventory.Collectables = data.Collectables;
            Inventory.Weapons = data.Weapons;
            Inventory.Armours = data.Armours;
            Inventory.Amulets = data.Amulets;
            //Inventory.ShoppingBag = data.ShoppingBag;

            //Generate image for each item
            foreach (Collectable item in Inventory.Collectables)
            {
                item.ItemImage = ItemData.GetItemImage("Collectables/" + item.ImageName);
            }
            foreach (Weapon item in Inventory.Weapons)
            {
                item.ItemImage = ItemData.GetItemImage("Weapons/" + item.ImageName);
            }
            foreach (Armour item in Inventory.Armours)
            {
                item.ItemImage = ItemData.GetItemImage("Armours/" + item.ImageName);
            }
            foreach (Amulet item in Inventory.Amulets)
            {
                item.ItemImage = ItemData.GetItemImage("Amulets/" + item.ImageName);
            }

            Inventory.LastSaveTime = data.LastSaveTime;

            //Load team
            Inventory.Team = data.Team;
            for (int i = 0; i < 3; i++)
            {
                Inventory.Team[i].CharImage = CharacterData.GetCharacterImage(Inventory.Team[i].ImageName);
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

        public static bool HasUnsavedChanges()
        {
            //Scraped
            return false;
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

            //#Team
            data.Team = Inventory.Team;

            //#Economy
            data.Coins = Inventory.Coins;
            //data.ShoppingBag = Inventory.ShoppingBag;

            //#Save Time
            data.LastSaveTime = DateTime.Now;

            return data;
        }
    }
}
