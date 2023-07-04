using JRPG_ClassLibrary.Entities;
using JRPG_Project.ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.Windows.Media;

namespace JRPG_ClassLibrary
{
    public static class Stages
    {
        public static Stage CurrentStage { get; set; }
        public static void CreateStage(Grid grid, string stageName)
        {
            //[1]Initialize stage properties
            CurrentStage = new Stage();
            CurrentStage.Name = stageName;
            CurrentStage.Platform = grid;

            //Read stage data from file
            string json = File.ReadAllText($@"../../Stages/{stageName}");

            //Initialize stage properties
            InitializeStageProperties(json);

            //[2]Build stage
            BuildStage();

            //[3]Place player
            PlacePlayer();

            //[4]Place lootboxes
            PlaceLootboxes();
        }


        private static void InitializeStageProperties(string json)
        {
            //Clear grid
            CurrentStage.Platform.Children.Clear();
            CurrentStage.Platform.ColumnDefinitions.Clear();
            CurrentStage.Platform.RowDefinitions.Clear();

            //Get tile list
            CurrentStage.TileList = JsonConvert.DeserializeObject<List<Tile>>(json);

            //Initialize tile elements
            foreach (Tile tile in CurrentStage.TileList)
            {
                tile.TileElement = new Border();
                tile.TileElement.Background = tile.TileColor;
                tile.TileElement.BorderBrush = Brushes.Black;
                tile.TileElement.BorderThickness = new Thickness(1);
                tile.TileElement.CornerRadius = new CornerRadius(2);
            }

            //Set column and row properties
            CurrentStage.Columns = CurrentStage.TileList.Max(t => t.Position.X) + 1;
            CurrentStage.Rows = CurrentStage.TileList.Max(t => t.Position.Y) + 1;

            //Set column and row definitions
            for (int i = 0; i < CurrentStage.Columns; i++)
            {
                CurrentStage.Platform.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < CurrentStage.Rows; i++)
            {
                CurrentStage.Platform.RowDefinitions.Add(new RowDefinition());
            }

            //Platform props
            CurrentStage.Platform.Width = 800;
            CurrentStage.Platform.Height = 600;

            //Set tile width and height properties
            CurrentStage.TileWidth = (int)CurrentStage.Platform.Width / CurrentStage.Columns;
            CurrentStage.TileHeight = (int)CurrentStage.Platform.Height / CurrentStage.Rows;
        }

        private static void BuildStage()
        {
            ///Place all the tiles on the grid
            foreach(Tile tile in CurrentStage.TileList)
            {
                Grid.SetColumn(tile.TileElement, tile.Position.X);
                Grid.SetRow(tile.TileElement, tile.Position.Y);
                CurrentStage.Platform.Children.Add(tile.TileElement);
            }
        }

        private static void PlacePlayer()
        {
            //Find tile where player is not null
            Tile playerTile = CurrentStage.TileList.Find(t => t.Player != null);

            //Set player position property
            CurrentStage.Player.Position = playerTile.Player.Position; //Set player property
            
            //Place player on that tile
            Grid.SetColumn(CurrentStage.Player.Icon, playerTile.Position.X);
            Grid.SetRow(CurrentStage.Player.Icon, playerTile.Position.Y);
            CurrentStage.Platform.Children.Add(CurrentStage.Player.Icon);
        }

        private static void PlaceLootboxes()
        {
            foreach (Tile tile in CurrentStage.TileList.Where(t => t.TypeLootbox != null))
            {
                //Create lootbox
                MapLootbox lootbox = new MapLootbox();
                lootbox.Type = tile.TypeLootbox;
                lootbox.Position = tile.Position;

                //Place lootbox on tile
                //Grid.SetColumn(lootbox.Icon, tile.Position.X);
                //Grid.SetRow(lootbox.Icon, tile.Position.Y);
                //CurrentStage.Platform.Children.Add(lootbox.Icon);

                //Add lootbox icon to tile.element
                tile.TileElement.Child = lootbox.Icon;

                //[?]TODO: Create a list for lootboxes
            }
        }
    }
}
