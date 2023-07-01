using JRPG_ClassLibrary;
using JRPG_ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace JRPG_Project.Tabs
{
    /// <summary>
    /// Interaction logic for PlaygroundTab.xaml
    /// </summary>
    public partial class PlaygroundTab : UserControl
    {
        int loadTimeMS = 0;
        public PlaygroundTab()
        {
            InitializeComponent();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //Security check
            if (GameData.DB_Game.Tables.Count == 0)
            {
                GameData.InitializeDatabase();
            }

            Stages.CreateStage(MainGrid, "NewPlatform");

            watch.Stop();
            loadTimeMS = (int)watch.ElapsedMilliseconds;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Attach keydown event to window
            Window.GetWindow(this).KeyDown += PlaygroundTab_KeyDown;
        }

        private void PlaygroundTab_KeyDown(object sender, KeyEventArgs e)
        {
            //Send directional keys to player controls
            if (PlayerControls.DirectionalKeys.Contains(e.Key))
            {
                PlayerControls.HandleInput(e.Key);
            }
        }
    }
}
