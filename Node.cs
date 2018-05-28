using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDictionary
{
    public class Node
    {

        private long DirNodo = -1;
        private char tipo = 'H';
        private List<long> lApuntadores;
        private List<int> idatos;

        public Node()
        {
            lApuntadores = new List<long>();
            idatos = new List<int>();
        }

        public long nodeDir
        {
            get { return DirNodo; }
            set { DirNodo = value; }
        }

        public List<long> directions
        {
            get { return lApuntadores; }
            set { lApuntadores = value; }
        }

        public List<int> dataL
        {
            get { return idatos; }
            set { idatos = value; }
        }

        public char type
        {
            get { return tipo; }
            set { tipo = value; }
        }

    }
}
