using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDictionary
{
    public class PrimaryKey
    {
        private object clave = 0;
        private int claveInt { get; set; }
        private char[] claveChar { get; set; }
        private long dirPk = -1;

        public object oClave
        {
            get { return clave; }
            set { clave = value; }
        }

        public long lDireccion
        {
            get { return dirPk; }
            set { dirPk = value; }
        }
    }
}
