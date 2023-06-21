using JRPG_ClassLibrary;
using JRPG_ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace JRPG_Project.Tabs
{
    /// <summary>
    /// Interaction logic for PlaygroundTab.xaml
    /// </summary>
    public partial class PlaygroundTab : UserControl
    {
        public PlaygroundTab()
        {
            InitializeComponent();
            InitializePlayfield();
            RunKeyDetector();
        }

        private bool isKeyDetectorActive = false;

        private bool isPlayerMoving = false;

        private List<string> availableMobs = new List<string>()
        {
            "PLAYER", "ENEMY", "ITEM"
        };

        private List<string> availableTiles = new List<string>()
        {
            "DEFAULT", "SEA"
        };

        private List<Key> keys = new List<Key>()
        {
            Key.Up, Key.Right, Key.Down, Key.Left
        };

        private List<Mob> mobList = new List<Mob>();

        private async void RunKeyDetector()
        {
            //Set key detector active;
            isKeyDetectorActive = true;

            //Check if a key has been pressed
            while (isKeyDetectorActive)
            {
                //Wait 100ms
                await Task.Delay(10);

                if (Interaction.GetKey() is null)
                {
                    continue;
                }

                if (keys.Contains((Key)Interaction.GetKey()))
                {
                    MovePlayer(Interaction.GetKey().ToString().ToUpper());
                }
            }
        }

        private void StopKeyDetector()
        {
            isKeyDetectorActive = false;
        }

        private void InitializePlayfield()
        {
            //Fill the playfield with tiles
            //Get the amount of columns and rows of the playfield
            int columns = MainGrid.ColumnDefinitions.Count;
            int rows = MainGrid.RowDefinitions.Count;
            int totalFields = columns * rows;

            //Set current column and row
            int currentColumn = 0;
            int currentRow = 0;

            //Create tiles
            for (int i = 0; i < totalFields; i++)
            {
                //Create tile
                Border tile = CreateTile("DEFAULT");


                //Fill field from left to right, top to bottom
                if (currentColumn == columns)
                {
                    currentRow++;
                    currentColumn = 0;
                }

                Grid.SetColumn(tile, currentColumn);
                Grid.SetRow(tile, currentRow);

                //Add tile to playfield
                MainGrid.Children.Add(tile);
                currentColumn++;
            }

            //#Player
            //Create new player element
            Image player = CreateMob("PLAYER");

            //Add player to the center of the playfield
            Grid.SetColumn(player, 2);
            Grid.SetRow(player, 1);
            MainGrid.Children.Add(player);

            //Add player to mobList
            mobList.Add(new Mob()
            {
                ImageObject = player,
                CurrentX = 2,
                CurrentY = 1,
                Name = "Player"
            });


            //#Item
            //Create 3 new item element
            for (int i = 0; i < 5; i++)
            {
                Image item = CreateMob("ITEM");
                Grid.SetColumn(item, i * 2);
                Grid.SetRow(item, 2);
                MainGrid.Children.Add(item);

                //Add item to mobList
                mobList.Add(new Mob()
                {
                    ImageObject = item,
                    CurrentX = i * 2,
                    CurrentY = 2,
                    Name = "Item"
                });
            }
        }

        private Border CreateTile(string type)
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
                case "DEFAULT":
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

        private Image CreateMob(string request)
        {
            //Trim and convert to upper case
            request = request.Trim().ToUpper();

            //Check if request is available
            if (!availableMobs.Contains(request))
            {
                return null;
            }

            //Create image element
            Image mob = new Image();
            mob.BeginInit();
            mob.Height = 50;
            mob.Width = 50;
            mob.Stretch = System.Windows.Media.Stretch.Fill;
            //mob.EndInit();

            //Switch request
            switch (request)
            {
                case "PLAYER":
                    {
                        mob.Source = new BitmapImage(new Uri("../Resources/Assets/Characters/player.png", UriKind.Relative));
                        break;
                    }
                case "ITEM":
                    {
                        mob.Source = new BitmapImage(new Uri("../Resources/Assets/Items/item-box.png", UriKind.Relative));
                        break;
                    }
            }

            mob.EndInit();
            return mob;
        }

        private Mob GetPlayer()
        {
            //Check if there is a player element
            if (mobList.Contains(mobList.Find(mob => mob.Name == "Player")))
            {
                return mobList.Find(mob => mob.Name == "Player");
            }
            else
            {
                return null;
            }
        }

        private bool IsTargetTileFree(int targetX, int targetY)
        {
            //Check if the tile is out of bounds
            if (targetX < 0 || targetX > MainGrid.ColumnDefinitions.Count - 1 ||
                targetY < 0 || targetY > MainGrid.RowDefinitions.Count - 1)
            {
                return false;
            }

            //Check if the tile doesn't contain a mob
            if (mobList.Contains(mobList.Find(mob => mob.CurrentX == targetX && mob.CurrentY == targetY)))
            {
                return false;
            }

            return true;
        }

        private async void MovePlayer(string direction)
        {
            //Is player already moving?
            if (isPlayerMoving)
            {
                return;
            }

            //Get player element and image
            Mob player = GetPlayer();
            Image playerIcon = player.ImageObject as Image;

            //Cancel if player is not available
            if (player is null)
            {
                return;
            }

            //Get the amount of columns and rows of the playfield
            int columns = MainGrid.ColumnDefinitions.Count;
            int rows = MainGrid.RowDefinitions.Count;

            //Rotate player
            RotatePlayer(direction, player.ImageObject as Image);

            //Calculate the target tile
            int targetX = player.CurrentX;
            int targetY = player.CurrentY;

            switch (direction)
            {
                case "UP":
                    {
                        targetY--;
                        break;
                    }
                case "RIGHT":
                    {
                        targetX++;
                        break;
                    }
                case "DOWN":
                    {
                        targetY++;
                        break;
                    }
                case "LEFT":
                    {
                        targetX--;
                        break;
                    }
            }

            //Cancel if space is not free
            if (!IsTargetTileFree(targetX, targetY))
            {
                AnimatePlayerCollision(targetX, targetY, player);
                return;
            }

            //Move player
            AnimatePlayerMovement(targetX, targetY, player);
        }

        private async void AnimatePlayerMovement(int targetX, int targetY, Mob player)
        {
            //Set player moving to true
            isPlayerMoving = true;

            //Get player image
            Image playerIcon = player.ImageObject as Image;

            //What direction is the player moving
            int directionX = targetX - player.CurrentX; //either -1, 0 or 1; -1 = left, 0 = none, 1 = right
            int directionY = targetY - player.CurrentY; //either -1, 0 or 1; -1 = up, 0 = none, 1 = down

            //Get the size of the tiles
            double tileWidth = MainGrid.ColumnDefinitions[0].ActualWidth; //Output = 80
            double tileHeight = MainGrid.RowDefinitions[0].ActualHeight; //Output = 60

            //Get image margin
            Thickness margin = playerIcon.Margin;

            //Default props
            int speed = 5;
            int delay = 1;

            //Animate movement
            if (directionX == 1)
            {
                //Move right
                while(Math.Abs(margin.Right) < Math.Abs(tileWidth))
                {
                    margin.Left += speed;
                    margin.Right -= speed;
                    playerIcon.Margin = margin;
                    await Task.Delay(delay);
                }
            }
            else if (directionX == -1)
            {
                //Move left
                while (Math.Abs(margin.Right) < Math.Abs(tileWidth))
                {
                    margin.Left -= speed;
                    margin.Right += speed;
                    playerIcon.Margin = margin;
                    await Task.Delay(delay);
                }
            }
            else if (directionY == 1)
            {
                //Move down
                while (Math.Abs(margin.Top) < Math.Abs(tileHeight))
                {
                    margin.Top += speed;
                    margin.Bottom -= speed;
                    playerIcon.Margin = margin;
                    await Task.Delay(delay);
                }
            }
            else if (directionY == -1)
            {
                //Move up
                while (Math.Abs(margin.Bottom) < Math.Abs(tileHeight))
                {
                    margin.Top -= speed;
                    margin.Bottom += speed;
                    playerIcon.Margin = margin;
                    await Task.Delay(delay);
                }
            }

            //Apply changes
            playerIcon.Margin = new Thickness(0);

            Grid.SetColumn(playerIcon, targetX);
            player.CurrentX = targetX;
            Grid.SetRow(playerIcon, targetY);
            player.CurrentY = targetY;

            //Set player moving to false
            isPlayerMoving = false;
        }

        private async void AnimatePlayerCollision(int targetX, int targetY, Mob player)
        {
            //Set player moving to true
            isPlayerMoving = true;

            //Get player image
            Image playerIcon = player.ImageObject as Image;

            //What direction is the player moving
            int directionX = targetX - player.CurrentX; //either -1, 0 or 1; -1 = left, 0 = none, 1 = right
            int directionY = targetY - player.CurrentY; //either -1, 0 or 1; -1 = up, 0 = none, 1 = down

            //Get the size of the tiles
            double tileWidth = MainGrid.ColumnDefinitions[0].ActualWidth; //Output = 80
            double tileHeight = MainGrid.RowDefinitions[0].ActualHeight; //Output = 60

            //Get image margin
            Thickness margin = playerIcon.Margin;

            //Default props
            int speed = 2;
            int delay = 1;

            //Move player 50% of the tile and then back
            if (directionX == 1)
            {
                //Move forward
                while(Math.Abs(margin.Right) < Math.Abs(tileWidth / 4))
                {
                    margin.Left += speed;
                    margin.Right -= speed;
                    playerIcon.Margin = margin;
                    await Task.Delay(delay);
                }

                //Move back
                while (Math.Abs(margin.Right) > 0)
                {
                    margin.Left -= speed;
                    margin.Right += speed;
                    playerIcon.Margin = margin;
                    await Task.Delay(delay);
                }
            }
            else if (directionX == -1)
            {
                //Move forward
                while (Math.Abs(margin.Right) < Math.Abs(tileWidth / 4))
                {
                    margin.Left -= speed;
                    margin.Right += speed;
                    playerIcon.Margin = margin;
                    await Task.Delay(delay);
                }

                //Move back
                while (Math.Abs(margin.Right) > 0)
                {
                    margin.Left += speed;
                    margin.Right -= speed;
                    playerIcon.Margin = margin;
                    await Task.Delay(delay);
                }
            }
            else if (directionY == 1)
            {
                //Move forward
                while(Math.Abs(margin.Top) < Math.Abs(tileHeight / 4))
                {
                    margin.Top += speed;
                    margin.Bottom -= speed;
                    playerIcon.Margin = margin;
                    await Task.Delay(delay);
                }

                //Move back
                while (Math.Abs(margin.Top) > 0)
                {
                    margin.Top -= speed;
                    margin.Bottom += speed;
                    playerIcon.Margin = margin;
                    await Task.Delay(delay);
                }
            }
            else if (directionY == -1)
            {
                //Move forward
                while(Math.Abs(margin.Bottom) < Math.Abs(tileHeight / 4))
                {
                    margin.Top -= speed;
                    margin.Bottom += speed;
                    playerIcon.Margin = margin;
                    await Task.Delay(delay);
                }

                //Move back
                while (Math.Abs(margin.Bottom) > 0)
                {
                    margin.Top += speed;
                    margin.Bottom -= speed;
                    playerIcon.Margin = margin;
                    await Task.Delay(delay);
                }
            }

            isPlayerMoving = false;
        }

        private void RotatePlayer(string direction, Image player)
        {
            switch (direction)
            {
                case "UP":
                    {
                        //Rotate player up
                        player.RenderTransform = new RotateTransform(0);
                        break;
                    }
                case "RIGHT":
                    {
                        //Rotate player to the right
                        player.RenderTransform = new RotateTransform(90);
                        break;
                    }
                case "DOWN":
                    {
                        //Rotate player down
                        player.RenderTransform = new RotateTransform(180);
                        break;
                    }
                case "LEFT":
                    {
                        //Rotate player to the left
                        player.RenderTransform = new RotateTransform(-90);
                        break;
                    }
            }

            //Set player in the center
            player.RenderTransformOrigin = new Point(0.5, 0.5);

            //Set player in front of the tiles
            Panel.SetZIndex(player, 1);
        }
    }
}
