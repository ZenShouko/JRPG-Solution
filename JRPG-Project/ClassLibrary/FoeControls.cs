using JRPG_ClassLibrary;
using JRPG_ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;

namespace JRPG_Project.ClassLibrary
{
    public static class FoeControls
    {
        public static bool FoeTurn { get; private set; } = false;
        private static int speed = 16; //might remove
        private static int animationDelay = 10; //might remove
        private static List<string> directions = new List<string>()
            { "UP", "DOWN", "LEFT", "RIGHT" };

        private static List<string> plannedCoordinates = new List<string>(); //Turn into a dictionary to know who is moving where

        public static void MoveFoes()
        {
            //Activate the foe turn
            FoeTurn = true;

            //Clear the task list & planned coordinates
            plannedCoordinates.Clear();

            //Run through all foes
            foreach (MapFoe foe in Stages.CurrentStage.FoeList)
            {
                //Is foe already chasing the player?
                if (foe.HasDetectedPlayer)
                {
                    //Should foe stop the chase? (if X or Y distance is greater than 3)
                    MapPlayer player = Stages.CurrentStage.Player;
                    if (Math.Abs(player.Position.X - foe.Position.X) > 3 || Math.Abs(player.Position.Y - foe.Position.Y) > 3)
                    {
                        //stop chase
                        foe.HasDetectedPlayer = false;
                        foe.Icon.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Platform/foe-neutral.png", UriKind.Relative));
                    }
                    else
                    {
                        CalculatePersuit(foe);
                        continue;
                    }
                }

                //Check if player is in range
                if (FoeDetection(foe))
                {
                    //If player is in range, cancel movement
                    continue;
                }

                //If player is not in range, calculate movement
                switch (foe.MovementBehaviour.ToUpper().Replace(" ", ""))
                {
                    case "STRAIGHTFORWARD":
                        {
                            CalculateStraightForward(foe);
                            break;
                        }
                }
            }

            //Deactivate the foe turn
            FoeTurn = false;
        }

        public static bool FoeDetection(MapFoe foe)
        {
            //Checks if the player is 1 tile away from a foe. Diagonal detection is not included.
            List<string> detectionRange = GetDetectionList();

            //Check if any foe is on a tile in the detection range
            if (detectionRange.Contains($"{foe.Position.X};{foe.Position.Y}"))
            {
                //If so, beep.
                foe.Icon.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Platform/foe-alert.png", UriKind.Relative));
                foe.HasDetectedPlayer = true;
                return true;
            }

            return false;
        }

        private static List<string> GetDetectionList()
        {
            //Get player
            MapPlayer player = Stages.CurrentStage.Player;

            //Define tiles that count as detected
            return new List<string>()
            {
                $"{player.Position.X + 1};{player.Position.Y}", //Right
                $"{player.Position.X - 1};{player.Position.Y}", //Left
                $"{player.Position.X};{player.Position.Y + 1}", //Down
                $"{player.Position.X};{player.Position.Y - 1}" //Up
            };
        }

        private static void CalculatePersuit(MapFoe foe)
        {
            //Get player position
            MapPlayer player = Stages.CurrentStage.Player;

            //Calculate the distance between the foe and the player
            int distanceX = Math.Abs(player.Position.X - foe.Position.X);
            int distanceY = Math.Abs(player.Position.Y - foe.Position.Y);

            //Decide on which direction to move
            //Reset for each loop
            foe.Position.DirectionX = 0;
            foe.Position.DirectionY = 0;

            if (distanceX > distanceY)
            {
                //Left or right?
                if (player.Position.X > foe.Position.X)
                {
                    //Right
                    foe.Position.DirectionX = 1;
                }
                else
                {
                    //Left
                    foe.Position.DirectionX = -1;
                }
            }
            else
            {
                //Up or down?
                if (player.Position.Y > foe.Position.Y)
                {
                    //Down
                    foe.Position.DirectionY = 1;
                }
                else
                {
                    //Up
                    foe.Position.DirectionY = -1;
                }
            }

            //Assign targetTile
            Tile targetTile = Tiles.GetTile(foe.Position.X + foe.Position.DirectionX, foe.Position.Y + foe.Position.DirectionY);

            //Check if targetTile is valid
            if (!IsTileAccessible(targetTile))
            {
                //Check if ohter directions are valid
                if (foe.Position.DirectionX != 0)
                {
                    //Reset direction
                    foe.Position.DirectionX = 0;

                    //Up or down?
                    if (player.Position.Y > foe.Position.Y)
                    {
                        //Down
                        foe.Position.DirectionY = 1;
                    }
                    else
                    {
                        //Up
                        foe.Position.DirectionY = -1;
                    }
                }
                else if (foe.Position.DirectionY != 0)
                {
                    //Reset direction
                    foe.Position.DirectionY = 0;

                    //Left or right?
                    if (player.Position.X > foe.Position.X)
                    {
                        //Right
                        foe.Position.DirectionX = 1;
                    }
                    else
                    {
                        //Left
                        foe.Position.DirectionX = -1;
                    }
                }

                //Assign targetTile
                targetTile = Tiles.GetTile(foe.Position.X + foe.Position.DirectionX, foe.Position.Y + foe.Position.DirectionY);

                if (!IsTileAccessible(targetTile))
                {
                    //Cancel movement
                    return;
                }
            }
            else if (targetTile.Position.X == player.Position.X && targetTile.Position.Y == player.Position.Y)
            {
                //Attack player
                Stages.BattleTrigger();
            }

            //Add the movement to the list
            plannedCoordinates.Add($"{targetTile.Position.X};{targetTile.Position.Y}");
            MoveFoe(foe);
        }

