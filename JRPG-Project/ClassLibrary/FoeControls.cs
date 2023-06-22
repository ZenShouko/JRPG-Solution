using JRPG_ClassLibrary;
using JRPG_ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Entities;
using System;
using System.Collections.Generic;
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
        private static int animationDelay = 1;

        public async static void MoveFoes()
        {
            //Activate the foe turn
            FoeTurn = true;

            //Get all the foes on the current level
            foreach (Foe foe in Levels.CurrentLevel.FoeList)
            {
                switch (foe.MovementBehaviour.ToUpper())
                {
                    case "STRAIGHTFORWARD":
                        {
                            await MoveFoeStraightForwardAsync(foe); break;
                        }
                }
            }

            //Deactivate the foe turn
            FoeTurn = false;
        }

        private static async Task MoveFoeStraightForwardAsync(Foe foe)
        {
            Tile targetTile = null;
            //Was the foe already heading in a direction?
            if (foe.DirectionX == 0 && foe.DirectionY == 0)
            {
                while (true)
                {
                    //Reset for each loop
                    foe.DirectionX = 0;
                    foe.DirectionY = 0;

                    //Pick a random direction
                    switch (rm.Next(0, 4))
                    {
                        case 0: //Up
                            {
                                foe.DirectionY = -1;
                                break;
                            }
                        case 1: //Down
                            {
                                foe.DirectionY = 1;
                                break;
                            }
                        case 2: //Left
                            {
                                foe.DirectionX = -1;
                                break;
                            }
                        case 3: //Right
                            {
                                foe.DirectionX = 1;
                                break;
                            }
                    }

                    //Is the direction valid?
                    targetTile = Tiles.GetTile(foe.X + foe.DirectionX, foe.Y + foe.DirectionY);
                    if (targetTile is null || !targetTile.IsWalkable)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                //Check if the foe can continue in the same direction
                targetTile = Tiles.GetTile(foe.X + foe.DirectionX, foe.Y + foe.DirectionY);

                while(targetTile is null || !targetTile.IsWalkable)
                {
                    //Create a new targetTile
                    //Reset for each loop
                    foe.DirectionX = 0;
                    foe.DirectionY = 0;

                    //Pick a random direction
                    switch (rm.Next(0, 4))
                    {
                        case 0: //Up
                            {
                                foe.DirectionY = -1;
                                break;
                            }
                        case 1: //Down
                            {
                                foe.DirectionY = 1;
                                break;
                            }
                        case 2: //Left
                            {
                                foe.DirectionX = -1;
                                break;
                            }
                        case 3: //Right
                            {
                                foe.DirectionX = 1;
                                break;
                            }
                    }

                    targetTile = Tiles.GetTile(foe.X + foe.DirectionX, foe.Y + foe.DirectionY);
                }
            }

            await AnimateMovement(foe);
        }

        private static async Task AnimateMovement(Foe foe)
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
