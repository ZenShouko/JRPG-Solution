using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Project.ClassLibrary.Entities
{
    public class ItemValueFormula
    {
        public string ItemType { get; set; }
        public double DmgMultiplier { get; set; }
        public double DefMultiplier { get; set; }
        public double HpMultiplier { get; set; }
        public double SpdMultiplier { get; set; }
        public double CrcMultiplier { get; set; }
        public double CrdMultiplier { get; set; }
        public double StaMultiplier { get; set; }
        public double StrMultiplier { get; set; }
    }
}
