namespace AsyncServerApp
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

        private void InitializeComponent()
        {
            txtHost = new TextBox();
            txtPort = new TextBox();
            btnListen = new Button();
            lstMessages = new ListBox();
            txtMessage = new TextBox();
            btnSend = new Button();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // txtHost
            // 
            txtHost.Location = new Point(62, 16);
            txtHost.Margin = new Padding(3, 4, 3, 4);
            txtHost.Name = "txtHost";
            txtHost.Size = new Size(114, 27);
            txtHost.TabIndex = 0;
            txtHost.Text = "127.0.0.1";
            // 
            // txtPort
            // 
            txtPort.Location = new Point(233, 16);
            txtPort.Margin = new Padding(3, 4, 3, 4);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(62, 27);
            txtPort.TabIndex = 1;
            txtPort.Text = "8080";
            // 
            // btnListen
            // 
            btnListen.Location = new Point(315, 16);
            btnListen.Margin = new Padding(3, 4, 3, 4);
            btnListen.Name = "btnListen";
            btnListen.Size = new Size(86, 31);
            btnListen.TabIndex = 2;
            btnListen.Text = "Listen";
            btnListen.UseVisualStyleBackColor = true;
            btnListen.Click += btnListen_Click;
            // 
            // lstMessages
            // 
            lstMessages.FormattingEnabled = true;
            lstMessages.Location = new Point(14, 60);
            lstMessages.Margin = new Padding(3, 4, 3, 4);
            lstMessages.Name = "lstMessages";
            lstMessages.Size = new Size(495, 344);
            lstMessages.TabIndex = 3;
            lstMessages.SelectedIndexChanged += lstMessages_SelectedIndexChanged;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(14, 425);
            txtMessage.Margin = new Padding(3, 4, 3, 4);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(403, 27);
            txtMessage.TabIndex = 4;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(424, 424);
            btnSend.Margin = new Padding(3, 4, 3, 4);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(86, 32);
            btnSend.TabIndex = 5;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 21);
            label1.Name = "label1";
            label1.Size = new Size(43, 20);
            label1.TabIndex = 6;
            label1.Text = "Host:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(190, 21);
            label2.Name = "label2";
            label2.Size = new Size(38, 20);
            label2.TabIndex = 7;
            label2.Text = "Port:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(523, 472);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnSend);
            Controls.Add(txtMessage);
            Controls.Add(lstMessages);
            Controls.Add(btnListen);
            Controls.Add(txtPort);
            Controls.Add(txtHost);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Async Chat Server";
            FormClosing += Form1_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.ListBox lstMessages;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
