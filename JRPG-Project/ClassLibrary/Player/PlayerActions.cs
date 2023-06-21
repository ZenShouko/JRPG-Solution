using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Project.ClassLibrary.Player
{
    public static class PlayerActions
    {
        public static void CollectTileItem(Tile tile)
        {
            //If empty, return
            if (tile is null || tile.MOB is null) { return; }

            //Does tile contain a collectable item?
            if (Levels.CurrentLevel.Collectables.Contains(tile.MOB.Name))
            {
                //Remove mob from playfield
                Levels.CurrentLevel.Playfield.Children.Remove(tile.MOB.Icon);

                //Remove mob from moblist
                Levels.CurrentLevel.MobList.Remove(tile.MOB);

                //Remove mob from tile
                tile.MOB = null;

                //Add item to inventory
            }
        }
    }
}
