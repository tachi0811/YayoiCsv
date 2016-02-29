﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;

namespace YayoiCsv
{
    /// <summary>
    /// 経費入力
    /// </summary>
    public partial class ViewShiwake : Form
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ViewShiwake()
        {
            InitializeComponent();
            
        }

        /// <summary>
        /// グリッドの初期処理
        /// </summary>
        private void InitGrid()
        {
            var dv = Static.ShiwakeDs.KeihiSum.AsDataView();
            dataGridView1.DataSource = dv;
        }


        private void InputShiwake_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            InitGrid();
        }

        /// <summary>
        /// 閉じるボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Static.CloseChildForm(this.GetType());
        }
    }
}
