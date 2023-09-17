using JRPG_ClassLibrary;
using JRPG_Project.ClassLibrary.Data;
using JRPG_Project.ClassLibrary.Entities;
using JRPG_Project.ClassLibrary.Player;
using JRPG_Project.ClassLibrary.Universal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JRPG_Project.Tabs
{
    public partial class BattleTab : UserControl
    {
        private void BattlePrediction()
        {
            //Get total threat score of team
            int teamThreatScore = 0;
            foreach (Character c in Inventory.Team)
            {
                teamThreatScore += CharacterData.GetThreatScore(c);
            }

            //Get total threat score of foes
            int foeThreatScore = 0;
            foreach (Character c in FoeTeam)
            {
                foeThreatScore += CharacterData.GetThreatScore(c);
            }

            //Get difference
            if (teamThreatScore > foeThreatScore)
            {
                //Player team is stronger
                BattleLog.Add($"📖 Your team ({teamThreatScore} TS) is stronger than the enemy team ({foeThreatScore} TS)! The numbers are in your favor!");
            }
            else if (teamThreatScore < foeThreatScore)
            {
                //Foe team is stronger
                BattleLog.Add($"📖 The enemy team ({foeThreatScore} TS) is stronger than your team ({teamThreatScore} TS)! Odds are stacked against us!");
            }
            else
            {
                //Teams are equal
                BattleLog.Add($"📖 Your team is equal to the enemy team! ({teamThreatScore} TS) We have an equal team??");
            }
        }

        public BattleTab(List<Character> foeTeam)
        {
            InitializeComponent();
            SetRandomBackground();
            SoundManager.PlaySound("battle-start.wav");
            this.FoeTeam = foeTeam;
            foreach (Character c in Stages.CurrentStage.TeamHpDefDeduct.Keys)
            {
                PlayerTeam.Add(c);
            }

            BattlePrediction();
            InitializeBattle();
        }
        //Foe teams
        public List<Character> FoeTeam = new List<Character>();
        List<Character> PlayerTeam = new List<Character>();
        //Dictionary<Character, Border> CharacterBorder = new Dictionary<Character, Border>();
        Dictionary<Character, Canvas> CharacterCanvas = new Dictionary<Character, Canvas>();
        int speedToggle = 1; //Higher means faster
        List<string> BattleLog = new List<string>();
        bool PauseBattle = false; //Allows pausing the battle
        bool playerWin = false; //Determines if the player won the battle

        #region Prep
        private void SetRandomBackground()
        {
            //There are a total of 6 backgrounds named "pixel-rain1.jpg" to "pixel-rain6.jpg"
            int numb = Interaction.GetRandomNumber(1, 6);
            MainGrid.Background = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/Resources/Assets/GUI/pixel-rain{numb}.jpg", UriKind.RelativeOrAbsolute)));
        }

        private void InitializeBattle()
        {
            //Add all characters to the hp dictionary AND stamina dictionary
            foreach (Character character in PlayerTeam)
            {
                CharHpDef.Add(character, (character.GetAccumulatedStats().HP - Stages.CurrentStage.TeamHpDefDeduct[character].Item1,
                    character.GetAccumulatedStats().DEF - Stages.CurrentStage.TeamHpDefDeduct[character].Item2));
                CharStaStrMax.Add(character, (character.GetAccumulatedStats().STA, character.GetAccumulatedStats().STR, character.GetAccumulatedStats().STA));
            }
            foreach (Character character in FoeTeam)
            {
                CharHpDef.Add(character, (character.GetAccumulatedStats().HP, character.GetAccumulatedStats().DEF));
                CharStaStrMax.Add(character, (character.GetAccumulatedStats().STA, character.GetAccumulatedStats().STR, character.GetAccumulatedStats().STA));
            }

            //Place foes on the grid
            for (int i = 0; i < FoeTeam.Count(); i++)
            {
                Canvas canvas = GetCharacterBorder(FoeTeam[i]);
                FoePanel.Children.Add(canvas);

                //Add canvas to dictionary
                CharacterCanvas.Add(FoeTeam[i], canvas);
            }

            //Place player team on the grid
            for (int i = 0; i < PlayerTeam.Count(); i++)
            {
                //Create canvas
                Canvas canvas = GetCharacterBorder(PlayerTeam[i]);
                PlayerPanel.Children.Add(canvas);

                //Display dead characters as dead
                if (CharHpDef[PlayerTeam[i]].Item1 <= 0)
                {
                    canvas.Opacity = 0.64;
                    continue;
                }

                //Add canvas to dictionary
                CharacterCanvas.Add(PlayerTeam[i], canvas);
            }

            //Fade Main Grid in
            FadeMainGridIn();

            //TEST, make player really strongk
            //PlayerTeam[0].Stats.DMG = 100;
            //PlayerTeam[0].Stats.CRD = 100;
            //PlayerTeam[0].Stats.CRC = 100;
            //PlayerTeam[0].Stats.SPD = 100;
            //PlayerTeam[1].Stats.DMG = 100;
            //PlayerTeam[1].Stats.CRD = 100;
            //PlayerTeam[1].Stats.CRC = 100;
            //PlayerTeam[1].Stats.SPD = 100;
            //PlayerTeam[2].Stats.DMG = 100;
            //PlayerTeam[2].Stats.CRD = 100;
            //PlayerTeam[2].Stats.CRC = 100;
            //PlayerTeam[2].Stats.SPD = 100;

            //Log
            BattleLog.Add("Battle started!");

            //Simulate battle
            SimulateBattle();
        }

        private Canvas GetCharacterBorder(Character character)
        {
            //Tooltip
            ToolTip tooltip = new ToolTip();
            tooltip.Style = FindResource("CharacterTooltip") as Style;
            tooltip.Content = $"{character.Name}\n" +
                $"===============\n" +
                $"HP: {character.GetAccumulatedStats().HP} | DEF: {character.GetAccumulatedStats().DEF} | SPD: {character.GetAccumulatedStats().SPD}\n" +
                $"DMG: {character.GetAccumulatedStats().DMG} | CRD: {character.GetAccumulatedStats().CRD} | CRC: {character.GetAccumulatedStats().CRC}\n" +
                $"STA: {character.GetAccumulatedStats().STA} | STR: {character.GetAccumulatedStats().STR}";

            //Canvas (parent)
            Canvas canvas = new Canvas();
            canvas.Width = 125;
            canvas.Height = 125;
            canvas.HorizontalAlignment = HorizontalAlignment.Center;
            canvas.VerticalAlignment = VerticalAlignment.Center;
            canvas.Margin = new Thickness(8, 0, 8, 0);

            //Border
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1, 1, 2, 2);
            border.CornerRadius = new CornerRadius(2);
            border.Width = 125;
            //border.Margin = new Thickness(8, 0, 8, 0);
            border.Background = (Brush)new BrushConverter().ConvertFrom("#272829");
            ToolTipService.SetInitialShowDelay(border, 100);
            border.ToolTip = tooltip;
            border.MouseEnter += (s, e) => DisplayDetails(character);
            canvas.Children.Add(border);

            //Stackpanel
            StackPanel stackPanel = new StackPanel();
            border.Child = stackPanel;

            //Def bar
            ProgressBar defBar = new ProgressBar();
            defBar.Height = 10;
            defBar.Width = 116;
            defBar.Margin = new Thickness(2);
            defBar.Maximum = character.GetAccumulatedStats().DEF;
            defBar.Value = CharHpDef[character].Item2;
            defBar.Style = (Style)FindResource("XpProgressBar");
            defBar.Foreground = (Brush)new BrushConverter().ConvertFrom("#525FE1");
            stackPanel.Children.Add(defBar);

            //Image
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
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
            hpBar.Height = 10;
            hpBar.Width = 116;
            hpBar.Margin = new Thickness(2);
            hpBar.Maximum = character.GetAccumulatedStats().HP;
            hpBar.Value = CharHpDef[character].Item1;
            hpBar.Style = (Style)FindResource("XpProgressBar");
            hpBar.Foreground = (Brush)new BrushConverter().ConvertFrom("#F31559");
            stackPanel.Children.Add(hpBar);

            //Sta bar
            ProgressBar staBar = new ProgressBar();
            staBar.Height = 8;
            staBar.Width = 110;
            staBar.Margin = new Thickness(2);
            staBar.Maximum = character.GetAccumulatedStats().STA;
            staBar.Value = staBar.Maximum;
            staBar.Style = (Style)FindResource("XpProgressBar");
            staBar.Foreground = (Brush)new BrushConverter().ConvertFrom("#F4D160");
            stackPanel.Children.Add(staBar);

            //Add hp and def bar to dictionary
            CharHpDefStaBar.Add(character, (hpBar, defBar, staBar));

            return canvas;
        }

        private void FinishBattle()
        {
            if (playerWin)
            {
                //play win sound
                SoundManager.PlaySound("battle-win.wav");

                //Get Xp and coins
                (int xp, int coins) = CalculateXpCoinRewards();

                //Give every character xp
                foreach (Character c in Inventory.Team)
                {
                    int oldLevel = c.Level;
                    LevelData.AddXP(c, xp);

                    //Check if character leveled up
                    if (c.Level > oldLevel)
                    {
                        BattleLog.Add($"🎉{c.Name} leveled up! (Lv. {oldLevel} -> {c.Level})");
                        CharacterLevelUpWindow window = new CharacterLevelUpWindow(c);
                        window.ShowDialog();
                    }
                }

                //Give coins
                Inventory.Coins += coins;

                //Anmimate reward
                AnimateRewards(xp, coins);

                //Log
                BattleLog.Add($"Battle finished! {xp} XP and {coins} coins earned!");
            }
            else
            {
                //play lose sound
                SoundManager.PlaySound("battle-lose.wav");

                //Lose (10% + 10) of coins
                int coinsLost = (int)Math.Ceiling((Inventory.Coins + 10) * 0.1);
                Inventory.Coins -= coinsLost;

                //Safety check
                if (Inventory.Coins < 0)
                    Inventory.Coins = 0;

                //Animate loss
                AnimateRewards(0, -coinsLost);

                //Log
                BattleLog.Add($"Battle finished! You lost {coinsLost} coins!");
            }
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
                            BtnGodSpeed.Visibility = Visibility.Collapsed;
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

                //Skip regeneration if round = 1
                if (Round <= 1)
                    continue;

                //play refill sound
                SoundManager.PlaySound("refill.wav");

                //Regenerate stamina
                RegenerateStamina();
                RegenerateArmour();
                UpdateAllBars();

                //PAUSE
                await Task.Delay(1200 / speedToggle);
            }

            //Battle is over
            //Check if player won
            if (CharHpDef.Any(x => x.Key.ID.Contains("CH") && x.Value.Item1 > 0))
                playerWin = true;

            FinishBattle();
        }

        private void CreateQueue()
        {
            //Update round
            Round++;
            GlobalAnouncer($"--> round {Round}");
            TxtRound.Text = $"Round {Round}";
            AnimateRoundText();
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
                double speed = (double)character.GetAccumulatedStats().SPD;

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
                bar.Value += 2 * speedToggle > 1 ? 2 : 1;
                await Task.Delay(12 / speedToggle);
            }

            bar.Value = limit;
        }

        private void HighlightCharacter(Character character)
        {
            foreach (var pair in CharacterCanvas)
            {
                //Get border element from canvas
                Border border = pair.Value.Children.OfType<Border>().FirstOrDefault();
                if (pair.Key == character)
                {
                    //Highlight
                    border.BorderBrush = Brushes.SeaGreen;
                    border.BorderThickness = new Thickness(2, 1, 2, 3);
                }
                else
                {
                    //Unhighlight
                    border.BorderBrush = Brushes.Black;
                    border.BorderThickness = new Thickness(2, 1, 2, 2);
                }
            }
        }

        private void HighlightTarget(Character character)
        {
            if (character is null)
            {
                //Unhighlight target border
                foreach (var pair in CharacterCanvas)
                {
                    //Get border element from canvas
                    Border border = pair.Value.Children.OfType<Border>().FirstOrDefault();

                    if (border.BorderBrush == Brushes.MediumVioletRed)
                    {
                        border.BorderBrush = Brushes.Black;
                        border.BorderThickness = new Thickness(2, 1, 2, 2);
                    }
                }

                return;
            }

            foreach (var pair in CharacterCanvas)
            {
                if (!pair.Key.ID.Contains(character.ID.Substring(0, 1)))
                    continue;

                //Get border element from canvas
                Border border = pair.Value.Children.OfType<Border>().FirstOrDefault();

                if (pair.Key == character)
                {
                    //Highlight
                    border.BorderBrush = Brushes.MediumVioletRed;
                    border.BorderThickness = new Thickness(2, 1, 2, 3);
                }
                else
                {
                    //Unhighlight
                    border.BorderBrush = Brushes.Black;
                    border.BorderThickness = new Thickness(2, 1, 2, 2);
                }
            }
        }

        private Character GetTarget(Character currentChar)
        {
            //Vars
            Character target = new Character();
            string targetDistinguisher = currentChar.ID.Contains("F") ? "CH" : "F";
            string allyDistinguisher = currentChar.ID.Contains("F") ? "F" : "CH";

            //Calculate team damage with remaining people in queue
            int teamDamage = Queue
                .SkipWhile(x => x != currentChar)
                .Where(x => x.ID.Contains(allyDistinguisher))
                .Sum(x => x.GetAccumulatedStats().DMG);

            //Target list
            List<Character> targets = CharHpDef.Keys.Where(x => x.ID.Contains(targetDistinguisher)).ToList();
            //Remove all targets that are dead
            targets.RemoveAll(x => CharHpDef[x].Item1 <= 0);

            while (true)
            {
                //[1] Get lowest health target with no defence
                target = targets.OrderBy(x => CharHpDef[x].Item1).Where(x => CharHpDef[x].Item2 <= 0).FirstOrDefault();

                //-> Can we finish off the target?
                if (target != null && CharHpDef[target].Item1 <= teamDamage)
                    break;

                //[2] Get lowest defence target
                target = targets.OrderBy(x => CharHpDef[x].Item2).Where(x => CharHpDef[x].Item2 > 0).FirstOrDefault();

                //-> Can we break his shield?
                if (target != null && CharHpDef[target].Item2 <= teamDamage)
                    break;

                //[3] Get target with 20% or less health or less HP than remaining team damage AND no defence
                target = targets.Where(x => CharHpDef[x].Item1 <= CharHpDefStaBar[x].Item1.Maximum * 0.2 && CharHpDef[x].Item2 <= 0 ||
                x.Stats.HP < teamDamage && CharHpDef[x].Item2 <= 0).FirstOrDefault();

                //-> Check if team damage is enough to kill target
                if (target != null && CharHpDef[target].Item1 <= teamDamage)
                    break;

                //[4] Get target with 20% or less defence or less defence than remaining team damage
                target = targets
                    .OrderBy(x => CharHpDef[x].Item2)
                    .Where(x => CharHpDef[x].Item2 <= CharHpDefStaBar[x].Item2.Maximum * 0.2 || CharHpDef[x].Item2 < teamDamage)
                    .Where(x => CharHpDef[x].Item2 > 0) //filter out defenceless targets
                    .FirstOrDefault();

                //-> Can the team combined break the target's shield?
                if (target != null)
                    break;

                //[5] Get target with highest threat score
                target = targets.OrderByDescending(x => CharacterData.GetThreatScore(x)).FirstOrDefault();

                //-> Can we kill target within 3 turns?
                int teamDamage3Turns = (Queue.Where(x => x.ID.Contains(allyDistinguisher)).Sum(x => x.GetAccumulatedStats().DMG) * 2) + teamDamage;
                if (CharHpDef[target].Item1 + CharHpDef[target].Item2 <= teamDamage3Turns)
                    break;

                //[Default] Else, Pick a random target from the player's team
                target = targets[Interaction.GetRandomNumber(0, targets.Count() - 1)];
                break;
            }

            return target;
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

            //Play sound
            if (brokeDef)
                SoundManager.PlaySound($"shield-break{Interaction.GetRandomNumber(1, 3)}.wav");
            else if (crit)
                SoundManager.PlaySound($"crit-hit{Interaction.GetRandomNumber(1, 2)}.wav");
            else
                SoundManager.PlaySound($"hit{Interaction.GetRandomNumber(1, 3)}.wav");

            //Animate hit
            AnimateHit(target);

            //Update HP and DEF bar
            UpdateBars(attacker);
            UpdateBars(target);

            //log
            if (crit && brokeDef)
                BattleLog.Add($"💥🦾{attacker.Name} attacked {target.Name} for {dmg} damage! Critical hit! Broke defense!");
            else if (crit)
                BattleLog.Add($"💥{attacker.Name} attacked {target.Name} for {dmg} damage! Critical hit!");
            else if (brokeDef)
                BattleLog.Add($"🔥🦾{attacker.Name} attacked {target.Name} for {dmg} damage! Broke defense!");
            else
                BattleLog.Add($"🔥{attacker.Name} attacked {target.Name} for {dmg} damage!");
        }

        private (int, bool) CalculateDmg(Character attacker)
        {
            //Base dmg
            double DMG = attacker.GetAccumulatedStats().DMG;
            bool crit = false;

            //Do we have a crit hit?
            if (Interaction.GetRandomNumber(0, 100) <= attacker.GetAccumulatedStats().CRC)
            {
                //Yes, crit hit
                DMG += (DMG / 100) * attacker.GetAccumulatedStats().CRD;
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

            //Round up
            DMG = Math.Ceiling(DMG);

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
            double staToDmgRatio = (double)CharStaStrMax[c].Item3 / c.GetAccumulatedStats().DMG;
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
                double sta = CharStaStrMax[c].Item1 + (CharStaStrMax[c].Item2 / Interaction.GetRandomNumber(1, 2));
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
                double armour = CharStaStrMax[c].Item2 / Interaction.GetRandomNumber(4, 8);
                armour = Math.Ceiling(armour);

                //Regenerate armour
                CharHpDef[c] = (CharHpDef[c].Item1, CharHpDef[c].Item2 + (int)armour);

                //Balance armour if above max
                if (CharHpDef[c].Item2 > c.GetAccumulatedStats().DEF)
                    CharHpDef[c] = (CharHpDef[c].Item1, c.GetAccumulatedStats().DEF);
            }
        }

        private (int, int) CalculateXpCoinRewards()
        {
            int xpReward = 0;
            int coinsReward = 0;

            //Calculate XP reward
            int value = 0;
            foreach (Character c in FoeTeam)
            {
                value += CharacterData.GetValue(c);
            }

            xpReward = (int)Math.Ceiling(Convert.ToDouble(value) * (0.64 - (Round / 32)));

            //Calculate coins reward
            coinsReward = (int)Math.Ceiling(Convert.ToDouble(value) * 0.32);

            if (Round < 2)
            {
                coinsReward *= 2;
            }
            else
            {
                int roundPunishment = Convert.ToInt32(Convert.ToDouble(coinsReward) % 10);
                coinsReward -= roundPunishment * Round;
            }

            //Avoid negative values
            if (xpReward < 0)
                xpReward = 0;
            if (coinsReward < 0)
                coinsReward = 0;

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
            TxtDetails.Text = $"{c.Name}: [{CharHpDef[c].Item1} HP]  [{CharHpDef[c].Item2} DEF]  [{CharStaStrMax[c].Item1} STA] [{CharacterData.GetThreatScore(c)} TS]";
        }

        private void GlobalAnouncer(string text)
        {
            TxtGlobalAnouncer.Text = text;
        }

        #endregion

        #region Animations

        private async void FadeMainGridIn()
        {
            //Fade in main grid
            while (MainGrid.Opacity < 1)
            {
                MainGrid.Opacity += 0.01;
                await Task.Delay(12);
            }
        }

        private async void AnimateAttack(Character attacker)
        {
            //Vars
            int margin = 0;
            //get border from CharacterCanvas 
            Canvas canvas = CharacterCanvas[attacker];
            Border border = canvas.Children.OfType<Border>().FirstOrDefault();
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
            Canvas canvas = CharacterCanvas[target];
            Border border = (Border)canvas.Children[0];
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
            Canvas canvas = CharacterCanvas[target];

            //Create a textblock to display the damage
            TextBlock txt = new TextBlock();

            txt.Text = crit ? $"💥{dmg}!" : $"🔥{dmg}";
            txt.FontSize = crit ? 24 : 22;
            txt.FontWeight = FontWeights.Bold;
            txt.Foreground = target.ID.Contains("CH") == true ? (Brush)new BrushConverter().ConvertFrom("#d11149") /*Red*/ : (Brush)new BrushConverter().ConvertFrom("#0affc2") /*green*/;
            txt.TextTrimming = TextTrimming.None;
            txt.TextWrapping = TextWrapping.WrapWithOverflow;
            txt.HorizontalAlignment = HorizontalAlignment.Stretch;

            // Create a Border to provide the semi-transparent background
            Border backgroundBorder = new Border();
            backgroundBorder.HorizontalAlignment = HorizontalAlignment.Left;
            backgroundBorder.VerticalAlignment = VerticalAlignment.Bottom;
            Brush backColor = new SolidColorBrush(Colors.Black);
            backColor.Opacity = 0.8;
            backgroundBorder.Background = backColor;
            backgroundBorder.Margin = target.ID.Contains("CH") ? new Thickness(-10, -28, 0, 28) : new Thickness(-10, 0, 0, -182);
            backgroundBorder.CornerRadius = new CornerRadius(2);
            backgroundBorder.Padding = new Thickness(4, 0, 4, 0);
            backgroundBorder.Child = txt;
            backgroundBorder.Opacity = 0.1;
            Canvas.SetZIndex(backgroundBorder, 100);

            //Add textblock to stackpanel
            canvas.Children.Add(backgroundBorder);

            //Animate from left to right
            while (backgroundBorder.Margin.Left < 10)
            {
                backgroundBorder.Margin = new Thickness(backgroundBorder.Margin.Left + 1, backgroundBorder.Margin.Top, backgroundBorder.Margin.Right - 1, backgroundBorder.Margin.Bottom);

                //Fade in
                if (backgroundBorder.Opacity < 1)
                    backgroundBorder.Opacity += 0.05;

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
            canvas.Children.Remove(backgroundBorder);
        }

        private async void AnimateDeath(Character victim)
        {
            //play death sound
            SoundManager.PlaySound("ko.wav");

            //Get border from CharacterCanvas
            Canvas canvas = CharacterCanvas[victim];
            Border border = canvas.Children.OfType<Border>().FirstOrDefault();

            //Animate death symbol
            AnimateDeathSymbol(victim);

            //Lower opacity to 0.64
            while (border.Opacity > 0.64)
            {
                border.Opacity -= 0.04;
                await Task.Delay(20 / speedToggle);
            }
        }

        private async void AnimateDeathSymbol(Character victim)
        {
            //Get border from CharacterCanvas
            Canvas canvas = CharacterCanvas[victim];
            Border border = canvas.Children.OfType<Border>().FirstOrDefault();

            //Create a textblock to display the skull symbol
            TextBlock skull = new TextBlock();
            Brush backColor = new SolidColorBrush(Colors.Black);
            backColor.Opacity = 0.05;
            skull.Background = backColor;
            skull.Padding = new Thickness(4, 2, 4, 2);
            skull.Text = "😵";
            skull.FontSize = 32;
            skull.Foreground = Brushes.White;
            skull.TextTrimming = TextTrimming.None;
            skull.Margin = new Thickness(35, 50, 0, 0);
            skull.Opacity = 0;

            // Add textblock to stackpanel
            canvas.Children.Add(skull);

            //Animate to up
            while (skull.Margin.Top > -40)
            {
                skull.Margin = new Thickness(skull.Margin.Left, skull.Margin.Top - 1, skull.Margin.Right, skull.Margin.Bottom);

                //Fade in
                if (skull.Opacity < 1)
                    skull.Opacity += 0.5;

                await Task.Delay(13 / speedToggle);
            }

            //Wait 2
            await Task.Delay(2400 / speedToggle);

            //Fade out
            while (skull.Opacity > 0)
            {
                skull.Opacity -= 0.1;
                await Task.Delay(15 / speedToggle);
            }

            //Remove element
            canvas.Children.Remove(skull);
        }

        private async void AnimateRoundText()
        {
            int margin = 0;

            //Bounce down
            while (margin > -10)
            {
                margin--;
                TxtRound.Margin = new Thickness(TxtRound.Margin.Left, -margin, TxtRound.Margin.Right, margin);
                await Task.Delay(8 / speedToggle);
            }

            //Move back up
            while (margin < 0)
            {
                margin++;
                TxtRound.Margin = new Thickness(TxtRound.Margin.Left, -margin, TxtRound.Margin.Right, margin);
                await Task.Delay(8 / speedToggle);
            }
        }

        private async void AnimateRewards(int xp, int coins)
        {
            Border border = new Border();
            border.Width = 200;
            border.Height = 100;
            border.CornerRadius = new CornerRadius(2, 4, 4, 2);
            Brush backBrush = new SolidColorBrush(Colors.Black);
            backBrush.Opacity = 0.8;
            border.Background = backBrush;
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1, 1, 2, 2);
            Grid.SetColumnSpan(border, 1);
            Grid.SetRowSpan(border, MainGrid.RowDefinitions.Count);
            border.HorizontalAlignment = HorizontalAlignment.Center;
            border.VerticalAlignment = VerticalAlignment.Center;
            border.Margin = new Thickness(0, 10, 0, -10);

            //Create a stackpanel to hold the textblocks
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Vertical;
            stack.HorizontalAlignment = HorizontalAlignment.Center;
            stack.VerticalAlignment = VerticalAlignment.Center;
            border.Child = stack;

            //Create textblocks
            TextBlock txtXP = new TextBlock();
            txtXP.Text = $"+{xp} XP";
            txtXP.FontSize = 24;
            txtXP.FontWeight = FontWeights.Bold;
            txtXP.Foreground = (Brush)new BrushConverter().ConvertFrom("#0affc2");
            txtXP.TextTrimming = TextTrimming.None;
            txtXP.TextWrapping = TextWrapping.WrapWithOverflow;
            txtXP.HorizontalAlignment = HorizontalAlignment.Center;

            TextBlock txtCoins = new TextBlock();
            txtCoins.Text = $"+{coins} Coins";
            txtCoins.FontSize = 24;
            txtCoins.FontWeight = FontWeights.Bold;
            txtCoins.Foreground = Brushes.Goldenrod;
            txtCoins.TextTrimming = TextTrimming.None;
            txtCoins.TextWrapping = TextWrapping.WrapWithOverflow;
            txtCoins.HorizontalAlignment = HorizontalAlignment.Center;

            //Add textblocks to stackpanel
            stack.Children.Add(txtXP);
            stack.Children.Add(txtCoins);

            //Add border to grid
            MainGrid.Children.Add(border);

            //Animate from bottom to top
            while (border.Margin.Bottom < 20)
            {
                border.Margin = new Thickness(border.Margin.Left, border.Margin.Top - 1, border.Margin.Right, border.Margin.Bottom + 1);
                await Task.Delay(32);
            }

            await Task.Delay(4000 / speedToggle);

            //Fade out
            while (border.Opacity > 0)
            {
                border.Opacity -= 0.05;
                await Task.Delay(10);
            }

            //Remove element
            MainGrid.Children.Remove(border);
        }

        #endregion

        #region Button Events
        private void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            if (playerWin)
                SoundManager.PlaySound("equip.wav");
            else
                SoundManager.PlaySound("unequip.wav");

            //Add deductions to team
            foreach (Character c in PlayerTeam)
                Stages.CurrentStage.TeamHpDefDeduct[c] = (c.GetAccumulatedStats().HP - CharHpDef[c].Item1, c.GetAccumulatedStats().DEF - CharHpDef[c].Item2);

            //Return to previous tab
            Interaction.CloseBattleTab(playerWin);
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
            //Pause game if not paused
            if (!PauseBattle)
            {
                PauseBattle = true;
                BtnPause.Content = "Paused..";
            }

            SoundManager.PlaySound("click-short.wav");
            LogWindow window = new LogWindow(BattleLog);
            window.ShowDialog();

            //Unpause game after closing log window
            if (window.IsActive == false)
            {
                PauseBattle = false;
                BtnPause.Content = "Pause";
            }
        }

        private void BtnGodSpeed_Click(object sender, RoutedEventArgs e)
        {
            //Unpause game if paused
            if (PauseBattle)
            {
                BtnPause.Content = "Pause";
                PauseBattle = false;
            }

            //Put speed toggle on 100
            speedToggle = 20;

            //Background kala
            ((Button)sender).Background = Brushes.Crimson;

            BtnSpeedToggle.IsEnabled = false;
            BtnSpeedToggle.Background = Brushes.Gray;
            BtnPause.IsEnabled = false;
            BtnPause.Background = Brushes.Gray;
        }

        #endregion
    }
}
