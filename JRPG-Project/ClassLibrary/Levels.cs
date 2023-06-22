using JRPG_ClassLibrary.Entities;
using JRPG_Project.ClassLibrary;
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

            //List that will contain the structure for creation (To pass on to other methods)
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
                for (int i = 0; i < CurrentLevel.Rows; i++)
                {
                    structure.Add(reader.ReadLine());
                }

                AddMobsToTileList(structure);

                //#Collectables
                CreateCollectablesList(reader.ReadLine());

                //Create platform
                CreatePlatform();

                //#Foes
                structure.Clear();
                for (int i = 0; i < CurrentLevel.Rows; i++)
                {
                    structure.Add(reader.ReadLine());
                }

                AddFoes(structure);
            }
        }

        private static void SetLevelProperties(string propLine)
        {
            string[] props = propLine.Split(';');

            CurrentLevel.Name = props[0];
            CurrentLevel.LevelNumber = Convert.ToInt32(props[1]);
            CurrentLevel.Columns = Convert.ToInt32(props[2]);
            CurrentLevel.Rows = Convert.ToInt32(props[3]);
            CurrentLevel.TileWidth = (int)CurrentLevel.Playfield.Width / CurrentLevel.Columns;
            CurrentLevel.TileHeight = (int)CurrentLevel.Playfield.Height / CurrentLevel.Rows;
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
                    Border tileElement = Tiles.CreateTile(tileName);

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
            player.X = Convert.ToInt32(pos[0]);
            player.Y = Convert.ToInt32(pos[1]);

            //Create player
            Grid.SetColumn(player.Icon, player.X);
            Grid.SetRow(player.Icon, player.Y);
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
                    mob.X = currentColumn;
                    Grid.SetRow(mob.Icon, currentRow);
                    mob.Y = currentRow;

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

        private static void CreateCollectablesList(string line)
        {
            string[] collectables = line.Split(';');
            foreach (string collectable in collectables)
            {
                CurrentLevel.Collectables.Add(collectable);
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

        private static void AddFoes(List<string> structure)
        {
            int x = 0;
            int y = 0;

            foreach (string line in structure)
            {
                string[] parts = line.Split(';');

                foreach (string foeName in parts)
                {
                    if (foeName == "X")
                    {
                        x++;
                        continue;
                    }

                    Foe foe = Foes.CreateFoe(foeName);

                    Grid.SetColumn(foe.Icon, x);
                    foe.X = x;
                    Grid.SetRow(foe.Icon, y);
                    foe.Y = y;

                    CurrentLevel.Playfield.Children.Add(foe.Icon);
                    CurrentLevel.FoeList.Add(foe);
                    x++;
                }

                //Next row
                y++;
                x = 0;
            }
        }

    }
}
