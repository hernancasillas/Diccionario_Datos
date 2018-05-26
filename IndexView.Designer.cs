namespace DataDictionary
{
    partial class IndexView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IndexView));
            this.dataGridViewPK = new System.Windows.Forms.DataGridView();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTREE = new System.Windows.Forms.DataGridView();
            this.Data = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataDir = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTREE)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewPK
            // 
            this.dataGridViewPK.AllowUserToAddRows = false;
            this.dataGridViewPK.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPK.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Data,
            this.DataDir});
            this.dataGridViewPK.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewPK.Name = "dataGridViewPK";
            this.dataGridViewPK.Size = new System.Drawing.Size(243, 332);
            this.dataGridViewPK.TabIndex = 0;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(261, 12);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(863, 332);
            this.dataGridView2.TabIndex = 1;
            // 
            // dataGridViewTREE
            // 
            this.dataGridViewTREE.AllowUserToAddRows = false;
            this.dataGridViewTREE.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTREE.Location = new System.Drawing.Point(12, 350);
            this.dataGridViewTREE.Name = "dataGridViewTREE";
            this.dataGridViewTREE.Size = new System.Drawing.Size(1112, 299);
            this.dataGridViewTREE.TabIndex = 2;
            // 
            // Data
            // 
            this.Data.HeaderText = "Data";
            this.Data.Name = "Data";
            // 
            // DataDir
            // 
            this.DataDir.HeaderText = "DataDir";
            this.DataDir.Name = "DataDir";
            // 
            // IndexView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(19)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(1136, 661);
            this.Controls.Add(this.dataGridViewTREE);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.dataGridViewPK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "IndexView";
            this.Text = "IndexView";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTREE)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewPK;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridView dataGridViewTREE;
        private System.Windows.Forms.DataGridViewTextBoxColumn Data;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataDir;
    }
}