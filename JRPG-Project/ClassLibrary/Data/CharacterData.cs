using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Items;
using JRPG_Project.ClassLibrary.Player;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace JRPG_Project.ClassLibrary.Data
{
    public static class CharacterData
    {
        public static List<Character> CharacterList = new List<Character>();

        public static void InitializeList()
        {
            //Read json file
            string json = File.ReadAllText(@"../../Resources/Data/Characters.json");
            CharacterList = JsonConvert.DeserializeObject<List<Character>>(json);

            //Generate image for each character AND equip their default weapon
            foreach (Character character in CharacterList)
            {
                character.CharImage = GetCharacterImage(character.ImageName);
                ApplyDefaultEquipment(character);
            }
        }

        public static Image GetCharacterImage(string imageName)
        {
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Characters/" + imageName, UriKind.Relative));
            return img;
        }

        private static void ApplyDefaultEquipment(Character character)
        {
            /// '/' = no equipment || 1st = weapon || 2st = armour || 3st = amulet
            string[] parts = character.EquipmentIDs.Split(';');

            //Equip weapon
            if (parts[0] != "/")
            {
                character.Weapon = ItemData.ListWeapons.Find(x => x.ID == parts[0]);
            }

            //Equip armour
            if (parts[1] != "/")
            {
                character.Armour = ItemData.ListArmours.Find(x => x.ID == parts[1]);
            }

            //Equip amulet
            if (parts[2] != "/")
            {
                character.Amulet = ItemData.ListAmulets.Find(x => x.ID == parts[2]);
            }
        }

        public static void UnequipItem(BaseItem item)
        {
            if (item is Weapon wpn)
            {
                Inventory.Team[0].Weapon = Inventory.Team[0].Weapon == wpn ? null : Inventory.Team[0].Weapon;
                Inventory.Team[1].Weapon = Inventory.Team[1].Weapon == wpn ? null : Inventory.Team[1].Weapon;
                Inventory.Team[2].Weapon = Inventory.Team[2].Weapon == wpn ? null : Inventory.Team[2].Weapon;
            }
            else if (item is Armour arm)
            {
                Inventory.Team[0].Armour = Inventory.Team[0].Armour == arm ? null : Inventory.Team[0].Armour;
                Inventory.Team[1].Armour = Inventory.Team[1].Armour == arm ? null : Inventory.Team[1].Armour;
                Inventory.Team[2].Armour = Inventory.Team[2].Armour == arm ? null : Inventory.Team[2].Armour;
            }
            else if (item is Amulet amu)
            {
                Inventory.Team[0].Amulet = Inventory.Team[0].Amulet == amu ? null : Inventory.Team[0].Amulet;
                Inventory.Team[1].Amulet = Inventory.Team[1].Amulet == amu ? null : Inventory.Team[1].Amulet;
                Inventory.Team[2].Amulet = Inventory.Team[2].Amulet == amu ? null : Inventory.Team[2].Amulet;
            }
        }

        public static void EquipItem(BaseItem item, int charIndex)
        {
            UnequipItem(item);

            //Equip item
            if (item.UniqueID.Contains("weapon"))
            {
                //Cast item to weapon
                Weapon wpn = item as Weapon;

                //Equip weapon
                Inventory.Team[charIndex].Weapon = wpn;
            }
            else if (item.UniqueID.Contains("armour"))
            {
                Armour arm = item as Armour;
                Inventory.Team[charIndex].Armour = arm;
            }
            else if (item.UniqueID.Contains("amulet"))
            {
                Amulet amu = item as Amulet;
                Inventory.Team[charIndex].Amulet = amu;
            }
            else
            {
                throw new Exception("Item type not recognized.");
            }
        }

        public static void ClearEquipment(int charIndex, string type)
        {
            //Remove equipment
            if (type.ToUpper() == "WEAPON")
            {
                Inventory.Team[charIndex].Weapon = null;
            }
            else if (type.ToUpper() == "ARMOUR")
            {
                Inventory.Team[charIndex].Armour = null;
            }
            else if (type.ToUpper() == "AMULET")
            {
                Inventory.Team[charIndex].Amulet = null;
            }
        }
    }
}