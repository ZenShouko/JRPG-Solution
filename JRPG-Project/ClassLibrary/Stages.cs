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

namespace JRPG_ClassLibrary
{
    public static class Stages
    {
        public static Stage CurrentStage { get; set; }
        public static void CreateStage(Grid grid, string stageName)
        {
            //Does level exist?
            string filePath = Path.Combine(@"../../Levels/" + stageName + ".json");
            if (!File.Exists(filePath))
            {
                throw new Exception("Level does not exist.");
            }

            //Create level object
            CurrentStage = new Stage();

            //Read json file
            string json = File.ReadAllText(filePath);
            CurrentStage.TileList = JsonConvert.DeserializeObject<List<Tile>>(json);

            //Set default props platform
            CurrentStage.Platform = grid;
            CurrentStage.Platform.Width = 800;
            CurrentStage.Platform.Height = 600;
            CurrentStage.Platform.ColumnDefinitions.Clear();
            CurrentStage.Platform.RowDefinitions.Clear();

            SetLevelProperties(stageName);
            CreatePlatform();
        }

        private static void SetLevelProperties(string stagename)
        {
            //Stage name
            CurrentStage.Name = stagename;

            //Set amount of columns and rows
            CurrentStage.Columns = CurrentStage.TileList.Select(t => t.Position.X).Max() + 1;
            CurrentStage.Rows = CurrentStage.TileList.Select(t => t.Position.Y).Max() + 1;

            //Set tile width and height
            CurrentStage.TileWidth = (int)CurrentStage.Platform.Width / CurrentStage.Columns;
            CurrentStage.TileHeight = (int)CurrentStage.Platform.Height / CurrentStage.Rows;

            //Set the amount of columns of the platform
            for (int i = 0; i < CurrentStage.Columns; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(CurrentStage.TileWidth);
                CurrentStage.Platform.ColumnDefinitions.Add(col);
            }

            //Set the amount of rows of the platform
            for (int i = 0; i < CurrentStage.Rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(CurrentStage.TileHeight);
                CurrentStage.Platform.RowDefinitions.Add(new RowDefinition());
            }
        }

        public static void SetPlayer(string structure)
        {
            
        }

        private static void AddItemsToPlayfield(List<string> mobStructure)
        {
            
        }

        private static void CreatePlatform()
        {
            //Add tiles to playfield
            foreach (Tile tile in CurrentStage.TileList)
            {
                CurrentStage.Platform.Children.Add(tile.TileElement);

                //TODO: put this in a seperate method
                if (tile.Player is null)
                {
                    continue;
                }
                else
                {
                    CurrentStage.Platform.Children.Add(tile.Player.Icon);
                }
            }
        }

        private static void AddFoes(List<string> structure)
        {
            
        }
    }
}
