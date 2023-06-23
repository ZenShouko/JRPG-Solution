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


        private static List<Key> keys = new List<Key>()
        {
            Key.Up, Key.Right, Key.Down, Key.Left
        };

        private static bool isKeyDetectorActive = false;

        private static bool isPlayerMoving = false; //Do not allow player to move if already moving

        public static async void RunKeyDetector()
        {
            //Set key detector active;
            isKeyDetectorActive = true;

            //Check if a key has been pressed
            while (isKeyDetectorActive)
            {
                //Wait 100ms
                await Task.Delay(50);

                if (Interaction.GetKey() is null)
                {
                    continue;
                }

                if (keys.Contains((Key)Interaction.GetKey()))
                {
                    await StartPlayerMovementAsync(Interaction.GetKey().ToString().ToUpper());
                }
            }
        }

        public static void StopKeyDetector()
        {
            isKeyDetectorActive = false;
        }

        public static Mob GetPlayer()
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

        private static async Task StartPlayerMovementAsync(string direction)
        {
            //Is player already moving? Or is it the foe turn?
            if (isPlayerMoving || FoeControls.FoeTurn) { return; }

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

            //Collission detection
            if (Tiles.GetTile(targetX, targetY) is null || !Tiles.GetTile(targetX, targetY).IsWalkable)
            {
                await AnimateMovement(true, player);
            }
            else
            {
                await AnimateMovement(false, player);
            }

            //(Start foe turn)
            FoeControls.MoveFoes();
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

        private static async Task AnimateMovement(bool collision, Mob player)
        {
            //Set player moving to true
            isPlayerMoving = true;

            //Get image margin
            Thickness margin = player.Icon.Margin;

            //Modify tile size
            int dividor = collision ? 4 : 1;
            int speed = collision ? collisionSpeed : playerSpeed;

            //Move
            while (Math.Abs(margin.Right) < Math.Abs(Levels.CurrentLevel.TileWidth / dividor) &&
                Math.Abs(margin.Top) < Math.Abs(Levels.CurrentLevel.TileHeight / dividor))
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



            while (FoeControls.FoeTurn)
            {
                await Task.Delay(50);
                continue;
            }

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
