using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Player;
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
    public partial class BattleTab : UserControl
    {
        public BattleTab(List<Character> foeTeam)
        {
            InitializeComponent();
            this.FoeTeam = foeTeam;
            InitializeBattle();
        }
        //Foe teams
        List<Character> FoeTeam = new List<Character>();
        Dictionary<Character, Border> CharacterBorder = new Dictionary<Character, Border>();
        int speedToggle = 1; //Higher means faster

        #region Prep
        private void InitializeBattle()
        {
            //Place foes on the grid
            for (int i = 0; i < FoeTeam.Count(); i++)
            {
                Border border = GetCharacterBorder(FoeTeam[i]);
                FoePanel.Children.Add(border);

                //Add border to dictionary
                CharacterBorder.Add(FoeTeam[i], border);
            }

            //Place player team on the grid
            for (int i= 0; i < Inventory.Team.Count(); i++)
            {
                Border border = GetCharacterBorder(Inventory.Team[i]);
                PlayerPanel.Children.Add(border);

                //Add border to dictionary
                CharacterBorder.Add(Inventory.Team[i], border);
            }

            //Create character queue
            Queue.AddRange(Inventory.Team);
            Queue.AddRange(FoeTeam);
            Queue = Queue.OrderByDescending(x => x.Stats.SPD).ToList();

            //Add all characters to the hp dictionary
            foreach (Character character in Queue)
            {
                CharHpDef.Add(character, (character.GetAccumelatedStats().HP, character.GetAccumelatedStats().DEF));
            }

            //Simulate battle
            SimulateBattle();
        }

        private Border GetCharacterBorder(Character character)
        {
            //Border
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1, 1, 2, 2);
            border.CornerRadius = new CornerRadius(2);
            border.Width = 125;
            border.Margin = new Thickness(8, 0, 8, 0);
            border.Background = (Brush)new BrushConverter().ConvertFrom("#445069");

            //Stackpanel
            StackPanel stackPanel = new StackPanel();
            border.Child = stackPanel;

            //Def bar
            ProgressBar defBar = new ProgressBar();
            defBar.Height = 6;
            defBar.Width = 85;
            defBar.Margin = new Thickness(2);
            defBar.Maximum = character.GetAccumelatedStats().DEF;
            defBar.Value = defBar.Maximum;
            defBar.Style = (Style)FindResource("XpProgressBar");
            defBar.Foreground = Brushes.RoyalBlue;
            stackPanel.Children.Add(defBar);

            //Image
            Image img = new Image();
            img.Source = character.CharImage.Source;
            img.Stretch = Stretch.UniformToFill;
            img.Height = 100;
            img.Width = 125;
            img.Margin = new Thickness(0, 4, 0, 4);
            img.HorizontalAlignment = HorizontalAlignment.Center;
            img.VerticalAlignment = VerticalAlignment.Center;
            stackPanel.Children.Add(img);

            //Hp bar
            ProgressBar hpBar = new ProgressBar();
            hpBar.Height = 8;
            hpBar.Width = 90;
            hpBar.Margin = new Thickness(2);
            hpBar.Maximum = character.GetAccumelatedStats().HP;
            hpBar.Value = hpBar.Maximum;
            hpBar.Style = (Style)FindResource("XpProgressBar");
            hpBar.Foreground = Brushes.Crimson;
            stackPanel.Children.Add(hpBar);

            //Add hp and def bar to dictionary
            CharHpDefBar.Add(character, (hpBar, defBar));

            return border;
        }
        #endregion

        #region Battle
        List<Character> Queue = new List<Character>();
        Dictionary<Character, (int, int)> CharHpDef = new Dictionary<Character, (int, int)>();
        Dictionary<Character, (ProgressBar, ProgressBar)> CharHpDefBar = new Dictionary<Character, (ProgressBar, ProgressBar)>();
        bool Battle = true; //True = battle is still going on, false = battle is over
        private async void SimulateBattle()
        {
            //Battoru hajimaruyo!
            await Task.Delay(1400 / speedToggle);

            //Battle loop
            while (Battle)
            {
                //Loop through queue
                foreach (Character character in Queue)
                {
                    //Check if character is dead
                    if (CharHpDef[character].Item1 <= 0)
                    {
                        continue;
                    }

                    //highlight character
                    HighlightCharacter(character);

                    //Clear global anouncer
                    GlobalAnouncer($"");

                    //PAUSE
                    await Task.Delay(500 / speedToggle);

                    //Pick a random target
                    Character targetCharacter = GetTarget(character);
                    HighlightTarget(targetCharacter);

                    //Attack target
                    Attack(character, targetCharacter);


                    //Is target dead?
                    if (CharHpDef[targetCharacter].Item1 <= 0)
                    {
                        //Remove target from grid
                        if (FoeTeam.Contains(targetCharacter))
                        {
                            CharacterBorder[targetCharacter].Opacity = 0.4;
                        }
                        else
                        {
                            CharacterBorder[targetCharacter].Opacity = 0.4;
                        }

                        //PAUSE
                        await Task.Delay(1000 / speedToggle);

                        //Check if battle is over
                        if (IsBattleOver())
                        {
                            //PAUSE
                            await Task.Delay(2000 / speedToggle);

                            Battle = false;
                            BtnFinish.IsEnabled = true;
                            break;
                        }
                    }

                    //PAUSE
                    await Task.Delay(1500 / speedToggle);

                    //Unhighlight
                    HighlightTarget(null);
                }
            }

            //Battle is over
            if (CharHpDef.Any(x => x.Key.ID.Contains("CH") && x.Value.Item1 > 0))
            {
                //player won, hand out rewards
                (int xp, int coins) = CalculateXpCoinRewards();
                SmallAnouncer($"You gained {xp} xp and {coins} coins!");
            }
        }

        private void HighlightCharacter(Character character)
        {
            foreach(var pair in CharacterBorder)
            {
                if (pair.Key == character)
                {
                    //Highlight
                    pair.Value.BorderBrush = Brushes.Azure;
                    pair.Value.BorderThickness = new Thickness(2, 1, 2, 3);
                }
                else
                {
                    //Unhighlight
                    pair.Value.BorderBrush = Brushes.Black;
                    pair.Value.BorderThickness = new Thickness(2, 1, 2, 2);
                }
            }
        }

        private void HighlightTarget(Character character)
        {
            if (character is null)
            {
                //Unhighlight target border
                foreach (var pair in CharacterBorder)
                {
                    if (pair.Value.BorderBrush == Brushes.MediumVioletRed)
                    {
                        pair.Value.BorderBrush = Brushes.Black;
                        pair.Value.BorderThickness = new Thickness(2, 1, 2, 2);
                    }
                }

                return;
            }

            foreach(var pair in CharacterBorder)
            {
                if (!pair.Key.ID.Contains(character.ID.Substring(0, 1)))
                    continue;

                if (pair.Key == character)
                {
                    //Highlight
                    pair.Value.BorderBrush = Brushes.MediumVioletRed;
                    pair.Value.BorderThickness = new Thickness(2, 1, 2, 3);
                }
                else
                {
                    //Unhighlight
                    pair.Value.BorderBrush = Brushes.Black;
                    pair.Value.BorderThickness = new Thickness(2, 1, 2, 2);
                }
            }
        }

        private Character GetTarget(Character currentChar)
        {
            //Return null if no targets available
            if (currentChar.ID.Contains("CH"))
            {
                if (CharHpDef.All(x => x.Key.ID.Contains("F") && x.Value.Item1 <= 0))
                    return null;
            }
            else
            {
                if (CharHpDef.All(x => x.Key.ID.Contains("CH") && x.Value.Item1 <= 0))
                    return null;
            }


            Character target = new Character();

            while (true)
            {
                if (currentChar.ID.Contains("F")) //Current character is a foe
                {
                    //Pick a random target from the player's team
                    target = Inventory.Team[Interaction.GetRandomNumber(0, Inventory.Team.Count() - 1)];
                }
                else
                {
                    //Pick a random target from the foe's team
                    target = FoeTeam[Interaction.GetRandomNumber(0, FoeTeam.Count() - 1)];
                }

                //Check if target is dead, else return target
                if (CharHpDef[target].Item1 <= 0)
                    continue;
                else
                    return target;
            }
        }

        private void Attack(Character attacker, Character target)
        {
            //#Anouncer:
            GlobalAnouncer($"attacks {target.Name}");
            
            //Animate Attack
            AnimateAttack(attacker);

            //Get damage
            (int dmg, bool crit) = CalculateDmg(attacker);

            //Display damage
            DisplayNumbers(dmg, crit, target);

            //Lower defence first, then HP
            CharHpDef[target] = (CharHpDef[target].Item1, CharHpDef[target].Item2 - dmg);

            //If target's defence is below 0, lower HP
            if (CharHpDef[target].Item2 <= 0)
            {
                CharHpDef[target] = (CharHpDef[target].Item1 - Math.Abs(CharHpDef[target].Item2), 0);
            }

            //Animate hit
            AnimateHit(target);

            //Update HP and DEF bar
            CharHpDefBar[target].Item1.Value = CharHpDef[target].Item1;
            CharHpDefBar[target].Item2.Value = CharHpDef[target].Item2;

            //#Anounce remaining HP DEF
            if (crit)
                SmallAnouncer($"Critical! [{CharHpDef[target].Item1}HP | {CharHpDef[target].Item2}DEF]");
            else
                SmallAnouncer($"Ok! [{CharHpDef[target].Item1}HP | {CharHpDef[target].Item2}DEF]");
        }

        private (int, bool) CalculateDmg(Character attacker)
        {
            //Base dmg
            int DMG = attacker.GetAccumelatedStats().DMG;

            //Do we have a crit hit?
            if (Interaction.GetRandomNumber(0, 100) <= attacker.GetAccumelatedStats().CRC)
            {
                //Yes, crit hit
                return (DMG * attacker.GetAccumelatedStats().CRD, true);
            }

            //No, crit hit
            return (DMG, false);
        }

        private (int, int) CalculateXpCoinRewards()
        {
            int xpReward = 0;
            int coinsReward = 0;

            // ???
            //Profit

            return (xpReward, coinsReward);
        }

        private bool IsBattleOver()
        {
            //True if either team is dead
            if (!CharHpDef.Any(x => x.Key.ID.Contains("CH") && x.Value.Item1 > 0))
            {
                //Player lost
                GlobalAnouncer("くそっ！負けちゃった");
                return true;
            }
            else if (!CharHpDef.Any(x => x.Key.ID.Contains("F") && x.Value.Item1 > 0))
            {
                //Player won
                GlobalAnouncer("勝ったぞ！");
                return true;
            }
            else
                return false;
        }

        private void SmallAnouncer(string text)
        {
            TxtSmallAnouncer.Text = text;
        }

        private void GlobalAnouncer(string text)
        {
            TxtGlobalAnouncer.Text = text;
        }

        #endregion

        #region Animations

        private async void AnimateAttack(Character attacker)
        {
            //Vars
            int margin = 0;
            Border border = CharacterBorder[attacker];
            bool attackerIsPlayer = attacker.ID.Contains("CH");

            //Move 10 pixels up if attacker is player, else move down
            while (Math.Abs(margin) < 10)
            {
                margin++;
                border.Margin = attackerIsPlayer == true ? new Thickness(border.Margin.Left, -margin, border.Margin.Right, margin) 
                    : new Thickness(border.Margin.Left, margin, border.Margin.Right, -margin);
                await Task.Delay(1);
            }

            //Move 10 pixels down if attacker is player, else move up
            while (Math.Abs(margin) > 0)
            {
                margin--;
                border.Margin = attackerIsPlayer == true ? new Thickness(border.Margin.Left, -margin, border.Margin.Right, margin) 
                    : new Thickness(border.Margin.Left, margin, border.Margin.Right, -margin);
                await Task.Delay(1);
            }
        }

        private async void AnimateHit(Character target)
        {
            //Vars
            int margin = 0;
            Border border = CharacterBorder[target];
            bool targetIsPlayer = target.ID.Contains("CH");

            //Move 4 pixels up if target is player, else move down
            while (Math.Abs(margin) < 3)
            {
                margin++;
                border.Margin = targetIsPlayer == false ? new Thickness(border.Margin.Left, -margin, border.Margin.Right, margin) 
                    : new Thickness(border.Margin.Left, margin, border.Margin.Right, -margin);
                await Task.Delay(100);
            }

            //Move 4 pixels down if target is player, else move up
            while (Math.Abs(margin) > 0)
            {
                margin--;
                border.Margin = targetIsPlayer == false ? new Thickness(border.Margin.Left, -margin, border.Margin.Right, margin) 
                    : new Thickness(border.Margin.Left, margin, border.Margin.Right, -margin);
                await Task.Delay(100);
            }
        }

        private async void DisplayNumbers(int dmg, bool crit, Character target)
        {
            //Border we're working with
            Border border = CharacterBorder[target];

            //Get stackpanel from border
            StackPanel stackPanel = (StackPanel)border.Child;

            //Create a textblock to display the damage
            TextBlock txt = new TextBlock();

            txt.Text = crit ? dmg.ToString() + "!!" : dmg.ToString();
            txt.FontSize = crit ? 24 : 22;
            txt.FontWeight = FontWeights.Bold;
            txt.Foreground = Brushes.MediumVioletRed;

            // Create a Border to provide the semi-transparent background
            Border backgroundBorder = new Border();
            backgroundBorder.HorizontalAlignment = HorizontalAlignment.Left;
            backgroundBorder.VerticalAlignment = VerticalAlignment.Bottom;
            backgroundBorder.Background = new SolidColorBrush(Color.FromArgb(40, 0, 0, 0)); // Adjust the color and opacity as needed
            backgroundBorder.Margin = new Thickness(0, 0, 0, -28);
            backgroundBorder.CornerRadius = new CornerRadius(2);
            backgroundBorder.Padding = new Thickness(4, 0, 4, 0);
            backgroundBorder.Child = txt;

            //Add textblock to stackpanel
            stackPanel.Children.Add(backgroundBorder);

            //Animate from left to right
            while (backgroundBorder.Margin.Left < 14)
            {
                backgroundBorder.Margin = new Thickness(backgroundBorder.Margin.Left + 1, backgroundBorder.Margin.Top, backgroundBorder.Margin.Right - 1, backgroundBorder.Margin.Bottom);
                await Task.Delay(10);
            }

            await Task.Delay(2000 / speedToggle);

            //Fade out
            while (backgroundBorder.Opacity > 0)
            {
                backgroundBorder.Opacity -= 0.1;
                await Task.Delay(10);
            }

            //Remove element
            stackPanel.Children.Remove(backgroundBorder);
        }


        #endregion

        private void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            //Return to previous tab
            Interaction.CloseBattleTab();
        }

        private void BtnSpeedToggle_Click(object sender, RoutedEventArgs e)
        {
            speedToggle = speedToggle == 1 ? 2 : 1;
            BtnSpeedToggle.Content = speedToggle == 1 ? "Faster" : "Slower";
        }
    }
}
