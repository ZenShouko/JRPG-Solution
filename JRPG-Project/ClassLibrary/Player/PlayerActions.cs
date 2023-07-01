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
            if (tile is null || tile.Player is null) { return; }

            //Does tile contain a collectable item?
            //if (Stages.CurrentStage.Collectables.Contains(tile.Player.Name))
            //{
            //    //Remove mob from playfield
            //    Stages.CurrentStage.Platform.Children.Remove(tile.Player.Icon);

            //    //Remove mob from moblist
            //    Stages.CurrentStage.MobList.Remove(tile.Player);

            //    //Remove mob from tile
            //    tile.Player = null;

            //    //TODO: Add item to inventory
            //}
        }
    }
}
