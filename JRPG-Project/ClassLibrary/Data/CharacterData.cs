﻿using JRPG_Project.ClassLibrary.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private static Image GetCharacterImage(string imageName)
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
    }
}
