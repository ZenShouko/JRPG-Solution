using JRPG_ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JRPG_Project.ClassLibrary
{
    public static class BattleControls
    {
        public static void StartBattle()
        {
            //Create battle
            Battle battle = new Battle();

            //MessageBox.Show("Battle started.");
        }

        public static void InitiateBattle(bool byPlayer, Foe foe)
        {
            //Did player initiate battle?
            if (byPlayer)
            {
                //Yes, player initiated battle
                MessageBox.Show($"Player initiated battle with {foe.Name}.");
            }
            else
            {
                //No, foe initiated battle
                MessageBox.Show($"{foe.Name} initiated battle with player.");
            }

            StartBattle();
        }
    }
}
