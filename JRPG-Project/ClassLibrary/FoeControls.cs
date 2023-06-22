using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JRPG_Project.ClassLibrary
{
    public static class FoeControls
    {
        public static bool FoeTurn { get; private set; } = false;
        private static Random rm = new Random();
        private static int speed = 16;
        private static int animationDelay = 10;
        private static List<string> directions = new List<string>()
            { "UP", "DOWN", "LEFT", "RIGHT" };

        private static List<string> plannedCoordinates = new List<string>();

        public static async void MoveFoes()
        {
            //Activate the foe turn
            FoeTurn = true;

            //Clear the task list & planned coordinates
            plannedCoordinates.Clear();

            //Run through all foes
            foreach (Foe foe in Levels.CurrentLevel.FoeList)
            {
                switch (foe.MovementBehaviour.ToUpper())
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

        private static void CalculateStraightForward(Foe foe)
        {
            Tile targetTile = null;
            List<string> attempts = new List<string>(); //Used to prevent infinite loops

            //Was the foe already heading in a direction?
            if (foe.DirectionX == 0 && foe.DirectionY == 0)
            {
                while (true)
                {
                    //Reset for each loop
                    foe.DirectionX = 0;
                    foe.DirectionY = 0;

                    //Get a list with directions that haven't been attempted yet
                    List<string> availableDirections = directions.Except(attempts).ToList();

                    //If all directions have been attempted, cancel movement
                    if (availableDirections.Count == 0)
                    {
                        return;
                    }

                    //Pick a random direction
                    switch (availableDirections[rm.Next(0, availableDirections.Count)])
                    {
                        case "UP":
                            {
                                foe.DirectionY = -1;
                                attempts.Add("UP");
                                break;
                            }
                        case "DOWN":
                            {
                                foe.DirectionY = 1;
                                attempts.Add("DOWN");
                                break;
                            }
                        case "LEFT":
                            {
                                foe.DirectionX = -1;
                                attempts.Add("LEFT");
                                break;
                            }
                        case "RIGHT":
                            {
                                foe.DirectionX = 1;
                                attempts.Add("RIGHT");
                                break;
                            }
                    }

                    //Assign targetTile
                    targetTile = Tiles.GetTile(foe.X + foe.DirectionX, foe.Y + foe.DirectionY);

                    //Check if targetTile is valid
                    if (targetTile is null || !targetTile.IsWalkable || IsTileBooked(targetTile))
                    {
                        continue;
                    }

                    //If so, break out of the loop
                    break;
                }
            }
            else
            {
                while (true)
                {
                    //Check if the foe can continue in the same direction
                    targetTile = Tiles.GetTile(foe.X + foe.DirectionX, foe.Y + foe.DirectionY);

                    if (targetTile is null || !targetTile.IsWalkable || IsTileBooked(targetTile))
                    {
                        //Create a new targetTile
                        //Reset for each loop
                        foe.DirectionX = 0;
                        foe.DirectionY = 0;

                        //Get a list with directions that haven't been attempted yet
                        List<string> availableDirections = directions.Except(attempts).ToList();

                        //If there are no directions left, cancel movement
                        if (availableDirections.Count == 0)
                        {
                            return;
                        }

                        //Pick a random direction
                        switch (availableDirections[rm.Next(0, availableDirections.Count)])
                        {
                            case "UP":
                                {
                                    foe.DirectionY = -1;
                                    attempts.Add("UP");
                                    break;
                                }
                            case "DOWN":
                                {
                                    foe.DirectionY = 1;
                                    attempts.Add("DOWN");
                                    break;
                                }
                            case "LEFT":
                                {
                                    foe.DirectionX = -1;
                                    attempts.Add("LEFT");
                                    break;
                                }
                            case "RIGHT":
                                {
                                    foe.DirectionX = 1;
                                    attempts.Add("RIGHT");
                                    break;
                                }
                        }

                        //Assign the new targetTile
                        targetTile = Tiles.GetTile(foe.X + foe.DirectionX, foe.Y + foe.DirectionY);

                        continue;
                    }

                    //If so, break out of the loop
                    break;
                }
            }

            //Add the movement to the list
            plannedCoordinates.Add($"{targetTile.X};{targetTile.Y}");

            //Move player
            AnimateMovement(foe);
        }

        private static bool IsTileBooked(Tile tile)
        {
            //Check if the tile is already booked or already has a foe on it

            //get foe on tile
            Foe foeOnTile = Levels.CurrentLevel.FoeList.FirstOrDefault(f => f.X == tile.X && f.Y == tile.Y);

            if (foeOnTile != null || plannedCoordinates.Contains($"{tile.X};{tile.Y}"))
            {
                return true;
            }

            return false;
        }

        private static async void AnimateMovement(Foe foe)
        {
            //Get image margin
            Thickness margin = foe.Icon.Margin;

            //Move
            while (Math.Abs(margin.Right) < Math.Abs(Levels.CurrentLevel.TileWidth) &&
                Math.Abs(margin.Top) < Math.Abs(Levels.CurrentLevel.TileHeight))
            {
                margin.Left += speed * foe.DirectionX;
                margin.Right -= speed * foe.DirectionX;
                margin.Top += speed * foe.DirectionY;
                margin.Bottom -= speed * foe.DirectionY;
                foe.Icon.Margin = margin;
                await Task.Delay(animationDelay);
            }

            MoveFoe(foe);
        }

        private static void MoveFoe(Foe foe)
        {
            //Reset margin
            foe.Icon.Margin = new Thickness(0);

            //Execute the move
            foe.X += foe.DirectionX;
            Grid.SetColumn(foe.Icon, foe.X);
            foe.Y += foe.DirectionY;
            Grid.SetRow(foe.Icon, foe.Y);
        }
    }
}
