namespace ServerGUI
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
            btnStart = new Button();
            lstServerMessage = new ListBox();
            lstBoardMessage = new ListBox();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(12, 12);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(75, 154);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // lstServerMessage
            // 
            lstServerMessage.FormattingEnabled = true;
            lstServerMessage.ItemHeight = 15;
            lstServerMessage.Location = new Point(93, 12);
            lstServerMessage.Name = "lstServerMessage";
            lstServerMessage.Size = new Size(450, 154);
            lstServerMessage.TabIndex = 1;
            // 
            // lstBoardMessage
            // 
            lstBoardMessage.FormattingEnabled = true;
            lstBoardMessage.ItemHeight = 15;
            lstBoardMessage.Location = new Point(12, 179);
            lstBoardMessage.Name = "lstBoardMessage";
            lstBoardMessage.Size = new Size(531, 304);
            lstBoardMessage.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(556, 496);
            Controls.Add(lstBoardMessage);
            Controls.Add(lstServerMessage);
            Controls.Add(btnStart);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button btnStart;
        private ListBox lstServerMessage;
        private ListBox lstBoardMessage;
    }
}
