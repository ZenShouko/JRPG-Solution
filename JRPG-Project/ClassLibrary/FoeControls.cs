//using JRPG_ClassLibrary;
//using JRPG_ClassLibrary.Entities;
//using JRPG_Project.ClassLibrary.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media.Imaging;

//namespace JRPG_Project.ClassLibrary
//{
//    public static class FoeControls
//    {
//        public static bool FoeTurn { get; private set; } = false;
//        private static Random rm = new Random();
//        private static int speed = 16;
//        private static int animationDelay = 10;
//        private static List<string> directions = new List<string>()
//            { "UP", "DOWN", "LEFT", "RIGHT" };

//        private static List<string> plannedCoordinates = new List<string>();

//        public static void MoveFoes()
//        {
//            //Activate the foe turn
//            FoeTurn = true;

//            //Clear the task list & planned coordinates
//            plannedCoordinates.Clear();

//            //Run through all foes
//            //foreach (MapFoe foe in Stages.CurrentStage.FoeList)
//            //{
//            //    //Is foe already chasing the player?
//            //    if (foe.HasDetectedPlayer)
//            //    {
//            //        //Should foe stop the chase? (if X or Y distance is greater than 3)
//            //        MapPlayer player = PlayerControls.GetPlayer();
//            //        if (Math.Abs(player.X - foe.X) > 3 || Math.Abs(player.Y - foe.Y) > 3)
//            //        {
//            //            //stop chase
//            //            foe.HasDetectedPlayer = false;
//            //            foe.Icon.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Characters/"+ foe.IconNames.Split(';')[0] +".png", UriKind.Relative));
//            //        }
//            //        else
//            //        {
//            //            CalculatePersuit(foe);
//            //            continue;
//            //        }
//            //    }

//            //    //Check if player is in range
//            //    if (FoeDetection(foe))
//            //    {
//            //        //If player is in range, cancel movement
//            //        continue;
//            //    }

//            //    //If player is not in range, calculate movement
//            //    switch (foe.MovementBehaviour.ToUpper())
//            //    {
//            //        case "STRAIGHTFORWARD":
//            //            {
//            //                CalculateStraightForward(foe);
//            //                break;
//            //            }
//            //    }
//            //}

//            ////Deactivate the foe turn
//            //FoeTurn = false;
//        }

//        private static void CalculateStraightForward(MapFoe foe)
//        {
//            Tile targetTile = null;

//            //Was the foe already heading in a direction?
//            if (foe.DirectionX != 0 || foe.DirectionY != 0)
//            {
//                //Check if the foe can continue in the same direction
//                targetTile = Tiles.GetTile(foe.X + foe.DirectionX, foe.Y + foe.DirectionY);

//                if (IsTileAccessible(targetTile))
//                {
//                    plannedCoordinates.Add($"{targetTile.X};{targetTile.Y}");
//                    AnimateMovement(foe);
//                    return;
//                }
//            }

//            //If not, pick a random direction
//            List<string> attemptedDirections = new List<string>();
//            List<string> availableDirections = new List<string>();

//            while (true)
//            {
//                //Reset for each loop
//                foe.DirectionX = 0;
//                foe.DirectionY = 0;

//                //Get a list with directions that haven't been attempted yet
//                availableDirections = directions.Except(attemptedDirections).ToList();

//                //If all directions have been attempted, cancel movement
//                if (availableDirections is null || availableDirections.Count == 0)
//                {
//                    return;
//                }

//                //Pick a random direction
//                switch (availableDirections[rm.Next(0, availableDirections.Count)])
//                {
//                    case "UP":
//                        {
//                            foe.DirectionY = -1;
//                            attemptedDirections.Add("UP");
//                            break;
//                        }
//                    case "DOWN":
//                        {
//                            foe.DirectionY = 1;
//                            attemptedDirections.Add("DOWN");
//                            break;
//                        }
//                    case "LEFT":
//                        {
//                            foe.DirectionX = -1;
//                            attemptedDirections.Add("LEFT");
//                            break;
//                        }
//                    case "RIGHT":
//                        {
//                            foe.DirectionX = 1;
//                            attemptedDirections.Add("RIGHT");
//                            break;
//                        }
//                }

//                //Assign targetTile
//                targetTile = Tiles.GetTile(foe.X + foe.DirectionX, foe.Y + foe.DirectionY);

//                //Check if targetTile is valid
//                if (IsTileAccessible(targetTile))
//                {
//                    break;
//                }
//            }

//            //Add the movement to the list
//            plannedCoordinates.Add($"{targetTile.X};{targetTile.Y}");

//            //Move player
//            AnimateMovement(foe);
//        }

//        private static void CalculatePersuit(MapFoe foe)
//        {
//            //Get player position
//            MapPlayer player = PlayerControls.GetPlayer();

//            //Calculate the distance between the foe and the player
//            int distanceX = Math.Abs(player.X - foe.X);
//            int distanceY = Math.Abs(player.Y - foe.Y);

//            //Decide on which direction to move
//            //Reset for each loop
//            foe.DirectionX = 0;
//            foe.DirectionY = 0;

