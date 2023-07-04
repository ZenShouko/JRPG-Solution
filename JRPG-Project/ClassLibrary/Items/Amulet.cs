using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Project.ClassLibrary.Items
{
    public class Amulet : BaseItem
    {
        public Stats Stats { get; set; } //Stats of the amulet
        public string CharmID { get; set; } //ID of the charm
        public Charm Charm { get; set; } //Charm of the amulet
    }
}
