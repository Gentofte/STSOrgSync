namespace ReportTool
{
    partial class ReportToolForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportToolForm));
            this.generateButton = new System.Windows.Forms.Button();
            this.cvrTextBox = new System.Windows.Forms.TextBox();
            this.uuidTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.logWindow = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(280, 135);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(141, 23);
            this.generateButton.TabIndex = 0;
            this.generateButton.Text = "Generate Report";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButtonClick);
            // 
            // cvrTextBox
            // 
            this.cvrTextBox.Location = new System.Drawing.Point(130, 54);
            this.cvrTextBox.Name = "cvrTextBox";
            this.cvrTextBox.Size = new System.Drawing.Size(291, 20);
            this.cvrTextBox.TabIndex = 1;
            // 
            // uuidTextBox
            // 
            this.uuidTextBox.Location = new System.Drawing.Point(130, 94);
            this.uuidTextBox.Name = "uuidTextBox";
            this.uuidTextBox.Size = new System.Drawing.Size(291, 20);
            this.uuidTextBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(68, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "CVR";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(68, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "UUID";
            // 
            // logWindow
            // 
            this.logWindow.Location = new System.Drawing.Point(71, 230);
            this.logWindow.Name = "logWindow";
            this.logWindow.Size = new System.Drawing.Size(350, 115);
            this.logWindow.TabIndex = 5;
            this.logWindow.Text = "";
            // 
            // ReportToolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 393);
            this.Controls.Add(this.logWindow);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.uuidTextBox);
            this.Controls.Add(this.cvrTextBox);
            this.Controls.Add(this.generateButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReportToolForm";
            this.Text = "Report Tool";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.TextBox cvrTextBox;
        private System.Windows.Forms.TextBox uuidTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox logWindow;
    }
}

