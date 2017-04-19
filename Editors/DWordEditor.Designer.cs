namespace CrackSoft.RegExplore.Editors
{
    partial class DWordEditor
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoDecimal = new System.Windows.Forms.RadioButton();
            this.rdoHex = new System.Windows.Forms.RadioButton();
            this.txtData = new CrackSoft.UI.Controls.NumericTextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.TabIndex = 5;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(205, 149);
            this.btnCancel.TabIndex = 4;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(124, 149);
            this.btnOK.TabIndex = 3;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoDecimal);
            this.groupBox1.Controls.Add(this.rdoHex);
            this.groupBox1.Location = new System.Drawing.Point(139, 65);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(141, 68);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Base";
            // 
            // rdoDecimal
            // 
            this.rdoDecimal.AutoSize = true;
            this.rdoDecimal.Location = new System.Drawing.Point(11, 42);
            this.rdoDecimal.Name = "rdoDecimal";
            this.rdoDecimal.Size = new System.Drawing.Size(63, 17);
            this.rdoDecimal.TabIndex = 2;
            this.rdoDecimal.Text = "&Decimal";
            this.rdoDecimal.UseVisualStyleBackColor = true;
            this.rdoDecimal.CheckedChanged += new System.EventHandler(this.base_CheckedChanged);
            // 
            // rdoHex
            // 
            this.rdoHex.AutoSize = true;
            this.rdoHex.Checked = true;
            this.rdoHex.Location = new System.Drawing.Point(11, 19);
            this.rdoHex.Name = "rdoHex";
            this.rdoHex.Size = new System.Drawing.Size(86, 17);
            this.rdoHex.TabIndex = 1;
            this.rdoHex.TabStop = true;
            this.rdoHex.Text = "&Hexadecimal";
            this.rdoHex.UseVisualStyleBackColor = true;
            this.rdoHex.CheckedChanged += new System.EventHandler(this.base_CheckedChanged);
            // 
            // txtData
            // 
            this.txtData.AllowDecimal = false;
            this.txtData.AllowGrouping = false;
            this.txtData.AllowNegative = false;
            this.txtData.HexNumber = true;
            this.txtData.Location = new System.Drawing.Point(12, 84);
            this.txtData.MaxLength = 8;
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(111, 20);
            this.txtData.TabIndex = 0;
            this.txtData.Text = "0";
            // 
            // DWordEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 187);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtData);
            this.Name = "DWordEditor";
            this.Text = "Edit DWORD Value";
            this.Controls.SetChildIndex(this.txtData, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.txtName, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoDecimal;
        private System.Windows.Forms.RadioButton rdoHex;
        private CrackSoft.UI.Controls.NumericTextBox txtData;
    }
}