//            if (distanceX > distanceY)
//            {
//                //Left or right?
//                if (player.X > foe.X)
//                {
//                    //Right
//                    foe.DirectionX = 1;
//                }
//                else
//                {
//                    //Left
//                    foe.DirectionX = -1;
//                }
//            }
//            else
//            {
//                //Up or down?
//                if (player.Y > foe.Y)
//                {
//                    //Down
//                    foe.DirectionY = 1;
//                }
//                else
//                {
//                    //Up
//                    foe.DirectionY = -1;
//                }
//            }

//            //Assign targetTile
//            Tile targetTile = Tiles.GetTile(foe.X + foe.DirectionX, foe.Y + foe.DirectionY);

//            //Check if targetTile is valid
//            if (!IsTileAccessible(targetTile))
//            {
//                //Check if ohter directions are valid
//                if (foe.DirectionX != 0)
//                {
//                    //Reset direction
//                    foe.DirectionX = 0;

//                    //Up or down?
//                    if (player.Y > foe.Y)
//                    {
//                        //Down
//                        foe.DirectionY = 1;
//                    }
//                    else
//                    {
//                        //Up
//                        foe.DirectionY = -1;
//                    }
//                }
//                else if (foe.DirectionY != 0)
//                {
//                    //Reset direction
//                    foe.DirectionY = 0;

//                    //Left or right?
//                    if (player.X > foe.X)
//                    {
//                        //Right
//                        foe.DirectionX = 1;
//                    }
//                    else
//                    {
//                        //Left
//                        foe.DirectionX = -1;
//                    }
//                }

//                //Assign targetTile
//                targetTile = Tiles.GetTile(foe.X + foe.DirectionX, foe.Y + foe.DirectionY);

//                if (!IsTileAccessible(targetTile))
//                {
//                    //Cancel movement
//                    return;
//                }
//            }
//            else if (targetTile.X == player.X && targetTile.Y == player.Y)
//            {
//                //Attack player
//                BattleControls.InitiateBattle(false, foe);
//            }

//            //Add the movement to the list
//            plannedCoordinates.Add($"{targetTile.X};{targetTile.Y}");
//            AnimateMovement(foe);
//        }

//        private static bool IsTileAccessible(Tile tile)
//        {
//            //Check if the tile is out of bounds or is not walkable
//            if (tile is null || !tile.IsWalkable)
//            {
//                return false;
//            }

//            //Check if the tile is already booked or already has a foe on it
//            //get foe on tile
//            MapFoe foeOnTile = Stages.CurrentStage.FoeList.FirstOrDefault(f => f.X == tile.X && f.Y == tile.Y);

//            if (foeOnTile != null || plannedCoordinates.Contains($"{tile.X};{tile.Y}"))
//            {
//                return false;
//            }

//            return true;
//        }

//        private static async void AnimateMovement(MapFoe foe)
//        {
//            //Get image margin
//            Thickness margin = foe.Icon.Margin;

//            //Move
//            while (Math.Abs(margin.Right) < Math.Abs(Stages.CurrentStage.TileWidth) &&
//                Math.Abs(margin.Top) < Math.Abs(Stages.CurrentStage.TileHeight))
//            {
//                margin.Left += speed * foe.DirectionX;
//                margin.Right -= speed * foe.DirectionX;
//                margin.Top += speed * foe.DirectionY;
//                margin.Bottom -= speed * foe.DirectionY;
//                foe.Icon.Margin = margin;
//                await Task.Delay(animationDelay);
//            }

//            MoveFoe(foe);
//        }

//        private static void MoveFoe(MapFoe foe)
//        {
//            //Reset margin
//            foe.Icon.Margin = new Thickness(0);

//            //Execute the move
//            foe.X += foe.DirectionX;
//            Grid.SetColumn(foe.Icon, foe.X);
//            foe.Y += foe.DirectionY;
//            Grid.SetRow(foe.Icon, foe.Y);

//            //Check if player is in detection range
//            _ = FoeDetection(foe);
//        }

//        public static bool FoeDetection(MapFoe foe)
//        {
//            //Checks if the player is 1 tile away from a foe. Diagonal detection is not included.
//            List<string> detectionRange = GetDetectionList();

//            //Check if any foe is on a tile in the detection range
//            if (detectionRange.Contains($"{foe.X};{foe.Y}"))
//            {
//                //If so, beep.
//                foe.Icon.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Characters/" + foe.IconNames.Split(';')[1] + ".png", UriKind.Relative));
//                foe.HasDetectedPlayer = true;
//                return true;
//            }

//            return false;
//        }

//        private static List<string> GetDetectionList()
//        {
//            //Get player
//            MapPlayer player = PlayerControls.GetPlayer();

//            //Define tiles that count as detected
//            return new List<string>()
//            {
//                $"{player.X + 1};{player.Y}", //Right
//                $"{player.X - 1};{player.Y}", //Left
//                $"{player.X};{player.Y + 1}", //Down
//                $"{player.X};{player.Y - 1}" //Up
//            };
//        }
//    }
//}
