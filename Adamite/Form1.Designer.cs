namespace Adamite
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlDragDrop = new Panel();
            lblInfo = new Label();
            pnlDragDrop.SuspendLayout();
            SuspendLayout();
            // 
            // pnlDragDrop
            // 
            pnlDragDrop.BorderStyle = BorderStyle.FixedSingle;
            pnlDragDrop.Controls.Add(lblInfo);
            pnlDragDrop.Location = new Point(14, 16);
            pnlDragDrop.Margin = new Padding(3, 4, 3, 4);
            pnlDragDrop.Name = "pnlDragDrop";
            pnlDragDrop.Size = new Size(402, 135);
            pnlDragDrop.TabIndex = 0;
            // 
            // lblInfo
            // 
            lblInfo.Location = new Point(0, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(400, 133);
            lblInfo.TabIndex = 0;
            lblInfo.Text = "Drag terranx.exe here";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(430, 168);
            Controls.Add(pnlDragDrop);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Adamite";
            pnlDragDrop.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlDragDrop;
        private Label lblInfo;
    }
}
