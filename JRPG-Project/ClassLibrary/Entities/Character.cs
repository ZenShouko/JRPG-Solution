﻿using JRPG_Project.ClassLibrary.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JRPG_Project.ClassLibrary.Entities
{
    public class Character
    {
        public string ID { get; set; }
        public string Name { get; set; } //Unique ... I don't remember what I meant by this
        public string Description { get; set; }
        public int Level { get; set; } = 1;
        public Stats Stats { get; set; } = new Stats();
        public string EquipmentIDs { get; set; }
        public string ImageName { get; set; }
        public Weapon Weapon { get; set; }
        public Armour Armour { get; set; }
        public Amulet Amulet { get; set; }
        public Image CharImage { get; set; }

        public Character()
        {
            //Assign default stats
            //Stats = new Stats() 
            //{ 
            //    HP = 40, DEF = 10, 
            //    DMG = 5, SPD = 20, 
            //    CRC = 20, CRD = 25,
            //    STA = 50, STR = 10
            //};
        }
    }
}
