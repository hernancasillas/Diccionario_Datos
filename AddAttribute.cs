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
    public partial class AddAttribute : Form
    {
        public AddAttribute(List<Entity> entities)
        {
            InitializeComponent();

            foreach(Entity ent in entities)
            {
                string s = new string(ent.name);

                comboBox1.Items.Add(s);
            }

            indexComboBox2.SelectedIndex = 0;
        }

        public string name { get { return nameTextBox.Text; } }

        public char type
        {
            get
            {
                switch(typeComboBox.SelectedItem)
                {
                    case "Int":
                        return 'i';

                    case "String":
                        return 's';

                    case "Char":
                        return 'c';
                }

                return 'i';
            }
        }

        public int lenght
        {
            get
            {
                if (lengthTextBox.Text.Length > 0)
                    return int.Parse(lengthTextBox.Text);
                else
                    return 4;
            }
        }

        public int indexType
        {
            get
            {
                switch (indexComboBox2.SelectedIndex)
                {
                    case 0:
                        return 0;

                    case 1:
                        return 1;

                    case 2:
                        return 2;

                    case 3:
                        return 3;

                    case 4:
                        return 4;

                    case 5:
                        return 5;
                }

                return 0;
            }
        }

        private void AddAttribute_Load(object sender, EventArgs e)
        {
            AddButton.DialogResult = DialogResult.OK;
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (typeComboBox.SelectedIndex)
            {
                case 0: lengthTextBox.Text = "4";
                    break;
                case 1: lengthTextBox.Text = "1";
                    break;
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddButton.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
