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
            //Wait 3 seconds
            await Task.Delay(3000);

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

                    //Anounce turn
                    GlobalAnouncer($"{character.Name}'s turn");

                    //PAUSE
                    await Task.Delay(750);

                    //Pick a random target
                    Character targetCharacter = GetTarget(character);
                    HighlightTarget(targetCharacter);

                    //Attack target
                    Attack(character, targetCharacter);

                    //PAUSE
                    await Task.Delay(5000);

                    //Unhighlight
                    HighlightTarget(null);
                }
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
            GlobalAnouncer($"{attacker.Name} attacks {target.Name}!");
            

            //Lower defence first, then HP
            CharHpDef[target] = (CharHpDef[target].Item1, CharHpDef[target].Item2 - attacker.GetAccumelatedStats().DMG);

            //If target's defence is below 0, lower HP
            if (CharHpDef[target].Item2 <= 0)
            {
                CharHpDef[target] = (CharHpDef[target].Item1 - Math.Abs(CharHpDef[target].Item2), 0);
            }

            //Update HP and DEF bar
            CharHpDefBar[target].Item1.Value = CharHpDef[target].Item1;
            CharHpDefBar[target].Item2.Value = CharHpDef[target].Item2;
        }

        private void FoeAnouncer(string text)
        {
            TxtFoeAnouncer.Text = text;
        }

        private void PlayerAnouncer(string text)
        {
            TxtPlayerAnouncer.Text = text;
        }

        private void GlobalAnouncer(string text)
        {
            TxtGlobalAnouncer.Text = text;
        }

        #endregion
    }
}
