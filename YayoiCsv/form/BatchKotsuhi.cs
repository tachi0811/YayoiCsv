using System;
using System.Linq;
using System.Windows.Forms;

namespace YayoiCsv
{
    /// <summary>
    /// 交通費、平日出力バッチ
    /// </summary>
    public partial class BatchKotsuhi : Form
    {
        enum ClickType
        {
            Shiwake,
            Csv,
            YayoiCsv,
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BatchKotsuhi()
        {
            InitializeComponent();
            SetCombo();
        }

        /// <summary>
        /// 補助科目から旅費交通費の補助科目を設定
        /// </summary>
        private void SetCombo()
        {
            cmbHojo.Items.Clear();
            cmbHojo.Items.AddRange(Static.HKmkList.Where(x => x.KmkName == "旅費交通費").Select(x => x.HkmkName).ToArray());
        }

        /// <summary>
        /// CSV出力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCSV_Click(object sender, EventArgs e)
        {
            OutputCSV(ClickType.Csv);
        }

        /// <summary>
        /// CSV出力する
        /// </summary>
        /// <param name="isYayoi">弥生か？</param>
        private void OutputCSV(ClickType clickType)
        {
            if (IsOutput() == false) return;

            var date = new DateTime(Static.Nendo, 1, 1);
            var sb = new System.Text.StringBuilder();

            if (rdoHoliday.Checked)
            {
                while (Static.Nendo == date.Year)
                {
                    // 休日は、含む
                    int week = (int)date.DayOfWeek;
                    if (!Static.HolidayList.Any(x => x.Date == date) && !(week == 0 || week == 6))
                    {
                        date = date.AddDays(1);
                        continue;
                    }

                    if (clickType == ClickType.YayoiCsv)
                    {
                        // 弥生で取込めるフォーマット
                        sb.AppendLine(CreateYayoiCSV(date));
                    }
                    else if (clickType == ClickType.Csv)
                    {
                        // 日付,科目,補助科目,金額,摘要
                        sb.AppendLine(CreateCSV(date));
                    }
                    else
                    {
                        // 経費を足す
                        AddShiwake(date);
                    }

                    date = date.AddDays(1);
                }

            }
            else
            {
                while (Static.Nendo == date.Year)
                {
                    if (rdoNormal.Checked)
                    {
                        // 休日は除外
                        if (Static.HolidayList.Any(x => x.Date == date))
                        {
                            date = date.AddDays(1);
                            continue;
                        }

                        // 土日も外す
                        int week = (int)date.DayOfWeek;
                        if (week == 0 || week == 6)
                        {
                            date = date.AddDays(1);
                            continue;
                        }
                    }

                    if (clickType == ClickType.YayoiCsv)
                    {
                        // 弥生で取込めるフォーマット
                        sb.AppendLine(CreateYayoiCSV(date));
                    }
                    else if (clickType == ClickType.Csv)
                    {
                        // 日付,科目,補助科目,金額,摘要
                        sb.AppendLine(CreateCSV(date));
                    }
                    else
                    {
                        // 経費を足す
                        AddShiwake(date);
                    }

                    date = date.AddDays(1);

                }
            }

            if (clickType != ClickType.Shiwake)
            {
                if (IsSaveFile(sb.ToString(), clickType == ClickType.YayoiCsv) == true)
                {
                    MessageBox.Show("CSV出力しました。", "保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("経費に追加しました。", "保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Static.SaveShiwakeXML();
            }
        }

        /// <summary>
        /// 出力できるか調べる
        /// </summary>
        /// <returns></returns>
        private bool IsOutput()
        {
            if (txtKin.Text == "")
            {
                MessageBox.Show("金額は必須です。\n金額を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtKin.Focus();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 対象文字列を保存する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="isYayoi">弥生の場合 true</param>
        private bool IsSaveFile(string target, bool isYayoi = false)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                // 摘要もファイル名に含める
                string tekiyo = txtTekiyo.Text.Trim() != "" ? "(" + txtTekiyo.Text.Trim() + ")" : "";

                // 初期表示するディレクトリを設定する
                saveDialog.InitialDirectory = @"C:\";
                saveDialog.FileName = Static.Nendo.ToString() + "年度_旅費交通費" + tekiyo + (isYayoi ? "(弥生)" : "");
                saveDialog.Filter = "テキスト ファイル|*.csv;";

                var result = saveDialog.ShowDialog(Static.ParentForm);
                if (result == DialogResult.OK)
                {
                    using (var writeFileStream = new System.IO.StreamWriter(saveDialog.FileName, false, System.Text.Encoding.GetEncoding("SJIS")))
                    {
                        writeFileStream.Write(target.ToString());
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// CSVファイルを１行作成する
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private string CreateCSV(DateTime date)
        {
            return string.Concat(date.ToString("yyyy/MM/dd").ToDoubleQuote(), ",",
                        "旅費交通費".ToDoubleQuote(), ",",
                        cmbHojo.Text.Trim().ToDoubleQuote(), ",",
                        txtKin.Text.Trim().ToDoubleQuote(), ",",
                        txtTekiyo.Text.Trim().ToDoubleQuote());
        }

        /// <summary>
        /// CSVファイルを１行作成する
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private string CreateYayoiCSV(DateTime date)
        {
            return Static.CreateYayoiCSVRow(date, "旅費交通費",
                cmbHojo.Text.Trim(),
                txtKin.Text.Trim(),
                txtTekiyo.Text);
        }

        /// <summary>
        /// 経費を足す
        /// </summary>
        /// <param name="date"></param>
        private void AddShiwake(DateTime date)
        {
            var row = Static.ShiwakeDs.Shiwake.NewShiwakeRow();
            row.KmkKbn = KmkKbn.経費.ToString();
            row.No = Static.GetShiwakeNo();
            row.CustomDate = date.ToString("MMdd");
            row.KrKmkName = "旅費交通費";
            row.KrHKmkName = cmbHojo.Text.Trim();
            row.Tekiyo = txtTekiyo.Text.Trim();
            row.Kingaku = decimal.Parse(txtKin.Text.Trim());
            row.KsKmkName = "現金";
            row.KsHKmkName = "";
            Static.ShiwakeDs.Shiwake.AddShiwakeRow(row);
        }

        /// <summary>
        /// 弥生で取り込めるCSVを出力する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnYayoiCSV_Click(object sender, EventArgs e)
        {
            OutputCSV(ClickType.YayoiCsv);
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

        private void btnShiwakeAdd_Click(object sender, EventArgs e)
        {
            OutputCSV(ClickType.Shiwake);
        }
    }
}
