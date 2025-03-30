namespace ClientGUI
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
            txtPort = new TextBox();
            txtHost = new TextBox();
            label1 = new Label();
            label2 = new Label();
            btnConnect = new Button();
            btnStressTest = new Button();
            btnDisconnect = new Button();
            btnSendMessage = new Button();
            txtMessage = new TextBox();
            lstMessage = new ListBox();
            picState = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)picState).BeginInit();
            SuspendLayout();
            // 
            // txtPort
            // 
            txtPort.Location = new Point(456, 75);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(80, 23);
            txtPort.TabIndex = 0;
            txtPort.Text = "10004";
            // 
            // txtHost
            // 
            txtHost.Location = new Point(456, 31);
            txtHost.Name = "txtHost";
            txtHost.Size = new Size(80, 23);
            txtHost.TabIndex = 1;
            txtHost.Text = "127.0.0.1";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(456, 13);
            label1.Name = "label1";
            label1.Size = new Size(32, 15);
            label1.TabIndex = 2;
            label1.Text = "Host";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(456, 57);
            label2.Name = "label2";
            label2.Size = new Size(29, 15);
            label2.TabIndex = 3;
            label2.Text = "Port";
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(456, 104);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(80, 23);
            btnConnect.TabIndex = 4;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // btnStressTest
            // 
            btnStressTest.Location = new Point(452, 216);
            btnStressTest.Name = "btnStressTest";
            btnStressTest.Size = new Size(84, 23);
            btnStressTest.TabIndex = 5;
            btnStressTest.Text = "Test";
            btnStressTest.UseVisualStyleBackColor = true;
            btnStressTest.Click += btnStressTest_Click;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Location = new Point(456, 133);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(80, 23);
            btnDisconnect.TabIndex = 6;
            btnDisconnect.Text = "Disconnect";
            btnDisconnect.UseVisualStyleBackColor = true;
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // btnSendMessage
            // 
            btnSendMessage.Location = new Point(720, 416);
            btnSendMessage.Name = "btnSendMessage";
            btnSendMessage.Size = new Size(68, 23);
            btnSendMessage.TabIndex = 7;
            btnSendMessage.Text = "Send";
            btnSendMessage.UseVisualStyleBackColor = true;
            btnSendMessage.Click += button1_Click;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(542, 416);
            txtMessage.Name = "txtMessage";
            txtMessage.PlaceholderText = "Send a Message ....";
            txtMessage.Size = new Size(172, 23);
            txtMessage.TabIndex = 8;
            // 
            // lstMessage
            // 
            lstMessage.FormattingEnabled = true;
            lstMessage.ItemHeight = 15;
            lstMessage.Location = new Point(542, 16);
            lstMessage.Name = "lstMessage";
            lstMessage.Size = new Size(246, 394);
            lstMessage.TabIndex = 9;
            // 
            // picState
            // 
            picState.Location = new Point(456, 162);
            picState.Name = "picState";
            picState.Size = new Size(80, 48);
            picState.TabIndex = 10;
            picState.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(picState);
            Controls.Add(lstMessage);
            Controls.Add(txtMessage);
            Controls.Add(btnSendMessage);
            Controls.Add(btnDisconnect);
            Controls.Add(btnStressTest);
            Controls.Add(btnConnect);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtHost);
            Controls.Add(txtPort);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)picState).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtPort;
        private TextBox txtHost;
        private Label label1;
        private Label label2;
        private Button btnConnect;
        private Button btnStressTest;
        private Button btnDisconnect;
        private Button btnSendMessage;
        private TextBox txtMessage;
        private ListBox lstMessage;
        private PictureBox picState;
    }
}
