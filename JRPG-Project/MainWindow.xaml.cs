using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using System.Windows;

namespace JRPG_Project
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //Initialize data/components
            GameData.InitializeData();
            InitializeComponent();

            //Set Grid & Open main tab
            Interaction.Grid = grid;
            Interaction.OpenTab("MainTab");
        }
    }
}
