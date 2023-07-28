using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JRPG_Project.ClassLibrary
{
    /// <summary>
    /// Contains the base properties for all items
    ///</summary>
    public abstract class BaseItem
    {
        public string UniqueID { get; set; } //Unique on inventory level
        public string ID { get; set; } //Unique on global level
        public int Level { get; set; } = 1; //Level of the item
        public string Rarity { get; set; } //Rarity of the item
        public string Name { get; set; }
        public string Description { get; set; } //Description of the item
        public string ImageName { get; set; } //Name of the image, used when deserializing
        public int Value { get; set; } //Value of the item

        [JsonIgnore]
        public Image ItemImage { get; set; } //Image of the item

        public override string ToString()
        {
            return $"[{Level}] {Name}";
        }

        public virtual void CopyFrom(BaseItem otherItem)
        {
            UniqueID = Guid.NewGuid().ToString();
            ID = otherItem.ID;
            Level = otherItem.Level;
            Rarity = otherItem.Rarity;
            Name = otherItem.Name;
            Description = otherItem.Description;
            ImageName = otherItem.ImageName;
            Value = otherItem.Value;
        }
    }
}
