using JRPG_Project.ClassLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Project.ClassLibrary.Entities
{
    public class Collectable : BaseItem
    {
        public bool IsSellable { get; set; } //Can the item be sold?

        public override void CopyFrom(BaseItem otherItem)
        {
            base.CopyFrom(otherItem);

            if (otherItem is Collectable col)
            {
                IsSellable = col.IsSellable;
                ItemImage = ItemData.GetItemImage("Collectables/" + col.ImageName);
            }
        }
    }
}
