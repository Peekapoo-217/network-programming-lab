namespace ServerApp
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.txtRootPath = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnSelectRoot = new System.Windows.Forms.Button();
            this.lbLog = new System.Windows.Forms.ListBox();
            this.lbStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtRootPath
            // 
            this.txtRootPath.Location = new System.Drawing.Point(30, 80);
            this.txtRootPath.Name = "txtRootPath";
            this.txtRootPath.ReadOnly = true;
            this.txtRootPath.Size = new System.Drawing.Size(500, 22);
            this.txtRootPath.TabIndex = 0;
            this.txtRootPath.Text = "Chưa chọn thư mục...";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // btnSelectRoot
            // 
            this.btnSelectRoot.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectRoot.Location = new System.Drawing.Point(550, 70);
            this.btnSelectRoot.Name = "btnSelectRoot";
            this.btnSelectRoot.Size = new System.Drawing.Size(200, 40);
            this.btnSelectRoot.TabIndex = 2;
            this.btnSelectRoot.Text = "Chọn Thư Mục && Start";
            this.btnSelectRoot.UseVisualStyleBackColor = true;
            // 
            // lbLog
            // 
            this.lbLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLog.FormattingEnabled = true;
            this.lbLog.ItemHeight = 18;
            this.lbLog.Location = new System.Drawing.Point(30, 180);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(720, 238);
            this.lbLog.TabIndex = 3;
            this.lbLog.SelectedIndexChanged += new System.EventHandler(this.lbLog_SelectedIndexChanged);
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatus.ForeColor = System.Drawing.Color.Red;
            this.lbStatus.Location = new System.Drawing.Point(30, 130);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(184, 20);
            this.lbStatus.TabIndex = 4;
            this.lbStatus.Text = "Server chưa khởi động";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(280, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(238, 32);
            this.label1.TabIndex = 5;
            this.label1.Text = "FTP-like Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(30, 155);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 18);
            this.label2.TabIndex = 6;
            this.label2.Text = "Log hoạt động:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.lbLog);
            this.Controls.Add(this.btnSelectRoot);
            this.Controls.Add(this.txtRootPath);
            this.Name = "Form1";
            this.Text = "FTP Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtRootPath;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button btnSelectRoot;
        private System.Windows.Forms.ListBox lbLog;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

