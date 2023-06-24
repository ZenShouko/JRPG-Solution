using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace JRPG_Project.ClassLibrary
{
    public static class Foes
    {
        public static List<string> FoeList = new List<string>() 
        {
            "ILLUFOE;"
        };

        public static Foe CreateFoe(string name)
        {
            DataRow row = FoeData.FoeTable.Select($"Name = '{name}'").FirstOrDefault();

            if (row is null) { return null; }

            Foe foe = new Foe()
            {
                Name = (string)row["Name"],
                Level = (int)row["Level"],
                HP = (int)row["HP"],
                IconNames = "foe-neutral;foe-alert",
                MovementBehaviour = (string)row["MovementBehaviour"],
            };

            string iconName = foe.IconNames.Split(';')[0];
            foe.Icon = new Image()
            {
                Source = new BitmapImage(new Uri($"../../Resources/Assets/Characters/{iconName}.png", UriKind.Relative)),
                Width = 50,
                Height = 50,
            };

            return foe;
        }
    }
}
