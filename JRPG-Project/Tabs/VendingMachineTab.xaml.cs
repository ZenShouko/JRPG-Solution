using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Player;
using JRPG_Project.ClassLibrary.Universal;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Material = JRPG_Project.ClassLibrary.Items.Material;

namespace JRPG_Project
{
    /// <summary>
    /// Postponed.
    /// </summary>
    public partial class VendingMachineTab : UserControl
    {
        public VendingMachineTab(TextBlock txtLuck, TextBlock txtCoins)
        {
            InitializeComponent();
            TxtLuck = txtLuck;
            TxtLuck.Text = "Luck: " + luckModifier;
            BuyButton = BtnBuy;
            TxtCoins = txtCoins;
            TxtCoins.Text = Inventory.Coins.ToString();
        }
        public int luckModifier { get; private set; } = 0;
        TextBlock TxtLuck = new TextBlock();
        TextBlock TxtCoins = new TextBlock();

        Button BuyButton = new Button();

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //Playsound
            SoundManager.PlaySound("machine-click.wav");

            //Do we have enough money?
            if (Inventory.Coins < 100)
            {
                MessageBox.Show("You don't have enough coins to buy a ticket.");
                return;
            }

            //Remove coins
            Inventory.Coins -= 100;
            TxtCoins.Text = Inventory.Coins.ToString();

            //[1]playsound
            //SoundManager.PlaySound("machine-insert.wav");
            //[1] Push ticket to the side
            await MoveTicketAway();

            //[2] Get Random reward
            Material reward = GetMaterial();

            //[3] Display reward
            AddRewardToPanel(reward);

            //[||] Pause
            await Task.Delay(400);

            //[4] Play sound
            SoundManager.PlaySound("machine-printing.wav");
            //[4] Push ticket back in
            await MoveTicketIn();
        }

        private async Task MoveTicketAway()
        {
            int margin = 0;
            while (margin < 800)
            {
                margin += 25;
                BorderTicket.Margin = new Thickness(margin, 0, (margin * -1), 0);
                await Task.Delay(12);
            }
        }

        private async Task MoveTicketIn()
        {
            //Reset border position
            BorderTicket.Margin = new Thickness(-800, BorderTicket.Margin.Top, 800, BorderTicket.Margin.Bottom);

            int loop = 0;
            while (BorderTicket.Margin.Left < 0)
            {
                loop++;
                BorderTicket.Margin = new Thickness(BorderTicket.Margin.Left + 25, BorderTicket.Margin.Top, BorderTicket.Margin.Right - 25, BorderTicket.Margin.Bottom);

                if (loop == 6)
                {
                    await Task.Delay(125);
                    loop = 0;
                }

                await Task.Delay(12);
            }
        }

        private async void ClaimAnimation()
        {
            //Prep
            ScaleTransform scale = new ScaleTransform();
            scale.ScaleX = 1;
            scale.ScaleY = 1;
            BorderTicket.RenderTransform = scale;

            //Scale down
            while (scale.ScaleX > 0.96)
            {
                scale.ScaleX -= 0.01;
                scale.ScaleY -= 0.01;

                BorderTicket.Margin = new Thickness(BorderTicket.Margin.Left + 12, BorderTicket.Margin.Top + 2, BorderTicket.Margin.Right, BorderTicket.Margin.Bottom);
                await Task.Delay(12);
            }

            //wait
            await Task.Delay(200);

            //Scale up
            while (scale.ScaleX < 1)
            {
                scale.ScaleX += 0.01;
                scale.ScaleY += 0.01;
                BorderTicket.Margin = new Thickness(BorderTicket.Margin.Left - 12, BorderTicket.Margin.Top - 2, BorderTicket.Margin.Right, BorderTicket.Margin.Bottom);
                await Task.Delay(10);
            }
        }

        private Material GetMaterial()
        {
            Material reward = new Material();

            //32% chance for orb
            if (Interaction.GetRandomNumber(0, 100) <= 32)
                reward.CopyFrom(ItemData.ListMaterials[0]);
            else
                reward.CopyFrom(ItemData.ListMaterials[Interaction.GetRandomNumber(0, ItemData.ListMaterials.Count - 1)]);

            return reward;
        }

