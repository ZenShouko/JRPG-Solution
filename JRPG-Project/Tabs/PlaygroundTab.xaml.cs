using JRPG_ClassLibrary;
using JRPG_ClassLibrary.Entities;
using System;
using System.Collections.Generic;
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
        public PlaygroundTab()
        {
            InitializeComponent();
            Levels.CreateLevel(MainGrid, "Testfield");
            PlayerControls.RunKeyDetector();
        }
    }
}
