using JRPG_ClassLibrary.Entities;
using JRPG_Project.ClassLibrary;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Player;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
//using static System.Net.Mime.MediaTypeNames;
using System.Windows.Media;

namespace JRPG_ClassLibrary
{
    public static class PlayerControls
    {
        public static Grid MainGrid { get; set; }
        private static int playerSpeed = 5;
        private static int speedX = 0;
        private static int speedY = 0;
        private static int collisionSpeed = 2;
        private static int animationDelay = 1;
        private static int tileWidth;
        private static int tileHeight;


        private static List<Key> keys = new List<Key>()
        {
            Key.Up, Key.Right, Key.Down, Key.Left
        };

        private static bool isKeyDetectorActive = false;

        private static bool isPlayerMoving = false; //Do not allow player to move if already moving
        public static void InitializeControls()
        {
            tileWidth = (int)Levels.CurrentLevel.Playfield.Width / Levels.CurrentLevel.Playfield.ColumnDefinitions.Count;
            tileHeight = (int)Levels.CurrentLevel.Playfield.Height / Levels.CurrentLevel.Playfield.RowDefinitions.Count;
        }

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
                    StartPlayerMovement(Interaction.GetKey().ToString().ToUpper());
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

        private static void StartPlayerMovement(string direction)
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
            int targetX = player.X;
            int targetY = player.Y;

            switch (direction)
            {
                case "UP":
                    {
                        speedY = -1;
                        targetY--;
                        break;
                    }
                case "RIGHT":
                    {
                        speedX = 1;
                        targetX++;
                        break;
                    }
                case "DOWN":
                    {
                        speedY = 1;
                        targetY++;
                        break;
                    }
                case "LEFT":
                    {
                        speedX = -1;
                        targetX--;
                        break;
                    }
            }

            //Cancel if space is not free
            if (Tiles.GetTile(targetX, targetY) is null || !Tiles.GetTile(targetX, targetY).IsWalkable)
            {
                AnimateMovement(true, player);
            }
            else
            {
                AnimateMovement(false, player);
            }
        }

        private static void RotatePlayer(string direction, Image player)
        {
            switch (direction)
            {
                case "UP":
                    {
                        player.RenderTransform = new RotateTransform(0);
                        break;
                    }
                case "RIGHT":
                    {
                        player.RenderTransform = new RotateTransform(90);
                        break;
                    }
                case "DOWN":
                    {
                        player.RenderTransform = new RotateTransform(180);
                        break;
                    }
                case "LEFT":
                    {
                        player.RenderTransform = new RotateTransform(-90);
                        break;
                    }
            }

            //Set player in the center
            player.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        private static async void AnimateMovement(bool collision, Mob player)
        {
            //Set player moving to true
            isPlayerMoving = true;

            //Get image margin
            Thickness margin = player.Icon.Margin;

            //Modify tile size
            int dividor = collision ? 4 : 1;
            int speed = collision ? collisionSpeed : playerSpeed;

            //Move
            while (Math.Abs(margin.Right) < Math.Abs(tileWidth / dividor) &&
                Math.Abs(margin.Top) < Math.Abs(tileHeight / dividor))
            {
                margin.Left += speed * speedX;
                margin.Right -= speed * speedX;
                margin.Top += speed * speedY;
                margin.Bottom -= speed * speedY;
                player.Icon.Margin = margin;
                await Task.Delay(animationDelay);
            }

            if (collision)
            {
                //Move back
                while (Math.Abs(margin.Right) > 0 || Math.Abs(margin.Top) > 0)
                {
                    margin.Left -= collisionSpeed * speedX;
                    margin.Right += collisionSpeed * speedX;
                    margin.Top -= collisionSpeed * speedY;
                    margin.Bottom += collisionSpeed * speedY;
                    player.Icon.Margin = margin;
                    await Task.Delay(animationDelay);
                }
            }
            else
            {
                MovePlayer(Tiles.GetTile(player.X + speedX, player.Y + speedY));
            }

            //Reset
            speedX = 0;
            speedY = 0;

            //Foe turn
            FoeControls.MoveFoes();

            //set player moving to false
            isPlayerMoving = false;
        }

        private static void MovePlayer(Tile tile)
        {
            Mob player = GetPlayer();

            //Apply changes
            player.Icon.Margin = new Thickness(0);

            Grid.SetColumn(player.Icon, tile.X);
            player.X = tile.X;
            Grid.SetRow(player.Icon, tile.Y);
            player.Y = tile.Y;

            //Collect item
            PlayerActions.CollectTileItem(tile);
        }


    }
}
