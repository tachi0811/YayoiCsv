using System;
using System.Windows.Forms;

namespace YayoiCsv
{
    public partial class InputNendo : control.FormEx
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InputNendo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 年度を決定する
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">event</param>
        private void btnNendoSelect_Click(object sender, EventArgs e)
        {

            if (cmbNendo.SelectedItem == null || cmbNendo.SelectedItem.ToString() == "")
            {
                return;
            }

            Static.Nendo = int.Parse(cmbNendo.SelectedItem.ToString());
            Static.CloseChildForm(this.GetType());
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
