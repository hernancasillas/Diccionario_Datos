using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataDictionary
{
    public partial class IndexView : Form
    {
        Entity entity;
        DataTable dataTFk;
        DataTable dataTTree;
        bool isPk, isFk, isTree;

        public IndexView(Entity entity, DataTable dataTfk, DataTable dataTree)
        {
            this.entity = entity;
            InitializeComponent();
            foreach (Attribute att in this.entity.attributes)
            {
                switch (att.indexType)
                {
                    case 2:
                        isPk = true;
                        break;
                    case 3:
                        isFk = true;
                        break;
                    case 4:
                        isTree = true;
                        break;
                        /*case 2:
                            dataGridViewPK.Visible = true;
                            dataGridView2.Visible = false;
                            addDataPK();
                            break;
                        case 3: dataGridView2.Visible = true;
                            dataGridViewPK.Visible = true;
                            this.dataTFk = dataTfk;
                            dataGridView2.DataSource = dataTFk;
                            addDataFK();
                            break;*/
                }
            }
            addDataPK();
            this.dataTFk = dataTfk;
            dataGridView2.DataSource = dataTFk;
            addDataFK();
            this.dataTTree = dataTree;
            dataGridViewTREE.DataSource = dataTTree;
            addDataTree();

        }

        private void addDataPK()
        {

            foreach (PrimaryKey pk in entity.pk)
            {
                //dataGridViewPK.Rows.Add(new object[] { pk.oClave, pk.lDireccion });
            }

            int ind = 0;

            foreach (PrimaryKey pk in entity.pk)
            {
                int i = 1;
                int j = 0; int k = 0;
                dataGridViewPK.Rows.Add();
                dataGridViewPK.Rows[ind].Cells[0].Value = pk.oClave;
                dataGridViewPK.Rows[ind].Cells[1].Value = pk.lDireccion;
                ind++;
            }
        }

        private void addDataFK()
        {
            DataRow r;
            dataTFk.Clear();
            for (int i = 0; i < entity.fk.Count; i++)
            {
                r = dataTFk.NewRow();
                int j = 0;


                r[j] = entity.fk[i].oClave;

                j = 1;
                for (int c = 0; c < entity.fk[i].lDirecciones.Count; c++)
                {
                    r[j] = entity.fk[i].lDirecciones[c];
                    j++;
                }
                dataTFk.Rows.Add(r);
            }
        }

        private void addDataTree()
        {
            DataRow r;
            dataTTree.Clear();
            for (int i = 0; i < entity.nodos.Count; i++)
            {
                r = dataTTree.NewRow();
                int j = 0, d = 0, a = 0;
                r[j] = entity.nodos[i].lDireccionN;
                j = 1;
                r[j] = entity.nodos[i].cTipo;
                j = 2;
                while (d < entity.nodos[i].iDatos.Count || a < entity.nodos[i].lDirecciones.Count)
                {
                    if (j % 2 == 0)
                    {
                        r[j] = entity.nodos[i].lDirecciones[a];
                        a++;
                    }
                    else
                    {
                        r[j] = entity.nodos[i].iDatos[d];
                        d++;
                    }
                    j++;
                }
                dataTTree.Rows.Add(r);
            }
        }
    }
}
