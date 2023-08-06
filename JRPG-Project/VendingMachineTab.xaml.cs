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
using System.Windows.Threading;
using XamlAnimatedGif.Decoding;

namespace JRPG_Project
{
    /// <summary>
    /// Postponed.
    /// </summary>
    public partial class VendingMachineTab : UserControl
    {
        public VendingMachineTab()
        {
            InitializeComponent();
            TicketBorder = BorderTicket;
        }
        Border TicketBorder = new Border();

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //[1] Push ticket to the side
            await MoveTicketAway();
        }

        private async Task MoveTicketAway()
        {
            int margin = 0;
            while (margin < 800)
            {
                margin += 10;
                TicketBorder.Margin = new Thickness(margin, 0, (margin * -1), 0);
                await Task.Delay(4);
            }
        }
    }
}
