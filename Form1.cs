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
    public partial class Form1 : Form
    {
        AuxFile file, datFile;
        string name, entName;
        static public List<Entity> entities = new List<Entity>();
        bool repeated, modify, modifyAtt;
        Entity selectedEntity;
        int selectedEntityIndex;

        public Form1()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            long head = -1;
            string name;

            SaveFileDialog newFile = new SaveFileDialog();
            newFile.Title = "New File";
            newFile.DefaultExt = "bin";
            newFile.Filter = "Bin Files (*.bin)| *.bin";
            newFile.AddExtension = true;

            if (newFile.ShowDialog() == DialogResult.OK)
            {
                toolStripStatusLabel1.Text = "File created at: " + newFile.FileName;

                file = new AuxFile();
                name = newFile.FileName;
                file.fileName = name;
                file.stream = new FileStream(name, FileMode.Create);

                if (file.stream != null)
                {
                    BinaryWriter binaryWriter = new BinaryWriter(file.stream);
                    binaryWriter.Write(head);
                    binaryWriter.Close();
                }
                else
                    MessageBox.Show("The file cannot be created.");
                
            }


        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Bin Files (*.bin)| *.bin";

            if (open.ShowDialog() == DialogResult.OK)
            {
                file = new AuxFile();

                if (file.stream != null)
                    file.stream.Close();

                name = open.FileName;
                file.fileName = name;

                file.stream = new FileStream(name, FileMode.Open);

                BinaryReader reader = new BinaryReader(file.stream, Encoding.UTF8);

                if (file.stream.Length != 0)//Mientras no sea fin de archivo
                {
                    file.stream.Position = 0;
                    long head = reader.ReadInt64();
                    long pos = head;

                    if (head != -1)//Si hay entidades
                    {
                        pos = head;//Declarar un auxiliar
                        while (pos != -1)//Mientras exista minimo una entidad
                        {
                            file.stream.Position = pos;//Asignar la cabecera a la posicion del archivo
                            Entity newEntity = new Entity(); //Carga entidad

                            newEntity.name = reader.ReadChars(30);
                            newEntity.entityDir = reader.ReadInt64();
                            newEntity.attributeDir = reader.ReadInt64();
                            newEntity.dataDir = reader.ReadInt64();
                            newEntity.nextDir = reader.ReadInt64();

                            string name = new string(newEntity.name);

                            comboBox1.Items.Add(name);//Agrega la entidad creada al combobox para visualizar

                            if (newEntity.attributeDir != -1)//si existe atributos en la entidad
                            {
                                Attribute newAtt = new Attribute(); //= CapturAt(nuevaEnt.DirAtrib);//Carga atributo

                                file.stream.Position = newEntity.attributeDir;

                                newAtt.name = reader.ReadChars(30);
                                newAtt.type = reader.ReadChar();
                                newAtt.length = reader.ReadInt32();
                                newAtt.attributeDir = reader.ReadInt64();
                                newAtt.indexType = reader.ReadInt32();
                                newAtt.indexDir = reader.ReadInt64();
                                newAtt.nextAttDir = reader.ReadInt64();

                                newEntity.attributes.Add(newAtt);

                                /*if (newAtt.indexType != -1)
                                {

                                    R.BaseStream.Seek(newAtt.indexDir, SeekOrigin.Begin);
                                    if (newAtt.iIndice == 2)
                                    {
                                        for (int p = 0; p < 10; p++)
                                        {
                                            Mpk = new Pk();
                                            if (newAtt.cTipo == 'C')
                                            {
                                                auxPk = string.Empty;
                                                NomPk = R.ReadChars(newAtt.iLongitud);
                                                for (int i = 0; i < NomPk.Count(); i++)
                                                {
                                                    auxPk += NomPk[i];
                                                }
                                                Mpk.oClave = auxPk;
                                                Mpk.lDireccion = R.ReadInt64();
                                                ent.ListPk.Add(Mpk);
                                            }
                                            else
                                            {
                                                auxPk = string.Empty;
                                                Mpk.oClave = R.ReadInt32();
                                                Mpk.lDireccion = R.ReadInt64();
                                                ent.ListPk.Add(Mpk);
                                            }
                                        }
                                    }

                                    if (newAtt.iIndice == 3)
                                    {
                                        for (int f = 0; f < 5; f++)
                                        {
                                            Mfk = new Fk();
                                            if (newAtt.cTipo == 'C')
                                            {
                                                auxFk = string.Empty;
                                                NomFk = R.ReadChars(newAtt.iLongitud);
                                                for (int i = 0; i < NomFk.Count(); i++)
                                                {
                                                    auxFk += NomFk[i];
                                                }
                                                Mfk.oClave = auxFk;
                                                for (int d = 0; d < 10; d++)
                                                {
                                                    Mfk.lDirecciones.Add(R.ReadInt64());
                                                }
                                                ent.ListFk.Add(Mfk);

                                            }
                                            else
                                            {
                                                Mfk.oClave = R.ReadInt32();
                                                for (int d = 0; d < 10; d++)
                                                {
                                                    Mfk.lDirecciones.Add(R.ReadInt64());
                                                }
                                                ent.ListFk.Add(Mfk);
                                            }
                                        }
                                    }
                                    if (newAtt.iIndice == 4)
                                    {
                                        Nodo n;
                                        Nodo nuevo = LeeNodo(newAtt.lDirIndice);
                                        ent.listNodos.Add(nuevo);
                                        if (nuevo.cTipo == 'R')
                                        {
                                            for (int i = 0; i < nuevo.iDatos.Count + 1; i++)
                                            {
                                                n = LeeNodo(nuevo.lDirecciones[i]);
                                                ent.listNodos.Add(n);
                                                if (n.cTipo == 'I')
                                                {
                                                    for (int j = 0; j < n.lDirecciones.Count; j++)
                                                    {
                                                        Nodo nh = LeeNodo(n.lDirecciones[j]);
                                                        ent.listNodos.Add(nh);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                /*if (newAtt.lDirSigAtributo != -1)
                                {
                                    R.BaseStream.Seek(newAtt.lDirSigAtributo, SeekOrigin.Begin);
                                }*/

                                while (newAtt.nextAttDir != -1) // Mientras exista otro atributo
                                {
                                    Attribute aux = new Attribute();

                                    file.stream.Position = newAtt.nextAttDir;

                                    aux.name = reader.ReadChars(30);
                                    aux.type = reader.ReadChar();
                                    aux.length = reader.ReadInt32();
                                    aux.attributeDir = reader.ReadInt64();
                                    aux.indexType = reader.ReadInt32();
                                    aux.indexDir = reader.ReadInt64();
                                    aux.nextAttDir = reader.ReadInt64();


                                    newAtt = aux;

                                    newEntity.attributes.Add(newAtt);//Agrega el siguiente atributo
                                }

                                
                            }


                            entities.Add(newEntity); //Agrega la entidad a memoria
                            addDataAttribute(newEntity);
                            pos = newEntity.nextDir;//Asigna a pos la dirreccion de la siguiente entidad
                        }
                    }
                    else
                        MessageBox.Show("ARCHIVO VACIO");
                }
                else
                    MessageBox.Show("ARCHIVO VACIO");

                addData();//Muestra los datos de la entidad en el data grid
                
                reader.Close();//Cierra la lectura actual
                file.stream.Close();//Cierra la secuencia actual

                toolStripStatusLabel1.Text = "File " + name + " opened.";

            }
        }

        #region Entity

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (file != null)
            {
                //if (!modify)
                {
                    DialogResult addEDialog;
                    AddEntity addEntity = new AddEntity();

                    if(modify)
                    {
                        addEntity.Text = "Modify Entity";
                        addEntity.AddButton.Text = "Modify";
                    }
                    addEDialog = addEntity.ShowDialog();

                    if (addEntity.name == "")
                        MessageBox.Show("The entity must have a name");

                    if (addEDialog == DialogResult.OK)
                    {
                        repeatedEntity(addEntity.name);

                        if (!modify && !repeated)
                        {
                            long head = file.getHead();

                            Entity newEnt = new Entity();

                            newEnt.name = stringToCharArray(addEntity.name);
                            newEnt.attributeDir = -1;
                            newEnt.entityDir = file.getLenght();
                            newEnt.dataDir = -1;
                            newEnt.nextDir = -1;

                            if (head == -1)
                            {
                                file.modifyHead(newEnt.entityDir);
                                entities.Add(newEnt);
                                file.addEntity(newEnt);
                            }
                            else
                            {
                                entities.Add(newEnt);
                                alphabeticalSort();
                                updateIndex();
                            }

                        }
                        else
                        {
                            if (!repeated)
                            {
                                entities[dataGridView1.CurrentCell.RowIndex].name = stringToCharArray(addEntity.name);
                                file.modifyEntity(entities[dataGridView1.CurrentCell.RowIndex].entityDir, entities[dataGridView1.CurrentCell.RowIndex]);
                                alphabeticalSort();
                                updateIndex();
                                modify = false;
                            }
                        }

                        addData();
                        toolStripStatusLabel2.Text = " || Entity " + addEntity.name + " added";
                        comboBox1.Items.Add(addEntity.name);

                    }
                }
            }
            else
                MessageBox.Show("There is no file, create one.");

        }

        private void modifyEntToolStripMenuItem_Click(object sender, EventArgs e)
        {
                modify = true;
                addToolStripMenuItem_Click(sender, e);   
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.RowIndex != 0 && dataGridView1.CurrentCell.RowIndex != entities.Count - 1)
            {
                entities[dataGridView1.CurrentCell.RowIndex - 1].nextDir = entities[dataGridView1.CurrentCell.RowIndex + 1].entityDir;
                file.modifyEntity(entities[dataGridView1.CurrentCell.RowIndex - 1].entityDir, entities[dataGridView1.CurrentCell.RowIndex - 1]);
                file.modifyEntity(entities[dataGridView1.CurrentCell.RowIndex + 1].entityDir, entities[dataGridView1.CurrentCell.RowIndex + 1]);
            }
            else
            {
                if (dataGridView1.CurrentCell.RowIndex != 0 && dataGridView1.CurrentCell.RowIndex == entities.Count - 1)
                {
                    entities[dataGridView1.CurrentCell.RowIndex - 1].nextDir = -1;
                    file.modifyEntity(entities[dataGridView1.CurrentCell.RowIndex - 1].entityDir, entities[dataGridView1.CurrentCell.RowIndex - 1]);
                }
                else
                {
                    if (entities.Count != 1)
                        file.modifyHead(entities[dataGridView1.CurrentCell.RowIndex + 1].entityDir);
                }

            }
            entities.RemoveAt(dataGridView1.CurrentCell.RowIndex);
            addData();
        }

        public void repeatedEntity(string name)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                var cadena = new string(stringToCharArray(name));
                string cadena2 = new string(entities[i].name);
                if (cadena2 == cadena)
                {
                    repeated = true;
                    MessageBox.Show("That entity already exists.");
                }

            }
        }

        #endregion

        #region File Management Methods

        private void addData()
        {
            dataGridView1.Rows.Clear();

            if (entities.Count != 0)
                dataGridView1.RowCount = entities.Count;

            for (int i = 0; i < entities.Count; i++)
            {
                string dato = new string(entities[i].name);

                dataGridView1.Rows[i].Cells[0].Value = dato;
                dataGridView1.Rows[i].Cells[1].Value = entities[i].entityDir;
                dataGridView1.Rows[i].Cells[2].Value = entities[i].attributeDir;
                dataGridView1.Rows[i].Cells[3].Value = entities[i].dataDir;
                dataGridView1.Rows[i].Cells[4].Value = entities[i].nextDir;
            }
        }

        private void addDataAttribute(Entity entity)
        {
            dataGridView2.Rows.Clear();

            if (entity.attributes.Count != 0)
                dataGridView2.RowCount = entity.attributes.Count;

            if(entities.Count != 0)
                foreach(Entity n in entities)
                {
                    string entName = new string(n.name);

                    for(int i = 0; i < n.attributes.Count; i++)
                    {
                        string attName = new string(n.attributes[i].name);

                        dataGridView2.Rows[i].Cells[0].Value = entName;
                        dataGridView2.Rows[i].Cells[1].Value = attName;
                        dataGridView2.Rows[i].Cells[2].Value = n.attributes[i].type;
                        dataGridView2.Rows[i].Cells[3].Value = n.attributes[i].length;
                        dataGridView2.Rows[i].Cells[4].Value = n.attributes[i].indexType;
                        dataGridView2.Rows[i].Cells[5].Value = n.attributes[i].attributeDir;
                        dataGridView2.Rows[i].Cells[6].Value = n.attributes[i].indexDir;
                        dataGridView2.Rows[i].Cells[7].Value = n.attributes[i].nextAttDir;
                        
                    }
                }


        }

        private void alphabeticalSort()
        {
            List<String> listStrings = new List<string>();
            List<Entity> listEntities = new List<Entity>();

            foreach (Entity ent in entities)
                listStrings.Add(new String(ent.name));
            listStrings.Sort();

            foreach (String s in listStrings)
            {
                foreach (Entity e in entities)
                {
                    if (String.Compare(s, new string(e.name)) == 0)
                    {
                        listEntities.Add(e);
                    }
                }
            }

            entities.Clear();
            entities = listEntities;
        }

        private void updateIndex()
        {
            for (int i = 0; i < entities.Count - 1; i++)
                entities[i].nextDir = entities[i + 1].entityDir;

            for (int i = 0; i < entities.Count; i++)
                file.modifyEntity(entities[i].entityDir, entities[i]);

            file.modifyHead(entities[0].entityDir);
        }

        private void updateAttIndex()
        {
            foreach (Entity ent in entities)
                for (int i = 0; i < ent.attributes.Count - 1; i++)
                    ent.attributes[i].nextAttDir = ent.attributes[i + 1].attributeDir;

            foreach (Entity ent in entities)
                for (int i = 0; i < ent.attributes.Count; i++)
                    file.modifyAttribute(ent.attributes[i].attributeDir, ent.attributes[i]);
        }

        private char[] stringToCharArray(string cad)
        {
            char[] str = new char[30];
            for (int i = 0; i < cad.Count(); i++)
                str[i] = cad[i];
            return str;
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(file != null)
            {
                if (file.viewEntities())
                    addData();
            }
            
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            addData();
            addDataAttribute(selectedEntity);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if(i == comboBox1.SelectedIndex)
                {
                    selectedEntity = entities[i];
                }
            }
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.RowIndex != 0 && dataGridView1.CurrentCell.RowIndex != entities[selectedEntityIndex].attributes.Count - 1)
            {
                entities[selectedEntityIndex].attributes[dataGridView1.CurrentCell.RowIndex - 1].nextAttDir = entities[selectedEntityIndex].attributes[dataGridView1.CurrentCell.RowIndex + 1].attributeDir;
                file.modifyAttribute(entities[selectedEntityIndex].attributes[dataGridView1.CurrentCell.RowIndex - 1].attributeDir, entities[selectedEntityIndex].attributes[dataGridView1.CurrentCell.RowIndex - 1]);
                file.modifyAttribute(entities[selectedEntityIndex].attributes[dataGridView1.CurrentCell.RowIndex + 1].attributeDir, entities[selectedEntityIndex].attributes[dataGridView1.CurrentCell.RowIndex + 1]);
            }
            else
            {
                if (dataGridView1.CurrentCell.RowIndex != 0 && dataGridView1.CurrentCell.RowIndex == entities[selectedEntityIndex].attributes.Count - 1)
                {
                    entities[selectedEntityIndex].attributes[dataGridView1.CurrentCell.RowIndex - 1].nextAttDir = -1;
                    file.modifyAttribute(entities[selectedEntityIndex].attributes[dataGridView1.CurrentCell.RowIndex - 1].attributeDir, entities[selectedEntityIndex].attributes[dataGridView1.CurrentCell.RowIndex - 1]);
                }
                else
                {
                    if (entities[selectedEntityIndex].attributes.Count != 1)
                    {
                        file.modifyAttributePointer(entities[selectedEntityIndex].entityDir, entities[selectedEntityIndex], entities[selectedEntityIndex].attributes[dataGridView1.CurrentCell.RowIndex + 1].attributeDir);
                    }
                }
                entities[selectedEntityIndex].attributes.RemoveAt(dataGridView1.CurrentCell.RowIndex);
                addDataAttribute(selectedEntity);
            }
        }

        

        private void modifyAttToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            modifyAtt = true;
            addAttToolStripMenuItem1_Click(sender, e);
        }

        #endregion

        #region Attribute

        private void addAttToolStripMenuItem1_Click(object sender, EventArgs e)
        {


            if (file != null)
            {
                //if (!modify)
                {
                    DialogResult addAttDialog;
                    AddAttribute addAttribute = new AddAttribute(entities);

                    if (modifyAtt)// && dataGridView2.CurrentCell.RowIndex != null)
                    {
                        addAttribute.Text = "Modify Attribute";
                        addAttribute.AddButton.Text = "Modify";
                        //addAttribute.nameTextBox.Text = entities[addAttribute.comboBox1.SelectedIndex].attributes[dataGridView2.CurrentCell.RowIndex].name.ToString();
                    }

                    addAttDialog = addAttribute.ShowDialog();

                    /*if (addAttribute.name == "")
                        MessageBox.Show("The attribute must have a name");*/

                    if (addAttDialog == DialogResult.OK)
                    {

                        if (!modifyAtt && !repeated)
                        {
                            long head = file.getHead();

                            Attribute newAtt = new Attribute(addAttribute.type, addAttribute.lenght, addAttribute.indexType);

                            newAtt.name = stringToCharArray(addAttribute.name);
                            newAtt.attributeDir = file.getLenght();
                            newAtt.indexDir = -1;
                            newAtt.nextAttDir = -1;

                            entities[addAttribute.comboBox1.SelectedIndex].attributes.Add(newAtt);
                            selectedEntity = entities[addAttribute.comboBox1.SelectedIndex];
                            selectedEntityIndex = addAttribute.comboBox1.SelectedIndex;
                            file.addAtribute(newAtt);
                            updateAttIndex();
                            entities[addAttribute.comboBox1.SelectedIndex].attributeDir = entities[addAttribute.comboBox1.SelectedIndex].attributes[0].attributeDir;
                            updateIndex();
                            addData();
                            addDataAttribute(selectedEntity);
                        }
                        else
                        {
                            if(modifyAtt)
                            {

                                entities[addAttribute.comboBox1.SelectedIndex].attributes[dataGridView2.CurrentCell.RowIndex].name = stringToCharArray(addAttribute.name);
                                entities[addAttribute.comboBox1.SelectedIndex].attributes[dataGridView2.CurrentCell.RowIndex].type = addAttribute.type;
                                entities[addAttribute.comboBox1.SelectedIndex].attributes[dataGridView2.CurrentCell.RowIndex].length = addAttribute.lenght;
                                entities[addAttribute.comboBox1.SelectedIndex].attributes[dataGridView2.CurrentCell.RowIndex].indexType = addAttribute.indexType;
                                file.modifyAttribute(entities[addAttribute.comboBox1.SelectedIndex].attributes[dataGridView2.CurrentCell.RowIndex].attributeDir, entities[addAttribute.comboBox1.SelectedIndex].attributes[dataGridView2.CurrentCell.RowIndex]);
                                updateAttIndex();
                                addDataAttribute(entities[addAttribute.comboBox1.SelectedIndex]);
                                modifyAtt = false;
                            }
                            modifyAtt = false;
                        }

                        
                        toolStripStatusLabel3.Text = " || Attribute " + addAttribute.name + " added";

                    }
                    else
                    {
                        MessageBox.Show("No attribute added");
                    }
                }
            }
            else
                MessageBox.Show("There is no file, create one.");
        }

        #endregion

        #region Data

        private void addDataToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (file != null)
            {
                DialogResult addDataDialog;
                AddData addData = new AddData(entities, file, Directory.GetCurrentDirectory());

                addDataDialog = addData.ShowDialog();

                if (addDataDialog == DialogResult.OK)
                {
                    /*if (datFile == null)
                    {
                        datFile = new AuxFile();
                        string s = new string(entities[addData.comboBoxEnt.SelectedIndex].name);

                        datFile.stream = new FileStream("C:\\Users\\Hernan\\source\\repos\\Archivos\\Diccionario_Datos\\Diccionario_Datos\\Examples\\"
                            + addData.name + ".dat", FileMode.Create);

                    }
                    else
                    {
                        

                    }*/
                }
            }
        }

        #endregion
    }
}
