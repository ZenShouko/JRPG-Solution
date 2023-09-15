using JRPG_ClassLibrary.Entities;
using JRPG_Project.ClassLibrary;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Player;
using JRPG_Project.ClassLibrary.Universal;
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
        private static int playerSpeed = 6;
        private static int speedX = 0;
        private static int speedY = 0;
        private static int collisionSpeed = 3;
        private static int animationDelay = 1;


        public static List<Key> DirectionalKeys = new List<Key>()
        {
            Key.Up, Key.Right, Key.Down, Key.Left
        };

        private static MapPlayer Player = new MapPlayer();

        private static bool isPlayerMoving = false; //Do not allow player to move if already moving

        public async static void HandleInput(Key e)
        {
            if (isPlayerMoving) { return; }

            //Assign player to local variable, for simplicity sake
            Player = Stages.CurrentStage.Player;

            //Cancel actions if player is null
            if (Player is null) { return; }

            //Handle movement
            await StartPlayerMovementAsync(e.ToString().ToUpper());
        }

        public static async Task StartPlayerMovementAsync(string direction)
        {
            RotatePlayer(direction);
            //Is player already moving? Or is it the foe turn?
            if (isPlayerMoving /*|| FoeControls.FoeTurn*/) { return; }

            //Calculate the target tile
            Stages.CurrentStage.Player.Position.DirectionX = 0;
            Stages.CurrentStage.Player.Position.DirectionY = 0;

            switch (direction)
            {
                case "UP":
                    {
                        speedY = -1; //For animation
                        Player.Position.DirectionY = -1; //For placment
                        break;
                    }
                case "RIGHT":
                    {
                        speedX = 1;
                        Player.Position.DirectionX = 1;
                        break;
                    }
                case "DOWN":
                    {
                        speedY = 1;
                        Player.Position.DirectionY = 1;
                        break;
                    }
                case "LEFT":
                    {
                        speedX = -1;
                        Player.Position.DirectionX = -1;
                        break;
                    }
            }

            //Get target tile
            Tile targetTile = Stages.CurrentStage.TileList.Find(t =>
            t.Position.X == Player.Position.X + Player.Position.DirectionX &&
                           t.Position.Y == Player.Position.Y + Player.Position.DirectionY);

            //Collission detection
            if (targetTile is null || !targetTile.IsWalkable)
            {
                SoundManager.PlaySound("collision.wav");
                await AnimateCollision(targetTile);
            }
            else
            {
                //SoundManager.PlaySound("step.wav"); //Too taxing on SoundManager
                await AnimateMovement(targetTile);
            }
        }

        private static void RotatePlayer(string direction)
        {
            switch (direction)
            {
                case "UP":
                    {
                        Stages.CurrentStage.Player.Icon.RenderTransform = new RotateTransform(0);
                        break;
                    }
                case "RIGHT":
                    {
                        Stages.CurrentStage.Player.Icon.RenderTransform = new RotateTransform(90);
                        break;
                    }
                case "DOWN":
                    {
                        Stages.CurrentStage.Player.Icon.RenderTransform = new RotateTransform(180);
                        break;
                    }
                case "LEFT":
                    {
                        Stages.CurrentStage.Player.Icon.RenderTransform = new RotateTransform(-90);
                        break;
                    }
            }

            //Set player in the center
            Stages.CurrentStage.Player.Icon.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        private static async Task AnimateCollision(Tile targetTile)
        {
            //Set player moving to true
            isPlayerMoving = true;

            //Get image margin
            Thickness margin = Player.Icon.Margin;

            //Move
            while (Math.Abs(margin.Right) < 16 &&
                Math.Abs(margin.Top) < 16)
            {
                margin.Left += (playerSpeed / 2) * speedX;
                margin.Right -= (playerSpeed / 2) * speedX;
                margin.Top += (playerSpeed / 2) * speedY;
                margin.Bottom -= (playerSpeed / 2) * speedY;
                Player.Icon.Margin = margin;
                await Task.Delay(animationDelay);
            }

            //Move back
            while (Math.Abs(margin.Right) > 0 || Math.Abs(margin.Top) > 0)
            {
                margin.Left -= (playerSpeed / 2) * speedX;
                margin.Right += (playerSpeed / 2) * speedX;
                margin.Top -= (playerSpeed / 2) * speedY;
                margin.Bottom += (playerSpeed / 2) * speedY;
                Player.Icon.Margin = margin;
                await Task.Delay(animationDelay);
            }

            //Reset
            EndTurnAsync();

            //set player moving to false
            isPlayerMoving = false;
        }

        private static async Task AnimateMovement(Tile targetTile)
        {
            //Set player moving to true
            isPlayerMoving = true;

            //Get image margin
            Thickness margin = Player.Icon.Margin;

            //Move
            while (Math.Abs(margin.Right) < 24 &&
                Math.Abs(margin.Top) < 24)
            {
                margin.Left += playerSpeed * speedX;
                margin.Right -= playerSpeed * speedX;
                margin.Top += playerSpeed * speedY;
                margin.Bottom -= playerSpeed * speedY;
                Player.Icon.Margin = margin;
                await Task.Delay(animationDelay);
            }

            //Wait
            //await Task.Delay(0);

            //Reset
            MovePlayer(targetTile);

            //set player moving to false
            isPlayerMoving = false;
        }

        private static void MovePlayer(Tile tile)
        {
            //Remove player from current tile
            Tile playerTile = Stages.CurrentStage.TileList.Find(t => t.Position.X == Player.Position.X && t.Position.Y == Player.Position.Y);
            playerTile.Player = null;

            //Add player to new tile
            tile.Player = Player;

            //Apply changes
            Player.Icon.Margin = new Thickness(0);

            Player.Position.X = tile.Position.X;
            Player.Position.Y = tile.Position.Y;

            EndTurnAsync();
        }

        /// <summary>
        /// Ends the turn of player. This method will check for items or battles with foes.
        /// </summary>
        /// <param name="tile">Current tile of player.</param>
        private static async Task EndTurnAsync()
        {
            //Get current tile
            Tile tile = Stages.CurrentStage.TileList.Find(t => t.Position.X == Player.Position.X && t.Position.Y == Player.Position.Y);

            //Reset directional speed
            speedX = 0;
            speedY = 0;

            //Update visible platform
            Stages.UpdateVisiblePlatform();

            //Battle?
            Stages.BattleTrigger();

            //Collect item
            PlayerActions.CollectTileItem(tile);

            //Update visible platform
            Stages.UpdateVisiblePlatform();

            //If tile contains a lootbox, wait for the lootbox window to close
            if (tile.TypeLootbox != null)
            {
                //Wait for the lootbox window to close
                while (tile.TypeLootbox != null)
                {
                    await Task.Delay(100);
                }
            }

            //(Start foe turn)
            FoeControls.MoveFoes();
        }
    }
}
