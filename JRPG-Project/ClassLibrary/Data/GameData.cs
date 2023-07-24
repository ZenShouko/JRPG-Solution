using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;

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

            //Load Player Data
            Load();

            //@Generate default team if no save file
            if (Inventory.Team.Count == 0)
            {
                Inventory.Team.Add(CharacterData.CharacterList.Find(x => x.ID == "CH1"));
                //Inventory.Team.Add(CharacterData.CharacterList.Find(x => x.ID == "CH2"));
                //Inventory.Team.Add(CharacterData.CharacterList.Find(x => x.ID == "CH3"));
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

            //Copy PlayerData to Inventory
            Inventory.Team = data.Team;
            Inventory.Capacity = data.Capacity;
            Inventory.LastSaveTime = data.LastSaveTime;

            foreach (string itemID in data.Collectables)
            {
                Inventory.Collectables.Add(ItemData.ListCollectables.Find(x => x.ID == itemID));
            }
            foreach (string itemID in data.Weapons)
            {
                Inventory.Weapons.Add(ItemData.ListWeapons.Find(x => x.ID == itemID));
            }
            foreach (string itemID in data.Armours)
            {
                Inventory.Armours.Add(ItemData.ListArmours.Find(x => x.ID == itemID));
            }
            foreach (string itemID in data.Amulets)
            {
                Inventory.Amulets.Add(ItemData.ListAmulets.Find(x => x.ID == itemID));
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
            PlayerData oldData = new PlayerData();

            //Check if file exists
            if (IsThereASaveFile())
            {
                //Read & Deserialize file
                string json = File.ReadAllText(@"../../Resources/Data/PlayerData.json");
                oldData = JsonConvert.DeserializeObject<PlayerData>(json);
            }

            //Serialize current PlayerData
            PlayerData newData = GetCurrentData();

            //#Compare
            //Check if collectables are different
            if (oldData.Collectables.Count != Inventory.Collectables.Count)
            {
                return true;
            }
            foreach (string itemID in oldData.Collectables)
            {
                if (Inventory.Collectables.Find(x => x.ID == itemID) == null)
                {
                    return true;
                }
            }

            //Check if weapons are different
            if (oldData.Weapons.Count != Inventory.Weapons.Count)
            {
                return true;
            }
            foreach (string itemID in oldData.Weapons)
            {
                if (Inventory.Weapons.Find(x => x.ID == itemID) == null)
                {
                    return true;
                }
            }

            //Check if armours are different
            if (oldData.Armours.Count != Inventory.Armours.Count)
            {
                return true;
            }
            foreach (string itemID in oldData.Armours)
            {
                if (Inventory.Armours.Find(x => x.ID == itemID) == null)
                {
                    return true;
                }
            }

            //Check if amulets are different
            if (oldData.Amulets.Count != Inventory.Amulets.Count)
            {
                return true;
            }
            foreach (string itemID in oldData.Amulets)
            {
                if (Inventory.Amulets.Find(x => x.ID == itemID) == null)
                {
                    return true;
                }
            }

            //No unsaved changes
            return false;
        }
        public static bool IsThereASaveFile()
        {
            return File.Exists(@"../../Resources/Data/PlayerData.json");
        }

        public static PlayerData GetCurrentData()
        {
            //Create PlayerData object
            PlayerData data = new PlayerData();

            //Copy Data To PlayerData [TODO: TEAM]
            data.Capacity = Inventory.Capacity;

            foreach (Collectable item in Inventory.Collectables)
            {
                data.Collectables.Add(item.ID);
            }
            foreach (Weapon item in Inventory.Weapons)
            {
                data.Weapons.Add(item.ID);
            }
            foreach (Armour item in Inventory.Armours)
            {
                data.Armours.Add(item.ID);
            }
            foreach (Amulet item in Inventory.Amulets)
            {
                data.Amulets.Add(item.ID);
            }

            data.LastSaveTime = DateTime.Now;

            return data;
        }
    }
}
