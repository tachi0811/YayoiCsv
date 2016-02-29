namespace YayoiCsv
{
    partial class BatchKotsuhi
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
            this.btnYayoiCSV = new System.Windows.Forms.Button();
            this.grpBox = new System.Windows.Forms.GroupBox();
            this.rdoYear = new System.Windows.Forms.RadioButton();
            this.rdoHoliday = new System.Windows.Forms.RadioButton();
            this.rdoNormal = new System.Windows.Forms.RadioButton();
            this.btnCSV = new System.Windows.Forms.Button();
            this.txtKin = new YayoiCsv.control.TextBoxEx();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTekiyo = new YayoiCsv.control.TextBoxEx();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbHojo = new System.Windows.Forms.ComboBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnShiwakeAdd = new System.Windows.Forms.Button();
            this.grpBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnYayoiCSV
            // 
            this.btnYayoiCSV.Location = new System.Drawing.Point(229, 143);
            this.btnYayoiCSV.Name = "btnYayoiCSV";
            this.btnYayoiCSV.Size = new System.Drawing.Size(84, 51);
            this.btnYayoiCSV.TabIndex = 5;
            this.btnYayoiCSV.Text = "弥生Import形式CSV";
            this.btnYayoiCSV.Click += new System.EventHandler(this.btnYayoiCSV_Click);
            // 
            // grpBox
            // 
            this.grpBox.Controls.Add(this.rdoYear);
            this.grpBox.Controls.Add(this.rdoHoliday);
            this.grpBox.Controls.Add(this.rdoNormal);
            this.grpBox.Location = new System.Drawing.Point(12, 2);
            this.grpBox.Name = "grpBox";
            this.grpBox.Size = new System.Drawing.Size(248, 57);
            this.grpBox.TabIndex = 0;
            this.grpBox.TabStop = false;
            // 
            // rdoYear
            // 
            this.rdoYear.AutoSize = true;
            this.rdoYear.Location = new System.Drawing.Point(158, 22);
            this.rdoYear.Name = "rdoYear";
            this.rdoYear.Size = new System.Drawing.Size(56, 19);
            this.rdoYear.TabIndex = 2;
            this.rdoYear.Text = "1年分";
            this.rdoYear.UseVisualStyleBackColor = true;
            // 
            // rdoHoliday
            // 
            this.rdoHoliday.AutoSize = true;
            this.rdoHoliday.Location = new System.Drawing.Point(82, 22);
            this.rdoHoliday.Name = "rdoHoliday";
            this.rdoHoliday.Size = new System.Drawing.Size(70, 19);
            this.rdoHoliday.TabIndex = 1;
            this.rdoHoliday.Text = "休日のみ";
            this.rdoHoliday.UseVisualStyleBackColor = true;
            // 
            // rdoNormal
            // 
            this.rdoNormal.AutoSize = true;
            this.rdoNormal.Checked = true;
            this.rdoNormal.Location = new System.Drawing.Point(6, 22);
            this.rdoNormal.Name = "rdoNormal";
            this.rdoNormal.Size = new System.Drawing.Size(70, 19);
            this.rdoNormal.TabIndex = 0;
            this.rdoNormal.TabStop = true;
            this.rdoNormal.Text = "平日のみ";
            this.rdoNormal.UseVisualStyleBackColor = true;
            // 
            // btnCSV
            // 
            this.btnCSV.Location = new System.Drawing.Point(139, 143);
            this.btnCSV.Name = "btnCSV";
            this.btnCSV.Size = new System.Drawing.Size(84, 51);
            this.btnCSV.TabIndex = 4;
            this.btnCSV.Text = "CSV";
            this.btnCSV.UseVisualStyleBackColor = true;
            this.btnCSV.Click += new System.EventHandler(this.btnCSV_Click);
            // 
            // txtKin
            // 
            this.txtKin.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtKin.InputType = YayoiCsv.control.InputType.Number;
            this.txtKin.Location = new System.Drawing.Point(119, 88);
            this.txtKin.Name = "txtKin";
            this.txtKin.Size = new System.Drawing.Size(194, 23);
            this.txtKin.TabIndex = 2;
            this.txtKin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.YellowGreen;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Location = new System.Drawing.Point(12, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 23);
            this.label1.TabIndex = 4;
            this.label1.Text = "金額";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.YellowGreen;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label3.Location = new System.Drawing.Point(12, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 23);
            this.label3.TabIndex = 6;
            this.label3.Text = "補助科目";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTekiyo
            // 
            this.txtTekiyo.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.txtTekiyo.InputType = YayoiCsv.control.InputType.String;
            this.txtTekiyo.Location = new System.Drawing.Point(119, 114);
            this.txtTekiyo.Name = "txtTekiyo";
            this.txtTekiyo.Size = new System.Drawing.Size(194, 23);
            this.txtTekiyo.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.YellowGreen;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Location = new System.Drawing.Point(12, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 23);
            this.label2.TabIndex = 9;
            this.label2.Text = "摘要";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbHojo
            // 
            this.cmbHojo.FormattingEnabled = true;
            this.cmbHojo.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.cmbHojo.Location = new System.Drawing.Point(119, 62);
            this.cmbHojo.Name = "cmbHojo";
            this.cmbHojo.Size = new System.Drawing.Size(194, 23);
            this.cmbHojo.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.IndianRed;
            this.btnClose.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnClose.Location = new System.Drawing.Point(266, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(47, 40);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "×";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnShiwakeAdd
            // 
            this.btnShiwakeAdd.Location = new System.Drawing.Point(12, 143);
            this.btnShiwakeAdd.Name = "btnShiwakeAdd";
            this.btnShiwakeAdd.Size = new System.Drawing.Size(84, 51);
            this.btnShiwakeAdd.TabIndex = 11;
            this.btnShiwakeAdd.Text = "経費に追加";
            this.btnShiwakeAdd.UseVisualStyleBackColor = true;
            this.btnShiwakeAdd.Click += new System.EventHandler(this.btnShiwakeAdd_Click);
            // 
            // BatchKotsuhi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 204);
            this.ControlBox = false;
            this.Controls.Add(this.btnShiwakeAdd);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.cmbHojo);
            this.Controls.Add(this.txtTekiyo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtKin);
            this.Controls.Add(this.btnCSV);
            this.Controls.Add(this.grpBox);
            this.Controls.Add(this.btnYayoiCSV);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Meiryo UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BatchKotsuhi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "交通費一括出力";
            this.grpBox.ResumeLayout(false);
            this.grpBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnYayoiCSV;
        private System.Windows.Forms.GroupBox grpBox;
        private System.Windows.Forms.RadioButton rdoYear;
        private System.Windows.Forms.RadioButton rdoHoliday;
        private System.Windows.Forms.RadioButton rdoNormal;
        private System.Windows.Forms.Button btnCSV;
        private control.TextBoxEx txtKin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private control.TextBoxEx txtTekiyo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbHojo;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnShiwakeAdd;
    }
}