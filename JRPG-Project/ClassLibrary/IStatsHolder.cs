using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Project.ClassLibrary
{
    /// <summary>
    /// Interface for classes that hold stats.
    /// </summary>
    public interface IStatsHolder
    {
        Stats Stats { get; set; }
    }
}
