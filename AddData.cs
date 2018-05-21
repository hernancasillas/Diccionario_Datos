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

        AuxFile file, ddFile, idxFile;
        static long head = -1, pos = head;
        bool newFile, isPk, isFk, isTree;
        string fileName, ddFileName, idxFileName, dir;
        int ind, ip = 0, indFk, fki = 0, indFound, advance;

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
                    advance = (attribute.length + 8) * 10;

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
                            copiaListaOrdenadaPk();
                            copiaListaOrdenadaFk();
                            verificaPk(writer);
                            verificaFk(writer);
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

                                        if(atrib.indexDir == -1 && atrib.indexType != 0 && atrib.indexType != 1)
                                        {
                                            BinaryWriter writer = createIndexFile();
                                            //openIndexFile();
                                            copiaListaOrdenadaPk();
                                            copiaListaOrdenadaFk();
                                            verificaPk(writer);
                                            verificaFk(writer);
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
                                                for (int p = 0; p < 10; p++)
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
                                            copiaListaOrdenadaPk();
                                            copiaListaOrdenadaFk();
                                            verificaPk(write);
                                            if(isFk)
                                            verificaFk(write);
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
            for (ip = 0; ip < 10; ip++)
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
                        ddFile.modifyAttribute(posdd, selectedEnt.attributes[i]);
                        w.BaseStream.Seek(pos, SeekOrigin.Begin);
                        ind = i;
                        for (int c = 0; c < 10; c++)
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
                for (ip = 0; ip < 10; ip++)
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
                        if (isPk && type == 'C')
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

        private void buttonAdd_Click(object sender, EventArgs e)
        {

        }

       /* private void buttonNew_Click(object sender, EventArgs e)
        {
            SaveFileDialog newFile = new SaveFileDialog();//Ubicacion para guardar el archivo

            newFile.Title = "New DAT File";
            newFile.DefaultExt = "dat";
            newFile.Filter = "Dat Files (*.dat)| *.dat";
            newFile.AddExtension = true;

            DialogResult dr = newFile.ShowDialog();

            if (dr == DialogResult.OK)
            {
                //File.WriteAllText(newFile.FileName, );//Guarda el archivo sobrescribiendolo en la direccion
                file.stream = new FileStream(newFile.FileName, FileMode.Create);
                head = -1;
                dataGridView1.Rows.Clear();
                entities = new List<Entity>();
                //tbNomb.Text = sfd.FileName;
                fileName = newFile.FileName;
                file.stream.Close();
                //comboBoxAtt.Enabled = true;
                comboBoxEnt.Enabled = true;
            }
        }*/
        
        /*
        private void buttonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();

            DialogResult dr = openFile.ShowDialog();
            if (dr == DialogResult.OK)
            {
                fileName = openFile.FileName;
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
                                }
                                dat.nextDir = reader.ReadInt64();//Lee la direccion del registro siguiente

                                selectedEnt.data.Add(dat);
                                while (dat.nextDir != -1)//Mientras ternga registros
                                {
                                    Data aux = new Data();

                                    file.stream.Position = dat.nextDir;

                                    aux.dataDir = reader.ReadInt64();//Lee la direccion del registro actual
                                    foreach (Attribute atrib in selectedEnt.attributes)
                                    {
                                        if (atrib.type == 'I')
                                            aux.number.Add(reader.ReadInt32());//Añade a la lista de enteros de los registros
                                        else
                                            aux.str.Add(reader.ReadChars(atrib.length));//Añade a la lista de cadenas de los registros
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
                showData(selectedEnt.data);//Muestra los datos de la entidad en el data grid
                reader.Close();//Cierra la lectura actual
                file.stream.Close();//Cierra la secuencia actual
                statusStrip1.Text = "File " + fileName + " opened.";
            }
        }*/
        
        private void buttonModify_Click(object sender, EventArgs e)
        {
            DialogResult addDialog;
            Data ant = new Data();
            Data newData = new Data();
            long ap = Convert.ToInt64(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value);
            bool b = false;
            Attribute at = new Attribute();
            int pc = 0, pe = 0, ee = 0, ce = 0;

            int i = 0, ind = 0;
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
                            /*if (selectedEnt.pk.Count > 0)
                            {
                                for (int p = 0; p < sordedPKlist.Count; p++)
                                {
                                    if (sordedPKlist[p].oClave.ToString() == selectedEnt.data[indice].Lregistros[ind].ToString())
                                    {
                                        if (selectedEnt.attributes[ind].type == 'C')
                                        {
                                            sordedPKlist[p].oClave = DataLLenaRegistros[ind, 0].Value.ToString();
                                        }
                                        else
                                        {
                                            sordedPKlist[p].oClave = Convert.ToInt32(DataLLenaRegistros[ind, 0].Value);
                                        }
                                    }
                                }

                                sordedPKlist = sordedPKlist.OrderBy(lof => lof.oClave).ToList();
                                ordenaPk();
                                llenaDataPk();
                                idxFile.modificaPk(selectedEnt, ind);
                            }*/
                            /*if (att.type == 'I')
                            {
                                num = Convert.ToInt32(add.DataText);
                                newData.number.Add(num);
                            }
                            else
                                newData.str.Add(stringToCharArray(add.DataText, att.length));*/
                        }

                    }
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
            IndexView indexView = new IndexView(selectedEnt, dataTFk);
            indexView.ShowDialog();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
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
                MessageBox.Show("NO Data to Delete");
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
            idxFile.stream = new FileStream(idxFileName, FileMode.Open);
            idxFile.stream.Position = 0;
        }

        private BinaryWriter createIndexFile()
        {
            idxFileName = dir + "\\" + name + ".idx";
            idxFile = new AuxFile();
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
                    if(selectedEnt.attributes.Count > 1)
                    selectedEnt.data.Add(data);
                    file.stream.Position = data.dataDir;

                    if (att.indexType == 1)
                    {
                        if (new string(att.name) == c)
                        { at = att; pc = ce; pe = ee; }

                        if (att.type == 'C')
                            ce++;
                        else
                            ee++;

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
                                    if(isPk)
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
                            }
                            llenaDataFk();
                        }
                        idxFile.stream.Close();
                        write.Close();
                    }
                    
                    

                }
            }

            /*if (at.type == 'C')
                Ordena(selectedEnt.data, pc, 0);
            else
                Ordena(selectedEnt.data, pe, 1);

            showData(selectedEnt.data);*/

            

            
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
                            dataGridView1.Rows[ind].Cells[i].Value = re.number[j++];
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
