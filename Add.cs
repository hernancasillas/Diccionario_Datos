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
    public partial class Add : Form
    {
        private string attName;

        public Add(string attName)
        {
            InitializeComponent();
            this.attName = attName;
            label1.Text = "Add data for " + attName + " please";
            label1.Location = new Point((this.Width) / 5, label1.Location.Y);
            label1.Visible = false;
            this.Text = label1.Text;
            textBox1.Location = new Point((this.Width - textBox1.Width) / 2, textBox1.Location.Y);
            button1.Location = new Point((this.Width - button1.Width) / 2, button1.Location.Y);
        }

        public string DataText
        {
            get
            {
                return textBox1.Text;
            }
        }


        private void Add_Load(object sender, EventArgs e)
        {
            button1.DialogResult = DialogResult.OK;
        }
    }
}
