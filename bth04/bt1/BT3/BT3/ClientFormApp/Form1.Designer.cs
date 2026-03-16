namespace ClientFormApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();
            this.txtSo1 = new TextBox();
            this.txtSo2 = new TextBox();
            this.txtKetQua = new TextBox();
            this.btnKetNoi = new Button();
            this.btnGui = new Button();
            this.SuspendLayout();

            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 30);
            this.label1.Name = "label1";
            this.label1.Text = "So thu nhat:";

            // label2
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 70);
            this.label2.Name = "label2";
            this.label2.Text = "So thu hai:";

            // label3
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 150);
            this.label3.Name = "label3";
            this.label3.Text = "Ket qua:";

            // txtSo1
            this.txtSo1.Location = new System.Drawing.Point(130, 27);
            this.txtSo1.Name = "txtSo1";
            this.txtSo1.Size = new System.Drawing.Size(150, 23);

            // txtSo2
            this.txtSo2.Location = new System.Drawing.Point(130, 67);
            this.txtSo2.Name = "txtSo2";
            this.txtSo2.Size = new System.Drawing.Size(150, 23);

            // txtKetQua
            this.txtKetQua.Location = new System.Drawing.Point(130, 147);
            this.txtKetQua.Name = "txtKetQua";
            this.txtKetQua.Size = new System.Drawing.Size(150, 23);
            this.txtKetQua.ReadOnly = true;

            // btnKetNoi
            this.btnKetNoi.Location = new System.Drawing.Point(30, 107);
            this.btnKetNoi.Name = "btnKetNoi";
            this.btnKetNoi.Size = new System.Drawing.Size(110, 30);
            this.btnKetNoi.Text = "Ket noi";
            this.btnKetNoi.UseVisualStyleBackColor = true;
            this.btnKetNoi.Click += new System.EventHandler(this.btnKetNoi_Click);

            // btnGui
            this.btnGui.Location = new System.Drawing.Point(170, 107);
            this.btnGui.Name = "btnGui";
            this.btnGui.Size = new System.Drawing.Size(110, 30);
            this.btnGui.Text = "Gui";
            this.btnGui.UseVisualStyleBackColor = true;
            this.btnGui.Enabled = false;
            this.btnGui.Click += new System.EventHandler(this.btnGui_Click);

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 200);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSo1);
            this.Controls.Add(this.txtSo2);
            this.Controls.Add(this.txtKetQua);
            this.Controls.Add(this.btnKetNoi);
            this.Controls.Add(this.btnGui);
            this.Name = "Form1";
            this.Text = "Client";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox txtSo1;
        private TextBox txtSo2;
        private TextBox txtKetQua;
        private Button btnKetNoi;
        private Button btnGui;
    }
}
