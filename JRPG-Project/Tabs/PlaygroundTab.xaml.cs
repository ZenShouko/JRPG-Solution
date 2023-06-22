using JRPG_ClassLibrary;
using JRPG_ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            Levels.CreateLevel(MainGrid, "Testfield");
            PlayerControls.RunKeyDetector();

            watch.Stop();
            loadTimeMS = (int)watch.ElapsedMilliseconds;
        }
    }
}
