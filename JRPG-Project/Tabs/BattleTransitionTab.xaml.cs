using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
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
    /// Interaction logic for BattleTransitionTab.xaml
    /// </summary>
    public partial class BattleTransitionTab : UserControl
    {
        public BattleTransitionTab()
        {
            InitializeComponent();
            PlayAnimation();
        }

        private async void PlayAnimation()
        {
            //Move foe in
            while (ImgFoe.Margin.Left > 0)
            {
                ImgFoe.Margin = new Thickness(ImgFoe.Margin.Left - 5, ImgFoe.Margin.Top, ImgFoe.Margin.Right + 5, ImgFoe.Margin.Bottom);
                await Task.Delay(6);
            }

            await Task.Delay(50);


            //Move player up
            while (ImgPlayer.Margin.Top > -8)
            {
                ImgPlayer.Margin = new Thickness(0, ImgPlayer.Margin.Top - 2, 0, ImgPlayer.Margin.Bottom + 2);
                await Task.Delay(12);
            }
            //Move player down
            while (ImgPlayer.Margin.Top < 0)
            {
                ImgPlayer.Margin = new Thickness(0, ImgPlayer.Margin.Top + 2, 0, ImgPlayer.Margin.Bottom - 2);
                await Task.Delay(16);
            }

            //[SHOCK]
            AnimateShock();

            ImgFoe.Source = new BitmapImage(new Uri(@"../../Resources/Assets/Platform/foe-alert.png", UriKind.RelativeOrAbsolute));

            await Task.Delay(60);

            //Move player and foe to each other
            while (ImgPlayer.Margin.Left < 55)
            {
                ImgPlayer.Margin = new Thickness(ImgPlayer.Margin.Left + 1, 0, ImgPlayer.Margin.Right - 1, 0);
                ImgFoe.Margin = new Thickness(ImgFoe.Margin.Left - 1, 0, ImgFoe.Margin.Right + 1, 0);
                await Task.Delay(10);
            }
        }

        private async void AnimateShock()
        {
            Emoji.Visibility = Visibility.Visible;
            while(Emoji.Margin.Top > 0)
            {
                Emoji.Margin = new Thickness(Emoji.Margin.Left, Emoji.Margin.Top - 1, Emoji.Margin.Right, Emoji.Margin.Bottom + 1);
                await Task.Delay(6);
            }

            await Task.Delay(50);
            Emoji.Visibility = Visibility.Collapsed;
        }
    }
}
