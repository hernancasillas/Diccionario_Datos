using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataDictionary
{
    public class AuxFile
    {
        private BinaryReader read;
        private BinaryWriter write;
        public FileStream stream { get; set; }
        public string fileName;

        #region File Methods

        public long getHead()
        {
            long head;
            stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            read = new BinaryReader(stream);

            stream.Seek(0, SeekOrigin.Begin);
            head = read.ReadInt64();

            read.Close();
            read.Dispose();

            stream.Close();
            stream.Dispose();

            return head;
        }

        public void modifyHead(long newCab)
        {
            stream = new FileStream(fileName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            write = new BinaryWriter(stream);

            stream.Seek(0, SeekOrigin.Begin);
            write.Write(newCab);

            stream.Close();
            stream.Dispose();

            write.Close();
            write.Dispose();
        }

        public long getLenght()
        {
            long lenght ;
            stream = new FileStream(fileName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            lenght = stream.Length;
            stream.Close();
            stream.Dispose();
            return lenght;
        }

        public Entity getNext(long pos)
        {
            Entity next;
            FileStream stream1 = new FileStream(fileName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(stream1);
            BinaryWriter bw = new BinaryWriter(stream1);
            byte[] buffer = new byte[50];

            bw.Seek((int)pos, SeekOrigin.Begin);
            br.Read(buffer, 0, 50);
            next = new Entity();
            next.name = new string(Encoding.GetEncoding(0).GetChars(buffer)).ToArray();
            next.nextDir = br.ReadInt64();
            next.attributeDir = br.ReadInt64();
            next.dataDir = br.ReadInt64();

            stream1.Close();
            stream1.Dispose();

            bw.Close();
            br.Close();
            return next;
        }

        public char[] stringToChar(String Cad, int longitud)
        {
            char[] aux = new char[longitud];
            for (int i = 0; i < Cad.Count(); i++)
            {
                aux[i] = Cad[i];
            }
            return aux;
        }

        #endregion

        #region Entity

        public void addEntity(Entity newEnt)
        {
            long position = getHead();
            long tamArch = getLenght();

            stream = new FileStream(fileName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            write = new BinaryWriter(stream);

            if (position == -1)
                stream.Seek(position, SeekOrigin.Begin);
            else
                stream.Seek(tamArch, SeekOrigin.Begin);

            write.Write(newEnt.name);
            write.Write(newEnt.entityDir);
            write.Write(newEnt.attributeDir);
            write.Write(newEnt.dataDir);
            write.Write(newEnt.nextDir);

            write.Close();
            write.Dispose();

            stream.Close();
            stream.Dispose();
        }

        public void modifyEntity(long direction, Entity entity)
        {
            stream = new FileStream(fileName, FileMode.Open, FileAccess.Write);
            write = new BinaryWriter(stream);

            stream.Seek(direction, SeekOrigin.Begin);

            write.Write(entity.name);
            write.Write(entity.entityDir);
            write.Write(entity.attributeDir);
            write.Write(entity.dataDir);
            write.Write(entity.nextDir);

            write.Close();
            write.Dispose();

            stream.Close();
            stream.Dispose();
        }

        public bool viewEntities( )
        {
            //stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            BinaryReader br = new BinaryReader(stream);
            BinaryWriter bw = new BinaryWriter(stream);
            byte[] buffer = new byte[50];

            long head;
            Entity aux;

            if (stream != null)
                stream.Close();

            //list.Items.Clear();
            if (br != null)
            {
                head = getHead();

                if (head != -1)
                {
                    aux = getNext(head);
                    //list.Items.Add(new string(aux.NOMBRE));

                    while (head != -1)
                    {
                        head = aux.nextDir;

                        if (head != -1)
                        {
                            aux = getNext(head);
                            //list.Items.Add(new string(aux.NOMBRE));
                        }
                    }

                    //stream.Close();
                    //stream.Dispose();
                    return true;
                }
                else
                    MessageBox.Show("There are no entities in the file.");

            }
           // stream.Close();
           // stream.Dispose();
            return false;
        }

        #endregion

        #region Attribute

        public void addAtribute(Attribute attribute)
        {
            long position = getHead();
            long tamaño = getLenght();

            stream = new FileStream(fileName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            write = new BinaryWriter(stream);

            if (position == -1)
                stream.Seek(position, SeekOrigin.Begin);
            else
                stream.Seek(tamaño, SeekOrigin.Begin);

            write.Write(attribute.name);
            write.Write(attribute.type);
            write.Write(attribute.length);
            write.Write(attribute.attributeDir);
            write.Write(attribute.indexType);
            write.Write(attribute.indexDir);
            write.Write(attribute.nextAttDir);
            

            write.Close();
            write.Dispose();
            stream.Close();

            stream.Dispose();
        }

        public void modifyAttributePointer(long dir, Entity ent, long attribute)
        {
            stream = new FileStream(fileName, FileMode.Open, FileAccess.Write);
            write = new BinaryWriter(stream);

            stream.Seek(dir, SeekOrigin.Begin);

            write.Write(ent.name);
            write.Write(attribute);
            write.Write(ent.entityDir);
            write.Write(ent.dataDir);
            write.Write(ent.nextDir);

            write.Close();
            write.Dispose();
            stream.Close();

            stream.Dispose();
        }

        public void modifyAttribute(long dir, Attribute attribute)
        {
            stream = new FileStream(fileName, FileMode.Open, FileAccess.Write);
            write = new BinaryWriter(stream);

            stream.Seek(dir, SeekOrigin.Begin);

            write.Write(attribute.name);
            write.Write(attribute.type);
            write.Write(attribute.length);
            write.Write(attribute.attributeDir);
            write.Write(attribute.indexType);
            write.Write(attribute.indexDir);
            write.Write(attribute.nextAttDir);

            write.Close();
            write.Dispose();

            stream.Close();
            stream.Dispose();
        }

        #endregion

        #region PK
        public void modificaPk(Entity Ent, int ind, BinaryWriter write)
        {

            write.BaseStream.Seek(Ent.attributes[ind].indexDir, SeekOrigin.Begin);
            foreach (PrimaryKey p in Ent.pk)
            {
                if (Ent.attributes[ind].type == 'C')
                {
                    write.Write(stringToChar(p.oClave.ToString(), Ent.attributes[ind].length));
                    write.Write(p.lDireccion);
                }
                else
                {
                    write.Write(Convert.ToInt32(p.oClave));
                    write.Write(p.lDireccion);
                }
            }
        }
        #endregion

        #region FK

        ////MÉTODO DE MODIFICACIÓN DE CLAVE SECUNDARIA
        public void ModificaFk(Entity Ent, int ind, BinaryWriter write)
        {
            write.BaseStream.Seek(Ent.attributes[ind].indexDir, SeekOrigin.Begin);
            foreach (ForeignKey f in Ent.fk)
            {
                if (Ent.attributes[ind].type == 'C')
                {
                    write.Write(stringToChar(f.oClave.ToString(), Ent.attributes[ind].length));

                    foreach (long l in f.lDirecciones)
                    {
                        write.Write(l);
                    }
                }
                else
                {
                    write.Write(Convert.ToInt32(f.oClave));

                    foreach (long l in f.lDirecciones)
                    {
                        write.Write(l);
                    }
                }
            }
        }

        #endregion

        #region B+

        //MÉTODO PARA ESCRIBIER EL NODO EN EL ARCHIVO
        public void writeNode(Nodo n, BinaryWriter w)
        {
            long dir = -1;
            int dat = 0;
            w.BaseStream.Seek(n.lDireccionN, SeekOrigin.Begin);
            w.Write(n.lDireccionN);
            w.Write(n.cTipo);

            if (n.cTipo == 'H')
            {
                for (int i = 0; i < 4; i++)
                {
                    if (n.iDatos.Count > i)
                    {
                        w.Write(n.lDirecciones[i]);
                        w.Write(n.iDatos[i]);
                    }
                    else
                    {
                        w.Write(dir);
                        w.Write(dat);
                    }
                }
                w.Write(dir);
            }
            else
            {
                w.Write(n.lDirecciones[0]);
                for (int i = 0; i < 4; i++)
                {
                    if (n.iDatos.Count > i)
                    {
                        w.Write(n.iDatos[i]);
                        w.Write(n.lDirecciones[i + 1]);
                    }
                    else
                    {
                        w.Write(dat);
                        w.Write(dir);
                    }
                }
            }
        }
        //MÉTODO PARA LEER UN NODO
        public Nodo readNode(long dirIndice, BinaryReader R)
        {
            Nodo nodo;
            long dir;
            int dato;
            nodo = new Nodo();
            R.BaseStream.Seek(dirIndice, SeekOrigin.Begin);
            nodo.lDireccionN = R.ReadInt64();
            nodo.cTipo = R.ReadChar();
            if (nodo.cTipo == 'H')
            {
                for (int i = 0; i < 4; i++)
                {
                    dir = R.ReadInt64();
                    dato = R.ReadInt32();
                    if (dato != 0)
                    {
                        nodo.lDirecciones.Add(dir);
                        nodo.iDatos.Add(dato);
                    }
                }
            }
            else
            {
                dir = R.ReadInt64();
                nodo.lDirecciones.Add(dir);
                for (int i = 0; i < 4; i++)
                {
                    dato = R.ReadInt32();
                    dir = R.ReadInt64();
                    if (dato != 0)
                    {
                        nodo.iDatos.Add(dato);
                        nodo.lDirecciones.Add(dir);
                    }
                }
            }
            return nodo;
        }

        #endregion

    }
}
