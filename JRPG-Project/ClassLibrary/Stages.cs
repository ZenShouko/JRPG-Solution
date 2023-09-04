using JRPG_ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Player;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            CurrentStage.VisiblePlatform = grid;

            //Read stage data from file
            string json = File.ReadAllText($@"../../Stages/{stageName}");

            //Initialize stage properties
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

            //Add foes to foelist
            InitializeFoes();

            //[4]Place lootboxes
            PlaceLootboxes();

            //[5]Create visible platform
            CreateVisiblePlatform();
            CurrentStage.TileWidth = CurrentStage.VisiblePlatform.Width / CurrentStage.Columns;
            CurrentStage.TileHeight = CurrentStage.VisiblePlatform.Height / CurrentStage.Rows;

            //[5.5]Update visible platform
            UpdateVisiblePlatform();

            //[6]Place player
            PlacePlayer();

            //[7]Place team
            LoadTeam();
        }

        private static void InitializeStageProperties(string json)
        {
            ////Clear grid
            //CurrentStage.Platform.Children.Clear();
            //CurrentStage.Platform.ColumnDefinitions.Clear();
            //CurrentStage.Platform.RowDefinitions.Clear();

            ////Get tile list
            //CurrentStage.TileList = JsonConvert.DeserializeObject<List<Tile>>(json);

            ////Initialize tile elements
            //foreach (Tile tile in CurrentStage.TileList)
            //{
            //    tile.TileElement = new Border();
            //    tile.TileElement.Background = tile.TileColor;
            //    tile.TileElement.BorderBrush = Brushes.Black;
            //    tile.TileElement.BorderThickness = new Thickness(1);
            //    tile.TileElement.CornerRadius = new CornerRadius(2);
            //}

            ////Set column and row properties
            //CurrentStage.Columns = CurrentStage.TileList.Max(t => t.Position.X) + 1;
            //CurrentStage.Rows = CurrentStage.TileList.Max(t => t.Position.Y) + 1;

            ////Set column and row definitions
            //for (int i = 0; i < CurrentStage.Columns; i++)
            //{
            //    CurrentStage.Platform.ColumnDefinitions.Add(new ColumnDefinition());
            //}
            //for (int i = 0; i < CurrentStage.Rows; i++)
            //{
            //    CurrentStage.Platform.RowDefinitions.Add(new RowDefinition());
            //}

            ////Platform props
            //CurrentStage.Platform.Width = 800;
            //CurrentStage.Platform.Height = 600;

            ////Set tile width and height properties
            //CurrentStage.TileWidth = (int)CurrentStage.Platform.Width / CurrentStage.Columns;
            //CurrentStage.TileHeight = (int)CurrentStage.Platform.Height / CurrentStage.Rows;
        }

        private static void BuildStage()
        {
            /////Place all the tiles on the grid
            //foreach (Tile tile in CurrentStage.TileList)
            //{
            //    Grid.SetColumn(tile.TileElement, tile.Position.X);
            //    Grid.SetRow(tile.TileElement, tile.Position.Y);
            //    CurrentStage.Platform.Children.Add(tile.TileElement);
            //}
        }

        private static void PlacePlayer()
        {
            //Find tile where player is not null
            Tile playerTile = CurrentStage.TileList.Find(t => t.Player != null);

            //Set player position property
            CurrentStage.Player.Position = playerTile.Player.Position; //Set player property

            //Place player on that tile
            //Grid.SetColumn(CurrentStage.Player.Icon, playerTile.Position.X);
            //Grid.SetRow(CurrentStage.Player.Icon, playerTile.Position.Y);
            Grid.SetColumn(CurrentStage.Player.Icon, 4);
            Grid.SetRow(CurrentStage.Player.Icon, 4);

            CurrentStage.VisiblePlatform.Children.Remove(CurrentStage.Player.Icon);
            CurrentStage.VisiblePlatform.Children.Add(CurrentStage.Player.Icon);
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

        private static void CreateVisiblePlatform()
        {
            //Clear visible platform
            CurrentStage.VisiblePlatform.Children.Clear();

            //Create 9x9 grid, 65x65 each
            for (int i = 0; i < 9; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(65);
                CurrentStage.VisiblePlatform.ColumnDefinitions.Add(col);
            }

            for (int i = 0; i < 9; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(65);
                CurrentStage.VisiblePlatform.RowDefinitions.Add(row);
            }

            //Set visible platform properties
            CurrentStage.VisiblePlatform.Width = 585;
            CurrentStage.VisiblePlatform.Height = 585;
            CurrentStage.VisiblePlatform.HorizontalAlignment = HorizontalAlignment.Center;
            CurrentStage.VisiblePlatform.VerticalAlignment = VerticalAlignment.Center;
        }

        public static void UpdateVisiblePlatform()
        {
            //Clear visible platform
            CurrentStage.VisiblePlatform.Children.Clear();

            //Get tile where player is not null
            Tile playerTile = CurrentStage.TileList.Find(t => t.Player != null);

            //Copy tiles to visible platform, starting from player position, 4x4 around player
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    //Get tile
                    Tile tile = CurrentStage.TileList.Find(t => t.Position.X == playerTile.Position.X - 4 + x && t.Position.Y == playerTile.Position.Y - 4 + y);

                    //[!] If tile is null, insert empty tile
                    if (tile == null)
                    {
                        InsertEmptyTile(x, y);
                        continue;
                    }

                    //Copy tile to visible platform
                    Border tileElement = new Border();
                    tileElement.Background = tile.TileColor;
                    tileElement.BorderBrush = Brushes.Black;
                    tileElement.BorderThickness = new Thickness(1);
                    tileElement.CornerRadius = new CornerRadius(2);
                    Grid.SetColumn(tileElement, x);
                    Grid.SetRow(tileElement, y);
                    Panel.SetZIndex(tileElement, 1);
                    CurrentStage.VisiblePlatform.Children.Add(tileElement);

                    //Copy lootbox to visible platform
                    if (tile.TypeLootbox != null)
                    {
                        //Create lootbox
                        MapLootbox lootbox = new MapLootbox();
                        lootbox.Type = tile.TypeLootbox;
                        lootbox.Position = tile.Position;

                        //Place lootbox on tile
                        Grid.SetColumn(lootbox.Icon, x);
                        Grid.SetRow(lootbox.Icon, y);
                        Panel.SetZIndex(lootbox.Icon, 2);
                        CurrentStage.VisiblePlatform.Children.Add(lootbox.Icon);
                    }

                    //Copy foe to visible platform
                    if (tile.Foe != null)
                    {
                        //Place foe on tile
                        Grid.SetColumn(tile.Foe.Icon, x);
                        Grid.SetRow(tile.Foe.Icon, y);
                        Panel.SetZIndex(tile.Foe.Icon, 3);
                        CurrentStage.VisiblePlatform.Children.Add(tile.Foe.Icon);
                    }
                }
            }

            //Copy player to visible platform
            CurrentStage.VisiblePlatform.Children.Remove(CurrentStage.Player.Icon);
            Panel.SetZIndex(CurrentStage.Player.Icon, 4);
            CurrentStage.VisiblePlatform.Children.Add(CurrentStage.Player.Icon);
        }

        private static void InsertEmptyTile(int x, int y)
        {
            Border tileElement = new Border();
            tileElement.Background = Brushes.Black;
            tileElement.BorderBrush = Brushes.Gray;
            tileElement.BorderThickness = new Thickness(1);
            tileElement.CornerRadius = new CornerRadius(2);
            Grid.SetColumn(tileElement, x);
            Grid.SetRow(tileElement, y);
            CurrentStage.VisiblePlatform.Children.Add(tileElement);
        }

        private static void InitializeFoes()
        {
            foreach (Tile tile in CurrentStage.TileList)
            {
                if (tile.Foe != null)
                {
                    InitializeFoeImages(tile.Foe); //Images are not saved in stagedata
                    CurrentStage.FoeList.Add(tile.Foe);
                }
            }
        }

        private static void InitializeFoeImages(MapFoe mf)
        {
            foreach (Character foe in mf.FoeTeam)
            {
                foe.CharImage = new Image();
                foe.CharImage.Source = FoeData.FoeList.FirstOrDefault(x => x.ID == foe.ID).CharImage.Source;
            }
        }

        private static void LoadTeam()
        {
            foreach (Character character in Inventory.Team)
            {
                CurrentStage.Team.Add(character);
            }
        }

        /// <summary>
        /// Checks if player and foe are on the same tile. If so, start battle.
        /// </summary>
        public static void BattleTrigger()
        {
            //[!] Battle trigger can be triggered even though a battle is going on. Hence why this safety check.
            if (CurrentStage.IsBattle) { return; }

            //Check if player is on a tile with foe
            Tile toile = CurrentStage.TileList.Find(t => t.Player != null && t.Foe != null);
            if (toile != null)
            {
                //Start battle
                CurrentStage.IsBattle = true;
                CurrentStage.BattlingWith = toile.Foe;
                Interaction.OpenBattletab(toile.Foe);
            }
        }
    }
}
