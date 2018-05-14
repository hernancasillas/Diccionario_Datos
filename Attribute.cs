using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDictionary
{
    public class Attribute
    {
        public char[] name { get; set; }
        public char type { get; set; }
        public int length { get; set; }
        public int indexType { get; set; }
        public long attributeDir { get; set; }
        public long indexDir { get; set; }
        public long nextAttDir { get; set; }
        //public bool index = false;

        public Attribute() //Constructor para lectura
        {
            this.name = null;
            this.type = 'i';
            this.length = 4;
            this.attributeDir = -1;
            this.indexType = 0;
            this.indexDir = -1;
            this.nextAttDir = -1;
   
        }

        public Attribute(char type, int length, int indexType)
        {
            name = new char[30];
            switch(type)
            {
                case 'i': type = 'I';
                    break;
                case 's': type = 'C';
                    break;
                case 'c': type = 'C';
                    break;
            }
            this.type = type;
            this.length = length;
            this.indexType = indexType;
            attributeDir = -1;
            indexDir = -1;
            nextAttDir = -1;
        }

        public void SaveAttribute(BinaryWriter W)//Graba en el archivo los datos de la entidad
        {
            W.Write(this.name);
            W.Write(this.type);
            W.Write(this.length);
            W.Write(this.attributeDir);
            W.Write(this.indexType);
            W.Write(this.indexDir);
            W.Write(this.nextAttDir);
        }
    }
}
