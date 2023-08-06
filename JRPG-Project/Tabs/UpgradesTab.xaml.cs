using JRPG_Project.ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JRPG_Project.Tabs
{
    /// <summary>
    /// Interaction logic for UpgradesTab.xaml
    /// </summary>
    public partial class UpgradesTab : UserControl
    {
        public UpgradesTab(BaseItem item, bool refinement)
        {
            InitializeComponent();
            BaseItem = item;
            if (!refinement)
                PrepareGuiForUpgrade();
        }
        BaseItem BaseItem { get; set; }

        private void PrepareGuiForUpgrade()
        {
            IStatsHolder ItemWStats = BaseItem as IStatsHolder;


        }
    }
}
