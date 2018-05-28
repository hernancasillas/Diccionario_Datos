using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDictionary
{
    public class Entity
    {
        public char[] name { get; set; }
        public long entityDir { get; set; }
        public long attributeDir { get; set; }
        public long dataDir { get; set; }
        public long nextDir { get; set; }
        public List<Attribute> attributes { get; set; }
        public List<Data> data { get; set; }
        public List<PrimaryKey> pk { get; set; }
        public List<ForeignKey> fk { get; set; }
        public List<Node> nodes { get; set; }

        public Entity()
        {
            name = new char[30];
            entityDir = -1;
            attributeDir = -1;
            dataDir = -1;
            nextDir = -1;
            attributes = new List<Attribute>();
            data = new List<Data>();
            pk = new List<PrimaryKey>();
            fk = new List<ForeignKey>();
            nodes = new List<Node>();
        }

        public void SaveEntity(FileStream Archivo, BinaryWriter W)//Graba en el archivo los datos de la entidad
        {
            W.Write(this.name);
            W.Write(this.entityDir);
            W.Write(this.attributeDir);
            W.Write(this.dataDir);
            W.Write(this.nextDir);
        }

        public void SaveEntity2(BinaryWriter W)//Graba en el archivo los datos de la entidad
        {
            W.Write(this.name);
            W.Write(this.entityDir);
            W.Write(this.attributeDir);
            W.Write(this.dataDir);
            W.Write(this.nextDir);
        }
    }
}
