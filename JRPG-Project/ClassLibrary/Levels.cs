using JRPG_ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JRPG_ClassLibrary
{
    public static class Levels
    {
        public static Level CurrentLevel { get; set; }
        public static void CreateLevel(Grid grid, string levelName)
        {
            string filePath = Path.Combine(@"../../Levels/" + levelName + ".csv");

            //Does level exist?
            if (!File.Exists(filePath))
            {
                throw new Exception("Level does not exist.");
            }

            //Create level object
            CurrentLevel = new Level();

            //Set playfield
            CurrentLevel.Playfield = grid;
            CurrentLevel.Playfield.Width = 800;
            CurrentLevel.Playfield.Height = 600;
            CurrentLevel.Playfield.ColumnDefinitions.Clear();
            CurrentLevel.Playfield.RowDefinitions.Clear();

            //Build level
            using (StreamReader reader = new StreamReader(filePath))
            {
                //Set level properties
                SetLevelProperties(reader.ReadLine());

                //List that will contain the structure for creation
                List<string> structure = new List<string>();

                //Create platform tiles
                for (int i = 0; i < CurrentLevel.Rows; i++)
                {
                    structure.Add(reader.ReadLine());
                }

                SetPlatformTiles(structure);


                //Get player position
                SetPlayer(reader.ReadLine());

                //Read mobs
                structure.Clear();
                while (!reader.EndOfStream)
                {
                    structure.Add(reader.ReadLine());
                }

                SetMobs(structure);
            }
        }

        private static void SetLevelProperties(string propLine)
        {
            string[] props = propLine.Split(';');

            CurrentLevel.Name = props[0];
            CurrentLevel.LevelNumber = Convert.ToInt32(props[1]);
            CurrentLevel.Columns = Convert.ToInt32(props[2]);
            CurrentLevel.Rows = Convert.ToInt32(props[3]);
        }

        private static void SetPlatformTiles(List<string> structure)
        {
            //Set the amount of columns of the playfield
            for (int i = 0; i < CurrentLevel.Columns; i++)
            {
                CurrentLevel.Playfield.ColumnDefinitions.Add(new ColumnDefinition());
            }

            //Set the amount of rows of the playfield
            for (int i = 0; i < CurrentLevel.Rows; i++)
            {
                CurrentLevel.Playfield.RowDefinitions.Add(new RowDefinition());
            }

            //Iterate through level design and set tiles
            int currentColumn = 0;
            int currentRow = 0;

            foreach (string row in structure)
            {
                string[] tileNames = row.Split(';');
                foreach(string tileName in tileNames)
                {
                    Border tileObject = CreateTile(tileName);

                    Grid.SetColumn(tileObject, currentColumn);
                    Grid.SetRow(tileObject, currentRow);

                    CurrentLevel.Playfield.Children.Add(tileObject);

                    currentColumn++;
                }

                //Next row
                currentRow++;
                currentColumn = 0;
            }
        }

        public static void SetPlayer(string structure)
        {
            //Create player
            Mob player = Mobs.CreateMob("Player");

            //Set player position
            string[] pos = structure.Split(';');
            player.CurrentX = Convert.ToInt32(pos[0]);
            player.CurrentY = Convert.ToInt32(pos[1]);

            //Create player
            Grid.SetColumn(player.Icon, player.CurrentX);
            Grid.SetRow(player.Icon, player.CurrentY);

            //Add player to level
            CurrentLevel.Playfield.Children.Add(player.Icon);
            CurrentLevel.MobList.Add(player);
        }

        private static void SetMobs(List<string> mobStructure)
        {
            int currentColumn = 0;
            int currentRow = 0;

            foreach (string line in mobStructure)
            {
                string[] mobs = line.Split(';');

                foreach (string mobName in mobs)
                {
                    if (mobName == "X")
                    {
                        currentColumn++;
                        continue;
                    }

                    Mob mob = Mobs.CreateMob(mobName);

                    Grid.SetColumn(mob.Icon, currentColumn);
                    mob.CurrentX = currentColumn;
                    Grid.SetRow(mob.Icon, currentRow);
                    mob.CurrentY = currentRow;

                    CurrentLevel.Playfield.Children.Add(mob.Icon);
                    CurrentLevel.MobList.Add(mob);

                    currentColumn++;
                }

                //Next row
                currentRow++;
                currentColumn = 0;
            }
        }

        private static List<string> availableTiles = new List<string>()
        {
            "DEF", "SEA"
        };

        private static Border CreateTile(string type)
        {
            //Check if type is available
            if (!availableTiles.Contains(type))
            {
                return null;
            }

            //Create tile
            Border tile = new Border();
            tile.BorderBrush = Brushes.Black;
            tile.BorderThickness = new Thickness(1);
            tile.CornerRadius = new CornerRadius(2);

            //Switch type
            switch (type)
            {
                case "DEF":
                    {
                        tile.Background = Brushes.Beige;
                        break;
                    }
                case "SEA":
                    {
                        tile.Background = Brushes.DarkSlateBlue;
                        break;
                    }
            }

            return tile;
        }
    }
}