        private static void CalculateStraightForward(MapFoe foe)
        {
            Tile targetTile = null;

            //Was the foe already heading in a direction?
            if (foe.Position.DirectionX != 0 || foe.Position.DirectionY != 0)
            {
                //Check if the foe can continue in the same direction
                targetTile = Tiles.GetTile(foe.Position.X + foe.Position.DirectionX, foe.Position.Y + foe.Position.DirectionY);

                if (IsTileAccessible(targetTile))
                {
                    plannedCoordinates.Add($"{targetTile.Position.X};{targetTile.Position.Y}");
                    MoveFoe(foe);
                    return;
                }
            }

            //If not, pick a random direction
            List<string> attemptedDirections = new List<string>();
            List<string> availableDirections = new List<string>();

            while (true)
            {
                //Reset for each loop
                foe.Position.DirectionX = 0;
                foe.Position.DirectionY = 0;

                //Get a list with directions that haven't been attempted yet
                availableDirections = directions.Except(attemptedDirections).ToList();

                //If all directions have been attempted, cancel movement
                if (availableDirections is null || availableDirections.Count == 0)
                {
                    return;
                }

                //Pick a random direction
                switch (availableDirections[Interaction.GetRandomNumber(0, availableDirections.Count - 1)])
                {
                    case "UP":
                        {
                            foe.Position.DirectionY = -1;
                            attemptedDirections.Add("UP");
                            break;
                        }
                    case "DOWN":
                        {
                            foe.Position.DirectionY = 1;
                            attemptedDirections.Add("DOWN");
                            break;
                        }
                    case "LEFT":
                        {
                            foe.Position.DirectionX = -1;
                            attemptedDirections.Add("LEFT");
                            break;
                        }
                    case "RIGHT":
                        {
                            foe.Position.DirectionX = 1;
                            attemptedDirections.Add("RIGHT");
                            break;
                        }
                }

                //Assign targetTile
                targetTile = Tiles.GetTile(foe.Position.X + foe.Position.DirectionX, foe.Position.Y + foe.Position.DirectionY);

                //Check if targetTile is valid
                if (IsTileAccessible(targetTile))
                {
                    break;
                }
            }

            //Add the movement to the list
            plannedCoordinates.Add($"{targetTile.Position.X};{targetTile.Position.Y}");

            //Move player
            MoveFoe(foe);
        }

        private static bool IsTileAccessible(Tile tile)
        {
            //Check if the tile is out of bounds or is not walkable
            if (tile is null || !tile.IsWalkable)
            {
                return false;
            }

            //Check if the tile is already booked or already has a foe on it
            //get foe on tile
            MapFoe foeOnTile = Stages.CurrentStage.FoeList.FirstOrDefault(f => f.Position.X == tile.Position.X && f.Position.Y == tile.Position.Y);

            if (foeOnTile != null || plannedCoordinates.Contains($"{tile.Position.X};{tile.Position.Y}"))
            {
                return false;
            }

            return true;
        }

        private static void MoveFoe(MapFoe foe)
        {
            //Reset margin  (might remove)
            foe.Icon.Margin = new Thickness(0);

            //Get current tile
            Tile currentTile = Stages.CurrentStage.TileList.FirstOrDefault(t => t.Position.X == foe.Position.X && t.Position.Y == foe.Position.Y);

            //Remove foe from current tile
            currentTile.Foe = null;

            //Modify Foe Position
            foe.Position.X += foe.Position.DirectionX;
            foe.Position.Y += foe.Position.DirectionY;

            //Reset direction
            //foe.Position.DirectionX = 0;
            //foe.Position.DirectionY = 0;

            //Get tile on new position
            Tile newTile = Stages.CurrentStage.TileList.FirstOrDefault(t => t.Position.X == foe.Position.X && t.Position.Y == foe.Position.Y);

            //Add foe to new tile
            newTile.Foe = foe;

            //Battle trigger
            Stages.BattleTrigger();

            //Check if player is in detection range
            _ = FoeDetection(foe);
        }
    }
}
