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
        int ind, ip = 0, indFk, indTree, fki = 0, indFound, advance, advanceFK, order = 2, indR = 0, indPk;
        int indDeleteNode, indNInterElim;

        Node node, Root;
        List<Node> Lnodo = new List<Node>();
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
                    advanceFK = ((8 * 20) + (attribute.length)) * 20;

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
                            copySortedListPK();
                            copySortedListFK();
                            verifyPK(writer);
                            verifyFK(writer);
                            verifyTree(writer);
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

                                            copySortedListPK();
                                            copySortedListFK();
                                            verifyPK(writer);
                                            verifyFK(writer);
                                            verifyTree(writer);
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
                                                            Mfk.directions.Add(R.ReadInt64());
                                                        }
                                                        selectedEnt.fk.Add(Mfk);

                                                    }
                                                    else
                                                    {
                                                        Mfk.oClave = R.ReadInt32();
                                                        for (int d = 0; d < 20; d++)
                                                        {
                                                            Mfk.directions.Add(R.ReadInt64());
                                                        }
                                                        selectedEnt.fk.Add(Mfk);
                                                    }
                                                }
                                            }
                                            if (atrib.indexType == 4)
                                            { 
                                                Node n;
                                                Node nuevo = idxFile.readNode(atrib.indexDir, R);
                                                selectedEnt.nodes.Add(nuevo);
                                                if (nuevo.type == 'R')
                                                {
                                                    for (int i = 0; i < nuevo.dataL.Count + 1; i++)
                                                    {
                                                        n = idxFile.readNode(nuevo.directions[i], R);
                                                        selectedEnt.nodes.Add(n);
                                                        if (n.type == 'I')
                                                        {
                                                            for (int j = 0; j < n.directions.Count; j++)
                                                            {
                                                                Node nh = idxFile.readNode(n.directions[j], R);
                                                                selectedEnt.nodes.Add(nh);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            copySortedListPK();
                                            copySortedListFK();
                                            verifyPK(write);
                                            if(isFk)
                                            verifyFK(write);
                                            if(isTree)
                                            verifyTree(write);
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

        private void sortPK()
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
        private void verifyPK(BinaryWriter w)
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
        private void copySortedListPK()
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
        private void verifyFK(BinaryWriter w)
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
                                fk.directions.Add(-1);
                            }
                            selectedEnt.fk.Add(fk);
                            if (selectedEnt.attributes[i].type == 'C')
                            {

                                w.Write(StringToChar(fk.oClave.ToString(), selectedEnt.attributes[i].length));
                                foreach (long l in fk.directions)
                                {
                                    w.Write(l);
                                }
                            }
                            else
                            {
                                w.Write(Convert.ToInt32(fk.oClave));
                                foreach (long l in fk.directions)
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
        private void sortFK()
        {
            for (int ifk = 0; ifk < 20; ifk++)
            {
                if (ifk < sortedFKlist.Count)
                {
                    selectedEnt.fk[ifk].oClave = sortedFKlist[ifk].oClave;
                    for (int j = 0; j < 20; j++)
                    {
                        if (j < sortedFKlist[ifk].directions.Count)
                            selectedEnt.fk[ifk].directions[j] = sortedFKlist[ifk].directions[j];
                        else
                            selectedEnt.fk[ifk].directions[j] = -1;
                    }
                }
                else
                {
                    if (selectedEnt.attributes[ind].type == 'C')
                        selectedEnt.fk[ifk].oClave = "0";
                    else selectedEnt.fk[ifk].oClave = 0;
                    for (int j = 0; j < 20; j++)
                    {
                        selectedEnt.fk[ifk].directions[j] = -1;
                    }

                }
            }
        }
        //MÉTODO QUE BUSCA SI YA EXISTE LA CLAVE E INSERTA LA DIRECCIÓN EN SU LISTA.
        private bool searchFK(int type)
        {
            bool band = false;
            string compare;
            for (int ifk = 0; ifk < 20; ifk++)
            {
                if (selectedEnt.fk[ifk].directions[0] != -1)
                {
                    if(data.str.Count !=0)
                    {
                        if (isPk && !isSK &&type == 'C')
                        {
                            compare = new string(data.str[indFk - 1]);
                            if (compare == selectedEnt.fk[ifk].oClave.ToString())
                            {
                                indFound = ifk;
                                sortedFKlist[ifk].directions.Add(data.dataDir);
                                band = true;

                            }
                        }
                        else if (!isPk && type == 'C' )
                        {
                            compare = new string(data.str[indFk]);
                            if (compare == selectedEnt.fk[ifk].oClave.ToString())
                            {
                                indFound = ifk;
                                sortedFKlist[ifk].directions.Add(data.dataDir);
                                band = true;

                            }
                        }
                        else if(isPk && isSK && type == 'C')
                        {
                            compare = new string(data.str[indFk - 2]);
                            if (compare == selectedEnt.fk[ifk].oClave.ToString())
                            {
                                indFound = ifk;
                                sortedFKlist[ifk].directions.Add(data.dataDir);
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
                                sortedFKlist[ifk].directions.Add(data.dataDir);
                                band = true;
                            }
                        //else if(!isPk && type == 'I')
                    }
                    
                }
            }
            return band;

        }
        //MÉTODO PARA LLENAR EL DATAGIRDVIEW DE LA CLAVE SECUNDARIA
        private void fillDataFK()
        {
            /*DataRow r;
            dataTFk.Clear();
            for (int i = 0; i < selectedEnt.fk.Count; i++)
            {
                r = dataTFk.NewRow();
                int j = 0;
                r[j] = selectedEnt.fk[i].oClave;
                j = 1;
                for (int c = 0; c < selectedEnt.fk[i].directions.Count; c++)
                {
                    r[j] = selectedEnt.fk[i].directions[c];
                    j++;
                }
                dataTFk.Rows.Add(r);
            }*/
        }
        //DESPUES DE ABRIR EL ARCHIVO COPIA LA LISTA DE LA CLAVE SECUNDARIA
        private void copySortedListFK()
        {
            if (selectedEnt.fk.Count > 0)
            {
                isFk = true;
                for (fki = 0; fki < 20; fki++)
                {
                    if (selectedEnt.fk[fki].directions[0] != -1)
                    {
                        fk = new ForeignKey();
                        fk.oClave = selectedEnt.fk[fki].oClave;
                        for (int d = 0; d < 20; d++)
                        {
                            if (selectedEnt.fk[fki].directions[d] != -1)
                            {
                                fk.directions.Add(selectedEnt.fk[fki].directions[d]);
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
            for (int i = 0; i < order * 2; i++)
            {
                dataTArbol.Columns.Add("Data" + (i + 1).ToString());
                dataTArbol.Columns.Add("P" + (i + 2).ToString());
            }
        }
        //MÉTODO DE VERIFICACIÓN DE ARBOL B+
        private void verifyTree(BinaryWriter w)
        {
            long pos = w.BaseStream.Length;

            if (isPk && !isFk)
                pos = advance;
            else if (isPk && isFk)
                pos = advance + advanceFK;
            else if (isFk && !isPk)
                pos = advanceFK;

            if (selectedEnt.nodes.Count == 0)
            {
                for (int i = 0; i < selectedEnt.attributes.Count; i++)
                {
                    if (selectedEnt.attributes[i].indexType == 4)
                    {
                        selectedEnt.attributes[i].indexDir = pos;
                        isTree = true;
                        DataTableArbol();
                        indTree = i;
                        node = new Node();
                        node.nodeDir = pos;
                        selectedEnt.nodes.Add(node);
                        idxFile.writeNode(node, w);
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
        private void insertKey(BinaryWriter w, int clave, long dir)
        {
            bool repeated = false;
            foreach (Node r in selectedEnt.nodes)//Checa si existe la raiz
            {
                if (r.type == 'R')
                    Root = r;
            }

            if (Root == null)
            {
                if (node.dataL.Contains(clave))
                    repeated = true;

                if (!repeated)
                {
                    if (node.dataL.Count < 4)
                    {
                        node.directions.Add(dir);
                        node.dataL.Add(clave);
                        if (node.dataL.Count > 1)
                            sortNodes(node);
                        idxFile.writeNode(node, w);
                        selectedEnt.nodes[0] = node;
                    }
                    else//Se crea la raiz
                    {
                        Node Aux = node;
                        node = new Node();
                        node.nodeDir = w.BaseStream.Length;
                        Aux.directions.Add(dir);
                        Aux.dataL.Add(clave);
                        sortNodes(Aux);
                        SplitNodeNinsertKeys(Aux, node);
                        sortNodes(node);
                        selectedEnt.nodes[0] = Aux;
                        selectedEnt.nodes.Add(node);
                        idxFile.writeNode(Aux, w);
                        idxFile.writeNode(node, w);
                        Root = new Node();
                        Root.type = 'R';
                        Root.nodeDir = w.BaseStream.Length;
                        Root.directions.Add(Aux.nodeDir);
                        Root.directions.Add(node.nodeDir);
                        Root.dataL.Add(node.dataL[0]);
                        selectedEnt.nodes.Add(Root);
                        idxFile.writeNode(Root, w);
                        indR = selectedEnt.nodes.Count - 1;
                    }
                }
                if (Root != null)
                {
                    selectedEnt.attributes[indTree].indexDir = Root.nodeDir;
                    ddFile.modifyAttribute(selectedEnt.attributes[indTree].attributeDir, selectedEnt.attributes[indTree]);
                }

            }
            else//Se crean los intermedios
            {
                Node rootNode = new Node();
                Node interNode = new Node();
                Node prevNode;
                rootNode = whereToInsert(Root, clave);
                prevNode = Root;
                if (rootNode.type == 'I')//checa en caso de que existan nodes intermedios
                {
                    prevNode = rootNode;
                    interNode = whereToInsert(rootNode, clave);
                    rootNode = interNode;
                }

                if (rootNode.dataL.Count < 4)
                {
                    rootNode.directions.Add(dir);
                    rootNode.dataL.Add(clave);
                    if (rootNode.dataL.Count > 1)
                        sortNodes(rootNode);
                    idxFile.writeNode(rootNode, w);
                    selectedEnt.nodes[indListNodo] = rootNode;
                }
                else
                {
                    if (prevNode == Root)
                    {
                        Node Aux = rootNode;
                        node = new Node();
                        node.nodeDir = w.BaseStream.Length;
                        Aux.directions.Add(dir);
                        Aux.dataL.Add(clave);
                        sortNodes(Aux);
                        SplitNodeNinsertKeys(Aux, node);
                        sortNodes(node);
                        selectedEnt.nodes[indListNodo] = Aux;
                        selectedEnt.nodes.Add(node);
                        idxFile.writeNode(Aux, w);
                        idxFile.writeNode(node, w);
                        if (Root.dataL.Count < 4)
                        {
                            Root.directions.Add(node.nodeDir);
                            Root.dataL.Add(node.dataL[0]);
                            sortNodes(Root);
                            selectedEnt.nodes[indR] = Root;
                        }
                        else
                        {
                            Node nint = new Node();
                            nint.type = 'I';
                            nint.nodeDir = w.BaseStream.Length;
                            Node nAux = Root;
                            nAux.dataL.Add(node.dataL[0]);
                            sortNodes(nAux);
                            SplitNodeNinsertKeys(nAux, nint);
                            sortNodes(nint);
                            nAux.type = 'I';
                            for (int i = 0; i < selectedEnt.nodes.Count; i++)
                            {
                                if (selectedEnt.nodes[i].nodeDir == Root.nodeDir)
                                    selectedEnt.nodes[i] = nAux;
                            }
                            idxFile.writeNode(nAux, w);
                            Root = new Node();
                            Root.type = 'R';
                            Root.directions.Add(nAux.nodeDir);
                            Root.directions.Add(nint.nodeDir);
                            Root.dataL.Add(nint.dataL[0]);
                            indR = selectedEnt.nodes.Count - 1;
                            nint.dataL.RemoveAt(0);
                            nint.directions.RemoveAt(0);
                            nint.directions.Add(node.nodeDir);
                            idxFile.writeNode(nint, w);
                            Root.nodeDir = w.BaseStream.Length;
                            selectedEnt.nodes.Add(nint);
                            idxFile.writeNode(Root, w);
                            selectedEnt.nodes.Add(Root);
                        }
                    }
                    else
                    {
                        Node Aux = rootNode;
                        node = new Node();
                        node.nodeDir = w.BaseStream.Length;
                        Aux.directions.Add(dir);
                        Aux.dataL.Add(clave);
                        sortNodes(Aux);
                        SplitNodeNinsertKeys(Aux, node);
                        sortNodes(node);
                        selectedEnt.nodes[indListNodo] = Aux;
                        selectedEnt.nodes.Add(node);
                        idxFile.writeNode(Aux, w);
                        idxFile.writeNode(node, w);
                        if (prevNode.dataL.Count < 4)
                        {
                            Node Intermedio = prevNode;
                            Intermedio.dataL.Add(node.dataL[0]);
                            Intermedio.directions.Add(node.nodeDir);
                            sortNodes(Intermedio);
                            for (int i = 0; i < selectedEnt.nodes.Count; i++)
                            {
                                if (selectedEnt.nodes[i].nodeDir == Intermedio.nodeDir)
                                    selectedEnt.nodes[i] = Intermedio;
                            }
                            idxFile.writeNode(Intermedio, w);
                            Intermedio = prevNode;
                        }
                        else
                        {
                            Node newInter = new Node();
                            newInter.nodeDir = w.BaseStream.Length;
                            newInter.type = 'I';

                            prevNode.dataL.Add(node.dataL[0]);
                            prevNode.directions.Add(node.nodeDir);
                            sortNodes(prevNode);
                            Root.dataL.Add(prevNode.dataL[2]);
                            SplitNodeNinsertKeys(prevNode, newInter);
                            sortNodes(newInter);
                            idxFile.writeNode(newInter, w);
                            idxFile.writeNode(Root, w);
                            idxFile.writeNode(prevNode, w);
                            for (int i = 0; i < selectedEnt.nodes.Count; i++)
                            {
                                if (selectedEnt.nodes[i].nodeDir == prevNode.nodeDir)
                                    selectedEnt.nodes[i] = prevNode;
                                if (selectedEnt.nodes[i].nodeDir == Root.nodeDir)
                                    selectedEnt.nodes[i] = Root;

                            }
                        }
                    }
                }
            }
            if (Root != null)
            {
                selectedEnt.attributes[indTree].indexDir = Root.nodeDir;
                ddFile.modifyAttribute(selectedEnt.attributes[indTree].attributeDir, selectedEnt.attributes[indTree]);
            }
        }
        //MÉTODO QUE DIVIDE UN NODO EN CASO DE QUE SE QUIERA INSERTAR UNA CLAVE Y ESTE YA ESTE LLENO
        private void SplitNodeNinsertKeys(Node Aux, Node nd)
        {
            int nEliminado = 0;
            int contE = 3;
            for (int i = 2; i < Aux.dataL.Count; i++)
            {
                nd.dataL.Add(Aux.dataL[i]);
                nd.directions.Add(Aux.directions[i]);
                nEliminado = i + 1;
            }

            if (nd.type == 'I')
            {
                contE = 2;
                nEliminado--;
                Aux.dataL.RemoveAt(nEliminado);
            }

            for (int nE = 0; nE < contE; nE++)
            {
                nEliminado--;
                Aux.dataL.RemoveAt(nEliminado);
                if (nd.type == 'I') nEliminado++;
                Aux.directions.RemoveAt(nEliminado);
                if (nd.type == 'I') nEliminado--;
            }
        }

        //CHECA EN QUE NODO VA A SER INSERTADO EL NUEVO DATO
        private Node whereToInsert(Node nd, int key)
        {
            Node aux = new Node();
            for (int r = 0; r < nd.dataL.Count; r++)
            {
                if (key > nd.dataL[r])
                {
                    for (int ni = 0; ni < selectedEnt.nodes.Count; ni++)
                    {
                        if (selectedEnt.nodes[ni].nodeDir == nd.directions[r + 1])
                        {
                            aux = selectedEnt.nodes[ni];
                            indListNodo = ni;
                        }
                    }
                }
                else
                {
                    for (int ni = 0; ni < selectedEnt.nodes.Count; ni++)
                    {
                        if (selectedEnt.nodes[ni].nodeDir == nd.directions[r])
                        {
                            aux = selectedEnt.nodes[ni];
                            indListNodo = ni;

                        }
                    }
                    r = nd.dataL.Count;
                }
            }
            return aux;
        }
        //MÉTODO DE ORDENACIÓN DE DATOS EN UN NODO
        private void sortNodes(Node n)
        {

            for (int oa = 0; oa < n.dataL.Count; oa++)
                for (int o = 0; o < n.dataL.Count - 1; o++)
                {
                    if (n.dataL[o] > n.dataL[o + 1])
                    {
                        if (n.type == 'H')
                        {
                            int menor = n.dataL[o + 1];
                            long apMenor = n.directions[o + 1];
                            n.dataL[o + 1] = n.dataL[o];
                            n.directions[o + 1] = n.directions[o];
                            n.dataL[o] = menor;
                            n.directions[o] = apMenor;
                        }
                        else
                        {
                            int menor = n.dataL[o + 1];
                            long apMenor = n.directions[o + 2];
                            n.dataL[o + 1] = n.dataL[o];
                            n.directions[o + 2] = n.directions[o + 1];
                            n.dataL[o] = menor;
                            n.directions[o + 1] = apMenor;
                        }
                    }
                }
        }
        //METODO PARA LLENAR EL DATAGRID DEL ARBOL B+
        private void fillDataGridTree()
        {
            DataRow r;
            dataTArbol.Clear();
            for (int i = 0; i < selectedEnt.nodes.Count; i++)
            {
                r = dataTArbol.NewRow();
                int j = 0, d = 0, a = 0;
                r[j] = selectedEnt.nodes[i].nodeDir;
                j = 1;
                r[j] = selectedEnt.nodes[i].type;
                j = 2;
                while (d < selectedEnt.nodes[i].dataL.Count || a < selectedEnt.nodes[i].directions.Count)
                {
                    if (j % 2 == 0)
                    {
                        r[j] = selectedEnt.nodes[i].directions[a];
                        a++;
                    }
                    else
                    {
                        r[j] = selectedEnt.nodes[i].dataL[d];
                        d++;
                    }
                    j++;
                }
                dataTArbol.Rows.Add(r);
            }

        }
        //MÉTODO QUE BUSCA QUE EL NODO HOJA EN QUE SE ENCUENTRA LA CLAVE QUE SE DESEA ELIMINAR
        private Node searchLeafNode(long Direccion)
        {
            Node nh = new Node();
            for (int Rr = 0; Rr < selectedEnt.nodes.Count; Rr++)//Recorrido de la raiz en busca del dato en las hojas
                if (selectedEnt.nodes[Rr].nodeDir == Direccion)
                    nh = selectedEnt.nodes[Rr];
            return nh;
        }
        //MÉTODO PARA ELIMINAR UNA CLAVE DEL ARBOL
        private void deleteKey(BinaryWriter w, int clave, long dir)
        {

            Node nE = new Node();
            Node nRight = null;
            Node nLeft = null;
            Node interRight = null;
            Node interLeft = null;
            Node prevNode = new Node();
            foreach (Node n in selectedEnt.nodes)
            {
                if (n.type == 'R')
                    Root = n;
            }
            if (Root == null)
            {
                nE = selectedEnt.nodes[0];
                for (int e = 0; e < nE.dataL.Count; e++)
                {
                    if (nE.dataL[e] == clave)
                    {
                        nE.dataL.RemoveAt(e);
                        nE.directions.RemoveAt(e);
                        idxFile.writeNode(nE, w);
                        selectedEnt.nodes[0] = nE;
                        if (nE.dataL.Count == 0)
                        {
                            selectedEnt.attributes[indTree].indexDir = -1;
                            ddFile.modifyAttribute(selectedEnt.attributes[indTree].attributeDir, selectedEnt.attributes[indTree]);
                        }
                    }
                }
            }
            else
            {
                goThroughNodes(ref nE, ref nRight, ref nLeft, clave, Root);
                if (nE.type != 'I')
                    prevNode = Root;
            }
            if (nE.type == 'I')
            {
                prevNode = nE;
                interRight = nRight;
                interLeft = nLeft;
                indNInterElim = indDeleteNode;
                goThroughNodes(ref nE, ref nRight, ref nLeft, clave, nE);
            }


            //ELIMNINACIÓN DE CLAVES
            if (Root != null)
            {
                for (int ln = 0; ln < nE.dataL.Count; ln++)
                    if (nE.dataL[ln] == clave)
                    {
                        nE.dataL.RemoveAt(ln);
                        nE.directions.RemoveAt(ln);
                        idxFile.writeNode(nE, w);
                        for (int c = 0; c < selectedEnt.nodes.Count; c++)
                            if (selectedEnt.nodes[c].nodeDir == nE.nodeDir)
                                selectedEnt.nodes[c] = nE;
                    }
                if (nE.dataL.Count < 2)//si quedó insuficiente 
                {

                    if (nLeft != null)//si tiene izquierdo
                    {
                        if (nLeft.dataL.Count > 2)
                            borrowKeyLeft(ref nE, ref nLeft, ref prevNode, ref interLeft, ref interRight, indDeleteNode, indNInterElim, w);
                        else
                            mergeNodeLeft(ref nE, ref nLeft, ref nRight, ref prevNode, ref interLeft, ref interRight, indDeleteNode, indNInterElim, w);
                    }
                    else//Derecho
                    {
                        if (nRight.dataL.Count > 2)
                            borrowKeyRight(ref nE, ref nRight, ref prevNode, ref interLeft, ref interRight, indDeleteNode, indNInterElim, w);
                        else//fusion con derecho
                            mergeNodeRight(ref nE, ref nRight, ref nLeft, ref prevNode, ref interLeft, ref interRight, indDeleteNode, indNInterElim, w);
                    }
                }

                if (Root.dataL.Count == 0)
                {
                    long posi = Root.directions[0];
                    for (int ln = 0; ln < selectedEnt.nodes.Count; ln++)
                        if (selectedEnt.nodes[ln].nodeDir == Root.nodeDir)
                            selectedEnt.nodes.RemoveAt(ln);
                    Root = null;
                }
            }
        }
        //MÉTODO PARA DONAR UN DATO SI EL NODO SE QUEDA  SIN CLAVES SUFICIENTES
        private void borrowKeyLeft(ref Node nE, ref Node nLeft, ref Node prevNode, ref Node interLeft, ref Node interRight, int indDelNode, int indDelInter, BinaryWriter w)
        {
            nE.dataL.Add(nLeft.dataL[nLeft.dataL.Count - 1]);
            nE.directions.Add(nLeft.directions[nLeft.dataL.Count - 1]);
            sortNodes(nE);
            nLeft.dataL.RemoveAt(nLeft.dataL.Count - 1);
            nLeft.directions.RemoveAt(nLeft.dataL.Count - 1);
            idxFile.writeNode(nLeft, w);
            prevNode.dataL[indDelNode] = nE.dataL[0];

            for (int c = 0; c < selectedEnt.nodes.Count; c++)
                if (selectedEnt.nodes[c].nodeDir == prevNode.nodeDir)
                    selectedEnt.nodes[c] = prevNode;

            idxFile.writeNode(prevNode, w);
            idxFile.writeNode(nE, w);

            for (int c = 0; c < selectedEnt.nodes.Count; c++)
                if (selectedEnt.nodes[c].nodeDir == nE.nodeDir)
                    selectedEnt.nodes[c] = nE;
        }
        //MÉTODO PARA DONAR UN DATO SI EL NODO SE QUEDA  SIN CLAVES SUFICIENTES
        private void borrowKeyRight(ref Node nE, ref Node nRight, ref Node prevNode, ref Node interLeft, ref Node interRight, int indDelNode, int indDelInter, BinaryWriter w)
        {
            nE.dataL.Add(nRight.dataL[0]);
            nE.directions.Add(nRight.directions[0]);
            nRight.dataL.RemoveAt(0);
            nRight.directions.RemoveAt(0);
            idxFile.writeNode(nRight, w);
            prevNode.dataL[indDelNode] = nRight.dataL[0];

            for (int c = 0; c < selectedEnt.nodes.Count; c++)
                if (selectedEnt.nodes[c].nodeDir == prevNode.nodeDir)
                    selectedEnt.nodes[c] = prevNode;

            idxFile.writeNode(prevNode, w);
            idxFile.writeNode(nE, w);

            for (int c = 0; c < selectedEnt.nodes.Count; c++)
                if (selectedEnt.nodes[c].nodeDir == nE.nodeDir)
                    selectedEnt.nodes[c] = nE;
        }
        //EN CASO DE QUE EL NODO NO PUEDA DONAR UNA CLAVE ESTE SE UNIRA CON EL NODO QUE ESTA PIDIENDO PRESTADO
        private void mergeNodeLeft(ref Node nE, ref Node nLeft, ref Node nRight, ref Node prevNode, ref Node interLeft, ref Node interRight, int indDelNode, int indDelInter, BinaryWriter w)
        {
            if (nE.type == 'I')
            {
                nLeft.dataL.Add(prevNode.dataL[indDelNode]);
                nLeft.directions.Add(nE.directions[0]);
                nLeft.dataL.Add(nE.dataL[0]);
                nLeft.directions.Add(nE.directions[1]);
            }
            if (nE.type == 'H')
            {
                nLeft.directions.Add(nE.directions[0]);
                nLeft.dataL.Add(nE.dataL[0]);
            }
            idxFile.writeNode(nLeft, w);
            int datEliminado = 0;
            prevNode.dataL.RemoveAt(indDelNode);
            prevNode.directions.RemoveAt(indDelNode + 1);
            idxFile.writeNode(prevNode, w);
            int ln = 0;
            for (; ln < selectedEnt.nodes.Count; ln++)
            {
                if (nLeft.nodeDir == selectedEnt.nodes[ln].nodeDir)
                    selectedEnt.nodes[ln] = nLeft;
                if (prevNode.nodeDir == selectedEnt.nodes[ln].nodeDir)
                    selectedEnt.nodes[ln] = prevNode;
                if (nE.nodeDir == selectedEnt.nodes[ln].nodeDir)
                    datEliminado = ln;
            }
            selectedEnt.nodes.RemoveAt(datEliminado);
            if (prevNode.type == 'I')
                deleteInter(ref prevNode, ref nRight, ref nLeft, ref prevNode, ref interLeft, ref interRight, indDelInter, w);
        }
        //EN CASO DE QUE EL NODO NO PUEDA DONAR UNA CLAVE ESTE SE UNIRA CON EL NODO QUE ESTA PIDIENDO PRESTADO
        private void mergeNodeRight(ref Node nE, ref Node nRight, ref Node nLeft, ref Node prevNode, ref Node interLeft, ref Node interRight, int indDelNode, int indDelInter, BinaryWriter w)
        {
            if (nE.type == 'I')
            {
                nE.dataL.Add(prevNode.dataL[indDelNode]);
                nE.directions.Add(nRight.directions[0]);
                for (int n = 0; n < nRight.dataL.Count; n++)
                {
                    nE.dataL.Add(nRight.dataL[n]);
                    nE.directions.Add(nRight.directions[n + 1]);
                }
            }
            if (nE.type == 'H')
            {
                for (int n = 0; n < nRight.dataL.Count; n++)
                {
                    nE.dataL.Add(nRight.dataL[n]);
                    nE.directions.Add(nRight.directions[n]);
                }
            }

            int keyDeleted = 0;
            prevNode.dataL.RemoveAt(indDelNode);
            prevNode.directions.RemoveAt(indDelNode + 1);
            for (int ln = 0; ln < selectedEnt.nodes.Count; ln++)
            {
                if (nE.nodeDir == selectedEnt.nodes[ln].nodeDir)
                    selectedEnt.nodes[ln] = nE;
                if (prevNode.nodeDir == selectedEnt.nodes[ln].nodeDir)
                    selectedEnt.nodes[ln] = prevNode;
                if (nRight.nodeDir == selectedEnt.nodes[ln].nodeDir)
                    keyDeleted = ln;
            }

            selectedEnt.nodes.RemoveAt(keyDeleted);

            if (prevNode.type == 'I')
                deleteInter(ref prevNode, ref nRight, ref nLeft, ref prevNode, ref interLeft, ref interRight, indDelInter, w);
        }
        //RECORRIDO DE LOS NODOS PARA ENCONTRAR EN DONDE SE ENCUENTRA LA CLAVE QUE SE QUIERE ELIMINAR
        private void goThroughNodes(ref Node nE, ref Node nRight, ref Node nLeft, int key, Node nD)
        {
            for (int rn = 0; rn < nD.dataL.Count; rn++)//Recorrido de la raiz para saber en que nodo se encuentra el dato que se quiere eliminar
            {
                if (key < nD.dataL[rn])
                {
                    for (int ln = 0; ln < selectedEnt.nodes.Count; ln++)
                    {
                        if (nD.directions[rn] == selectedEnt.nodes[ln].nodeDir)
                        {
                            nE = searchLeafNode(nD.directions[rn]);
                            indDeleteNode = rn;
                        }
                        if (rn < nD.directions.Count - 1)
                        {
                            if (selectedEnt.nodes[ln].nodeDir == nD.directions[rn + 1])
                                nRight = searchLeafNode(nD.directions[rn + 1]);
                            nLeft = null;
                        }
                        if (rn == nD.directions.Count - 1)
                        {
                            if (selectedEnt.nodes[ln].nodeDir == nD.directions[rn - 1])
                                nLeft = searchLeafNode(nD.directions[rn - 1]);
                            //indDeleteNode = rn - 1;
                        }
                    }
                    rn = selectedEnt.nodes.Count;
                }
                else
                {
                    for (int ln = 0; ln < selectedEnt.nodes.Count; ln++)
                    {
                        if (nD.directions[rn + 1] == selectedEnt.nodes[ln].nodeDir)
                        {
                            nE = searchLeafNode(nD.directions[rn + 1]);
                            indDeleteNode = rn;
                        }
                        if ((rn + 1) < nD.directions.Count - 1)
                        {
                            if (selectedEnt.nodes[ln].nodeDir == nD.directions[rn + 2])
                                nRight = searchLeafNode(nD.directions[rn + 2]);
                            // indDeleteNode = rn + 1;
                        }

                        if ((rn + 1) == nD.directions.Count - 1)
                        {
                            if (selectedEnt.nodes[ln].nodeDir == nD.directions[rn])
                                nLeft = searchLeafNode(nD.directions[rn]);
                            nRight = null;
                        }
                    }
                }
            }
        }
        //MÉTODO PARA RECORRER Y ELIMINAR INTERMEDIOS EN CASO DE QUE EXISTAN
        private void deleteInter(ref Node nE, ref Node nRight, ref Node nLeft, ref Node prevNode, ref Node interLeft, ref Node interRight, int indDelInter, BinaryWriter w)
        {
            if (nE.nodeDir != Root.nodeDir)
            {
                if (nE.dataL.Count < 2)
                {
                    if (interLeft != null)
                    {
                        if (interLeft.dataL.Count > 2)
                            borrowKeyLeft(ref nE, ref interLeft, ref prevNode, ref nRight, ref interRight, indDelInter, 0, w);
                        else
                            mergeNodeLeft(ref nE, ref interLeft, ref nRight, ref Root, ref nRight, ref interRight, indDelInter, 0, w);

                    }
                    else
                    {
                        if (interRight.dataL.Count > 2)
                            borrowKeyRight(ref nE, ref interRight, ref prevNode, ref nLeft, ref interLeft, indDelInter, 0, w);
                        else
                            mergeNodeRight(ref nE, ref interRight, ref nLeft, ref Root, ref nLeft, ref interLeft, indDelInter, 0, w);
                    }
                }
            }
            if (Root.dataL.Count == 0)
            {
                int p = 0; long posi = Root.directions[0];
                for (int ln = 0; ln < selectedEnt.nodes.Count; ln++)
                    if (selectedEnt.nodes[ln].nodeDir == Root.nodeDir)
                        selectedEnt.nodes.RemoveAt(ln);
                for (int ln = 0; ln < selectedEnt.nodes.Count; ln++)
                    if (selectedEnt.nodes[ln].nodeDir == posi)
                    {
                        selectedEnt.attributes[indTree].indexDir = selectedEnt.nodes[ln].nodeDir;
                        if (selectedEnt.nodes[ln].type == 'I')
                        {
                            selectedEnt.nodes[ln].type = 'R';
                            idxFile.writeNode(selectedEnt.nodes[ln], w);
                        }
                        ddFile.modifyAttribute(selectedEnt.attributes[indTree].attributeDir, selectedEnt.attributes[indTree]);
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
                int num = -1;
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
                                sortPK();
                                idxFile.modifyPK(selectedEnt, indx, write);
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
                                            if (sortedFKlist[f].directions.Count > 0)
                                            {
                                                for (int d = 0; d < sortedFKlist[f].directions.Count; d++)
                                                {
                                                    if (sortedFKlist[f].directions[d] == selectedEnt.data[ind].dataDir)
                                                    {
                                                        for (int ifk = 0; ifk < sortedFKlist.Count; ifk++)
                                                        {

                                                            //if (selectedEnt.data[ind].number[indx].ToString() == sortedFKlist[ifk].oClave.ToString())
                                                            if (newData.number[indFk].ToString() == sortedFKlist[ifk].oClave.ToString())
                                                            {
                                                                sortedFKlist[ifk].directions.Add(sortedFKlist[f].directions[d]);
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
                                                                fk.directions.Add(selectedEnt.data[indice].lDirRegistro);
                                                            }*/
                                                            //else
                                                            
                                                                fk = new ForeignKey();
                                                                fk.oClave = Convert.ToInt32(newData.number[indFk]);
                                                                fk.directions.Add(selectedEnt.data[ind].dataDir);
                                                            
                                                            sortedFKlist.Add(fk);
                                                            sortedFKlist[f].directions.RemoveAt(d);
                                                            if (sortedFKlist[f].directions.Count == 0)
                                                                sortedFKlist.RemoveAt(f);
                                                        }
                                                        else
                                                        {
                                                            sortedFKlist[f].directions.RemoveAt(d);
                                                            if (sortedFKlist[f].directions.Count == 0)
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
                                            if (sortedFKlist[f].directions.Count > 0)
                                            {
                                                for (int d = 0; d < sortedFKlist[f].directions.Count; d++)
                                                {
                                                    if (sortedFKlist[f].directions[d] == selectedEnt.data[ind].dataDir)
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
                                                                sortedFKlist[ifk].directions.Add(sortedFKlist[f].directions[d]);
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
                                                                fk.directions.Add(selectedEnt.data[indice].lDirRegistro);
                                                            }*/
                                                            //else
                                                            {
                                                                fk = new ForeignKey();
                                                                if (isPk)
                                                                    fk.oClave = new string(newData.str[indFk - 1]);
                                                                else
                                                                    fk.oClave = new string(newData.str[indFk]);
                                                                fk.directions.Add(selectedEnt.data[ind].dataDir);
                                                            }
                                                            sortedFKlist.Add(fk);
                                                            sortedFKlist[f].directions.RemoveAt(d);
                                                            if (sortedFKlist[f].directions.Count == 0)
                                                                sortedFKlist.RemoveAt(f);
                                                        }
                                                        else
                                                        {
                                                            sortedFKlist[f].directions.RemoveAt(d);
                                                            if (sortedFKlist[f].directions.Count == 0)
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
                                    fk.directions.Add(data.dataDir);
                                    sortedFKlist.Add(fk);
                                    sortedFKlist = sortedFKlist.OrderBy(lof => lof.oClave).ToList();
                                    sortFK();
                                    idxFile.modifyFK(selectedEnt, indFk, write);

                                }
                                else
                                {
                                    sortFK();
                                    idxFile.modifyFK(selectedEnt, indFk, write);
                                }*/
                                sortedFKlist = sortedFKlist.OrderBy(lof => lof.oClave).ToList();
                                sortFK();
                                fillDataFK();
                                idxFile.modifyFK(selectedEnt, indFk, write);
                            }
                            idxFile.stream.Close();
                            write.Close();

                        }
                        else if(att.indexType == 4)
                        {
                            if (idxFile.stream != null)
                                idxFile.stream.Close();

                            if (isPk && !isFk)
                                idxFile.stream.Position = advance;
                            else if (isPk && isFk)
                                idxFile.stream.Position = advance + advanceFK;
                            else if (isFk && !isPk)
                                idxFile.stream.Position = advanceFK;

                            idxFile.stream = new FileStream(idxFileName, FileMode.Open);
                            BinaryWriter write = new BinaryWriter(idxFile.stream, Encoding.UTF8);

                            if (isTree)
                            {
                                deleteKey(write, Convert.ToInt32(selectedEnt.data[ind].number[indTree]), selectedEnt.data[ind].dataDir);
                                insertKey(write, Convert.ToInt32(newData.number[indTree]), newData.dataDir);
                                fillDataGridTree();
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
                sortPK();
                llenaDataPk();
                idxFile.modifyPK(selectedEnt, ind, writerIdx);
            }
            if (isFk)
            {
                if (selectedEnt.fk.Count > 0)
                {
                    for (int f = 0; f < sortedFKlist.Count; f++)
                    {
                        for (int fd = 0; fd < sortedFKlist[f].directions.Count; fd++)
                        {
                            if (sortedFKlist[f].directions[fd] == selectedEnt.data[indice].dataDir)
                            {
                                sortedFKlist[f].directions.RemoveAt(fd);
                                if (sortedFKlist[f].directions.Count == 0)
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
                sortFK();
                fillDataFK();
                idxFile.modifyFK(selectedEnt, indFk, writerIdx);
            }

            if (isTree)
            {
                deleteKey(writerIdx, Convert.ToInt32(selectedEnt.data[indice].number[indTree]), selectedEnt.data[indice].dataDir);
                fillDataGridTree();
            }

            selectedEnt.data.RemoveAt(indice);
            verifySearchKey();
            changeDirections();

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


            showData(selectedEnt.data);
        }

        private void changeDirections()
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

        private void verifySearchKey()
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
                            sortPK();
                            idxFile.modifyPK(selectedEnt, ind, write);
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
                            if (!searchFK(selectedEnt.attributes[indFk].type))
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
                                fk.directions.Add(data.dataDir);
                                sortedFKlist.Add(fk);
                                sortedFKlist = sortedFKlist.OrderBy(lof => lof.oClave).ToList();
                                sortFK();
                                idxFile.modifyFK(selectedEnt, indFk, write);

                            }
                            else
                            {
                                sortFK();
                                idxFile.modifyFK(selectedEnt, indFk, write);
                            }
                            fillDataFK();
                        }
                        idxFile.stream.Close();
                        write.Close();
                    }
                    else if(att.indexType == 4)
                    {
                        if (idxFile.stream != null)
                            idxFile.stream.Close();
                        idxFile.stream = new FileStream(idxFileName, FileMode.Open);

                        if (isPk && !isFk)
                            idxFile.stream.Position = advance;
                        else if (isPk && isFk)
                            idxFile.stream.Position = advance + advanceFK;
                        else if (isFk && !isPk)
                            idxFile.stream.Position = advanceFK;

                        BinaryWriter write = new BinaryWriter(idxFile.stream, Encoding.UTF8);

                        if (isTree)
                        {
                            insertKey(write, Convert.ToInt32(data.number[indTree]), data.dataDir);
                            fillDataGridTree();
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
