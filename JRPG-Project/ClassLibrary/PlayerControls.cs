using JRPG_ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
//using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using JRPG_Project.ClassLibrary.Entities;

namespace JRPG_ClassLibrary
{
    public static class PlayerControls
    {
        public static Grid MainGrid { get; set; }
        private static int playerSpeed = 5;
        private static int collisionSpeed = 2;
        private static int animationDelay = 1;
        private static int tileWidth;
        private static int tileHeight;

        public static void InitializeControls()
        {
            tileWidth = (int)Levels.CurrentLevel.Playfield.Width / Levels.CurrentLevel.Playfield.ColumnDefinitions.Count;
            tileHeight = (int)Levels.CurrentLevel.Playfield.Height / Levels.CurrentLevel.Playfield.RowDefinitions.Count;
        }

        private static List<Key> keys = new List<Key>()
        {
            Key.Up, Key.Right, Key.Down, Key.Left
        };

        private static bool isKeyDetectorActive = false;

        private static bool isPlayerMoving = false;

        public static async void RunKeyDetector()
        {
            //Set key detector active;
            isKeyDetectorActive = true;

            //Initialize player controls
            InitializeControls();

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

        public static void StopKeyDetector()
        {
            isKeyDetectorActive = false;
        }

        private static Mob GetPlayer()
        {
            //Check if there is a player element
            if (Levels.CurrentLevel.MobList.Contains(Levels.CurrentLevel.MobList.Find(mob => mob.Name == "PLAYER")))
            {
                return Levels.CurrentLevel.MobList.Find(mob => mob.Name == "PLAYER");
            }
            else
            {
                return null;
            }
        }

        private static string CheckTargetTile(int targetX, int targetY)
        {
            //Check if the tile is out of bounds
            if (targetX < 0 || targetX > Levels.CurrentLevel.Columns - 1 ||
                targetY < 0 || targetY > Levels.CurrentLevel.Rows - 1)
            {
                return "VOID";
            }

            //Does tile contain a mob?
            if (Levels.CurrentLevel.MobList.Contains(Levels.CurrentLevel.MobList.Find(mob => mob.CurrentX == targetX && mob.CurrentY == targetY)))
            {
                return "STOP";
            }

            return "OK";
        }

        private static async void MovePlayer(string direction)
        {
            //Is player already moving?
            if (isPlayerMoving) { return; }

            //Get player element and image
            Mob player = GetPlayer();

            //Cancel if player is not available
            if (player is null) { return; }

            //Rotate player
            RotatePlayer(direction, player.Icon);

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

            //IS target tile available?


            //Cancel if space is not free
            if (!IsTileWalkable(targetX, targetY))
            {
                AnimatePlayerCollision(targetX, targetY, player);
                return;
            }

            //Move player
            AnimatePlayerMovement(targetX, targetY, player);

            //Does tile contain a collectable item?
            //CollectTileItem(targetX, targetY);
        }

        private static bool IsTileWalkable(int x, int y)
        {
            Tile tile = Levels.CurrentLevel.TileList.Find(t => t.X == x && t.Y == y);

            //Out of bounds?
            if (tile is null) { return false; }

            //Is tile walkable?
            return tile.IsWalkable;
        }

        private static void CollectTileItem(int x, int y)
        {
            Tile tile = Levels.CurrentLevel.TileList.Find(t => t.X == x && t.Y == y);

            if (tile is null || tile.MOB is null) { return; }

            //Does tile contain a collectable item?
            if (tile.MOB.Name == "ITEM")
            {
                Levels.CollectItem(tile);
            }
        }

        private static async void AnimatePlayerMovement(int targetX, int targetY, Mob player)
        {
            //Set player moving to true
            isPlayerMoving = true;

            //What direction is the player moving
            int directionX = targetX - player.CurrentX; //either -1, 0 or 1; -1 = left, 0 = none, 1 = right
            int directionY = targetY - player.CurrentY; //either -1, 0 or 1; -1 = up, 0 = none, 1 = down

            //Get image margin
            Thickness margin = player.Icon.Margin;

            //Animate movement
            if (directionX == 1)
            {
                //Move right
                while (Math.Abs(margin.Right) < Math.Abs(tileWidth))
                {
                    margin.Left += playerSpeed;
                    margin.Right -= playerSpeed;
                    player.Icon.Margin = margin;
                    await Task.Delay(animationDelay);
                }
            }
            else if (directionX == -1)
            {
                //Move left
                while (Math.Abs(margin.Right) < Math.Abs(tileWidth))
                {
                    margin.Left -= playerSpeed;
                    margin.Right += playerSpeed;
                    player.Icon.Margin = margin;
                    await Task.Delay(animationDelay);
                }
            }
            else if (directionY == 1)
            {
                //Move down
                while (Math.Abs(margin.Top) < Math.Abs(tileHeight))
                {
                    margin.Top += playerSpeed;
                    margin.Bottom -= playerSpeed;
                    player.Icon.Margin = margin;
                    await Task.Delay(animationDelay);
                }
            }
            else if (directionY == -1)
            {
                //Move up
                while (Math.Abs(margin.Bottom) < Math.Abs(tileHeight))
                {
                    margin.Top -= playerSpeed;
                    margin.Bottom += playerSpeed;
                    player.Icon.Margin = margin;
                    await Task.Delay(animationDelay);
                }
            }

            //Apply changes
            player.Icon.Margin = new Thickness(0);

            Grid.SetColumn(player.Icon, targetX);
            player.CurrentX = targetX;
            Grid.SetRow(player.Icon, targetY);
            player.CurrentY = targetY;

            //Is there a collectable item on the tile?
            CollectTileItem(targetX, targetY);

            //Set player moving to false
            isPlayerMoving = false;
        }

        private static async void AnimatePlayerCollision(int targetX, int targetY, Mob player)
        {
            //Set player moving to true
            isPlayerMoving = true;

            //What direction is the player moving
            int directionX = targetX - player.CurrentX; //either -1, 0 or 1; -1 = left, 0 = none, 1 = right
            int directionY = targetY - player.CurrentY; //either -1, 0 or 1; -1 = up, 0 = none, 1 = down

            //Get image margin
            Thickness margin = player.Icon.Margin;

            //Move player 50% of the tile and then back
            if (directionX == 1)
            {
                //Move forward
                while (Math.Abs(margin.Right) < Math.Abs(tileWidth / 4))
                {
                    margin.Left += collisionSpeed;
                    margin.Right -= collisionSpeed;
                    player.Icon.Margin = margin;
                    await Task.Delay(animationDelay);
                }

                //Move back
                while (Math.Abs(margin.Right) > 0)
                {
                    margin.Left -= collisionSpeed;
                    margin.Right += collisionSpeed;
                    player.Icon.Margin = margin;
                    await Task.Delay(animationDelay);
                }
            }
            else if (directionX == -1)
            {
                //Move forward
                while (Math.Abs(margin.Right) < Math.Abs(tileWidth / 4))
                {
                    margin.Left -= collisionSpeed;
                    margin.Right += collisionSpeed;
                    player.Icon.Margin = margin;
                    await Task.Delay(animationDelay);
                }

                //Move back
                while (Math.Abs(margin.Right) > 0)
                {
                    margin.Left += collisionSpeed;
                    margin.Right -= collisionSpeed;
                    player.Icon.Margin = margin;
                    await Task.Delay(animationDelay);
                }
            }
            else if (directionY == 1)
            {
                //Move forward
                while (Math.Abs(margin.Top) < Math.Abs(tileHeight / 4))
                {
                    margin.Top += collisionSpeed;
                    margin.Bottom -= collisionSpeed;
                    player.Icon.Margin = margin;
                    await Task.Delay(animationDelay);
                }

                //Move back
                while (Math.Abs(margin.Top) > 0)
                {
                    margin.Top -= collisionSpeed;
                    margin.Bottom += collisionSpeed;
                    player.Icon.Margin = margin;
                    await Task.Delay(animationDelay);
                }
            }
            else if (directionY == -1)
            {
                //Move forward
                while (Math.Abs(margin.Bottom) < Math.Abs(tileHeight / 4))
                {
                    margin.Top -= collisionSpeed;
                    margin.Bottom += collisionSpeed;
                    player.Icon.Margin = margin;
                    await Task.Delay(animationDelay);
                }

                //Move back
                while (Math.Abs(margin.Bottom) > 0)
                {
                    margin.Top += collisionSpeed;
                    margin.Bottom -= collisionSpeed;
                    player.Icon.Margin = margin;
                    await Task.Delay(animationDelay);
                }
            }

            isPlayerMoving = false;
        }

        private static void RotatePlayer(string direction, Image player)
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
