using JRPG_ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Entities;
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

            //List that will contain the structure for creation
            List<string> structure = new List<string>();

            //Create tileList
            using (StreamReader reader = new StreamReader(filePath))
            {
                //Set level properties
                SetLevelProperties(reader.ReadLine());

                //#Tiles
                for (int i = 0; i < CurrentLevel.Rows; i++)
                {
                    structure.Add(reader.ReadLine());
                }

                AddTilesToTileList(structure);

                //#Player
                SetPlayer(reader.ReadLine());

                //#Mobs
                structure.Clear();
                while (!reader.EndOfStream)
                {
                    structure.Add(reader.ReadLine());
                }

                AddMobsToTileList(structure);

                //Create platform
                CreatePlatform();
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

        private static void AddTilesToTileList(List<string> structure)
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
                foreach (string tileName in tileNames)
                {
                    //Create tile
                    Tile tile = new Tile();
                    tile.Type = tileName;
                    tile.X = currentColumn;
                    tile.Y = currentRow;

                    //Is tile walkable?
                    if (tileName == "DEF")
                    {
                        tile.IsWalkable = true;
                    }
                    else
                    {
                        tile.IsWalkable = false;
                    }

                    //Create tile element
                    Border tileElement = CreateTile(tileName);

                    Grid.SetColumn(tileElement, currentColumn);
                    Grid.SetRow(tileElement, currentRow);

                    tile.TileElement = tileElement;

                    //Add to tile list
                    CurrentLevel.TileList.Add(tile);

                    //Next column
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
            Grid.SetZIndex(player.Icon, 100);

            //Add player to level
            CurrentLevel.Playfield.Children.Add(player.Icon);
            CurrentLevel.MobList.Add(player);
        }

        private static void AddMobsToTileList(List<string> mobStructure)
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

                    //CurrentLevel.Playfield.Children.Add(mob.Icon);
                    CurrentLevel.MobList.Add(mob);

                    //Add mob to tile
                    Tile tile = CurrentLevel.TileList.Find(t => t.X == currentColumn && t.Y == currentRow);
                    tile.MOB = mob;

                    currentColumn++;
                }

                //Next row
                currentRow++;
                currentColumn = 0;
            }
        }

        private static void CreatePlatform()
        {
            //Add tiles to playfield
            foreach (Tile tile in CurrentLevel.TileList)
            {
                CurrentLevel.Playfield.Children.Add(tile.TileElement);
                if (tile.MOB is null)
                {
                    continue;
                }
                else
                {
                    CurrentLevel.Playfield.Children.Add(tile.MOB.Icon);
                }
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

        public static void CollectItem(Tile tile)
        {
            //Remove mob from playfield
            CurrentLevel.Playfield.Children.Remove(tile.MOB.Icon);

            //Remove mob from moblist
            CurrentLevel.MobList.Remove(tile.MOB);

            //Remove mob from tile
            tile.MOB = null;

            //Add item to inventory
        }
    }
}
