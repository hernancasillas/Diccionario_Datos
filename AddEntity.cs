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
    public partial class AddEntity : Form
    {
        public AddEntity()
        {
            InitializeComponent();
        }

        public string name { get { return textBox1.Text; } }

        private void AddEntity_Load(object sender, EventArgs e)
        {
            AddButton.DialogResult = DialogResult.OK;
        }
    }
}
