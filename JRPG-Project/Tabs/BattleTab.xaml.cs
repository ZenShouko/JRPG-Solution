using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JRPG_Project.Tabs
{
    public partial class BattleTab : UserControl
    {
        public BattleTab(List<Character> foeTeam)
        {
            InitializeComponent();
            this.FoeTeam = foeTeam;
            this.PlayerTeam = Stages.CurrentStage.Team;
            InitializeBattle();
        }
        //Foe teams
        List<Character> FoeTeam = new List<Character>();
        List<Character> PlayerTeam = new List<Character>();
        Dictionary<Character, Border> CharacterBorder = new Dictionary<Character, Border>();
        int speedToggle = 1; //Higher means faster
        List<string> BattleLog = new List<string>();
        bool PauseBattle = false; //Allows pausing the battle

        #region Prep
        private void InitializeBattle()
        {
            //Add all characters to the hp dictionary AND stamina dictionary
            foreach (Character character in PlayerTeam)
            {
                CharHpDef.Add(character, (character.GetAccumelatedStats().HP, character.GetAccumelatedStats().DEF));
                CharStaStrMax.Add(character, (character.GetAccumelatedStats().STA, character.GetAccumelatedStats().STR, character.GetAccumelatedStats().STA));
            }
            foreach (Character character in FoeTeam)
            {
                CharHpDef.Add(character, (character.GetAccumelatedStats().HP, character.GetAccumelatedStats().DEF));
                CharStaStrMax.Add(character, (character.GetAccumelatedStats().STA, character.GetAccumelatedStats().STR, character.GetAccumelatedStats().STA));
            }

            //Place foes on the grid
            for (int i = 0; i < FoeTeam.Count(); i++)
            {
                Border border = GetCharacterBorder(FoeTeam[i]);
                FoePanel.Children.Add(border);

                //Add border to dictionary
                CharacterBorder.Add(FoeTeam[i], border);
            }

            //Place player team on the grid
            for (int i = 0; i < PlayerTeam.Count(); i++)
            {
                //Skip dead characters
                if (PlayerTeam[i].GetAccumelatedStats().HP <= 0)
                    continue;

                //Create border
                Border border = GetCharacterBorder(PlayerTeam[i]);
                PlayerPanel.Children.Add(border);

                //Add border to dictionary
                CharacterBorder.Add(PlayerTeam[i], border);
            }

            //Log
            BattleLog.Add("Battle started!");

            //Simulate battle
            SimulateBattle();
        }

        private Border GetCharacterBorder(Character character)
        {
            //Tooltip
            ToolTip tooltip = new ToolTip();
            tooltip.Style = FindResource("CharacterTooltip") as Style;
            tooltip.Content = $"{character.Name}\n" +
                $"===============\n" +
                $"DMG: {character.GetAccumelatedStats().DMG} || CRD: {character.GetAccumelatedStats().CRD} || CRC: {character.GetAccumelatedStats().CRC}\n" +
                $"STA: {character.GetAccumelatedStats().STA} || STR: {character.GetAccumelatedStats().STR}";

            //Border
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1, 1, 2, 2);
            border.CornerRadius = new CornerRadius(2);
            border.Width = 125;
            border.Margin = new Thickness(8, 0, 8, 0);
            border.Background = (Brush)new BrushConverter().ConvertFrom("#445069");
            ToolTipService.SetInitialShowDelay(border, 100);
            border.ToolTip = tooltip;
            border.MouseEnter += (s, e) => DisplayDetails(character);

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

            //Sta bar
            ProgressBar staBar = new ProgressBar();
            staBar.Height = 6;
            staBar.Width = 85;
            staBar.Margin = new Thickness(2);
            staBar.Maximum = character.GetAccumelatedStats().STA;
            staBar.Value = staBar.Maximum;
            staBar.Style = (Style)FindResource("XpProgressBar");
            staBar.Foreground = Brushes.BlanchedAlmond;
            stackPanel.Children.Add(staBar);

            //Add hp and def bar to dictionary
            CharHpDefStaBar.Add(character, (hpBar, defBar, staBar));

            return border;
        }

        #endregion

        #region Battle
        List<Character> Queue = new List<Character>();
        List<Character> Benched = new List<Character>(); //Characters that are skipping a round to rest
        int Round = 0;
        Dictionary<Character, (int, int)> CharHpDef = new Dictionary<Character, (int, int)>();
        Dictionary<Character, (int, int, int)> CharStaStrMax = new Dictionary<Character, (int, int, int)>(); //Current stamina, stamina regeneration and max stamina
        Dictionary<Character, (ProgressBar, ProgressBar, ProgressBar)> CharHpDefStaBar = new Dictionary<Character, (ProgressBar, ProgressBar, ProgressBar)>();
        bool Battle = true; //True = battle is still going on, false = battle is over
        private async void SimulateBattle()
        {
            //Battoru hajimaruyo!
            await Task.Delay(1200 / speedToggle);

            //Battle loop
            while (Battle)
            {
                //Loop through queue
                foreach (Character character in Queue)
                {
                    //Do we need to pause?
                    while (PauseBattle)
                    {
                        await Task.Delay(100);
                    }

                    //Is character dead?
                    if (CharHpDef[character].Item1 <= 0)
                        continue;

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
                        AnimateDeath(targetCharacter);
                        BattleLog.Add($"💀{targetCharacter.Name} fell.");

                        //PAUSE
                        await Task.Delay(200 / speedToggle);

                        //Check if battle is over
                        if (IsBattleOver())
                        {
                            //PAUSE
                            await Task.Delay(1000 / speedToggle);

                            Battle = false;
                            BtnFinish.Visibility = Visibility.Visible;
                            BtnSpeedToggle.Visibility = Visibility.Collapsed;
                            BtnPause.Visibility = Visibility.Collapsed;
                            break;
                        }
                    }

                    //PAUSE
                    await Task.Delay(1500 / speedToggle);

                    //Unhighlight
                    HighlightTarget(null);
                }

                //Break if battle is over
                if (!Battle)
                    break;

                //Create character queue
                CreateQueue();

                //Regenerate stamina
                RegenerateStamina();
                RegenerateArmour();
                UpdateAllBars();

                //PAUSE
                await Task.Delay(1200 / speedToggle);
            }

            //Battle is over
            if (CharHpDef.Any(x => x.Key.ID.Contains("CH") && x.Value.Item1 > 0))
            {
                //player won, hand out rewards
                (int xp, int coins) = CalculateXpCoinRewards();
            }
        }

        private void CreateQueue()
        {
            //Update round
            Round++;
            GlobalAnouncer($"Stepping into round{Round}");
            TxtRound.Text = $"Round {Round}";
            BattleLog.Add($">Round{Round}");

            //Create queue
            Dictionary<Character, int> speedDictionary = new Dictionary<Character, int>();
            List<Character> characterList = new List<Character>();
            //Add both teams to characterList
            characterList.AddRange(PlayerTeam);
            characterList.AddRange(FoeTeam);

            //Add characters to dictionary and calculate speed
            foreach (Character character in characterList)
            {
                //Skip if dead
                if (CharHpDef[character].Item1 <= 0)
                {
                    continue;
                }

                //If character is benched, rest
                if (Benched.Contains(character))
                {
                    RestCharacter(character); //Rest character (regen stamina)
                    Benched.Remove(character); //Remove from benched list
                }

                //Calculate speed based on stamina
                double staRatio = (double)CharStaStrMax[character].Item1 / (double)CharStaStrMax[character].Item3;
                double speed = (double)character.GetAccumelatedStats().SPD;

                //If ratio is below default sta consumtion, skip | ratio < 50%, speed -= 25% | else speed = speed
                if (CharStaStrMax[character].Item1 < GetStaminaConsumtion(character))
                {
                    //Add to rest list (bench)
                    Benched.Add(character);
                    BattleLog.Add($"💤{character.Name} skips turn to rest.");
                    continue;
                }
                else if (staRatio < 0.5)
                {
                    speed *= 0.25;
                }

                //Add to dictionary
                speedDictionary.Add(character, Convert.ToInt16(speed));
            }

            //Sort dictionary by speed
            speedDictionary = speedDictionary.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            //Add characters to queue
            Queue.Clear();
            foreach (Character c in speedDictionary.Keys)
            {
                Queue.Add(c);
            }
        }

        private void RestCharacter(Character c)
        {
            //Regen 80% stamina
            double stamina = CharStaStrMax[c].Item3 * 0.8;

            //If stamina is above max, set to max else add stamina
            if (stamina > CharStaStrMax[c].Item3)
                CharStaStrMax[c] = (CharStaStrMax[c].Item3, CharStaStrMax[c].Item2, CharStaStrMax[c].Item3);
            else
                CharStaStrMax[c] = (Convert.ToInt16(stamina) + CharStaStrMax[c].Item1, CharStaStrMax[c].Item2, CharStaStrMax[c].Item3);
        }

        private void UpdateBars(Character c)
        {
            CharHpDefStaBar[c].Item1.Value = CharHpDef[c].Item1;
            CharHpDefStaBar[c].Item2.Value = CharHpDef[c].Item2;
            CharHpDefStaBar[c].Item3.Value = CharStaStrMax[c].Item1;
        }

        private void UpdateAllBars()
        {
            //Update all bars
            foreach (Character c in CharHpDef.Keys)
            {
                //skip if dead
                if (CharHpDef[c].Item1 <= 0)
                    continue;

                //Update bars
                IncreaseBar(CharHpDefStaBar[c].Item1, CharHpDef[c].Item1);
                IncreaseBar(CharHpDefStaBar[c].Item2, CharHpDef[c].Item2);
                IncreaseBar(CharHpDefStaBar[c].Item3, CharStaStrMax[c].Item1);
            }
        }

        private async void IncreaseBar(ProgressBar bar, int limit)
        {
            while (bar.Value < limit)
            {
                bar.Value += 2;
                await Task.Delay(12 / speedToggle);
            }

            bar.Value = limit;
        }

        private void HighlightCharacter(Character character)
        {
            foreach (var pair in CharacterBorder)
            {
                if (pair.Key == character)
                {
                    //Highlight
                    pair.Value.BorderBrush = Brushes.SeaGreen;
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

            foreach (var pair in CharacterBorder)
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
                    target = PlayerTeam[Interaction.GetRandomNumber(0, PlayerTeam.Count() - 1)];
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

            //Consume Stamina
            ConsumeStamina(attacker, dmg);

            //Display damage
            DisplayNumbers(dmg, crit, target);

            //Does target have defence?
            bool brokeDef = false;
            if (CharHpDef[target].Item2 <= 0)
            {
                //No defence, lower HP
                CharHpDef[target] = (CharHpDef[target].Item1 - dmg, 0);
            }
            else
            {
                //Lower defence first, then HP
                CharHpDef[target] = (CharHpDef[target].Item1, CharHpDef[target].Item2 - dmg);

                //If target's defence is below 0, lower HP
                if (CharHpDef[target].Item2 <= 0)
                {
                    brokeDef = true;
                    CharHpDef[target] = (CharHpDef[target].Item1 - Math.Abs(CharHpDef[target].Item2), 0);
                }
            }

            //Animate hit
            AnimateHit(target);

            //Update HP and DEF bar
            UpdateBars(attacker);
            UpdateBars(target);

            //log
            if (crit && brokeDef)
                BattleLog.Add($"🗡️🔥🦾{attacker.Name} attacked {target.Name} for {dmg} damage! Critical hit! Broke defense!");
            else if (crit)
                BattleLog.Add($"🗡️🔥{attacker.Name} attacked {target.Name} for {dmg} damage! Critical hit!");
            else if (brokeDef)
                BattleLog.Add($"🗡️🦾{attacker.Name} attacked {target.Name} for {dmg} damage! Broke defense!");
            else
                BattleLog.Add($"🗡️{attacker.Name} attacked {target.Name} for {dmg} damage!");
        }

        private (int, bool) CalculateDmg(Character attacker)
        {
            //Base dmg
            double DMG = attacker.GetAccumelatedStats().DMG;
            bool crit = false;

            //Do we have a crit hit?
            if (Interaction.GetRandomNumber(0, 100) <= attacker.GetAccumelatedStats().CRC)
            {
                //Yes, crit hit
                DMG *= attacker.GetAccumelatedStats().CRD;
                crit = true;
            }

            //No, crit hit
            //Get stamina balance (%)
            double staBalance = CharStaStrMax[attacker].Item1 / (double)CharStaStrMax[attacker].Item3;

            //If stamina > 60%, no effect. | If stamina < 15%, 50% less damage | else -15% damage
            if (staBalance < 0.15)
                DMG *= 0.5;
            else if (staBalance < 0.6)
                DMG *= 0.85;

            //Return damage
            return ((int)DMG, crit);
        }

        private void ConsumeStamina(Character c, int dmg)
        {
            //Get stamina to damage ratio
            double staToDmgRatio = (double)CharStaStrMax[c].Item3 / dmg;
            double staToConsume = 0;

            //If ratio is < 25%, consume 80% stamina | If ratio is < 50%, consume 45% stamina | else consume 25% stamina
            if (staToDmgRatio < 0.25)
                staToConsume = CharStaStrMax[c].Item3 * 0.80;
            else if (staToDmgRatio < 0.5)
                staToConsume = CharStaStrMax[c].Item3 * 0.45;
            else
                staToConsume = CharStaStrMax[c].Item3 * 0.25;

            //Round up
            staToConsume = Math.Ceiling(staToConsume);

            //Consume stamina
            CharStaStrMax[c] = (CharStaStrMax[c].Item1 - (int)staToConsume, CharStaStrMax[c].Item2, CharStaStrMax[c].Item3);
        }

        private int GetStaminaConsumtion(Character c)
        {
            ///Gives an idea of how much stamina will be consumed. This number is affected by crit stats, hence why it's not accurate.
            //Get stamina to damage ratio
            double staToDmgRatio = (double)CharStaStrMax[c].Item3 / c.GetAccumelatedStats().DMG;
            double staToConsume = 0;

            //If ratio is < 25%, consume 80% stamina | If ratio is < 50%, consume 45% stamina | else consume 25% stamina
            if (staToDmgRatio < 0.25)
                staToConsume = CharStaStrMax[c].Item3 * 0.80;
            else if (staToDmgRatio < 0.5)
                staToConsume = CharStaStrMax[c].Item3 * 0.45;
            else
                staToConsume = CharStaStrMax[c].Item3 * 0.25;

            //Round up
            staToConsume = Math.Ceiling(staToConsume);

            return (int)staToConsume;
        }

        private void RegenerateStamina()
        {
            //Regenerate stamina foreach character in queue
            foreach (Character c in Queue)
            {
                //calculate stamina
                double sta = CharStaStrMax[c].Item1 + (CharStaStrMax[c].Item2 / 2);
                sta = Math.Ceiling(sta);

                //If stamina is above max, set to max
                if (sta > CharStaStrMax[c].Item3)
                    sta = CharStaStrMax[c].Item3;

                //Regenerate stamina
                CharStaStrMax[c] = ((int)sta, CharStaStrMax[c].Item2, CharStaStrMax[c].Item3);
            }
        }

        private void RegenerateArmour()
        {
            foreach (Character c in Queue)
            {
                //If armour is 0, skip
                if (CharHpDef[c].Item2 == 0)
                    continue;

                //calculate armour
                double armour = CharStaStrMax[c].Item2 / 2;
                armour = Math.Ceiling(armour);

                //Regenerate armour
                CharHpDef[c] = (CharHpDef[c].Item1, CharHpDef[c].Item2 + (int)armour);

                //Balance armour if above max
                if (CharHpDef[c].Item2 > c.GetAccumelatedStats().DEF)
                    CharHpDef[c] = (CharHpDef[c].Item1, c.GetAccumelatedStats().DEF);
            }
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

        private void DisplayDetails(Character c)
        {
            TxtDetails.Text = $"{c.Name}: [{CharHpDef[c].Item1} HP]  [{CharHpDef[c].Item2} DEF]  [{CharStaStrMax[c].Item1} STA]";
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

            txt.Text = crit ? $"🔥{dmg}" + "!!" : $"🔥{dmg}";
            txt.FontSize = crit ? 24 : 22;
            txt.FontWeight = FontWeights.Bold;
            txt.Foreground = Brushes.MediumVioletRed;

            // Create a Border to provide the semi-transparent background
            Border backgroundBorder = new Border();
            backgroundBorder.HorizontalAlignment = HorizontalAlignment.Left;
            backgroundBorder.VerticalAlignment = VerticalAlignment.Bottom;
            backgroundBorder.Background = new SolidColorBrush(Color.FromArgb(104, 0, 0, 0)); // Adjust the color and opacity as needed
            backgroundBorder.Margin = new Thickness(0, 0, 0, -34);
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

        private async void AnimateDeath(Character victim)
        {
            Border border = CharacterBorder[victim];

            //Lower opacity to 0.4
            while (border.Opacity > 0.64)
            {
                border.Opacity -= 0.04;
                await Task.Delay(20 / speedToggle);
            }
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

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            //Do we need to pause or resume?
            if (PauseBattle)
            {
                //Resume
                BtnPause.Content = "Pause";
                PauseBattle = false;
            }
            else
            {
                //Pause
                BtnPause.Content = "Resume";
                PauseBattle = true;
            }
        }

        private void BtnLog_Click(object sender, RoutedEventArgs e)
        {
            LogWindow window = new LogWindow(BattleLog);
            window.ShowDialog();
        }
    }
}
