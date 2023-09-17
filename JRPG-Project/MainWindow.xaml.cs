using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Player;
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

            //Test
            //Inventory.Materials["M2"] = 2500;
            //Inventory.Coins = 1000000;
        }
    }
}
