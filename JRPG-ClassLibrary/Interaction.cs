using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_ClassLibrary
{
    public static class Interaction
    {
        private static object _key = null;
        public static void SetKey(object key)
        {
            //Prioritize reset
            if (key is null)
            {
                _key = null;
            }
            
            //Set key if not already set
            if (_key is null)
            {
                _key = key;
            }
        }

        public static object GetKey()
        {
            return _key;
        }
    }
}
