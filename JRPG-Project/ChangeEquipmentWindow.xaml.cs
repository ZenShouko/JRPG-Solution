using JRPG_Project.ClassLibrary;
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
using System.Windows.Shapes;

namespace JRPG_Project
{
    /// <summary>
    /// Interaction logic for ChangeEquipmentWindow.xaml
    /// </summary>
    public partial class ChangeEquipmentWindow : Window
    {
        public ChangeEquipmentWindow(string type, int charIndex)
        {
            InitializeComponent();
            LoadItems(type);
        }

        private void LoadItems(string type)
        {
            //Load items based on type
            if (type == "WEAPON")
            {
                foreach (Weapon weapon in Inventory.Weapons)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Tag = weapon.ID;
                    item.Content = weapon.Name;
                    ListboxItems.Items.Add(item);
                }
            }
        }
    }
}
