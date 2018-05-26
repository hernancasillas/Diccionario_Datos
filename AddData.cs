using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataDictionary
{
    public partial class AddData : Form
    {
        static List<Entity> entities;
        List<PrimaryKey> sordedPKlist = new List<PrimaryKey>();
        List<ForeignKey> sortedFKlist = new List<ForeignKey>();
        Entity selectedEnt;
        Attribute att;
        Data data;
        PrimaryKey pk, Mpk;
        ForeignKey fk, Mfk;

        DataTable dataTFk = new DataTable(); //Manejo DataGridView
        DataTable dataTArbol = new DataTable();

        AuxFile file, ddFile, idxFile;
        static long head = -1, pos = head;
        bool newFile, isPk, isFk, isTree, isSK;
        string fileName, ddFileName, idxFileName, dir;
        int ind, ip = 0, indFk, indArbol, fki = 0, indFound, advance, advanceFK, orden = 2, indR = 0, indPk;
        int indNodoEliminar, indNInterElim;

        Nodo nodo, Raiz;
        List<Nodo> Lnodo = new List<Nodo>();
        int indListNodo = 0;

        public AddData(List<Entity> Entities, AuxFile ddFile, string dir)
        {
            InitializeComponent();

            entities = Form1.entities;

            dataGridView1.Columns.Clear();

            file = new AuxFile();

            this.ddFile = ddFile;
            
            comboBoxAtt.Enabled = false;
            
            this.dir = dir;
            
            this.dir = this.dir + "\\Examples";
            
            comboBoxEnt.DataSource = entities;
            
            foreach (Entity entity in Entities)
            {
                string s = new string(entity.name);
                string str;
                listView1.Items.Add(s);
                int index = s.IndexOf("\0");
                if (index > 0)
                {
                    str = s.Substring(0, index);
                    comboBoxEnt.DisplayMember = "Hola";
                    //comboBoxEnt.ValueMember = str;
                }
                    
            }
            //comboBoxEnt.SelectedIndex = -1;
        }

        public string name
        {
            get
            {
                Entity currentEnt = (Entity)comboBoxEnt.SelectedItem;
                string str = new string(currentEnt.name);
                int index = str.IndexOf("\0");
                if (index > 0)
                    return str.Substring(0, index);
                else
                    return "";
            }
        }

        public string AttName
        {
            get
            {
                string str = comboBoxAtt.GetItemText(comboBoxAtt.SelectedItem);
                int index = str.IndexOf("\0");
                if (index > 0)
                    return str.Substring(0, index);
                else
                    return "";
            }
        }

        private void AddData_Load(object sender, EventArgs e)
        {
            buttonAdd.DialogResult = DialogResult.OK;
            //dir = Directory.GetCurrentDirectory();
            //dir = dir + "\\Examples";
        }

        private void comboBoxEnt_SelectedIndexChanged(object sender, EventArgs e)
        {

            selectedEnt = (Entity)comboBoxEnt.SelectedItem;
            
            if (selectedEnt != null)
            {
                foreach(Attribute attribute in selectedEnt.attributes)
                {
                    advance = (attribute.length + 8) * 20;
                    //advanceFK = (attribute.length);

                    string stAttName;

                    if (attribute.indexType == 2)
                    {
                        stAttName = new string(attribute.name);
                        stAttName = "PK: " + stAttName;
                        comboBoxIndex.Items.Add(stAttName);
                    }
                    else if(attribute.indexType == 3)
                    {
                        stAttName = new string(attribute.name);
                        stAttName = "FK: " + stAttName;
                        comboBoxIndex.Items.Add(stAttName);
                    }
                    else if (attribute.indexType == 4)
                    {
                        stAttName = new string(attribute.name);
                        stAttName = "B+: " + stAttName;
                        comboBoxIndex.Items.Add(stAttName);
                    }
                }

                if (selectedEnt.dataDir == -1)
                {
                    
                    fileName = dir + "\\" + name + ".dat";
                    file.stream = new FileStream(fileName, FileMode.Create);
                    head = -1;
                    dataGridView1.Rows.Clear();
                    entities = new List<Entity>();
                    foreach (Attribute attribute in selectedEnt.attributes)
                    {
                        string str = new string(attribute.name);

                        comboBoxAtt.Items.Add(str);
                        
                        string stAttName;

                        if (attribute.indexType == 2)
                        {
                            stAttName = new string(attribute.name);
                            stAttName = "PK: " + stAttName;
                            comboBoxIndex.Items.Add(stAttName);
                        }
                        else if(attribute.indexType == 3)
                        {
                            stAttName = new string(attribute.name);
                            stAttName = "FK: " + stAttName;
                            comboBoxIndex.Items.Add(stAttName);
                        }
                        else if(attribute.indexType == 4)
                        {
                            stAttName = new string(attribute.name);
                            stAttName = "B+: " + stAttName;
                            comboBoxIndex.Items.Add(stAttName);
                        }

                        if(attribute.indexType != 0 && attribute.indexType != 1 && attribute.indexDir == -1)
                        {
                            BinaryWriter writer = createIndexFile();
                            if (attribute.indexType == 1)
                                isSK = true;
                            copiaListaOrdenadaPk();
                            copiaListaOrdenadaFk();
                            verificaPk(writer);
                            verificaFk(writer);
                            verificaArbol(writer);
                        }

                        if (attribute.indexDir == -1)
                            if (attribute.indexType == 2) //PK
                            {
                               // BinaryWriter writer = createIndexFile();
                                /*ddFile.stream = new FileStream(ddFile.fileName, FileMode.Open, FileAccess.ReadWrite);
                                BinaryWriter writerDat = new BinaryWriter(ddFile.stream, Encoding.UTF8);

                                long[,] arr = new long[2, 20];

                                for (int i = 0; i < 2; i++)
                                    for (int j = 0; j < 20; j++)
                                    {
                                        arr[i, j] = -1;
                                        writer.Write(arr[i, j]);
                                    }

                                attribute.indexDir = 0;
                                ddFile.stream.Position = attribute.attributeDir;
                                attribute.SaveAttribute(writerDat);
                                ddFile.stream.Close();
                                writerDat.Close();
                                idxFile.stream.Close();
                                writer.Close();*/
                            }
                            else if(attribute.indexType == 3) //FK
                            {
                                //BinaryWriter writer = createIndexFile();

                               /* long[,] arr = new long[20, 20];

                                for (int i = 0; i < 20; i++)
                                    for (int j = 0; j < 20; j++)
                                    {
                                        arr[i, j] = -1;
                                        writer.Write(arr[i, j]);
                                    }*/
                            }
                    }


                    file.stream.Close();
                    comboBoxAtt.Enabled = true;
                    comboBoxEnt.Enabled = true;
                }
                else
                {
                    fileName = dir + "\\" + name + ".dat";
                    dataGridView1.Rows.Clear();
                    entities = new List<Entity>();
                    head = -1;
                    file.stream = new FileStream(fileName, FileMode.Open);//Crea el archivo con la ruta especificada
                    BinaryReader reader = new BinaryReader(file.stream, Encoding.UTF8);//Lee basado en la secuencia y codificacion de caracteres

                    if (file.stream.Length != 0)//Mientras no sea fin de archivo
                    {
                        file.stream.Position = 0;
                        head = reader.ReadInt64();//Lee la cabecera
                        if (head != -1)//Si hay entidades
                        {
                            pos = head;//Declarar un auxiliar
                            while (pos != -1)//Mientras exista minimo una entidad
                            {
                                file.stream.Position = pos;//Asignar la cabecera a la posicion del archivo

                                if (selectedEnt.dataDir != -1)//Si tiene Registros
                                {
                                    file.stream.Position = selectedEnt.dataDir;
                                    Data dat = new Data();//Registro
                                    dat.dataDir = reader.ReadInt64();//Lee la direccion del registro actual
                                    foreach (Attribute atrib in selectedEnt.attributes)
                                    {
                                        if (atrib.type == 'I')
                                            dat.number.Add(reader.ReadInt32());//Añade a la lista de enteros de los registros
                                        else
                                            dat.str.Add(reader.ReadChars(atrib.length));//Añade a la lista de cadenas de los registros

                                        if (atrib.indexType == 1)
                                            isSK = true;

                                        if(atrib.indexDir == -1 && atrib.indexType != 0 && atrib.indexType != 1)
                                        {
                                            BinaryWriter writer = createIndexFile();
                                            //openIndexFile();
                                            if (atrib.indexType == 1)
                                                isSK = true;

                                            copiaListaOrdenadaPk();
                                            copiaListaOrdenadaFk();
                                            verificaPk(writer);
                                            verificaFk(writer);
                                            verificaArbol(writer);
                                        }

                                        if (atrib.indexType != 0 && atrib.indexType != 1)
                                        {
                                            
                                        }
                                        

                                        if (atrib.indexType != -1 && atrib.indexType != 0 && atrib.indexType != 1)
                                        {
                                            openIndexFile();
                                            BinaryWriter write = new BinaryWriter(idxFile.stream, Encoding.UTF8);
                                            BinaryReader R = new BinaryReader(idxFile.stream, Encoding.UTF8);//readIndexFile();//new BinaryReader(idxFile.stream, Encoding.UTF8);    
                                            R.BaseStream.Seek(atrib.indexDir, SeekOrigin.Begin);


                                            if (atrib.indexType == 2)
                                            {
                                                string auxPk;
                                                char[] NomPk;
                                                for (int p = 0; p < 20; p++)
                                                {
                                                    Mpk = new PrimaryKey();
                                                    if (atrib.type == 'C')
                                                    {
                                                        auxPk = string.Empty;
                                                        NomPk = R.ReadChars(atrib.length);
                                                        for (int i = 0; i < NomPk.Count(); i++)
                                                        {
                                                            auxPk += NomPk[i];
                                                        }
                                                        Mpk.oClave = auxPk;
                                                        Mpk.lDireccion = R.ReadInt64();
                                                        selectedEnt.pk.Add(Mpk);
                                                    }
                                                    else
                                                    {
                                                        auxPk = string.Empty;
                                                        Mpk.oClave = R.ReadInt32();
                                                        Mpk.lDireccion = R.ReadInt64();
                                                        selectedEnt.pk.Add(Mpk);
                                                    }
                                                }
                                                
                                            }
                                            if (atrib.indexType == 3)
                                            {
                                                isFk = true;
                                                string auxFk;
                                                char[] NomFk;
                                                for (int f = 0; f < 20; f++)
                                                {
                                                    Mfk = new ForeignKey();
                                                    if (atrib.type == 'C')
                                                    {
                                                        auxFk = string.Empty;
                                                        NomFk = R.ReadChars(atrib.length);
                                                        for (int i = 0; i < NomFk.Count(); i++)
                                                        {
                                                            auxFk += NomFk[i];
                                                        }
                                                        Mfk.oClave = auxFk;
                                                        for (int d = 0; d < 20; d++)
                                                        {
                                                            Mfk.lDirecciones.Add(R.ReadInt64());
                                                        }
                                                        selectedEnt.fk.Add(Mfk);

                                                    }
                                                    else
                                                    {
                                                        Mfk.oClave = R.ReadInt32();
                                                        for (int d = 0; d < 20; d++)
                                                        {
                                                            Mfk.lDirecciones.Add(R.ReadInt64());
                                                        }
                                                        selectedEnt.fk.Add(Mfk);
                                                    }
                                                }
                                            }
                                            if (atrib.indexType == 4)
                                            { 
                                                Nodo n;
                                                Nodo nuevo = idxFile.readNode(atrib.indexDir, R);
                                                selectedEnt.nodos.Add(nuevo);
                                                if (nuevo.cTipo == 'R')
                                                {
                                                    for (int i = 0; i < nuevo.iDatos.Count + 1; i++)
                                                    {
                                                        n = idxFile.readNode(nuevo.lDirecciones[i], R);
                                                        selectedEnt.nodos.Add(n);
                                                        if (n.cTipo == 'I')
                                                        {
                                                            for (int j = 0; j < n.lDirecciones.Count; j++)
                                                            {
                                                                Nodo nh = idxFile.readNode(n.lDirecciones[j], R);
                                                                selectedEnt.nodos.Add(nh);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            copiaListaOrdenadaPk();
                                            copiaListaOrdenadaFk();
                                            verificaPk(write);
                                            if(isFk)
                                            verificaFk(write);
                                            verificaArbol(write);
                                            idxFile.stream.Close();
                                        }
                                        
                                    }
                                    dat.nextDir = reader.ReadInt64();//Lee la direccion del registro siguiente

                                    selectedEnt.data.Add(dat);
                                    while (dat.nextDir != -1)//Mientras ternga registros
                                    {
                                        Data aux = new Data();

                                        file.stream.Position = dat.nextDir;

                                        aux.dataDir = reader.ReadInt64();//Lee la direccion del registro actual
                                        foreach (Attribute att in selectedEnt.attributes)
                                        {
                                            if (att.type == 'I')
                                                aux.number.Add(reader.ReadInt32());//Añade a la lista de enteros de los registros
                                            else
                                                aux.str.Add(reader.ReadChars(att.length));//Añade a la lista de cadenas de los registros

                                        }
                                        aux.nextDir = reader.ReadInt64();//Lee la direccion del registro siguiente
                                        dat = aux;
                                        selectedEnt.data.Add(dat);
                                    }

                                }
                                entities.Add(selectedEnt); //Agrega la entidad a memoria
                                pos = selectedEnt.nextDir;//Asigna a pos la dirreccion de la siguiente entidad
                            }
                        }
                        else
                            MessageBox.Show("Empty File");
                    }
                    else
                        MessageBox.Show("Empty File");

                    foreach (Attribute attribute in selectedEnt.attributes)
                    {
                        string str = new string(attribute.name);
                        comboBoxAtt.Items.Add(str);

                        

                    }

                    showData(selectedEnt.data);//Muestra los datos de la entidad en el data grid
                    reader.Close();//Cierra la lectura actual
                    file.stream.Close();//Cierra la secuencia actual*/
                    comboBoxAtt.Enabled = true;
                    statusStrip1.Text = "File " + fileName + " opened.";
                }

                /*dataGridView1.Columns.Clear();
                
                dataGridView1.ColumnCount = 0;
                //dataGridView1.RowCount = 0;
                //dataGridView1.Rows.Clear();
                dataGridView1.ColumnCount = selectedEnt.attributes.Count + 2;
                dataGridView1.Columns[0].Name = "DataDir";

                int i = 1;
                comboBoxAtt.Items.Clear();

                foreach (Attribute attribute in selectedEnt.attributes)
                {
                    string str = new string(attribute.name);

                    
                    comboBoxAtt.Items.Add(str);

                    dataGridView1.Columns[i].Name = str;
                    i++;
                }

                dataGridView1.Columns[dataGridView1.ColumnCount - 1].Name = "NextDir";

                comboBoxAtt.Enabled = true;
                buttonOpen.Enabled = true;

                //file.stream = new FileStream("C:\\Users\\Hernan\\source\\repos\\Archivos\\Diccionario_Datos\\Diccionario_Datos\\Examples\\"
                //           + selectedEnt.name + ".dat", FileMode.Open);

                //if (file.stream != null)
                //  MostrarRegistro(selectedEnt.data);*/
            }
        }

        public char[] StringToChar(String Cad, int longitud)
        {
            char[] aux = new char[longitud];
            for (int i = 0; i < Cad.Count(); i++)
            {
                aux[i] = Cad[i];
            }
            return aux;
        }

        #region PK

        private void ordenaPk()
        {
            for (ip = 0; ip < 20; ip++)
            {
                if (ip < sordedPKlist.Count)
                {
                    selectedEnt.pk[ip].oClave = sordedPKlist[ip].oClave;
                    selectedEnt.pk[ip].lDireccion = sordedPKlist[ip].lDireccion;
                }
                else
                {
                    selectedEnt.pk[ip].oClave = "0";
                    selectedEnt.pk[ip].lDireccion = -1;
                }
            }
        }
        //VERIFICA SI LA ENTIDAD TIENE CLAVE PRIMARIA Y CREA LOS BLOQUES
        private void verificaPk(BinaryWriter w)
        {
            long pos = w.BaseStream.Length;
            long posdd = selectedEnt.attributeDir;
            if (selectedEnt.pk.Count == 0)//Checa si el todavia no se crea un bloque de clave primaria
            {
                for (int i = 0; i < selectedEnt.attributes.Count; i++)//recorrido de los atributos de la entidad seleccionada en busqueda de la cleve primaria
                {
                    if (selectedEnt.attributes[i].indexType == 2)
                    {
                        //dataTPk.Columns.Clear();
                        //dataTPk.Columns.Add("clave");
                        //dataTPk.Columns.Add("Dir.Clave");
                        isPk = true;
                        selectedEnt.attributes[i].indexDir = pos;
                        att = selectedEnt.attributes[i];
                        posdd = selectedEnt.attributes[i].attributeDir;
                        ddFile.modifyAttribute(posdd, selectedEnt.attributes[i]);
                        w.BaseStream.Seek(pos, SeekOrigin.Begin);
                        ind = i;
                        for (int c = 0; c < 20; c++)
                        {
                            pk = new PrimaryKey();
                            selectedEnt.pk.Add(pk);
                            if (selectedEnt.attributes[i].type == 'C')
                            {
                                w.Write(StringToChar(pk.oClave.ToString(), selectedEnt.attributes[i].length));
                                w.Write(pk.lDireccion);
                            }
                            else
                            {
                                w.Write(Convert.ToInt32(pk.oClave));
                                w.Write(pk.lDireccion);
                            }
                        }
                    }
                }



            }
            else//En Caso de que ya exista solo agrega los encabezados del datagridview de la clave primaria
            {
                //dataTPk.Columns.Clear();
               // dataTPk.Columns.Add("clave");
                //dataTPk.Columns.Add("Dir.Clave");
            }
        }
        //MÉTODO PARA LLENAR EL DATAGIRDVIEW DE LA CLAVE PRIMARIA
        private void llenaDataPk()
        {
            //dataTPk.Clear();
            foreach (PrimaryKey pk in selectedEnt.pk)
            {
               // dataTPk.Rows.Add(new object[] { pk.oClave, pk.lDireccion });
            }
        }
        //DESPUES DE ABRIR EL ARCHIVO COPIA LA LISTA DE LA CLAVE PRIMARIA
        private void copiaListaOrdenadaPk()
        {
            if (selectedEnt.pk.Count > 0)
            {
                isPk = true;
                for (ip = 0; ip < 20; ip++)
                {
                    if (selectedEnt.pk[ip].lDireccion != -1)
                    {
                        pk = new PrimaryKey();
                        pk.oClave = selectedEnt.pk[ip].oClave;
                        pk.lDireccion = selectedEnt.pk[ip].lDireccion;
                        sordedPKlist.Add(pk);
                    }
                }
            }
            else
            {
                sordedPKlist = new List<PrimaryKey>();
            }
        }
        #endregion

        #region FK

        /*
         * MÉTODOS PARA CLAVE SECUNDARIA             
        */
        //vERIFICA SI LA ENTIDAD CUENTA CON UNA CLAVE SECUNDARIA Y CREA LOS BLOQUES
        private void verificaFk(BinaryWriter w)
        {
            long pos;
            //w = new BinaryWriter(idxFile.stream, Encoding.UTF8);
            if(!isPk)
                pos = w.BaseStream.Length;
            else
                pos = advance;

            long posdd = selectedEnt.attributeDir;
            if (selectedEnt.fk.Count == 0)//Checa si el todavia no se crea un bloque de clave primaria
            {
                for (int i = 0; i < selectedEnt.attributes.Count; i++)//recorrido de los atributos de la entidad seleccionada en busqueda de la cleve primaria
                {
                    if (selectedEnt.attributes[i].indexType == 3)
                    {
                        isFk = true;
                        indFk = i;
                        selectedEnt.attributes[i].indexDir = pos;
                        att = selectedEnt.attributes[i];
                        posdd = selectedEnt.attributes[i].attributeDir;
                        ddFile.modifyAttribute(posdd, selectedEnt.attributes[i]);
                        w.BaseStream.Seek(pos, SeekOrigin.Begin);
                        for (int c = 0; c < 20; c++)
                        {
                            fk = new ForeignKey();

                            for (int j = 0; j < 20; j++)
                            {
                                fk.lDirecciones.Add(-1);
                            }
                            selectedEnt.fk.Add(fk);
                            if (selectedEnt.attributes[i].type == 'C')
                            {

                                w.Write(StringToChar(fk.oClave.ToString(), selectedEnt.attributes[i].length));
                                foreach (long l in fk.lDirecciones)
                                {
                                    w.Write(l);
                                }
                            }
                            else
                            {
                                w.Write(Convert.ToInt32(fk.oClave));
                                foreach (long l in fk.lDirecciones)
                                {
                                    w.Write(l);
                                }
                            }
                        }
                        dataTFk.Columns.Add("Data");
                        for (int c = 0; c < 20; c++)
                        {
                            dataTFk.Columns.Add("DataDir" + (c + 1).ToString());
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < selectedEnt.attributes.Count; i++)
                {
                    if (selectedEnt.attributes[i].indexType == 3)
                    {
                        isFk = true;
                        indFk = i;
                    }
                }
                dataTFk.Columns.Clear();
                dataTFk.Columns.Add("Data");
                for (int c = 0; c < 20; c++)
                {
                    dataTFk.Columns.Add("DataDir" + (c + 1).ToString());
                }
            }
        }
        //MÉTODO DE ORDENACIÓN DE CLAVE SECUNDARIA
        private void ordenaFk()
        {
            for (int ifk = 0; ifk < 20; ifk++)
            {
                if (ifk < sortedFKlist.Count)
                {
                    selectedEnt.fk[ifk].oClave = sortedFKlist[ifk].oClave;
                    for (int j = 0; j < 20; j++)
                    {
                        if (j < sortedFKlist[ifk].lDirecciones.Count)
                            selectedEnt.fk[ifk].lDirecciones[j] = sortedFKlist[ifk].lDirecciones[j];
                        else
                            selectedEnt.fk[ifk].lDirecciones[j] = -1;
                    }
                }
                else
                {
                    if (selectedEnt.attributes[ind].type == 'C')
                        selectedEnt.fk[ifk].oClave = "0";
                    else selectedEnt.fk[ifk].oClave = 0;
                    for (int j = 0; j < 20; j++)
                    {
                        selectedEnt.fk[ifk].lDirecciones[j] = -1;
                    }

                }
            }
        }
        //MÉTODO QUE BUSCA SI YA EXISTE LA CLAVE E INSERTA LA DIRECCIÓN EN SU LISTA.
        private bool busca(int type)
        {
            bool band = false;
            string compare;
            for (int ifk = 0; ifk < 20; ifk++)
            {
                if (selectedEnt.fk[ifk].lDirecciones[0] != -1)
                {
                    if(data.str.Count !=0)
                    {
                        if (isPk && !isSK &&type == 'C')
                        {
                            compare = new string(data.str[indFk - 1]);
                            if (compare == selectedEnt.fk[ifk].oClave.ToString())
                            {
                                indFound = ifk;
                                sortedFKlist[ifk].lDirecciones.Add(data.dataDir);
                                band = true;

                            }
                        }
                        else if (!isPk && type == 'C' )
                        {
                            compare = new string(data.str[indFk]);
                            if (compare == selectedEnt.fk[ifk].oClave.ToString())
                            {
                                indFound = ifk;
                                sortedFKlist[ifk].lDirecciones.Add(data.dataDir);
                                band = true;

                            }
                        }
                        else if(isPk && isSK && type == 'C')
                        {
                            compare = new string(data.str[indFk - 2]);
                            if (compare == selectedEnt.fk[ifk].oClave.ToString())
                            {
                                indFound = ifk;
                                sortedFKlist[ifk].lDirecciones.Add(data.dataDir);
                                band = true;

                            }
                        }
                        

                    }
                    

                    if(data.number.Count != 0)
                    {
                        if(type == 'I')
                            if (data.number[indFk].ToString() == selectedEnt.fk[ifk].oClave.ToString())
                            {
                                indFound = ifk;
                                sortedFKlist[ifk].lDirecciones.Add(data.dataDir);
                                band = true;
                            }
                        //else if(!isPk && type == 'I')
                    }
                    
                }
            }
            return band;

        }
        //MÉTODO PARA LLENAR EL DATAGIRDVIEW DE LA CLAVE SECUNDARIA
        private void llenaDataFk()
        {
            /*DataRow r;
            dataTFk.Clear();
            for (int i = 0; i < selectedEnt.fk.Count; i++)
            {
                r = dataTFk.NewRow();
                int j = 0;
                r[j] = selectedEnt.fk[i].oClave;
                j = 1;
                for (int c = 0; c < selectedEnt.fk[i].lDirecciones.Count; c++)
                {
                    r[j] = selectedEnt.fk[i].lDirecciones[c];
                    j++;
                }
                dataTFk.Rows.Add(r);
            }*/
        }
        //DESPUES DE ABRIR EL ARCHIVO COPIA LA LISTA DE LA CLAVE SECUNDARIA
        private void copiaListaOrdenadaFk()
        {
            if (selectedEnt.fk.Count > 0)
            {
                isFk = true;
                for (fki = 0; fki < 20; fki++)
                {
                    if (selectedEnt.fk[fki].lDirecciones[0] != -1)
                    {
                        fk = new ForeignKey();
                        fk.oClave = selectedEnt.fk[fki].oClave;
                        for (int d = 0; d < 20; d++)
                        {
                            if (selectedEnt.fk[fki].lDirecciones[d] != -1)
                            {
                                fk.lDirecciones.Add(selectedEnt.fk[fki].lDirecciones[d]);
                            }
                        }
                        sortedFKlist.Add(fk);
                    }
                }
            }
        }

        #endregion

        #region B+
        /*
        * MÉTODOS PARA ARBOL B+            
        */
        //MÉTODO PARALLENAR EL ENCABEZADO DEL DATAGRIDVIEW DEL ARBOL B+
        private void DataTableArbol()
        {
            dataTArbol.Columns.Clear();
            dataTArbol.Columns.Add("NodeDir");
            dataTArbol.Columns.Add("Type");
            dataTArbol.Columns.Add("P1");
            for (int i = 0; i < orden * 2; i++)
            {
                dataTArbol.Columns.Add("Data" + (i + 1).ToString());
                dataTArbol.Columns.Add("P" + (i + 2).ToString());
            }
        }
        //MÉTODO DE VERIFICACIÓN DE ARBOL B+
        private void verificaArbol(BinaryWriter w)
        {
            long pos = w.BaseStream.Length;
            if (selectedEnt.nodos.Count == 0)
            {
                for (int i = 0; i < selectedEnt.attributes.Count; i++)
                {
                    if (selectedEnt.attributes[i].indexType == 4)
                    {
                        selectedEnt.attributes[i].indexDir = pos;
                        isTree = true;
                        DataTableArbol();
                        indArbol = i;
                        nodo = new Nodo();
                        nodo.lDireccionN = pos;
                        selectedEnt.nodos.Add(nodo);
                        idxFile.writeNode(nodo, w);
                        pos = selectedEnt.attributes[i].attributeDir;
                        
                        ddFile.modifyAttribute(pos, selectedEnt.attributes[i]);
                    }
                }
            }
            else
            {
                DataTableArbol();
                isTree = true;
            }
        }
        //MÉTODO PARA INSERTAR UNA CLAVE AL ARBOL
        private void InsertaClave(BinaryWriter w, int clave, long dir)
        {
            bool repetido = false;
            foreach (Nodo r in selectedEnt.nodos)//Checa si existe la raiz
            {
                if (r.cTipo == 'R')
                    Raiz = r;
            }

            if (Raiz == null)
            {
                if (nodo.iDatos.Contains(clave))
                    repetido = true;

                if (!repetido)
                {
                    if (nodo.iDatos.Count < 4)
                    {
                        nodo.lDirecciones.Add(dir);
                        nodo.iDatos.Add(clave);
                        if (nodo.iDatos.Count > 1)
                            ordenaNodo(nodo);
                        idxFile.writeNode(nodo, w);
                        selectedEnt.nodos[0] = nodo;
                    }
                    else//Se crea la Raiz
                    {
                        Nodo Aux = nodo;
                        nodo = new Nodo();
                        nodo.lDireccionN = w.BaseStream.Length;
                        Aux.lDirecciones.Add(dir);
                        Aux.iDatos.Add(clave);
                        ordenaNodo(Aux);
                        DivideNodoyEliminaClaves(Aux, nodo);
                        ordenaNodo(nodo);
                        selectedEnt.nodos[0] = Aux;
                        selectedEnt.nodos.Add(nodo);
                        idxFile.writeNode(Aux, w);
                        idxFile.writeNode(nodo, w);
                        Raiz = new Nodo();
                        Raiz.cTipo = 'R';
                        Raiz.lDireccionN = w.BaseStream.Length;
                        Raiz.lDirecciones.Add(Aux.lDireccionN);
                        Raiz.lDirecciones.Add(nodo.lDireccionN);
                        Raiz.iDatos.Add(nodo.iDatos[0]);
                        selectedEnt.nodos.Add(Raiz);
                        idxFile.writeNode(Raiz, w);
                        indR = selectedEnt.nodos.Count - 1;
                    }
                }
                if (Raiz != null)
                {
                    selectedEnt.attributes[indArbol].indexDir = Raiz.lDireccionN;
                    ddFile.modifyAttribute(selectedEnt.attributes[indArbol].attributeDir, selectedEnt.attributes[indArbol]);
                }

            }
            else//Se crean los intermedios
            {
                Nodo rNodo = new Nodo();
                Nodo iNodo = new Nodo();
                Nodo nAnterior;
                rNodo = ChecaLugarDeinsersion(Raiz, clave);
                nAnterior = Raiz;
                if (rNodo.cTipo == 'I')//checa en caso de que existan nodos intermedios
                {
                    nAnterior = rNodo;
                    iNodo = ChecaLugarDeinsersion(rNodo, clave);
                    rNodo = iNodo;
                }

                if (rNodo.iDatos.Count < 4)
                {
                    rNodo.lDirecciones.Add(dir);
                    rNodo.iDatos.Add(clave);
                    if (rNodo.iDatos.Count > 1)
                        ordenaNodo(rNodo);
                    idxFile.writeNode(rNodo, w);
                    selectedEnt.nodos[indListNodo] = rNodo;
                }
                else
                {
                    if (nAnterior == Raiz)
                    {
                        Nodo Aux = rNodo;
                        nodo = new Nodo();
                        nodo.lDireccionN = w.BaseStream.Length;
                        Aux.lDirecciones.Add(dir);
                        Aux.iDatos.Add(clave);
                        ordenaNodo(Aux);
                        DivideNodoyEliminaClaves(Aux, nodo);
                        ordenaNodo(nodo);
                        selectedEnt.nodos[indListNodo] = Aux;
                        selectedEnt.nodos.Add(nodo);
                        idxFile.writeNode(Aux, w);
                        idxFile.writeNode(nodo, w);
                        if (Raiz.iDatos.Count < 4)
                        {
                            Raiz.lDirecciones.Add(nodo.lDireccionN);
                            Raiz.iDatos.Add(nodo.iDatos[0]);
                            ordenaNodo(Raiz);
                            selectedEnt.nodos[indR] = Raiz;
                        }
                        else
                        {
                            Nodo nint = new Nodo();
                            nint.cTipo = 'I';
                            nint.lDireccionN = w.BaseStream.Length;
                            Nodo nAux = Raiz;
                            nAux.iDatos.Add(nodo.iDatos[0]);
                            ordenaNodo(nAux);
                            DivideNodoyEliminaClaves(nAux, nint);
                            ordenaNodo(nint);
                            nAux.cTipo = 'I';
                            for (int i = 0; i < selectedEnt.nodos.Count; i++)
                            {
                                if (selectedEnt.nodos[i].lDireccionN == Raiz.lDireccionN)
                                    selectedEnt.nodos[i] = nAux;
                            }
                            idxFile.writeNode(nAux, w);
                            Raiz = new Nodo();
                            Raiz.cTipo = 'R';
                            Raiz.lDirecciones.Add(nAux.lDireccionN);
                            Raiz.lDirecciones.Add(nint.lDireccionN);
                            Raiz.iDatos.Add(nint.iDatos[0]);
                            indR = selectedEnt.nodos.Count - 1;
                            nint.iDatos.RemoveAt(0);
                            nint.lDirecciones.RemoveAt(0);
                            nint.lDirecciones.Add(nodo.lDireccionN);
                            idxFile.writeNode(nint, w);
                            Raiz.lDireccionN = w.BaseStream.Length;
                            selectedEnt.nodos.Add(nint);
                            idxFile.writeNode(Raiz, w);
                            selectedEnt.nodos.Add(Raiz);
                        }
                    }
                    else
                    {
                        Nodo Aux = rNodo;
                        nodo = new Nodo();
                        nodo.lDireccionN = w.BaseStream.Length;
                        Aux.lDirecciones.Add(dir);
                        Aux.iDatos.Add(clave);
                        ordenaNodo(Aux);
                        DivideNodoyEliminaClaves(Aux, nodo);
                        ordenaNodo(nodo);
                        selectedEnt.nodos[indListNodo] = Aux;
                        selectedEnt.nodos.Add(nodo);
                        idxFile.writeNode(Aux, w);
                        idxFile.writeNode(nodo, w);
                        if (nAnterior.iDatos.Count < 4)
                        {
                            Nodo Intermedio = nAnterior;
                            Intermedio.iDatos.Add(nodo.iDatos[0]);
                            Intermedio.lDirecciones.Add(nodo.lDireccionN);
                            ordenaNodo(Intermedio);
                            for (int i = 0; i < selectedEnt.nodos.Count; i++)
                            {
                                if (selectedEnt.nodos[i].lDireccionN == Intermedio.lDireccionN)
                                    selectedEnt.nodos[i] = Intermedio;
                            }
                            idxFile.writeNode(Intermedio, w);
                            Intermedio = nAnterior;
                        }
                        else
                        {
                            Nodo Nuevoint = new Nodo();
                            Nuevoint.lDireccionN = w.BaseStream.Length;
                            Nuevoint.cTipo = 'I';

                            nAnterior.iDatos.Add(nodo.iDatos[0]);
                            nAnterior.lDirecciones.Add(nodo.lDireccionN);
                            ordenaNodo(nAnterior);
                            Raiz.iDatos.Add(nAnterior.iDatos[2]);
                            DivideNodoyEliminaClaves(nAnterior, Nuevoint);
                            ordenaNodo(Nuevoint);
                            idxFile.writeNode(Nuevoint, w);
                            idxFile.writeNode(Raiz, w);
                            idxFile.writeNode(nAnterior, w);
                            for (int i = 0; i < selectedEnt.nodos.Count; i++)
                            {
                                if (selectedEnt.nodos[i].lDireccionN == nAnterior.lDireccionN)
                                    selectedEnt.nodos[i] = nAnterior;
                                if (selectedEnt.nodos[i].lDireccionN == Raiz.lDireccionN)
                                    selectedEnt.nodos[i] = Raiz;

                            }
                        }
                    }
                }
            }
            if (Raiz != null)
            {
                selectedEnt.attributes[indArbol].indexDir = Raiz.lDireccionN;
                ddFile.modifyAttribute(selectedEnt.attributes[indArbol].attributeDir, selectedEnt.attributes[indArbol]);
            }
        }
        //MÉTODO QUE DIVIDE UN NODO EN CASO DE QUE SE QUIERA INSERTAR UNA CLAVE Y ESTE YA ESTE LLENO
        private void DivideNodoyEliminaClaves(Nodo Aux, Nodo nd)
        {
            int nEliminado = 0;
            int contE = 3;
            for (int i = 2; i < Aux.iDatos.Count; i++)
            {
                nd.iDatos.Add(Aux.iDatos[i]);
                nd.lDirecciones.Add(Aux.lDirecciones[i]);
                nEliminado = i + 1;
            }

            if (nd.cTipo == 'I')
            {
                contE = 2;
                nEliminado--;
                Aux.iDatos.RemoveAt(nEliminado);
            }

            for (int nE = 0; nE < contE; nE++)
            {
                nEliminado--;
                Aux.iDatos.RemoveAt(nEliminado);
                if (nd.cTipo == 'I') nEliminado++;
                Aux.lDirecciones.RemoveAt(nEliminado);
                if (nd.cTipo == 'I') nEliminado--;
            }
        }
        //CHECA EN QUE NODO VA A SER INSERTADO EL NUEVO DATO
        private Nodo ChecaLugarDeinsersion(Nodo nd, int clave)
        {
            Nodo aux = new Nodo();
            for (int r = 0; r < nd.iDatos.Count; r++)
            {
                if (clave > nd.iDatos[r])
                {
                    for (int ni = 0; ni < selectedEnt.nodos.Count; ni++)
                    {
                        if (selectedEnt.nodos[ni].lDireccionN == nd.lDirecciones[r + 1])
                        {
                            aux = selectedEnt.nodos[ni];
                            indListNodo = ni;
                        }
                    }
                }
                else
                {
                    for (int ni = 0; ni < selectedEnt.nodos.Count; ni++)
                    {
                        if (selectedEnt.nodos[ni].lDireccionN == nd.lDirecciones[r])
                        {
                            aux = selectedEnt.nodos[ni];
                            indListNodo = ni;

                        }
                    }
                    r = nd.iDatos.Count;
                }
            }
            return aux;
        }
        //MÉTODO DE ORDENACIÓN DE DATOS EN UN NODO
        private void ordenaNodo(Nodo n)
        {

            for (int oa = 0; oa < n.iDatos.Count; oa++)
                for (int o = 0; o < n.iDatos.Count - 1; o++)
                {
                    if (n.iDatos[o] > n.iDatos[o + 1])
                    {
                        if (n.cTipo == 'H')
                        {
                            int menor = n.iDatos[o + 1];
                            long apMenor = n.lDirecciones[o + 1];
                            n.iDatos[o + 1] = n.iDatos[o];
                            n.lDirecciones[o + 1] = n.lDirecciones[o];
                            n.iDatos[o] = menor;
                            n.lDirecciones[o] = apMenor;
                        }
                        else
                        {
                            int menor = n.iDatos[o + 1];
                            long apMenor = n.lDirecciones[o + 2];
                            n.iDatos[o + 1] = n.iDatos[o];
                            n.lDirecciones[o + 2] = n.lDirecciones[o + 1];
                            n.iDatos[o] = menor;
                            n.lDirecciones[o + 1] = apMenor;
                        }
                    }
                }
        }
        //METODO PARA LLENAR EL DATAGRID DEL ARBOL B+
        private void llenaDataGridArbol()
        {

            DataRow r;
            dataTArbol.Clear();
            for (int i = 0; i < selectedEnt.nodos.Count; i++)
            {
                r = dataTArbol.NewRow();
                int j = 0, d = 0, a = 0;
                r[j] = selectedEnt.nodos[i].lDireccionN;
                j = 1;
                r[j] = selectedEnt.nodos[i].cTipo;
                j = 2;
                while (d < selectedEnt.nodos[i].iDatos.Count || a < selectedEnt.nodos[i].lDirecciones.Count)
                {
                    if (j % 2 == 0)
                    {
                        r[j] = selectedEnt.nodos[i].lDirecciones[a];
                        a++;
                    }
                    else
                    {
                        r[j] = selectedEnt.nodos[i].iDatos[d];
                        d++;
                    }
                    j++;
                }
                dataTArbol.Rows.Add(r);
            }

        }
        //MÉTODO QUE BUSCA QUE EL NODO HOJA EN QUE SE ENCUENTRA LA CLAVE QUE SE DESEA ELIMINAR
        private Nodo buscaNodoHoja(long Direccion)
        {

            Nodo nh = new Nodo();
            for (int Rr = 0; Rr < selectedEnt.nodos.Count; Rr++)//Recorrido de la raiz en busca del dato en las hojas
                if (selectedEnt.nodos[Rr].lDireccionN == Direccion)
                    nh = selectedEnt.nodos[Rr];
            return nh;
        }
        //MÉTODO PARA ELIMINAR UNA CLAVE DEL ARBOL
        private void EliminarClave(BinaryWriter w, int clave, long dir)
        {

            Nodo nE = new Nodo();
            Nodo nDer = null;
            Nodo nIzq = null;
            Nodo nDeri = null;
            Nodo nIzqi = null;
            Nodo nAnterior = new Nodo();
            foreach (Nodo n in selectedEnt.nodos)
            {
                if (n.cTipo == 'R')
                    Raiz = n;
            }
            if (Raiz == null)
            {
                nE = selectedEnt.nodos[0];
                for (int e = 0; e < nE.iDatos.Count; e++)
                {
                    if (nE.iDatos[e] == clave)
                    {
                        nE.iDatos.RemoveAt(e);
                        nE.lDirecciones.RemoveAt(e);
                        idxFile.writeNode(nE, w);
                        selectedEnt.nodos[0] = nE;
                        if (nE.iDatos.Count == 0)
                        {
                            selectedEnt.attributes[indArbol].indexDir = -1;
                            ddFile.modifyAttribute(selectedEnt.attributes[indArbol].attributeDir, selectedEnt.attributes[indArbol]);
                        }
                    }
                }
            }
            else
            {
                RecorreNodos(ref nE, ref nDer, ref nIzq, clave, Raiz);
                if (nE.cTipo != 'I')
                    nAnterior = Raiz;
            }
            if (nE.cTipo == 'I')
            {
                nAnterior = nE;
                nDeri = nDer;
                nIzqi = nIzq;
                indNInterElim = indNodoEliminar;
                RecorreNodos(ref nE, ref nDer, ref nIzq, clave, nE);
            }


            //ELIMNINACIÓN DE CLAVES
            if (Raiz != null)
            {
                for (int ln = 0; ln < nE.iDatos.Count; ln++)
                    if (nE.iDatos[ln] == clave)
                    {
                        nE.iDatos.RemoveAt(ln);
                        nE.lDirecciones.RemoveAt(ln);
                        idxFile.writeNode(nE, w);
                        for (int c = 0; c < selectedEnt.nodos.Count; c++)
                            if (selectedEnt.nodos[c].lDireccionN == nE.lDireccionN)
                                selectedEnt.nodos[c] = nE;
                    }
                if (nE.iDatos.Count < 2)//si quedó insuficiente 
                {

                    if (nIzq != null)//si tiene izquiero
                    {
                        if (nIzq.iDatos.Count > 2)
                            DonaclaveIzq(ref nE, ref nIzq, ref nAnterior, ref nIzqi, ref nDeri, indNodoEliminar, indNInterElim, w);
                        else
                            UneNdConNdIzq(ref nE, ref nIzq, ref nDer, ref nAnterior, ref nIzqi, ref nDeri, indNodoEliminar, indNInterElim, w);
                    }
                    else//Derecho
                    {
                        if (nDer.iDatos.Count > 2)
                            DonaclaveDer(ref nE, ref nDer, ref nAnterior, ref nIzqi, ref nDeri, indNodoEliminar, indNInterElim, w);
                        else//fusion con derecho
                            UneNdConNdDer(ref nE, ref nDer, ref nIzq, ref nAnterior, ref nIzqi, ref nDeri, indNodoEliminar, indNInterElim, w);
                    }
                }

                if (Raiz.iDatos.Count == 0)
                {
                    long posi = Raiz.lDirecciones[0];
                    for (int ln = 0; ln < selectedEnt.nodos.Count; ln++)
                        if (selectedEnt.nodos[ln].lDireccionN == Raiz.lDireccionN)
                            selectedEnt.nodos.RemoveAt(ln);
                    Raiz = null;
                }
            }
        }
        //MÉTODO PARA DONAR UN DATO SI EL NODO SE QUEDA  SIN CLAVES SUFICIENTES
        private void DonaclaveIzq(ref Nodo nE, ref Nodo nIzq, ref Nodo nAnterior, ref Nodo nIzqi, ref Nodo nDeri, int indNelim, int indNelimInter, BinaryWriter w)
        {
            nE.iDatos.Add(nIzq.iDatos[nIzq.iDatos.Count - 1]);
            nE.lDirecciones.Add(nIzq.lDirecciones[nIzq.iDatos.Count - 1]);
            ordenaNodo(nE);
            nIzq.iDatos.RemoveAt(nIzq.iDatos.Count - 1);
            nIzq.lDirecciones.RemoveAt(nIzq.iDatos.Count - 1);
            idxFile.writeNode(nIzq, w);
            nAnterior.iDatos[indNelim] = nE.iDatos[0];
            for (int c = 0; c < selectedEnt.nodos.Count; c++)
                if (selectedEnt.nodos[c].lDireccionN == nAnterior.lDireccionN)
                    selectedEnt.nodos[c] = nAnterior;
            idxFile.writeNode(nAnterior, w);
            idxFile.writeNode(nE, w);
            for (int c = 0; c < selectedEnt.nodos.Count; c++)
                if (selectedEnt.nodos[c].lDireccionN == nE.lDireccionN)
                    selectedEnt.nodos[c] = nE;
        }
        //MÉTODO PARA DONAR UN DATO SI EL NODO SE QUEDA  SIN CLAVES SUFICIENTES
        private void DonaclaveDer(ref Nodo nE, ref Nodo nDer, ref Nodo nAnterior, ref Nodo nIzqi, ref Nodo nDeri, int indNelim, int indNelimInter, BinaryWriter w)
        {
            nE.iDatos.Add(nDer.iDatos[0]);
            nE.lDirecciones.Add(nDer.lDirecciones[0]);
            nDer.iDatos.RemoveAt(0);
            nDer.lDirecciones.RemoveAt(0);
            idxFile.writeNode(nDer, w);
            nAnterior.iDatos[indNelim] = nDer.iDatos[0];
            for (int c = 0; c < selectedEnt.nodos.Count; c++)
                if (selectedEnt.nodos[c].lDireccionN == nAnterior.lDireccionN)
                    selectedEnt.nodos[c] = nAnterior;
            idxFile.writeNode(nAnterior, w);
            idxFile.writeNode(nE, w);
            for (int c = 0; c < selectedEnt.nodos.Count; c++)
                if (selectedEnt.nodos[c].lDireccionN == nE.lDireccionN)
                    selectedEnt.nodos[c] = nE;
        }
        //EN CASO DE QUE EL NODO NO PUEDA DONAR UNA CLAVE ESTE SE UNIRA CON EL NODO QUE ESTA PIDIENDO PRESTADO
        private void UneNdConNdIzq(ref Nodo nE, ref Nodo nIzq, ref Nodo nDer, ref Nodo nAnterior, ref Nodo nIzqi, ref Nodo nDeri, int indNelim, int indNelimInter, BinaryWriter w)
        {
            if (nE.cTipo == 'I')
            {
                nIzq.iDatos.Add(nAnterior.iDatos[indNelim]);
                nIzq.lDirecciones.Add(nE.lDirecciones[0]);
                nIzq.iDatos.Add(nE.iDatos[0]);
                nIzq.lDirecciones.Add(nE.lDirecciones[1]);
            }
            if (nE.cTipo == 'H')
            {
                nIzq.lDirecciones.Add(nE.lDirecciones[0]);
                nIzq.iDatos.Add(nE.iDatos[0]);
            }
            idxFile.writeNode(nIzq, w);
            int datEliminado = 0;
            nAnterior.iDatos.RemoveAt(indNelim);
            nAnterior.lDirecciones.RemoveAt(indNelim + 1);
            idxFile.writeNode(nAnterior, w);
            int ln = 0;
            for (; ln < selectedEnt.nodos.Count; ln++)
            {
                if (nIzq.lDireccionN == selectedEnt.nodos[ln].lDireccionN)
                    selectedEnt.nodos[ln] = nIzq;
                if (nAnterior.lDireccionN == selectedEnt.nodos[ln].lDireccionN)
                    selectedEnt.nodos[ln] = nAnterior;
                if (nE.lDireccionN == selectedEnt.nodos[ln].lDireccionN)
                    datEliminado = ln;
            }
            selectedEnt.nodos.RemoveAt(datEliminado);
            if (nAnterior.cTipo == 'I')
                EliminarIntermedios(ref nAnterior, ref nDer, ref nIzq, ref nAnterior, ref nIzqi, ref nDeri, indNelimInter, w);
        }
        //EN CASO DE QUE EL NODO NO PUEDA DONAR UNA CLAVE ESTE SE UNIRA CON EL NODO QUE ESTA PIDIENDO PRESTADO
        private void UneNdConNdDer(ref Nodo nE, ref Nodo nDer, ref Nodo nIzq, ref Nodo nAnterior, ref Nodo nIzqi, ref Nodo nDeri, int indNelim, int indNelimInter, BinaryWriter w)
        {
            if (nE.cTipo == 'I')
            {
                nE.iDatos.Add(nAnterior.iDatos[indNelim]);
                nE.lDirecciones.Add(nDer.lDirecciones[0]);
                for (int n = 0; n < nDer.iDatos.Count; n++)
                {
                    nE.iDatos.Add(nDer.iDatos[n]);
                    nE.lDirecciones.Add(nDer.lDirecciones[n + 1]);
                }
            }
            if (nE.cTipo == 'H')
            {
                for (int n = 0; n < nDer.iDatos.Count; n++)
                {
                    nE.iDatos.Add(nDer.iDatos[n]);
                    nE.lDirecciones.Add(nDer.lDirecciones[n]);
                }
            }

            int datEliminado = 0;
            nAnterior.iDatos.RemoveAt(indNelim);
            nAnterior.lDirecciones.RemoveAt(indNelim + 1);
            for (int ln = 0; ln < selectedEnt.nodos.Count; ln++)
            {
                if (nE.lDireccionN == selectedEnt.nodos[ln].lDireccionN)
                    selectedEnt.nodos[ln] = nE;
                if (nAnterior.lDireccionN == selectedEnt.nodos[ln].lDireccionN)
                    selectedEnt.nodos[ln] = nAnterior;
                if (nDer.lDireccionN == selectedEnt.nodos[ln].lDireccionN)
                    datEliminado = ln;
            }
            selectedEnt.nodos.RemoveAt(datEliminado);
            if (nAnterior.cTipo == 'I')
                EliminarIntermedios(ref nAnterior, ref nDer, ref nIzq, ref nAnterior, ref nIzqi, ref nDeri, indNelimInter, w);
        }
        //RECORRIDO DE LOS NODOS PARA ENCONTRAR EN DONDE SE ENCUENTRA LA CLAVE QUE SE QUIERE ELIMINAR
        private void RecorreNodos(ref Nodo nE, ref Nodo nDer, ref Nodo nIzq, int clave, Nodo nD)
        {
            for (int rn = 0; rn < nD.iDatos.Count; rn++)//Recorrido de la raiz para saber en que nodo se encuentra el dato que se quiere eliminar
            {
                if (clave < nD.iDatos[rn])
                {
                    for (int ln = 0; ln < selectedEnt.nodos.Count; ln++)
                    {
                        if (nD.lDirecciones[rn] == selectedEnt.nodos[ln].lDireccionN)
                        {
                            nE = buscaNodoHoja(nD.lDirecciones[rn]);
                            indNodoEliminar = rn;
                        }
                        if (rn < nD.lDirecciones.Count - 1)
                        {
                            if (selectedEnt.nodos[ln].lDireccionN == nD.lDirecciones[rn + 1])
                                nDer = buscaNodoHoja(nD.lDirecciones[rn + 1]);
                            nIzq = null;
                        }
                        if (rn == nD.lDirecciones.Count - 1)
                        {
                            if (selectedEnt.nodos[ln].lDireccionN == nD.lDirecciones[rn - 1])
                                nIzq = buscaNodoHoja(nD.lDirecciones[rn - 1]);
                            //indNodoEliminar = rn - 1;
                        }
                    }
                    rn = selectedEnt.nodos.Count;
                }
                else
                {
                    for (int ln = 0; ln < selectedEnt.nodos.Count; ln++)
                    {
                        if (nD.lDirecciones[rn + 1] == selectedEnt.nodos[ln].lDireccionN)
                        {
                            nE = buscaNodoHoja(nD.lDirecciones[rn + 1]);
                            indNodoEliminar = rn;
                        }
                        if ((rn + 1) < nD.lDirecciones.Count - 1)
                        {
                            if (selectedEnt.nodos[ln].lDireccionN == nD.lDirecciones[rn + 2])
                                nDer = buscaNodoHoja(nD.lDirecciones[rn + 2]);
                            // indNodoEliminar = rn + 1;
                        }

                        if ((rn + 1) == nD.lDirecciones.Count - 1)
                        {
                            if (selectedEnt.nodos[ln].lDireccionN == nD.lDirecciones[rn])
                                nIzq = buscaNodoHoja(nD.lDirecciones[rn]);
                            nDer = null;
                        }
                    }
                }
            }
        }
        //MÉTODO PARA RECORRER Y ELIMINAR INTERMEDIOS EN CASO DE QUE EXISTAN
        private void EliminarIntermedios(ref Nodo nE, ref Nodo nDer, ref Nodo nIzq, ref Nodo nAnterior, ref Nodo nIzqi, ref Nodo nDeri, int indNelimInter, BinaryWriter w)
        {
            if (nE.lDireccionN != Raiz.lDireccionN)
            {
                if (nE.iDatos.Count < 2)
                {
                    if (nIzqi != null)
                    {
                        if (nIzqi.iDatos.Count > 2)
                            DonaclaveIzq(ref nE, ref nIzqi, ref nAnterior, ref nDer, ref nDeri, indNelimInter, 0, w);
                        else
                            UneNdConNdIzq(ref nE, ref nIzqi, ref nDer, ref Raiz, ref nDer, ref nDeri, indNelimInter, 0, w);

                    }
                    else
                    {
                        if (nDeri.iDatos.Count > 2)
                            DonaclaveDer(ref nE, ref nDeri, ref nAnterior, ref nIzq, ref nIzqi, indNelimInter, 0, w);
                        else
                            UneNdConNdDer(ref nE, ref nDeri, ref nIzq, ref Raiz, ref nIzq, ref nIzqi, indNelimInter, 0, w);
                    }
                }
            }
            if (Raiz.iDatos.Count == 0)
            {
                int p = 0; long posi = Raiz.lDirecciones[0];
                for (int ln = 0; ln < selectedEnt.nodos.Count; ln++)
                    if (selectedEnt.nodos[ln].lDireccionN == Raiz.lDireccionN)
                        selectedEnt.nodos.RemoveAt(ln);
                for (int ln = 0; ln < selectedEnt.nodos.Count; ln++)
                    if (selectedEnt.nodos[ln].lDireccionN == posi)
                    {
                        selectedEnt.attributes[indArbol].indexDir = selectedEnt.nodos[ln].lDireccionN;
                        if (selectedEnt.nodos[ln].cTipo == 'I')
                        {
                            selectedEnt.nodos[ln].cTipo = 'R';
                            idxFile.writeNode(selectedEnt.nodos[ln], w);
                        }
                        ddFile.modifyAttribute(selectedEnt.attributes[indArbol].attributeDir, selectedEnt.attributes[indArbol]);
                    }
            }
        }

        #endregion

        private void buttonAdd_Click(object sender, EventArgs e)
        {

        }
        
        private void buttonModify_Click(object sender, EventArgs e)
        {
            DialogResult addDialog;
            Data ant = new Data();
            Data newData = new Data();
            long ap = Convert.ToInt64(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value);
            bool b = false;
            Attribute at = new Attribute();
            int pc = 0, pe = 0, ee = 0, ce = 0;

            int i = 0, ind = 0, indx = 0;
            foreach (Data reg in selectedEnt.data)
            {
                if (reg.dataDir == ap)
                { ant = reg; b = true; ind = i; }
                i++;
            }
            if (b)//Si si
            {
                file.stream = new FileStream(fileName, FileMode.Open);
                BinaryWriter writer = new BinaryWriter(file.stream, Encoding.UTF8);
                string cadena = ""; int num = -1;
                newData.dataDir = ant.dataDir;
                newData.nextDir = ant.nextDir;

                foreach (Attribute att in selectedEnt.attributes)
                {
                    string attNAME = new string(att.name);
                    Add add = new Add(attNAME);
                    addDialog = add.ShowDialog();
                    string c = new string(att.name);
                    bool found = false;
                    if (addDialog == DialogResult.OK)
                    {

                        if (att.type == 'I')
                        {
                            num = Convert.ToInt32(add.DataText);
                            newData.number.Add(num);
                        }
                        else
                            newData.str.Add(stringToCharArray(add.DataText, att.length));

                        if (att.indexType == 1)
                        {
                            if (new string(att.name) == c)
                            { at = att; pc = ce; pe = ee; }

                            if (att.type == 'C')
                                ce++;
                            else
                                ee++;
                           
                        }
                        else if (att.indexType == 2) //PK
                        {
                            if (idxFile.stream != null)
                            idxFile.stream.Close();

                            idxFile.stream = new FileStream(idxFileName, FileMode.Open);
                            BinaryWriter write = new BinaryWriter(idxFile.stream, Encoding.UTF8);
                            if (isPk || selectedEnt.pk.Count > 0)
                            {
                                for (int p = 0; p < sordedPKlist.Count; p++)
                                {
                                    //if (sordedPKlist[p].oClave.ToString() == selectedEnt.data[ind].Lregistros[ind].ToString())
                                    {
                                        if (selectedEnt.attributes[indx].type == 'C')
                                        {
                                            if(isSK)
                                                if (sordedPKlist[p].oClave.ToString() == selectedEnt.data[ind].str[indx - 1].ToString())
                                                    sordedPKlist[p].oClave = newData.str[indx - 1];
                                            else
                                                if (sordedPKlist[p].oClave.ToString() == selectedEnt.data[ind].str[indx].ToString())
                                                    sordedPKlist[p].oClave = newData.str[indx];

                                        }
                                        else
                                        {
                                            if (sordedPKlist[p].oClave.ToString() == selectedEnt.data[ind].number[indx].ToString())
                                                sordedPKlist[p].oClave = newData.number[indx];
                                        }
                                    }
                                }

                                //sordedPKlist = sordedPKlist.OrderBy(lof => lof.oClave).ToList();
                                sordedPKlist = sordedPKlist.OrderBy(lo => lo.oClave).ToList();
                                ordenaPk();
                                idxFile.modificaPk(selectedEnt, indx, write);
                                llenaDataPk();
                            }
                            idxFile.stream.Close();
                            write.Close();
                            
                        }
                        
                        else if(att.indexType == 3)
                        {
                            if (idxFile.stream != null)
                                idxFile.stream.Close();
                            idxFile.stream = new FileStream(idxFileName, FileMode.Open);

                            if (isPk)
                                idxFile.stream.Position = advance;

                            BinaryWriter write = new BinaryWriter(idxFile.stream, Encoding.UTF8);
                            if (isFk || selectedEnt.fk.Count > 0)
                            {
                                for (int f = 0; f < sortedFKlist.Count; f++)
                                {
                                    if (att.type == 'I')
                                    {
                                        if (sortedFKlist[f].oClave.ToString() == selectedEnt.data[ind].number[indFk].ToString())
                                        {
                                            if (sortedFKlist[f].lDirecciones.Count > 0)
                                            {
                                                for (int d = 0; d < sortedFKlist[f].lDirecciones.Count; d++)
                                                {
                                                    if (sortedFKlist[f].lDirecciones[d] == selectedEnt.data[ind].dataDir)
                                                    {
                                                        for (int ifk = 0; ifk < sortedFKlist.Count; ifk++)
                                                        {

                                                            //if (selectedEnt.data[ind].number[indx].ToString() == sortedFKlist[ifk].oClave.ToString())
                                                            if (newData.number[indFk].ToString() == sortedFKlist[ifk].oClave.ToString())
                                                            {
                                                                sortedFKlist[ifk].lDirecciones.Add(sortedFKlist[f].lDirecciones[d]);
                                                                ifk++;
                                                                found = true;
                                                            }
                                                        }
                                                        if (!found)
                                                        {
                                                            /*if (selectedEnt.attributes[indFk].type == 'C')
                                                            {
                                                                fk = new ForeignKey();
                                                                fk.oClave = DataLLenaRegistros[indFk, 0].Value.ToString();
                                                                fk.lDirecciones.Add(selectedEnt.data[indice].lDirRegistro);
                                                            }*/
                                                            //else
                                                            
                                                                fk = new ForeignKey();
                                                                fk.oClave = Convert.ToInt32(newData.number[indFk]);
                                                                fk.lDirecciones.Add(selectedEnt.data[ind].dataDir);
                                                            
                                                            sortedFKlist.Add(fk);
                                                            sortedFKlist[f].lDirecciones.RemoveAt(d);
                                                            if (sortedFKlist[f].lDirecciones.Count == 0)
                                                                sortedFKlist.RemoveAt(f);
                                                        }
                                                        else
                                                        {
                                                            sortedFKlist[f].lDirecciones.RemoveAt(d);
                                                            if (sortedFKlist[f].lDirecciones.Count == 0)
                                                                sortedFKlist.RemoveAt(f);
                                                            f--;
                                                        }

                                                    }
                                                }
                                            }
                                            else
                                            {
                                                
                                                //else
                                                {
                                                    sortedFKlist[f].oClave = Convert.ToInt32(newData.number[indFk]);
                                                }
                                            }
                                        }
                                        
                                    }
                                    else
                                    {
                                        string compare;
                                        if(isPk && !isSK)
                                            compare = new string(selectedEnt.data[ind].str[indFk - 1]);
                                        else if(!isPk && !isSK) 
                                            compare = new string(newData.str[indFk]);
                                        else //if(isPk && isSK)
                                            compare = new string(newData.str[indFk - 2]);


                                        if (sortedFKlist[f].oClave.ToString() == compare)
                                        {
                                            if (sortedFKlist[f].lDirecciones.Count > 0)
                                            {
                                                for (int d = 0; d < sortedFKlist[f].lDirecciones.Count; d++)
                                                {
                                                    if (sortedFKlist[f].lDirecciones[d] == selectedEnt.data[ind].dataDir)
                                                    {
                                                        for (int ifk = 0; ifk < sortedFKlist.Count; ifk++)
                                                        {
                                                            string compare2;

                                                            if (isPk && !isSK)
                                                                compare2 = new string(selectedEnt.data[ind].str[indFk - 1]);
                                                            else if (!isPk && !isSK)
                                                                compare2 = new string(newData.str[indFk]);
                                                            else// (isPk && isSK)
                                                                compare2 = new string(newData.str[indFk - 2]);

                                                            if (compare2 == sortedFKlist[ifk].oClave.ToString())
                                                            {
                                                                sortedFKlist[ifk].lDirecciones.Add(sortedFKlist[f].lDirecciones[d]);
                                                                ifk++;
                                                                found = true;
                                                            }
                                                        }
                                                        if (!found)
                                                        {
                                                            /*if (selectedEnt.attributes[indFk].type == 'C')
                                                            {
                                                                fk = new ForeignKey();
                                                                fk.oClave = DataLLenaRegistros[indFk, 0].Value.ToString();
                                                                fk.lDirecciones.Add(selectedEnt.data[indice].lDirRegistro);
                                                            }*/
                                                            //else
                                                            {
                                                                fk = new ForeignKey();
                                                                if (isPk)
                                                                    fk.oClave = new string(newData.str[indFk - 1]);
                                                                else
                                                                    fk.oClave = new string(newData.str[indFk]);
                                                                fk.lDirecciones.Add(selectedEnt.data[ind].dataDir);
                                                            }
                                                            sortedFKlist.Add(fk);
                                                            sortedFKlist[f].lDirecciones.RemoveAt(d);
                                                            if (sortedFKlist[f].lDirecciones.Count == 0)
                                                                sortedFKlist.RemoveAt(f);
                                                        }
                                                        else
                                                        {
                                                            sortedFKlist[f].lDirecciones.RemoveAt(d);
                                                            if (sortedFKlist[f].lDirecciones.Count == 0)
                                                                sortedFKlist.RemoveAt(f);
                                                            f--;
                                                        }

                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //if (selectedEnt.attributes[indFk].type == 'C')
                                                {
                                                    if (isPk)
                                                        sortedFKlist[f].oClave = new string(newData.str[indFk - 1]);
                                                    else
                                                        sortedFKlist[f].oClave = new string(newData.str[indFk]);
                                                }
                                                //else
                                                {
                                                  //  sortedFKlist[f].oClave = Convert.ToInt32(DataLLenaRegistros[indFk, 0].Value);
                                                }
                                            }
                                        }
                                    }
                                }
                                /*if (!busca(selectedEnt.attributes[indFk].type))
                                {
                                    fk = new ForeignKey();

                                    if (selectedEnt.attributes[indFk].type == 'I')
                                    {
                                        //num = Convert.ToInt32(add.DataText);
                                        //data.number.Add(num);
                                        fk.oClave = Convert.ToInt32(data.number[indFk]);
                                    }
                                    else
                                    {
                                        //data.str.Add(stringToCharArray(add.DataText, att.length));
                                        if (isPk)
                                            fk.oClave = new string(data.str[indFk - 1]);
                                        else
                                            fk.oClave = new string(data.str[indFk]);
                                    }
                                    fk.lDirecciones.Add(data.dataDir);
                                    sortedFKlist.Add(fk);
                                    sortedFKlist = sortedFKlist.OrderBy(lof => lof.oClave).ToList();
                                    ordenaFk();
                                    idxFile.ModificaFk(selectedEnt, indFk, write);

                                }
                                else
                                {
                                    ordenaFk();
                                    idxFile.ModificaFk(selectedEnt, indFk, write);
                                }*/
                                sortedFKlist = sortedFKlist.OrderBy(lof => lof.oClave).ToList();
                                ordenaFk();
                                llenaDataFk();
                                idxFile.ModificaFk(selectedEnt, indFk, write);
                            }
                            idxFile.stream.Close();
                            write.Close();

                        }
                        else if(att.indexType == 4)
                        {
                            if (idxFile.stream != null)
                                idxFile.stream.Close();

                            idxFile.stream = new FileStream(idxFileName, FileMode.Open);
                            BinaryWriter write = new BinaryWriter(idxFile.stream, Encoding.UTF8);

                            if (isTree)
                            {
                                EliminarClave(write, Convert.ToInt32(selectedEnt.data[ind].number[indArbol]), selectedEnt.data[ind].dataDir);
                                InsertaClave(write, Convert.ToInt32(newData.number[indArbol]), newData.dataDir);
                                llenaDataGridArbol();
                            }

                            idxFile.stream.Close();
                            write.Close();
                        }

                    }
                    indx += 1;
                }

                selectedEnt.data[ind] = newData;//Carga a memoria
                file.stream.Position = newData.dataDir;//La posicion del archivo sobre la direccion del actual registro
                newData.saveData(file.stream, writer, selectedEnt.attributes);//Escribe registro en el archivo

                dataGridView1.Rows.Clear();
                //showData(selectedEnt.data);
                file.stream.Close();

                if (at.indexType == 1)
                {
                    if (at.type == 'C')
                        Sort(selectedEnt.data, pc, 0);
                    else
                        Sort(selectedEnt.data, pe, 1);
                }

                showData(selectedEnt.data);

                MessageBox.Show("Data Modified");
            }
            else
                MessageBox.Show("NO Data to Modify");
            //tbDirADat.Text = EntidadEnt.ListDat[0].DirApAct.ToString();

        }

        private void comboBoxIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            IndexView indexView = new IndexView(selectedEnt, dataTFk, dataTArbol);
            indexView.ShowDialog();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int indice = dataGridView1.CurrentCell.RowIndex;

            file.stream = new FileStream(fileName, FileMode.Open);
            BinaryWriter writer = new BinaryWriter(file.stream, Encoding.UTF8);

            ddFile.stream = new FileStream(ddFile.fileName, FileMode.Open);
            BinaryWriter writerDat = new BinaryWriter(ddFile.stream, Encoding.UTF8);

            BinaryWriter writerIdx = null;

            if (isFk || isPk || isTree)
            {
                idxFile.stream = new FileStream(idxFileName, FileMode.Open);
                writerIdx = new BinaryWriter(idxFile.stream, Encoding.UTF8);
            }
                
            if (isPk)
            {
                if (selectedEnt.pk.Count > 0)
                {
                    for (int p = 0; p < sordedPKlist.Count; p++)
                    {
                        if (sordedPKlist[p].lDireccion == selectedEnt.data[indice].dataDir)
                        {
                            sordedPKlist.RemoveAt(p);

                        }
                    }
                }
                ordenaPk();
                llenaDataPk();
                idxFile.modificaPk(selectedEnt, ind, writerIdx);
            }
            if (isFk)
            {
                if (selectedEnt.fk.Count > 0)
                {
                    for (int f = 0; f < sortedFKlist.Count; f++)
                    {
                        for (int fd = 0; fd < sortedFKlist[f].lDirecciones.Count; fd++)
                        {
                            if (sortedFKlist[f].lDirecciones[fd] == selectedEnt.data[indice].dataDir)
                            {
                                sortedFKlist[f].lDirecciones.RemoveAt(fd);
                                if (sortedFKlist[f].lDirecciones.Count == 0)
                                {
                                    sortedFKlist.RemoveAt(f);
                                    if (sortedFKlist.Count > 1)
                                        f = sortedFKlist.Count - 1;
                                    else
                                        f = 0;
                                }
                            }
                        }
                    }
                }
                ordenaFk();
                llenaDataFk();
                idxFile.ModificaFk(selectedEnt, indFk, writerIdx);
            }
            if (isTree)
            {
                EliminarClave(writerIdx, Convert.ToInt32(selectedEnt.data[indice].number[indArbol]), selectedEnt.data[indice].dataDir);
                llenaDataGridArbol();
            }
            selectedEnt.data.RemoveAt(indice);
            verificaClaveDeBusqueda();
            CambiaDirecciones();
            /*if (selectedEnt.data.Count != 0)
                selectedEnt.dataDir = selectedEnt.data[0].dataDir;
            else
                selectedEnt.dataDir = -1;*/

            selectedEnt.data[ind].saveData(file.stream, writer, selectedEnt.attributes);
            writerDat.Seek((int)selectedEnt.entityDir, SeekOrigin.Begin);
            selectedEnt.SaveEntity2(writerDat);
            showData(selectedEnt.data);
            file.stream.Close();
            ddFile.stream.Close();
            idxFile.stream.Close();
            writer.Close();
            writerDat.Close();
            writerIdx.Close();
            //file.modificaRegistros(selectedEnt);
            
            //ddFile.modificaEntidad(selectedEnt);

            showData(selectedEnt.data);
        }

        private void CambiaDirecciones()
        {
            for (int i = 0; i < selectedEnt.data.Count; i++)
            {
                if (i != selectedEnt.data.Count - 1)
                {
                    selectedEnt.data[i].nextDir = selectedEnt.data[i + 1].nextDir;
                }
                else
                {
                    selectedEnt.data[i].nextDir = -1;
                }
            }
        }

        private void verificaClaveDeBusqueda()
        {
            for (int i = 0; i < selectedEnt.attributes.Count; i++)
            {
                if (selectedEnt.attributes[i].indexType == 1)
                {
                    if(selectedEnt.attributes[i].type == 'C')
                        selectedEnt.data = selectedEnt.data.OrderBy(r => r.str[i]).ToList();
                    else
                        selectedEnt.data = selectedEnt.data.OrderBy(r => r.number[i]).ToList();
                }
            }
        }
        /*
        Data auxData = new Data();
        long ap = Convert.ToInt64(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value);
        bool b = false;

        int i = 0, ind = 0;
        foreach (Data d in selectedEnt.data)
        {
            if (d.dataDir == ap)
            { auxData = d; b = true; ind = i; }
            i++;
        }
        if (b)
        {
            file.stream = new FileStream(fileName, FileMode.Open);
            BinaryWriter writer = new BinaryWriter(file.stream, Encoding.UTF8);
            ddFile.stream = new FileStream(ddFile.fileName, FileMode.Open);
            BinaryWriter writerDat = new BinaryWriter(ddFile.stream, Encoding.UTF8);
            //EliminaRegistro(auxReg, ind); //Elimina registro
            if (selectedEnt.dataDir == auxData.dataDir)//Si es el primer registro 
            {
                selectedEnt.dataDir = auxData.nextDir;//Cambia direccion de los datos  de la entidad
                file.stream.Position = selectedEnt.entityDir;
                selectedEnt.SaveEntity(file.stream, writer);//Escribe la entidad en el archivo
            }
            else//Si el regitro no fue el primero 
            {
                selectedEnt.data[ind - 1].nextDir = auxData.nextDir;//Se crea un anterior y se asigna el dato 
                file.stream.Position = selectedEnt.data[ind - 1].dataDir;
                selectedEnt.data[ind - 1].saveData(file.stream, writer, selectedEnt.attributes);//Escribe registro en el archivo
            }

            selectedEnt.data.RemoveAt(ind);
            dataGridView1.Rows.Clear();

            selectedEnt.SaveEntity2(writerDat);
            showData(selectedEnt.data);
            file.stream.Close();
            MessageBox.Show("Data Deleted");
        }
        else
            MessageBox.Show("NO Data to Delete");*/


        private char[] stringToCharArray(string cad, int num)
        {
            char[] str = new char[num];
            for (int i = 0; i < cad.Count(); i++)
                str[i] = cad[i];
            return str;
        }

        private void openIndexFile()
        {
            
            idxFileName = dir + "\\" + name + ".idx";
            idxFile = new AuxFile();
            if (idxFile.stream != null)
                idxFile.stream.Close();
            idxFile.stream = new FileStream(idxFileName, FileMode.Open);
            idxFile.stream.Position = 0;
        }

        private BinaryWriter createIndexFile()
        {
            idxFileName = dir + "\\" + name + ".idx";
            idxFile = new AuxFile();
            idxFile.fileName = idxFileName;
            idxFile.stream = new FileStream(idxFileName, FileMode.Create);
            idxFile.stream.Position = 0;
            BinaryWriter writer = new BinaryWriter(idxFile.stream);
            return writer;
        }

        private BinaryReader readIndexFile()
        {
            idxFile.stream.Close();
            idxFileName = dir + "\\" + name + ".idx";
            idxFile = new AuxFile();
            idxFile.stream = new FileStream(idxFileName, FileMode.Open);
            idxFile.stream.Position = 0;
            BinaryReader reader = new BinaryReader(idxFile.stream);
            return reader;
        }

        private void comboBoxAtt_SelectedIndexChanged(object sender, EventArgs e)
        {
            DialogResult addDialog;

            file.stream = new FileStream(fileName, FileMode.Open);
            
            BinaryWriter writer = new BinaryWriter(file.stream, Encoding.UTF8);
            
            BinaryReader reader = new BinaryReader(file.stream, Encoding.UTF8);

            data = new Data();

            bool firstOne = false; string str = ""; int num = -1;

            if (selectedEnt.dataDir == -1)
                firstOne = true;

            data.dataDir = file.stream.Length;
            Attribute at = new Attribute();
            int pc = 0, pe = 0, ee = 0, ce = 0;

            foreach (Attribute att in selectedEnt.attributes)
            {
                string attNAME = new string(att.name);
                Add add = new Add(attNAME);
                addDialog = add.ShowDialog();
                string c = new string(att.name);

                

                if (addDialog == DialogResult.OK)
                {
                    if (att.type == 'I')
                    {
                        num = Convert.ToInt32(add.DataText);
                        data.number.Add(num);
                    }
                    else
                        data.str.Add(stringToCharArray(add.DataText, att.length));

                    if (firstOne)
                    {
                        ddFile.stream = new FileStream(ddFile.fileName, FileMode.Open, FileAccess.ReadWrite);
                        BinaryWriter writerDat = new BinaryWriter(ddFile.stream, Encoding.UTF8);
                        file.stream.Position = 0;
                        data.dataDir = 0;
                        selectedEnt.dataDir = data.dataDir;
                        ddFile.stream.Position = selectedEnt.entityDir;
                        //selectedEnt.SaveEntity(ddFile.stream, writerDat);
                        selectedEnt.SaveEntity2(writerDat);
                        //ddFile.modifyEntity(selectedEnt.entityDir, selectedEnt);
                        ddFile.stream.Close();
                        writerDat.Close();
                    }
                    else
                    {
                        selectedEnt.data[selectedEnt.data.Count - 1].nextDir = data.dataDir;
                        file.stream.Position = selectedEnt.data[selectedEnt.data.Count - 1].dataDir;
                        selectedEnt.data[selectedEnt.data.Count - 1].saveData(file.stream, writer, selectedEnt.attributes);
                    }

                    data.nextDir = -1;
                    //if(selectedEnt.attributes.Count > 1)
                    //selectedEnt.data.Add(data);
                    file.stream.Position = data.dataDir;

                    if (att.indexType == 1)
                    {
                        if (new string(att.name) == c)
                        { at = att; pc = ce; pe = ee; }

                        if (att.type == 'C')
                            ce++;
                        else
                            ee++;

                        isSK = true;

                        /*if (att.type == 'I')
                        {
                            num = Convert.ToInt32(add.DataText);
                            data.number.Add(num);
                        }
                        else
                            data.str.Add(stringToCharArray(add.DataText, att.length));*/
                    }
                    else if(att.indexType == 0)
                    {
                        /*if (att.type == 'I')
                        {
                            num = Convert.ToInt32(add.DataText);
                            data.number.Add(num);
                        }
                        else
                            data.str.Add(stringToCharArray(add.DataText, att.length));*/
                    }
                    else if(att.indexType == 2)
                    {
                        if (idxFile.stream != null)
                            idxFile.stream.Close();
                        idxFile.stream = new FileStream(idxFileName, FileMode.Open);
                        BinaryWriter write = new BinaryWriter(idxFile.stream, Encoding.UTF8);
                        if (isPk || selectedEnt.pk.Count > 0)
                        {
                            pk = new PrimaryKey();
                            if (selectedEnt.attributes[ind].type == 'I')
                            {
                                //num = Convert.ToInt32(add.DataText);
                                //data.number.Add(num);
                                pk.oClave = data.number[ind];
                            }
                            else
                            {
                                //data.str.Add(stringToCharArray(add.DataText, att.length));
                                if(isSK)
                                    pk.oClave = new string(data.str[ind - 1]);
                                else
                                    pk.oClave = new string(data.str[ind]);
                            }

                            pk.lDireccion = data.dataDir;
                            sordedPKlist.Add(pk);
                            sordedPKlist = sordedPKlist.OrderBy(lo => lo.oClave).ToList();
                            ordenaPk();
                            idxFile.modificaPk(selectedEnt, ind, write);
                            llenaDataPk();
                        }
                        idxFile.stream.Close();
                        write.Close();
                    }
                    else if(att.indexType == 3)
                    {
                        if (idxFile.stream != null)
                            idxFile.stream.Close();
                        idxFile.stream = new FileStream(idxFileName, FileMode.Open);
                        
                        if (isPk)
                            idxFile.stream.Position = advance;

                        BinaryWriter write = new BinaryWriter(idxFile.stream, Encoding.UTF8);
                        if (isFk || selectedEnt.fk.Count > 0)
                        {
                            if (!busca(selectedEnt.attributes[indFk].type))
                            {
                                fk = new ForeignKey();

                                if (selectedEnt.attributes[indFk].type == 'I')
                                {
                                    //num = Convert.ToInt32(add.DataText);
                                    //data.number.Add(num);
                                    fk.oClave = Convert.ToInt32(data.number[indFk]);
                                }
                                else
                                {
                                    //data.str.Add(stringToCharArray(add.DataText, att.length));
                                    if(isPk && !isSK)
                                        fk.oClave = new string(data.str[indFk - 1]);
                                    else if(!isPk)
                                        fk.oClave = new string(data.str[indFk]);
                                    else if(isPk && isSK)
                                        fk.oClave = new string(data.str[indFk - 2]);
                                }
                                fk.lDirecciones.Add(data.dataDir);
                                sortedFKlist.Add(fk);
                                sortedFKlist = sortedFKlist.OrderBy(lof => lof.oClave).ToList();
                                ordenaFk();
                                idxFile.ModificaFk(selectedEnt, indFk, write);

                            }
                            else
                            {
                                ordenaFk();
                                idxFile.ModificaFk(selectedEnt, indFk, write);
                            }
                            llenaDataFk();
                        }
                        idxFile.stream.Close();
                        write.Close();
                    }
                    else if(att.indexType == 4)
                    {
                        if (idxFile.stream != null)
                            idxFile.stream.Close();
                        idxFile.stream = new FileStream(idxFileName, FileMode.Open);

                        if (isPk)
                            idxFile.stream.Position = advance;

                        BinaryWriter write = new BinaryWriter(idxFile.stream, Encoding.UTF8);

                        if (isTree)
                        {
                            InsertaClave(write, Convert.ToInt32(data.number[indArbol]), data.dataDir);
                            llenaDataGridArbol();
                        }
                        idxFile.stream.Close();

                    }
                    
                    

                }
            }

            /*if (at.type == 'C')
                Ordena(selectedEnt.data, pc, 0);
            else
                Ordena(selectedEnt.data, pe, 1);

            showData(selectedEnt.data);*/



            selectedEnt.data.Add(data);
            data.saveData(file.stream, writer, selectedEnt.attributes);
            writer.Close();
            file.stream.Close();
            
            if(at.indexType == 1)
            {
                if (at.type == 'C')
                    Sort(selectedEnt.data, pc, 0);
                else
                    Sort(selectedEnt.data, pe, 1);
            }
            

            //showData(selectedEnt.data);

            showData(selectedEnt.data);
            

        }

        public void Sort(List<Data> SortedList, int p, int tipo) //Sort the list and write it on the file
        {
            file.stream = new FileStream(fileName, FileMode.Open);
            if(ddFile.stream != null)
            {
                ddFile.stream.Close();
            }
            ddFile.stream = new FileStream(ddFile.fileName, FileMode.Open, FileAccess.ReadWrite);
            BinaryWriter writer = new BinaryWriter(file.stream, Encoding.UTF8);
            BinaryWriter writerDat = new BinaryWriter(ddFile.stream, Encoding.UTF8);
            List<string> lista = new List<string>();
            for (int i = 0; i < SortedList.Count; i++)
                for (int x = 0; x < SortedList.Count - 1; x++)
                    if (tipo == 1)
                    {
                        if (SortedList[x].number[p] > SortedList[x + 1].number[p]) //Sort the list
                        {
                            Data aux = SortedList[x];
                            SortedList[x] = SortedList[x + 1];
                            SortedList[x + 1] = aux;
                        }
                    }
                    else
                    {
                        /*foreach(char[] name in SortedList[x].str)
                        {
                            string strin = new string(name);
                            lista.Add(strin);
                        }
                        lista.Sort();
                        SortedList[x].str = new List<char[]>();
                        foreach(string name in lista)
                        {
                            char[]name2 = StringToChar(name, 30);
                            SortedList[x].str.Add(name2);
                        }*/
                        //SortedList[x].str = SortedList[x].str.OrderBy(r => new string(r)).ToList();

                        string s = new string(SortedList[x].str[p]);
                        string t = new string(SortedList[x + 1].str[p]);

                        int c = string.Compare(s, t);
                        if (c > 0)
                        {
                            Data aux = SortedList[x];
                            SortedList[x] = SortedList[x + 1];
                            SortedList[x + 1] = aux;
                        }
                        //SortedList[x].str[p] = SortedList[x].str[p].OrderBy(a => ).ToList();

                        /*if (SortedList[x].str[p].CompareTo(SortedList[x + 1].str[p]) > 0) //Sort the list
                        {
                            Data aux = SortedList[x];
                            SortedList[x] = SortedList[x + 1];
                            SortedList[x + 1] = aux;
                        }*/
                    }
            for (int x = 0; x < SortedList.Count; x++)
            {
                if ((x + 1) != SortedList.Count) //If there's a next
                    SortedList[x].nextDir = SortedList[x + 1].dataDir;
                else
                    SortedList[x].nextDir = -1;
            }
            foreach (Data data in SortedList) //Save the file
            {
                file.stream.Position = data.dataDir;
                data.saveData(file.stream, writer, selectedEnt.attributes); //Write data in the file
            }
            selectedEnt.dataDir = SortedList[0].dataDir;
            ddFile.stream.Position = selectedEnt.entityDir;

            selectedEnt.SaveEntity2(writerDat);
            writerDat.Close();
            ddFile.stream.Close();
            file.stream.Close();
            writer.Close();
        }

        public void showData(List<Data> listReg)
        {
            if (selectedEnt.dataDir != -1)
            {
                dataGridView1.Columns.Clear();
                dataGridView1.ColumnCount = 0;
                dataGridView1.RowCount = 0;
                dataGridView1.Rows.Clear();
                dataGridView1.ColumnCount = selectedEnt.attributes.Count + 2;
                dataGridView1.Columns[0].Name = "DataDir";
                dataGridView1.Columns[dataGridView1.ColumnCount - 1].Name = "NextDir";

                int x = 1;
                foreach (Attribute att in selectedEnt.attributes)
                {
                    string str = new string(att.name);
                    dataGridView1.Columns[x].Name = str;
                    x++;
                }

                int ind = 0;
                foreach (Data re in listReg)
                {
                    int i = 1;
                    int j = 0; int k = 0;
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[ind].Cells[0].Value = re.dataDir;
                    dataGridView1.Rows[ind].Cells[dataGridView1.ColumnCount - 1].Value = re.nextDir;
                    foreach (Attribute at in selectedEnt.attributes)
                    {
                        if (at.type.ToString() == "I")
                        {
                            dataGridView1.Rows[ind].Cells[i].Value = re.number[j++];
                        }
                        else
                        {
                            string str = new string(re.str[k++]);
                            dataGridView1.Rows[ind].Cells[i].Value = str;
                        }
                            
                        i++;
                    }
                    ind++;
                }
            }
        }

        
    }
}
