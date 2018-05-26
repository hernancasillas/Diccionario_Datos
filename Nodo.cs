using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDictionary
{
    public class Nodo
    {

        private long DirNodo = -1;
        private char tipo = 'H';
        private List<long> lApuntadores;
        private List<int> idatos;

        public Nodo()
        {
            lApuntadores = new List<long>();
            idatos = new List<int>();
        }

        public long lDireccionN
        {
            get { return DirNodo; }
            set { DirNodo = value; }
        }

        public List<long> lDirecciones
        {
            get { return lApuntadores; }
            set { lApuntadores = value; }
        }

        public List<int> iDatos
        {
            get { return idatos; }
            set { idatos = value; }
        }

        public char cTipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

    }
}
