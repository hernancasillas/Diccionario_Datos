using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDictionary
{
    public class ForeignKey
    {
        private object clave = 0;
        private List<long> LDir;

        public ForeignKey()
        {
            LDir = new List<long>();
        }

        public object oClave
        {
            get { return clave; }
            set { clave = value; }
        }

        public List<long> directions
        {
            get { return LDir; }
            set { LDir = value; }
        }
    }
}
