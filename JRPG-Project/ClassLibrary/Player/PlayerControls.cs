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


        public static List<Key> DirectionalKeys = new List<Key>()
        {
            Key.Up, Key.Right, Key.Down, Key.Left
        };

        private static MapPlayer Player = new MapPlayer();

        private static bool isPlayerMoving = false; //Do not allow player to move if already moving

        public static void HandleInput(Key e)
        {
            if (isPlayerMoving) { return; }

            //Assign player to local variable, for simplicity sake
            Player = Stages.CurrentStage.Player;

            //Cancel actions if player is null
            if (Player is null) { return; }

            //Handle movement
            StartPlayerMovementAsync(e.ToString().ToUpper());
        }

        public static MapPlayer GetPlayer()
        {
            ////Check if there is a player element
            //if (Stages.CurrentStage.MobList.Contains(Stages.CurrentStage.MobList.Find(mob => mob.Name == "PLAYER")))
            //{
            //    return Stages.CurrentStage.MobList.Find(mob => mob.Name == "PLAYER");
            //}
            //else
            //{
            //    return null;
            //}
            throw new NotImplementedException();
        }

        public static void StartPlayerMovementAsync(string direction)
        {
            RotatePlayer(direction);
            ////Is player already moving? Or is it the foe turn?
            //if (isPlayerMoving || FoeControls.FoeTurn) { return; }

            ////Get player element and image
            //MapPlayer player = GetPlayer();

            ////Cancel if player is not available
            //if (player is null) { return; }

            ////Rotate player
            //RotatePlayer(direction, player.Icon);

            ////Calculate the target tile
            Stages.CurrentStage.Player.Position.DirectionX = 0;
            Stages.CurrentStage.Player.Position.DirectionY = 0;

            switch (direction)
            {
                case "UP":
                    {
                        speedY = -1;
                        Player.Position.DirectionY = -1;
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

            ////Collission detection
            if (targetTile is null || !targetTile.IsWalkable)
            {
                //await AnimateMovement(true, null);
            }
            else
            {
                //await AnimateMovement(false, targetTile);
                MovePlayer(targetTile);
            }

            ////(Start foe turn)
            //FoeControls.MoveFoes();
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

        private static async Task AnimateMovement(bool collision, Tile targetTile)
        {
            ////Set player moving to true
            isPlayerMoving = true;

            ////Get image margin
            Thickness margin = Player.Icon.Margin;

            ////Modify tile size
            int dividor = collision ? 4 : 1;
            int speed = collision ? collisionSpeed : playerSpeed;

            ////Move
            while (Math.Abs(margin.Right) < Math.Abs(Stages.CurrentStage.TileWidth / dividor) &&
                Math.Abs(margin.Top) < Math.Abs(Stages.CurrentStage.TileHeight / dividor))
            {
                margin.Left += speed * speedX;
                margin.Right -= speed * speedX;
                margin.Top += speed * speedY;
                margin.Bottom -= speed * speedY;
                Player.Icon.Margin = margin;
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
                    Player.Icon.Margin = margin;
                    await Task.Delay(animationDelay);
                }
            }
            else
            {
                MovePlayer(targetTile);
            }

            ////Reset
            speedX = 0;
            speedY = 0;



            //while (FoeControls.FoeTurn)
            //{
            //    await Task.Delay(50);
            //    continue;
            //}

            ////set player moving to false
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

            //Grid.SetColumn(Player.Icon, tile.Position.X);
            Player.Position.X = tile.Position.X;
            //Grid.SetRow(Player.Icon, tile.Position.Y);
            Player.Position.Y = tile.Position.Y;

            ////Is there a foe on the tile?
            //MapFoe foe = Stages.CurrentStage.FoeList.Find(f => f.X == tile.X && f.Y == tile.Y);
            //if (foe != null)
            //{
            //    //Attack foe
            //    BattleControls.InitiateBattle(true, foe);
            //}

            //Collect item
            PlayerActions.CollectTileItem(tile);

            //Update visible platform
            Stages.UpdateVisiblePlatform();
        }
    }
}
