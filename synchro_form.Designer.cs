namespace Norms_physicalDev
{
    partial class synchro_form
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.zip_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Control;
            this.button1.Image = global::Norms_physicalDev.Properties.Resources.email1;
            this.button1.Location = new System.Drawing.Point(126, 25);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(54, 54);
            this.button1.TabIndex = 17;
            this.toolTip1.SetToolTip(this.button1, "Откроется страница Google");
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.White;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button2.Image = global::Norms_physicalDev.Properties.Resources.Sync_Cloud_381;
            this.button2.Location = new System.Drawing.Point(27, 25);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(54, 54);
            this.button2.TabIndex = 16;
            this.button2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.button2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.button2, "Синхронизировать с облаком (Если выполнен вход в учётную запись)");
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(188, 83);
            this.label1.TabIndex = 18;
            this.label1.Text = "*при выборе E-mail откроется окно браузера\r\nВам надо будет вручную отправить пись" +
    "мо и приложить к нему архив, который вы можете найти нажав на кнопку ниже";
            // 
            // zip_button
            // 
            this.zip_button.Location = new System.Drawing.Point(154, 177);
            this.zip_button.Name = "zip_button";
            this.zip_button.Size = new System.Drawing.Size(36, 23);
            this.zip_button.TabIndex = 19;
            this.zip_button.Text = "zip";
            this.zip_button.UseVisualStyleBackColor = true;
            this.zip_button.Click += new System.EventHandler(this.zip_button_Click);
            // 
            // synchro_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(212, 212);
            this.Controls.Add(this.zip_button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "synchro_form";
            this.ShowIcon = false;
            this.Text = "synchro_form";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button zip_button;
    }
}