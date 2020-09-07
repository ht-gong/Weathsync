namespace Win_END
{
    partial class ChildForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.HelpCheckBox = new System.Windows.Forms.CheckBox();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(31, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(412, 133);
            this.label1.TabIndex = 0;
            this.label1.Text = "请先点击“选择文件夹”选择需要监测excel文件的文件夹，\r\n接着点击“开始监测”即可。\r\n\r\n点击“最小化”图标将会将应用缩小至任务栏后台运行。\r\n\r\n注意：“" +
    "清空所有记录”将会将所有已读取至数据库的记录全部清空！\r\n若重新监测的话已在目标文件夹中的excel将会重新被读取。";
            // 
            // HelpCheckBox
            // 
            this.HelpCheckBox.AutoSize = true;
            this.HelpCheckBox.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.HelpCheckBox.Location = new System.Drawing.Point(158, 168);
            this.HelpCheckBox.Name = "HelpCheckBox";
            this.HelpCheckBox.Size = new System.Drawing.Size(119, 23);
            this.HelpCheckBox.TabIndex = 1;
            this.HelpCheckBox.Text = "不再显示此说明";
            this.HelpCheckBox.UseVisualStyleBackColor = true;
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(171, 197);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(90, 23);
            this.btnConfirm.TabIndex = 2;
            this.btnConfirm.Text = "确认";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // ChildForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(472, 239);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.HelpCheckBox);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChildForm";
            this.Text = "使用说明";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox HelpCheckBox;
        private System.Windows.Forms.Button btnConfirm;
    }
}