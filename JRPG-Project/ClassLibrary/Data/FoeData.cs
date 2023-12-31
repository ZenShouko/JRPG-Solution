﻿using JRPG_Project.ClassLibrary.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Controls;

namespace JRPG_Project.ClassLibrary.Data
{
    public static class FoeData
    {
        public static List<Character> FoeList = new List<Character>();

        public static void InitializeList()
        {
            //Get the data from the json file
            string json = File.ReadAllText(@"../../Resources/Data/FoeList.json");
            FoeList = JsonConvert.DeserializeObject<List<Character>>(json);

            //Generate image for each character
            foreach (Character character in FoeList)
            {
                character.CharImage = CharacterData.GetCharacterImage(character.ImageName);
            }
        }

        /// <summary>
        /// Consists of 2 maniani and 1 pumpkin head.
        /// </summary>
        /// <returns></returns>
        public static List<Character> GetGenericFoeTeam()
        {
            List<Character> list = new List<Character>();
            list.Add(GetCharacter("F1"));
            list.Add(GetCharacter("F2"));
            list.Add(GetCharacter("F1"));

            //Modify second foe
            list[2].Name += " きびしい";

            return list;
        }

        public static Character GetCharacter(string id)
        {
            //Vars
            Character reference = FoeList.Find(x => x.ID == id);
            Character character = new Character();

            //Safety meassure
            if (reference is null)
            {
                return null;
            }
            
            //Copy the reference
            character.ID = id;
            character.Name = reference.Name;
            character.Description = reference.Description;
            character.Level = reference.Level;
            character.Stats.HP = reference.Stats.HP;
            character.Stats.DMG = reference.Stats.DMG;
            character.Stats.DEF = reference.Stats.DEF;
            character.Stats.SPD = reference.Stats.SPD;
            character.Stats.STA = reference.Stats.STA;
            character.Stats.STR = reference.Stats.STR;
            character.Stats.CRC = reference.Stats.CRC;
            character.Stats.CRD = reference.Stats.CRD;
            character.ImageName = reference.ImageName;

            Image img = new Image();
            img.Source = CharacterData.GetCharacterImage(reference.ImageName).Source;
            character.CharImage = img;

            return character;
        }
    }
}