        private void AddRewardToPanel(Material reward)
        {
            //Get button from stackInfo
            Button btn = BuyButton;
            btn.Content = "Another one!";

            //Reset panel
            StackInfo.Children.Clear();

            //Title
            TextBlock TxtTitle = new TextBlock();
            TxtTitle.Text = "You got:";
            TxtTitle.FontSize = 30;
            TxtTitle.HorizontalAlignment = HorizontalAlignment.Center;
            TxtTitle.VerticalAlignment = VerticalAlignment.Center;
            TxtTitle.Foreground = Brushes.Aquamarine;
            StackInfo.Children.Add(TxtTitle);

            //Image
            Image matImg = new Image();
            matImg.Source = ItemData.GetItemImage(reward).Source;
            matImg.Width = 84;
            matImg.Height = 84;
            matImg.Stretch = Stretch.UniformToFill;
            matImg.HorizontalAlignment = HorizontalAlignment.Center;
            matImg.VerticalAlignment = VerticalAlignment.Center;
            matImg.Margin = new Thickness(0, 10, 0, 6);
            StackInfo.Children.Add(matImg);

            //Item name
            TextBlock TxtName = new TextBlock();
            TxtName.Text = reward.Name;
            TxtName.FontSize = 20;
            TxtName.HorizontalAlignment = HorizontalAlignment.Center;
            TxtName.VerticalAlignment = VerticalAlignment.Center;
            TxtName.Foreground = Brushes.MediumAquamarine;
            StackInfo.Children.Add(TxtName);

            //Claim reward button
            Button claimButton = new Button();
            claimButton.Content = "Claim Reward!";
            claimButton.HorizontalAlignment = HorizontalAlignment.Center;
            claimButton.VerticalAlignment = VerticalAlignment.Center;
            claimButton.Foreground = Brushes.Black;
            claimButton.Background = Brushes.SlateGray;
            claimButton.Style = (Style)FindResource("menu-button");
            claimButton.Click += (s, e) => ClaimButton_Click(s, e, reward);
            claimButton.Margin = new Thickness(20);

            //Add button to stack
            StackInfo.Children.Add(claimButton);
        }

        private void ClaimButton_Click(object sender, RoutedEventArgs e, Material reward)
        {
            //Did we get lucky?
            int multiplier = ImFeelingLucky();

            //Claim reward
            Inventory.Materials[reward.ID] += 1 * multiplier;

            //Restore ticket
            StackInfo.Children.RemoveAt(StackInfo.Children.Count - 1);
            StackInfo.Children.Add(BuyButton);

            //Play claim animation
            ClaimAnimation();

            //Show supa cool message to player if we got lucky
            if (multiplier == 5)
                SoundManager.PlaySound("machine-reward5x.wav");
            if (multiplier == 3)
                SoundManager.PlaySound("machine-reward3x.wav");
            else if (multiplier == 2)
                SoundManager.PlaySound("machine-reward2x.wav");
            else if (multiplier == 1)
                SoundManager.PlaySound("machine-reward.wav");

            //Update luck text
            TxtLuck.Text = "Luck: " + luckModifier;
        }

        /// <summary>
        /// Returns reward multiplier based on luck
        /// </summary>
        /// <returns></returns>
        private int ImFeelingLucky()
        {
            //Get second textblock from stackInfo
            TextBlock txt = (TextBlock)StackInfo.Children[2];

            //Increase luck modifier
            luckModifier++;

            //Roll the die
            int roll = Interaction.GetRandomNumber(1, 6);

            int bonusRoll = Interaction.GetRandomNumber(1, 3);
            if (bonusRoll == 1)
                roll += Interaction.GetRandomNumber(1, 6);
            else if (bonusRoll == 2)
                roll *= 2;
            else if (bonusRoll == 3)
                roll /= 2;

            //Check if we won. > 12 = 2x & > 16 = 3x & > 22 = 5x
            if (roll + luckModifier >= 26)
            {
                txt.Text = "5X " + txt.Text;
                txt.Foreground = Brushes.Aqua;
                return 5;
            }
            if (roll + luckModifier >= 18)
            {
                txt.Text = "3X " + txt.Text;
                txt.Foreground = Brushes.DeepPink;
                return 3;
            }
            else if (roll + luckModifier >= 12)
            {
                txt.Text = "2X " + txt.Text;
                txt.Foreground = Brushes.Gold;
                return 2;
            }
            else { return 1; }
        }
    }
}
