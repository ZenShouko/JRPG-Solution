using JRPG_ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Player;
using JRPG_Project.ClassLibrary.Universal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace JRPG_ClassLibrary
{
    public static class Stages
    {
        public static Stage CurrentStage { get; set; }

        public static void CreateStage(Grid platformGrid, Grid radar, string stageName)
        {
            //[1]Initialize stage properties
            CurrentStage = new Stage();
            CurrentStage.Name = stageName.Split('.')[0];
            CurrentStage.VisiblePlatform = platformGrid;
            CurrentStage.FoeRadar = radar;

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

            //Create progression dictionary
            InitializeProgression();

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

        private static void InitializeProgression()
        {
            //Add foes to progression
            CurrentStage.Progression.Add("Foes", (0, CurrentStage.FoeList.Count));

            //Add lootboxes to progression
            CurrentStage.Progression.Add("Lootboxes", (0, CurrentStage.TileList.Count(t => t.TypeLootbox != null)));
        }

        public static void UpdateProgression()
        {
            //Update slayn foes
            CurrentStage.Progression["Foes"] = (CurrentStage.Progression["Foes"].Item2 - CurrentStage.FoeList.Count, CurrentStage.Progression["Foes"].Item2);

            //Update collected lootboxes
            CurrentStage.Progression["Lootboxes"] =
                (CurrentStage.Progression["Lootboxes"].Item2 - CurrentStage.TileList.Count(t => t.TypeLootbox != null), CurrentStage.Progression["Lootboxes"].Item2);
        }

        private static void PlacePlayer()
        {
            //Find tile where player is not null
            Tile playerTile = CurrentStage.TileList.Find(t => t.Player != null);

            //Set player position property
            CurrentStage.Player.Position = new Coordinates();
            CurrentStage.Player.Position.X = playerTile.Player.Position.X;
            CurrentStage.Player.Position.Y = playerTile.Player.Position.Y;

            //Place player on that tile
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
                    Border tileElement = tile.TileElement;
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

        public static void UpdateFoeRadar()
        {
            //Clear foe radar
            CurrentStage.FoeRadar.Children.Clear();

            //Vars
            Coordinates position = CurrentStage.TileList.Find(t => t.Player != null).Position;

            //Calculate how far it needs to scan
            int leftDistance = position.X - 7;
            leftDistance = leftDistance < 0 ? 0 : leftDistance;
            int rightDistance = position.X + 7;
            //Is rightDistance bigger than the biggest X in tile list?
            rightDistance = rightDistance > CurrentStage.TileList.Max(t => t.Position.X) ? CurrentStage.TileList.Max(t => t.Position.X) : rightDistance;
            int upDistance = position.Y - 7;
            upDistance = upDistance < 0 ? 0 : upDistance;
            int downDistance = position.Y + 7;
            //Is downDistance bigger than the biggest Y in tile list?
            downDistance = downDistance > CurrentStage.TileList.Max(t => t.Position.Y) ? CurrentStage.TileList.Max(t => t.Position.Y) : downDistance;

            //Place placer in the center of the radar
            Ellipse placer = GetDot("royalblue");
            Grid.SetColumn(placer, 1);
            Grid.SetRow(placer, 1);
            CurrentStage.FoeRadar.Children.Add(placer);

            //Scan all the way to the left
            List<Tile> scanLeft = CurrentStage.TileList
                .Where(x => x.Position.X >= leftDistance && x.Position.X < position.X)
                .Where(x => x.Position.Y == position.Y)
                .ToList();

            //Get the first 2 tiles closest to the player
            List<Tile> filterLeft = scanLeft
                .OrderByDescending(x => x.Position.X)
                .Take(2)
                .ToList();

            if (filterLeft.Any(x => x.Foe != null))
            {
                //Draw red dot
                Ellipse dot = GetDot("red");
                Grid.SetColumn(dot, 0);
                Grid.SetRow(dot, 1);
                CurrentStage.FoeRadar.Children.Add(dot);
            }
            else
            {
                //[Left] medium
                filterLeft = scanLeft
                    .OrderByDescending(x => x.Position.X)
                    .Take(5)
                    .ToList();

                if (filterLeft.Any(x => x.Foe != null))
                {
                    Ellipse dot = GetDot("orange");
                    Grid.SetColumn(dot, 0);
                    Grid.SetRow(dot, 1);
                    CurrentStage.FoeRadar.Children.Add(dot);
                }
                else
                {
                    //[Left] far
                    filterLeft = scanLeft
                        .OrderByDescending(x => x.Position.X)
                        .Take(8)
                        .ToList();

                    if (filterLeft.Any(x => x.Foe != null))
                    {
                        Ellipse dot = GetDot("yellow");
                        Grid.SetColumn(dot, 0);
                        Grid.SetRow(dot, 1);
                        CurrentStage.FoeRadar.Children.Add(dot);
                    }
                }
            }

            //[Right]
            List<Tile> scanRight = CurrentStage.TileList
                .Where(x => x.Position.X <= rightDistance && x.Position.X > position.X)
                .Where(x => x.Position.Y == position.Y)
                .ToList();

            //Get the first 2 tiles closest to the player
            List<Tile> filterRight = scanRight
                .OrderBy(x => x.Position.X)
                .Take(2)
                .ToList();


            if (filterRight.Any(x => x.Foe != null))
            {
                Ellipse dot = GetDot("red");
                Grid.SetColumn(dot, 2);
                Grid.SetRow(dot, 1);
                CurrentStage.FoeRadar.Children.Add(dot);
            }
            else
            {
                //[Right] medium
                filterRight = scanRight
                    .OrderBy(x => x.Position.X)
                    .Take(5)
                    .ToList();

                if (filterRight.Any(x => x.Foe != null))
                {
                    Ellipse dot = GetDot("orange");
                    Grid.SetColumn(dot, 2);
                    Grid.SetRow(dot, 1);
                    CurrentStage.FoeRadar.Children.Add(dot);
                }
                else
                {
                    //[Right] far
                    filterRight = scanRight
                        .OrderBy(x => x.Position.X)
                        .Take(8)
                        .ToList();

                    if (filterRight.Any(x => x.Foe != null))
                    {
                        Ellipse dot = GetDot("yellow");
                        Grid.SetColumn(dot, 2);
                        Grid.SetRow(dot, 1);
                        CurrentStage.FoeRadar.Children.Add(dot);
                    }
                }
            }

            //[Up] close
            //Scan all the way up
            List<Tile> scanUp = CurrentStage.TileList
                .Where(x => x.Position.Y >= upDistance && x.Position.Y < position.Y)
                .Where(x => x.Position.X == position.X)
                .ToList();

            //Get the first 2 tiles closest to the player
            List<Tile> filterUp = scanUp
                .OrderByDescending(x => x.Position.Y)
                .Take(2)
                .ToList();

            if (filterUp.Any(x => x.Foe != null))
            {
                Ellipse dot = GetDot("red");
                Grid.SetColumn(dot, 1);
                Grid.SetRow(dot, 0);
                CurrentStage.FoeRadar.Children.Add(dot);
            }
            else
            {
                //[Up] medium
                filterUp = scanUp
                    .OrderByDescending(x => x.Position.Y)
                    .Take(5)
                    .ToList();

                if (filterUp.Any(x => x.Foe != null))
                {
                    Ellipse dot = GetDot("orange");
                    Grid.SetColumn(dot, 1);
                    Grid.SetRow(dot, 0);
                    CurrentStage.FoeRadar.Children.Add(dot);
                }
                else
                {
                    //[Up] far
                    filterUp = scanUp
                        .OrderByDescending(x => x.Position.Y)
                        .Take(8)
                        .ToList();

                    if (filterUp.Any(x => x.Foe != null))
                    {
                        Ellipse dot = GetDot("yellow");
                        Grid.SetColumn(dot, 1);
                        Grid.SetRow(dot, 0);
                        CurrentStage.FoeRadar.Children.Add(dot);
                    }
                }
            }

            //[Down] close
            //Scan all the way down
            List<Tile> scanDown = CurrentStage.TileList
                .Where(x => x.Position.Y <= downDistance && x.Position.Y > position.Y)
                .Where(x => x.Position.X == position.X)
                .ToList();

            //Get the first 2 tiles closest to the player
            List<Tile> filterDown = scanDown
                .OrderBy(x => x.Position.Y)
                .Take(2)
                .ToList();

            if (filterDown.Any(x => x.Foe != null))
            {
                Ellipse dot = GetDot("red");
                Grid.SetColumn(dot, 1);
                Grid.SetRow(dot, 2);
                CurrentStage.FoeRadar.Children.Add(dot);
            }
            else
            {
                //[Down] medium
                filterDown = scanDown
                    .OrderBy(x => x.Position.Y)
                    .Take(5)
                    .ToList();

                if (filterDown.Any(x => x.Foe != null))
                {
                    Ellipse dot = GetDot("orange");
                    Grid.SetColumn(dot, 1);
                    Grid.SetRow(dot, 2);
                    CurrentStage.FoeRadar.Children.Add(dot);
                }
                else
                {
                    //[Down] far
                    filterDown = scanDown
                        .OrderBy(x => x.Position.Y)
                        .Take(8)
                        .ToList();

                    if (filterDown.Any(x => x.Foe != null))
                    {
                        Ellipse dot = GetDot("yellow");
                        Grid.SetColumn(dot, 1);
                        Grid.SetRow(dot, 2);
                        CurrentStage.FoeRadar.Children.Add(dot);
                    }
                }
            }

            //[Diagonal Left Up]
            //[Close]
            List<Tile> scanDiagonalLeftUp = new List<Tile>();
            scanDiagonalLeftUp.Add(CurrentStage.TileList
                .FirstOrDefault(x => x.Position.X == position.X - 1 && x.Position.Y == position.Y - 1));
            scanDiagonalLeftUp.Add(CurrentStage.TileList
                .FirstOrDefault(x => x.Position.X == position.X - 2 && x.Position.Y == position.Y - 1));
            scanDiagonalLeftUp.Add(CurrentStage.TileList
                .FirstOrDefault(x => x.Position.X == position.X - 1 && x.Position.Y == position.Y - 2));

            //Filter
            scanDiagonalLeftUp = scanDiagonalLeftUp.Where(x => x != null).ToList();

            if (scanDiagonalLeftUp.Any(x => x.Foe != null))
            {
                Ellipse dot = GetDot("red");
                Grid.SetColumn(dot, 0);
                Grid.SetRow(dot, 0);
                CurrentStage.FoeRadar.Children.Add(dot);
            }
            else
            {
                //[Medium]
                scanDiagonalLeftUp.Add(CurrentStage.TileList
                    .FirstOrDefault(t => t.Position.X == position.X - 3 && t.Position.Y == position.Y - 1));
                scanDiagonalLeftUp.Add(CurrentStage.TileList
                    .FirstOrDefault(t => t.Position.X == position.X - 2 && t.Position.Y == position.Y - 2));
                scanDiagonalLeftUp.Add(CurrentStage.TileList
                    .FirstOrDefault(t => t.Position.X == position.X - 1 && t.Position.Y == position.Y - 3));

                //Filter
                scanDiagonalLeftUp = scanDiagonalLeftUp.Where(x => x != null).ToList();

                if (scanDiagonalLeftUp.Any(x => x.Foe != null))
                {
                    Ellipse dot = GetDot("orange");
                    Grid.SetColumn(dot, 0);
                    Grid.SetRow(dot, 0);
                    CurrentStage.FoeRadar.Children.Add(dot);
                }
                else
                {
                    //[Far]
                    scanDiagonalLeftUp.Add(CurrentStage.TileList
                        .FirstOrDefault(t => t.Position.X == position.X - 4 && t.Position.Y == position.Y - 1));
                    scanDiagonalLeftUp.Add(CurrentStage.TileList
                        .FirstOrDefault(t => t.Position.X == position.X - 3 && t.Position.Y == position.Y - 2));
                    scanDiagonalLeftUp.Add(CurrentStage.TileList
                        .FirstOrDefault(t => t.Position.X == position.X - 2 && t.Position.Y == position.Y - 3));
                    scanDiagonalLeftUp.Add(CurrentStage.TileList
                        .FirstOrDefault(t => t.Position.X == position.X - 1 && t.Position.Y == position.Y - 4));

                    //Filter
                    scanDiagonalLeftUp = scanDiagonalLeftUp.Where(x => x != null).ToList();

                    if (scanDiagonalLeftUp.Any(x => x.Foe != null))
                    {
                        Ellipse dot = GetDot("yellow");
                        Grid.SetColumn(dot, 0);
                        Grid.SetRow(dot, 0);
                        CurrentStage.FoeRadar.Children.Add(dot);
                    }
                }
            }

            //[Diagonal Left Down]
            // [Close]
            List<Tile> scanDiagonalLeftDown = new List<Tile>();
            scanDiagonalLeftDown.Add(CurrentStage.TileList
                .FirstOrDefault(t => t.Position.X == position.X - 1 && t.Position.Y == position.Y + 1));
            scanDiagonalLeftDown.Add(CurrentStage.TileList
                .FirstOrDefault(t => t.Position.X == position.X - 2 && t.Position.Y == position.Y + 1));
            scanDiagonalLeftDown.Add(CurrentStage.TileList
                .FirstOrDefault(t => t.Position.X == position.X - 1 && t.Position.Y == position.Y + 2));

            //Filter
            scanDiagonalLeftDown = scanDiagonalLeftDown.Where(x => x != null).ToList();

            if (scanDiagonalLeftDown.Any(x => x.Foe != null))
            {
                Ellipse dot = GetDot("red");
                Grid.SetColumn(dot, 0);
                Grid.SetRow(dot, 2);
                CurrentStage.FoeRadar.Children.Add(dot);
            }
            else
            {
                //[Medium]
                scanDiagonalLeftDown.Add(CurrentStage.TileList
                    .FirstOrDefault(t => t.Position.X == position.X - 3 && t.Position.Y == position.Y + 1));
                scanDiagonalLeftDown.Add(CurrentStage.TileList
                    .FirstOrDefault(t => t.Position.X == position.X - 2 && t.Position.Y == position.Y + 2));
                scanDiagonalLeftDown.Add(CurrentStage.TileList
                    .FirstOrDefault(t => t.Position.X == position.X - 1 && t.Position.Y == position.Y + 3));

                //Filter
                scanDiagonalLeftDown = scanDiagonalLeftDown.Where(x => x != null).ToList();

                if (scanDiagonalLeftDown.Any(x => x.Foe != null))
                {
                    Ellipse dot = GetDot("orange");
                    Grid.SetColumn(dot, 0);
                    Grid.SetRow(dot, 2);
                    CurrentStage.FoeRadar.Children.Add(dot);
                }
                else
                {
                    //[Far]
                    scanDiagonalLeftDown.Add(CurrentStage.TileList
                        .FirstOrDefault(t => t.Position.X == position.X - 4 && t.Position.Y == position.Y + 1));
                    scanDiagonalLeftDown.Add(CurrentStage.TileList
                        .FirstOrDefault(t => t.Position.X == position.X - 3 && t.Position.Y == position.Y + 2));
                    scanDiagonalLeftDown.Add(CurrentStage.TileList
                        .FirstOrDefault(t => t.Position.X == position.X - 2 && t.Position.Y == position.Y + 3));
                    scanDiagonalLeftDown.Add(CurrentStage.TileList
                        .FirstOrDefault(t => t.Position.X == position.X - 1 && t.Position.Y == position.Y + 4));

                    //Filter
                    scanDiagonalLeftDown = scanDiagonalLeftDown.Where(x => x != null).ToList();

                    if (scanDiagonalLeftDown.Any(x => x.Foe != null))
                    {
                        Ellipse dot = GetDot("yellow");
                        Grid.SetColumn(dot, 0);
                        Grid.SetRow(dot, 2);
                        CurrentStage.FoeRadar.Children.Add(dot);
                    }
                }
            }

            //[Diagonal Right Up]
            //[Close range]
            List<Tile> scanDiagonalRightUp = new List<Tile>();
            scanDiagonalRightUp.Add(CurrentStage.TileList
                .FirstOrDefault(t => t.Position.X == position.X + 1 && t.Position.Y == position.Y - 1));
            scanDiagonalRightUp.Add(CurrentStage.TileList
                .FirstOrDefault(t => t.Position.X == position.X + 2 && t.Position.Y == position.Y - 1));
            scanDiagonalRightUp.Add(CurrentStage.TileList
                .FirstOrDefault(t => t.Position.X == position.X + 1 && t.Position.Y == position.Y - 2));

            //Filter
            scanDiagonalRightUp = scanDiagonalRightUp.Where(x => x != null).ToList();

            if (scanDiagonalRightUp.Any(x => x.Foe != null))
            {
                Ellipse dot = GetDot("red");
                Grid.SetColumn(dot, 2);
                Grid.SetRow(dot, 0);
                CurrentStage.FoeRadar.Children.Add(dot);
            }
            else
            {
                //[Medium Range] 
                scanDiagonalRightUp.Add(CurrentStage.TileList
                    .FirstOrDefault(t => t.Position.X == position.X + 3 && t.Position.Y == position.Y - 1));
                scanDiagonalRightUp.Add(CurrentStage.TileList
                    .FirstOrDefault(t => t.Position.X == position.X + 2 && t.Position.Y == position.Y - 2));
                scanDiagonalRightUp.Add(CurrentStage.TileList
                    .FirstOrDefault(t => t.Position.X == position.X + 1 && t.Position.Y == position.Y - 3));

                //Filter
                scanDiagonalRightUp = scanDiagonalRightUp.Where(x => x != null).ToList();

                if (scanDiagonalRightUp.Any(x => x.Foe != null))
                {
                    Ellipse dot = GetDot("orange");
                    Grid.SetColumn(dot, 2);
                    Grid.SetRow(dot, 0);
                    CurrentStage.FoeRadar.Children.Add(dot);
                }
                else
                {
                    //[Far Range]
                    scanDiagonalRightUp.Add(CurrentStage.TileList
                        .FirstOrDefault(t => t.Position.X == position.X + 4 && t.Position.Y == position.Y - 1));
                    scanDiagonalRightUp.Add(CurrentStage.TileList
                        .FirstOrDefault(t => t.Position.X == position.X + 3 && t.Position.Y == position.Y - 2));
                    scanDiagonalRightUp.Add(CurrentStage.TileList
                        .FirstOrDefault(t => t.Position.X == position.X + 2 && t.Position.Y == position.Y - 3));
                    scanDiagonalRightUp.Add(CurrentStage.TileList
                        .FirstOrDefault(t => t.Position.X == position.X + 1 && t.Position.Y == position.Y - 4));

                    //Filter empty tiles
                    scanDiagonalRightUp = scanDiagonalRightUp.Where(x => x != null).ToList();

                    if (scanDiagonalRightUp.Any(x => x.Foe != null))
                    {
                        Ellipse dot = GetDot("yellow");
                        Grid.SetColumn(dot, 2);
                        Grid.SetRow(dot, 0);
                        CurrentStage.FoeRadar.Children.Add(dot);
                    }
                }
            }

            //[Diagonal Right Down]

            List<Tile> scanDiagonalRightDown = new List<Tile>();
            scanDiagonalRightDown.Add(CurrentStage.TileList
                .FirstOrDefault(t => t.Position.X == position.X + 1 && t.Position.Y == position.Y + 1));
            scanDiagonalRightDown.Add(CurrentStage.TileList
                .FirstOrDefault(t => t.Position.X == position.X + 2 && t.Position.Y == position.Y + 1));
            scanDiagonalRightDown.Add(CurrentStage.TileList
                .FirstOrDefault(t => t.Position.X == position.X + 1 && t.Position.Y == position.Y + 2));

            //Filter
            scanDiagonalRightDown = scanDiagonalRightDown.Where(x => x != null).ToList();

            if (scanDiagonalRightDown.Any(x => x.Foe != null))
            {
                Ellipse dot = GetDot("red");
                Grid.SetColumn(dot, 2);
                Grid.SetRow(dot, 2);
                CurrentStage.FoeRadar.Children.Add(dot);
            }
            else
            {
                scanDiagonalRightDown.Add(CurrentStage.TileList
                    .FirstOrDefault(t => t.Position.X == position.X + 3 && t.Position.Y == position.Y + 1));
                scanDiagonalRightDown.Add(CurrentStage.TileList
                    .FirstOrDefault(t => t.Position.X == position.X + 2 && t.Position.Y == position.Y + 2));
                scanDiagonalRightDown.Add(CurrentStage.TileList
                    .FirstOrDefault(t => t.Position.X == position.X + 1 && t.Position.Y == position.Y + 3));

                //Filter
                scanDiagonalRightDown = scanDiagonalRightDown.Where(x => x != null).ToList();

                if (scanDiagonalRightDown.Any(x => x.Foe != null))
                {
                    Ellipse dot = GetDot("orange");
                    Grid.SetColumn(dot, 2);
                    Grid.SetRow(dot, 2);
                    CurrentStage.FoeRadar.Children.Add(dot);
                }
                else
                {
                    scanDiagonalRightDown.Add(CurrentStage.TileList
                        .FirstOrDefault(t => t.Position.X == position.X + 4 && t.Position.Y == position.Y + 1));
                    scanDiagonalRightDown.Add(CurrentStage.TileList
                        .FirstOrDefault(t => t.Position.X == position.X + 3 && t.Position.Y == position.Y + 2));
                    scanDiagonalRightDown.Add(CurrentStage.TileList
                        .FirstOrDefault(t => t.Position.X == position.X + 2 && t.Position.Y == position.Y + 3));
                    scanDiagonalRightDown.Add(CurrentStage.TileList
                        .FirstOrDefault(t => t.Position.X == position.X + 1 && t.Position.Y == position.Y + 4));

                    //Filter
                    scanDiagonalRightDown = scanDiagonalRightDown.Where(x => x != null).ToList();

                    if (scanDiagonalRightDown.Any(x => x.Foe != null))
                    {
                        Ellipse dot = GetDot("yellow");
                        Grid.SetColumn(dot, 2);
                        Grid.SetRow(dot, 2);
                        CurrentStage.FoeRadar.Children.Add(dot);
                    }
                }
            }
        }

        /// <summary>
        /// Directions include: up, down, left, right, diagonalLeftUp, diagonalLeftDown, diagonalRightUp, diagonalRightDown
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private static List<Coordinates> GetDetectionRange(string direction)
        {
            //hmm :/
            Coordinates playerPosition = CurrentStage.TileList.Find(x => x.Player != null).Position;
            List<Coordinates> range = new List<Coordinates>();


            if (direction.ToLower() == "left")
            {
                for (int i = 1; i < 4; i++)
                {
                    range.Add(new Coordinates() { X = playerPosition.X - i, Y = playerPosition.Y });
                }

                return range;
            }
            else if (direction.ToLower() == "right")
            {
                for (int i = 1; i < 10; i++)
                {
                    range.Add(new Coordinates() { X = playerPosition.X + i, Y = playerPosition.Y });
                }
            }

            return range;
        }

        /// <summary>
        /// Color options: red, orange, yellow, royalblue
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static Ellipse GetDot(string color)
        {
            Ellipse dot = new Ellipse();

            if (color == "red")
            {
                dot.Fill = Brushes.Red;
                dot.Width = 32;
                dot.Height = 32;
            }
            else if (color == "orange")
            {
                dot.Fill = Brushes.Orange;
                dot.Width = 24;
                dot.Height = 24;
            }
            else if (color == "yellow")
            {
                dot.Fill = Brushes.Yellow;
                dot.Width = 20;
                dot.Height = 20;
            }
            else if (color == "royalblue")
            {
                dot.Fill = Brushes.RoyalBlue;
                dot.Width = 32;
                dot.Height = 32;
            }

            return dot;
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
                CurrentStage.TeamHpDefDeduct.Add(character, (0, 0));
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
                SoundManager.PlaySound("foe-encounter.wav");
                //Start battle
                CurrentStage.IsBattle = true;
                CurrentStage.BattlingWith = toile.Foe;
                Interaction.OpenBattletab(toile.Foe);
            }
        }

        public static int GetProgressionRewards()
        {
            //Foreach lootbox, 2 coin. Foreach foe, 10 coin. Double rewards for completion.
            try
            {
                double coins = Interaction.LastProgression["Lootboxes"].Item1 * 2 + Interaction.LastProgression["Foes"].Item1 * 10;

                //BONUS
                if (Interaction.LastProgression["Lootboxes"].Item1 == Interaction.LastProgression["Lootboxes"].Item2 && Interaction.LastProgression["Lootboxes"].Item2 > 0)
                    coins *= 1.5;
                if (Interaction.LastProgression["Foes"].Item1 == Interaction.LastProgression["Foes"].Item2 && Interaction.LastProgression["Foes"].Item2 > 0)
                    coins *= 2;

                return Convert.ToInt16(coins);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
