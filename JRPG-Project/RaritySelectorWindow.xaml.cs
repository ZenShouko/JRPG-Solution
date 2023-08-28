using System.Windows;

namespace JRPG_Project
{
    public partial class RaritySelectorWindow : Window
    {
        public bool Common { get; set; }
        public bool Special { get; set; }
        public bool Cursed { get; set; }
        public bool Legendary { get; set; }
        public bool EnhancedItems { get; set; }
        public RaritySelectorWindow()
        {
            InitializeComponent();
        }

        public RaritySelectorWindow(bool common, bool special, bool cursed, bool legendary, bool enhancedItems)
        {
            InitializeComponent();
            Common = common;
            Special = special;
            Cursed = cursed;
            Legendary = legendary;
            EnhancedItems = enhancedItems;

            //Set the checkboxes
            CommonBox.IsChecked = Common;
            SpecialBox.IsChecked = Special;
            CursedBox.IsChecked = Cursed;
            LegendaryBox.IsChecked = Legendary;
            EnhancedBox.IsChecked = EnhancedItems;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Common = CommonBox.IsChecked.Value;
            Special = SpecialBox.IsChecked.Value;
            Cursed = CursedBox.IsChecked.Value;
            Legendary = LegendaryBox.IsChecked.Value;
            EnhancedItems = EnhancedBox.IsChecked.Value;

            Window.GetWindow(this).DialogResult = true;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Select all
            Common = true;
            Special = true;
            Cursed = true;
            Legendary = true;
            EnhancedItems = true;

            Window.GetWindow(this).DialogResult = true;
            Close();
        }
    }
}
