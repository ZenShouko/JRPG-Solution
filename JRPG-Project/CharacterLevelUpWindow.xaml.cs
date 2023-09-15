using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Universal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JRPG_Project
{
    /// <summary>
    /// Interaction logic for CharacterLevelUpWindow.xaml
    /// </summary>
    public partial class CharacterLevelUpWindow : Window
    {
        public CharacterLevelUpWindow(Character c)
        {
            SoundManager.PlaySound("power-up.wav");
            InitializeComponent();
            InitializeGUI(c);
            Animate();
        }

        private void InitializeGUI(Character c)
        {
            TxtTitle.Text = "🎉 " + c.Name + " has leveled up!";
            CharImg.Source = CharacterData.GetCharacterImage(c).Source;
            ImgAni.Source = CharImg.Source;
            TxtDetails.Text = c.Name + " is now level " + c.Level + "!";
        }

        private async void Animate()
        {
            //Animate character image falling down
            ScaleTransform scale = new ScaleTransform();
            scale.ScaleX = 1;
            scale.ScaleY = 1;
            
            //Set scale center to center of window
            scale.CenterX = 180;
            scale.CenterY = 170;

            ImgAni.RenderTransform = scale;

            while (scale.ScaleX > 0)
            {
                scale.ScaleX -= 0.1;
                scale.ScaleY -= 0.1;
                ImgAni.Opacity -= 0.05;
                await Task.Delay(32);
            }

            //Collapse image
            ImgAni.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
