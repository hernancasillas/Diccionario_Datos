using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDictionary
{
    public class Data
    {
        public long dataDir; //Dirrecion actual del registro
        public List<int> number { get; set; }//Lista de enteros
        public List<char[]> str { get; set; } //Lista de cadenas
        public long nextDir;//Dirrecion del siguinte registro

        public Data()
        {
            this.dataDir = -1;
            this.nextDir = -1;
            this.number = new List<int>();
            this.str = new List<char[]>();
        }

        private char[] stringToCharArray(string cad, int num)
        {
            char[] str = new char[num];
            for (int i = 0; i < cad.Count(); i++)
                str[i] = cad[i];
            return str;
        }

        public void saveData(FileStream A, BinaryWriter W, List<Attribute> attributes)//Graba en el archivo los elementos del registro
        {
            W.Write(this.dataDir);
            int i = 0; int j = 0;
            foreach (Attribute att in attributes)
                if (att.type == 'C')//Si es char escribe en la lista de cadena
                {
                    //char[] strg = stringToCharArray(this.str[i++], att.length);
                    W.Write(this.str[i++]);
                }
                else//Si es entero escribe en la lista de enteros
                    W.Write(this.number[j++]);
            W.Write(this.nextDir);
        }

    }
}
