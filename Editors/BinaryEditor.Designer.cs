namespace CrackSoft.RegExplore.Editors
{
    partial class BinaryEditor
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
            this.txtData = new Be.Windows.Forms.HexBox();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.Size = new System.Drawing.Size(342, 20);
            this.txtName.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(279, 253);
            this.btnCancel.TabIndex = 2;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(198, 253);
            this.btnOK.TabIndex = 1;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtData
            // 
            this.txtData.BackColor = System.Drawing.SystemColors.Window;
            this.txtData.BackColorDisabled = System.Drawing.SystemColors.ControlDark;
            this.txtData.BytesPerLine = 8;
            this.txtData.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtData.LineInfoForeColor = System.Drawing.Color.Empty;
            this.txtData.LineInfoVisible = true;
            this.txtData.Location = new System.Drawing.Point(12, 81);
            this.txtData.Name = "txtData";
            this.txtData.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.txtData.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.txtData.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.txtData.Size = new System.Drawing.Size(342, 166);
            this.txtData.StringViewVisible = true;
            this.txtData.TabIndex = 0;
            this.txtData.UseFixedBytesPerLine = true;
            // 
            // BinaryEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 288);
            this.Controls.Add(this.txtData);
            this.Name = "BinaryEditor";
            this.Text = "Edit Binary Value";
            this.Controls.SetChildIndex(this.txtData, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.txtName, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Be.Windows.Forms.HexBox txtData;
    }
}