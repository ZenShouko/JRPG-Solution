using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace JRPG_Project.ClassLibrary
{
    public static class FoeControls
    {
        private static Random rm = new Random();
        public static void MoveFoes()
        {
            //Get all the foes on the current level
            foreach (Foe foe in Levels.CurrentLevel.FoeList)
            {
                switch (foe.MovementBehaviour.ToUpper())
                {
                    case "STRAIGHTFORWARD":
                        {
                            MoveFoeStraightForward(foe); break;
                        }
                }
            }
        }

        public static void MoveFoeStraightForward(Foe foe)
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

            MoveFoe(foe, targetTile);
        }

        private static void MoveFoe(Foe foe, Tile targetTile)
        {
            //Execute the move
            foe.X += foe.DirectionX;
            foe.Y += foe.DirectionY;
            
            //Update the foe's icon
            Grid.SetColumn(foe.Icon, foe.X);
            Grid.SetRow(foe.Icon, foe.Y);

            //Update the tile

        }
    }
}
