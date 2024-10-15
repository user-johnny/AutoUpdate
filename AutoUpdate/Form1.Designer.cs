namespace AutoUpdate
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
            textBoxStatus = new TextBox();
            SuspendLayout();
            // 
            // textBoxStatus
            // 
            textBoxStatus.BorderStyle = BorderStyle.None;
            textBoxStatus.Enabled = false;
            textBoxStatus.Font = new Font("Microsoft JhengHei UI", 14F);
            textBoxStatus.Location = new Point(1, 57);
            textBoxStatus.Name = "textBoxStatus";
            textBoxStatus.Size = new Size(483, 30);
            textBoxStatus.TabIndex = 0;
            textBoxStatus.Text = "...";
            textBoxStatus.TextAlign = HorizontalAlignment.Center;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(485, 143);
            Controls.Add(textBoxStatus);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "自動更新 (02) 8809-8098";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxStatus;
    }
}
