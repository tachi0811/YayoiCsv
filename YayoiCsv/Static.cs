﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Forms;
using System.Data;
using System.Xml.XmlConfiguration;
using System.Xml.Linq;
using System.Xml;

namespace YayoiCsv
{
    /// <summary>
    /// 静的メンバ
    /// </summary>
    public static class Static
    {
        #region プロパティ

        /// <summary>
        /// 年度
        /// </summary>
        public static int Nendo
        {
            get
            {
                return _nendo;
            }
            set
            {
                if (_nendo == 0)
                {
                    Static.InitShowChildForm();
                }

                _nendo = value;

                ParentForm.Title = "弥生会計 経費入力 サポート (" + value.ToString() + "年度)";
                SetNendoHoliday(Static.Nendo);
                ReadShiwakeXML();
                ParentForm.MenuEnabled(true);


            }
        }
        private static int _nendo;
        /// <summary>
        /// 休日
        /// </summary>
        public static List<Holiday> HolidayList { get; set; }

        /// <summary>
        /// 税率リスト
        /// </summary>
        public static List<Tax> TaxList { get; set; }

        /// <summary>
        /// 科目リスト
        /// </summary>
        public static List<Kamoku> KmkList { get; set; }

        /// <summary>
        /// 補助科目リスト
        /// </summary>
        public static List<HojoKamoku> HKmkList { get; set; }

        /// <summary>
        /// 親画面
        /// </summary>
        public static MDIParent ParentForm { get; set; }

        /// <summary>
        /// 画面リスト
        /// </summary>
        private static List<Form> ChildFormList { get; set; }

        /// <summary>
        /// 経費入力用のDataSet
        /// </summary>
        public static ShiwakeDs ShiwakeDs { get; set; }

        #endregion

        #region 仕訳データ関連

        public static void ShiwakeChanged()
        {
            decimal uriage = 0;
            decimal keihi = 0;

            if (ShiwakeDs.Shiwake.Count == 0)
            {
                return;
            }

            // ------------------------------------------------------------------
            // 経費合計を計算
            // ------------------------------------------------------------------
            ShiwakeDs.KeihiSum.Clear();
            var sumKeihiGroup = ShiwakeDs.Shiwake.Where(x => x.KmkKbn == KmkKbn.経費.ToString()).
                GroupBy(x => x.KrKmkName).Select(g => new { KrKmkName = g.Key, Kingaku = g.Sum(x => x.Kingaku) });

            foreach (var kmkSum in sumKeihiGroup)
            {
                var row = ShiwakeDs.KeihiSum.NewKeihiSumRow();
                row.KrKmkName = kmkSum.KrKmkName;
                row.Kingaku = kmkSum.Kingaku;
                ShiwakeDs.KeihiSum.AddKeihiSumRow(row);
            }

            // ------------------------------------------------------------------
            // 仕訳合計（現金、普通預金、定期預金、売掛金、事業貸主）
            // ------------------------------------------------------------------
            ShiwakeDs.ShisanSum.Clear();
            var sumGKr = ShiwakeDs.Shiwake.Where(x => x.KmkKbn == KmkKbn.資産.ToString()).
                GroupBy(x => x.KrKmkName).Select(g => new { KmkName = g.Key, Kingaku = g.Sum(x => x.Kingaku) });

            var sumGKs = ShiwakeDs.Shiwake.
                GroupBy(x => x.KsKmkName).Select(g => new { KmkName = g.Key, Kingaku = g.Sum(x => x.Kingaku) });

            foreach (var shisan in KmkList.Where(x => x.KmkKbn == KmkKbn.資産.ToString()))
            {
               
                var row = ShiwakeDs.ShisanSum.NewShisanSumRow();

                decimal kr = sumGKr.Where(x => x.KmkName == shisan.KmkName).Select(x => x.Kingaku).FirstOrDefault();
                decimal ks = sumGKs.Where(x => x.KmkName == shisan.KmkName).Select(x => x.Kingaku).FirstOrDefault();

                decimal kingaku = kr - ks;

                row.KmkName = shisan.KmkName;

                if (shisan.KmkName.IndexOf("売上高") >= 0)
                {
                    row.Kingaku = -1 * kingaku;
                    uriage = row.Kingaku;
                }
                else
                {
                    row.Kingaku = kingaku;
                }
                ShiwakeDs.ShisanSum.AddShisanSumRow(row);
            }

            var rowKeihi = ShiwakeDs.ShisanSum.NewShisanSumRow();
            rowKeihi.KmkName = KmkKbn.経費.ToString();
            rowKeihi.Kingaku = ShiwakeDs.KeihiSum.Sum(x => x.Kingaku);
            keihi = rowKeihi.Kingaku;
            ShiwakeDs.ShisanSum.AddShisanSumRow(rowKeihi);

            var rowUriageKeihi = ShiwakeDs.ShisanSum.NewShisanSumRow();
            rowUriageKeihi.KmkName = "売上高 - 軽費";
            rowUriageKeihi.Kingaku = uriage - keihi;
            ShiwakeDs.ShisanSum.AddShisanSumRow(rowUriageKeihi);

            // ------------------------------------------------------------------
            // 現金出納帳
            // ------------------------------------------------------------------
            SetGenkinSuitocho();

            // ------------------------------------------------------------------
            // 預金出納帳
            // ------------------------------------------------------------------
            SetYokinSuitocho();

        }

        private static void SetGenkinSuitocho()
        {
            ShiwakeDs.GenkinSuitocho.Clear();
            decimal zanKingaku = 0;
            foreach (var shiwake in ShiwakeDs.Shiwake.Where(x => x.KrKmkName == "現金" || x.KsKmkName == "現金").OrderByDescending(x => x.KmkKbn).ThenBy(x => x.CustomDate))
            {
                var row = ShiwakeDs.GenkinSuitocho.NewGenkinSuitochoRow();
                row.KmkName = shiwake.KrKmkName;

                row.CustomDate = shiwake.CustomDate;

                if (shiwake.KrKmkName == "現金")
                {
                    zanKingaku += shiwake.Kingaku;
                    row.KrKingaku = shiwake.Kingaku;
                    row.KsKingaku = 0;
                }
                else if (shiwake.KsKmkName == "現金")
                {
                    zanKingaku -= shiwake.Kingaku;
                    row.KrKingaku = 0;
                    row.KsKingaku = shiwake.Kingaku;
                }

                row.ZanKingaku = zanKingaku;

                ShiwakeDs.GenkinSuitocho.AddGenkinSuitochoRow(row);
            }
        }

        private static void SetYokinSuitocho()
        {
            ShiwakeDs.YokinSuitocho.Clear();
            decimal zanKingaku = 0;
            foreach (var shiwake in ShiwakeDs.Shiwake.Where(x => x.KrKmkName.IndexOf("預金") >= 0 || x.KsKmkName.IndexOf("預金") >= 0).OrderBy(x => x.KmkKbn).ThenBy(x => x.CustomDate))
            {
                var row = ShiwakeDs.YokinSuitocho.NewYokinSuitochoRow();
                row.KrKmkName = shiwake.KrKmkName;
                row.KrHKmkName = shiwake.KrHKmkName;

                row.KsKmkName = shiwake.KsKmkName;
                row.KsHKmkName = shiwake.KsHKmkName;

                row.CustomDate = shiwake.CustomDate;

                if (shiwake.KrKmkName.IndexOf("預金") >= 0)
                {
                    zanKingaku += shiwake.Kingaku;
                    row.KrKingaku = shiwake.Kingaku;
                    row.KsKingaku = 0;
                }
                else if (shiwake.KsKmkName.IndexOf("預金") >= 0)
                {
                    zanKingaku -= shiwake.Kingaku;
                    row.KrKingaku = 0;
                    row.KsKingaku = shiwake.Kingaku;
                }

                row.ZanKingaku = zanKingaku;

                ShiwakeDs.YokinSuitocho.AddYokinSuitochoRow(row);
            }
        }

        /// <summary>
        /// 仕訳データの番号取得
        /// </summary>
        /// <returns></returns>
        public static int GetShiwakeNo()
        {
            return Static.ShiwakeDs.Shiwake.Count == 0 ? 1 : Static.ShiwakeDs.Shiwake.Max(x => x.No) + 1;
        }

        #endregion

        #region 初期処理

        /// <summary>
        /// 初期処理
        /// </summary>
        public static void SystemInit()
        {
            SetTax();
            SetKmk();
            SetHKmk();
            ShiwakeDs = new ShiwakeDs();

            ReadFormPosition();
            
            var positionRow = Static.ShiwakeDs.FormPosition.Where(x => x.FormName == typeof(MDIParent).FullName).FirstOrDefault();
            if (positionRow != null)
            {
                if ((FormWindowState)positionRow.WindowState == FormWindowState.Maximized)
                {
                    Static.ParentForm.WindowState = FormWindowState.Maximized;
                }
                Static.ParentForm.Height = positionRow.Height;
                Static.ParentForm.Width = positionRow.Width;
            }

            ReadNendo();

        }

        #endregion

        #region 画面管理

        /// <summary>
        /// 子画面を開く
        /// </summary>
        /// <param name="type">開く画面</param>
        public static void ShowChildForm(Type type, int? height = null, int? width = null, int? top = null, int? left = null)
        {
            if (ChildFormList == null)
            {
                ChildFormList = new List<Form>();
            }

            if (!ChildFormList.Any(x => x.GetType() == type))
            {
                var form = (Form)Activator.CreateInstance(type) ;
                ChildFormList.Add(form);
                form.MdiParent = ParentForm;

                if (height != null) form.Height = (int)height;
                if (width != null) form.Width = (int)width;

                if (top != null)
                {
                    form.StartPosition = FormStartPosition.Manual;
                    form.Top = (int)top;
                    form.Left = (int)left;
                }
                form.Show();
            }
            else
            {
                var form = ChildFormList.Where(x => x.GetType() == type).First();
                form.Show();
                form.Activate();
            }
        }

        /// <summary>
        /// 子画面が開いているか確認
        /// </summary>
        /// <param name="type">調べる画面</param>
        /// <returns>true : false </returns>
        public static bool IsOpenChildForm(Type type)
        {
            if (ChildFormList.Any(x => x.GetType() == type))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 子画面を非表示にする
        /// </summary>
        /// <param name="type">閉じる画面</param>
        public static void CloseChildForm(Type type)
        {
            var form = ChildFormList.Where(x => x.GetType() == type).FirstOrDefault();
            if (form != null)
            {
                form.Close();
                ChildFormList.Remove(form);
            }
        }

        /// <summary>
        /// 初期子画面の配置
        /// </summary>
        public static void InitShowChildForm()
        {
            foreach (var row in Static.ShiwakeDs.FormPosition)
            {
                if (row.FormName != ParentForm.GetType().FullName)
                {
                    var t = Type.GetType(row.FormName);
                    ShowChildForm(t, row.Height, row.Width, row.Top, row.Left);
                }
            }
        }

        /// <summary>
        /// 画面の位置情報を保持する
        /// </summary>
        public static void SaveFormPosition()
        {
            Static.ShiwakeDs.FormPosition.Clear();
            SetFormPositionRow(ParentForm);

            foreach (var form in ChildFormList)
            {
                SetFormPositionRow(form);
            }
            Static.ShiwakeDs.FormPosition.WriteXml(@"xml\form_position.xml");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        static void SetFormPositionRow(Form form)
        {
            var row = Static.ShiwakeDs.FormPosition.NewFormPositionRow();
            row.FormName = form.GetType().FullName;
            row.WindowState = (int)form.WindowState;
            row.Height = form.Height;
            row.Width = form.Width;
            row.Top = form.Top;
            row.Left = form.Left;
            Static.ShiwakeDs.FormPosition.AddFormPositionRow(row);
        }

        /// <summary>
        /// 前回のフォームの場所を読み込む
        /// </summary>
        public static void ReadFormPosition()
        {
            if (System.IO.File.Exists(@"xml\form_position.xml"))
            {
                Static.ShiwakeDs.FormPosition.ReadXml(@"xml\form_position.xml");
            }
        }

        public  static void WriteNendo()
        {
            var xDoc = new XmlDocument();
            xDoc.AppendChild(xDoc.CreateXmlDeclaration("1.0", "UTF-8", null));
            var xEle = xDoc.CreateElement("last_nendo");
            xEle.InnerText = Nendo.ToString();
            xDoc.AppendChild(xEle);
            xDoc.Save(@"xml\nendo.xml");
        }

        public static void ReadNendo()
        {
            if (System.IO.File.Exists(@"xml\nendo.xml"))
            {
                var xDoc = XDocument.Load(@"xml\nendo.xml");
                Nendo = (int)xDoc.Element("last_nendo");
            }
        }

        #endregion

        #region 拡張メソッド

        /// <summary>
        /// DoubleQuoteをつける
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string ToDoubleQuote(this string target)
        {
            return "\"" + target + "\"";
        }

        #endregion

        #region 勘定科目／補助科目

        /// <summary>
        /// 科目リストを作成
        /// </summary>
        public static void SetKmk()
        {
            Static.KmkList = new List<Kamoku>();

            var xDoc = XDocument.Load(@"xml\kmk.xml");
            var xItems = xDoc.Element("kmks").Elements("kmk");

            foreach (var item in xItems.Select(x => new Kamoku()
            {
                KmkKbn = (string)x.Element("kbn"),
                KmkName = (string)x.Element("name")
            }))
            {
                Static.KmkList.Add(item);
            }
        }

        /// <summary>
        /// 補助科目リストを作成
        /// </summary>
        public static void SetHKmk()
        {
            Static.HKmkList = new List<HojoKamoku>();

            var xDoc = XDocument.Load(@"xml\hkmk.xml");
            var xItems = xDoc.Element("hkmks").Elements("hkmk");

            foreach (var item in xItems.Select(x => new HojoKamoku()
            {
                KmkName = (string)x.Element("kmkname"),
                HkmkName = (string)x.Element("hkmkname")
            }))
            {
                Static.HKmkList.Add(item);
            }
        }

        #endregion

        #region 消費税

        /// <summary>
        /// 消費税リストを作成
        /// </summary>
        public static void SetTax()
        {
            Static.TaxList = new List<Tax>();

            var xDoc = XDocument.Load(@"xml\tax.xml");
            var xItems = xDoc.Element("items").Elements("item");

            foreach (var item in xItems.Select(x => new Tax()
            {
                TaxRate = (int)x.Element("tax_rate"),
                StartDate = (DateTime)x.Element("start_date"),
                EndDate = (DateTime)x.Element("end_date")

            }))
            {
                Static.TaxList.Add(item);
            }
        }

        #endregion

        #region 祝日

        /// <summary>
        /// 指定した年度で、HolidayListを作成する
        /// </summary>
        /// <param name="nendo">年度</param>
        public static void SetNendoHoliday(int nendo)
        {
            Static.HolidayList = new List<Holiday>();

            if (!System.IO.File.Exists(System.IO.Path.Combine(Holiday.HolidayXmlFolder, nendo.ToString() + ".xml")))
            {
                // 月別の休日を作成
                CreateXmlHolidayMonth(nendo);

                // 年別の休日を作成
                CreateXmlHolidayYear(nendo);

                // 月別の休日Xmlを削除
                DeleteXmlHolidayMonth(nendo);
            }

            var xDoc = XDocument.Load(System.IO.Path.Combine(Holiday.HolidayXmlFolder, nendo.ToString() + ".xml"));
            var xHolidays = xDoc.Element("Holidays").Elements("Holiday");

            foreach (var holiday in xHolidays.Select(x => new Holiday() { Date = (DateTime)x.Element("Day"), Name = (string)x.Element("Name") }))
            {
                Static.HolidayList.Add(holiday);
            }
        }

        /// <summary>
        /// Xmlファイル作成（月別休日）
        /// </summary>
        static void CreateXmlHolidayMonth(int nendo)
        {
            if (System.IO.Directory.Exists(Holiday.HolidayXmlFolder) == false) System.IO.Directory.CreateDirectory(Holiday.HolidayXmlFolder);

            for (int i = 1; i < 13; i++)
            {
                string filePath = System.IO.Path.Combine(Holiday.HolidayXmlFolder, string.Concat(nendo, i.ToString("00"), ".xml"));

                if (System.IO.File.Exists(filePath) == false)
                {
                    var request = System.Net.WebRequest.Create("http://www.finds.jp/ws/calendar.php?y=" + nendo + "&m=" + i.ToString());
                    using (var response = request.GetResponse())
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            using (var reader = new System.IO.StreamReader(stream, Encoding.UTF8))
                            {
                                var writeXml = new System.Text.StringBuilder();

                                do
                                {
                                    string writeLine = reader.ReadLine();

                                    // 不要タグの削除
                                    if (writeLine.IndexOf("calendar") > 0
                                        || writeLine.IndexOf("status") > 0
                                        || writeLine.IndexOf("year") > 0
                                        || writeLine.IndexOf("month") > 0
                                        || writeLine.IndexOf("firstwday") > 0
                                        || writeLine.IndexOf("days") > 0
                                        || writeLine.IndexOf("hdays") > 0
                                        || writeLine.IndexOf("argument") > 0
                                        || writeLine.IndexOf("<type") > 0
                                        || writeLine.IndexOf("</type") > 0
                                        || writeLine.IndexOf("level") > 0)

                                    {
                                        continue;
                                    }
                                    else if (writeLine.IndexOf("/result") > 0)
                                    {
                                        writeXml.Append("</days>");
                                    }
                                    else if (writeLine.IndexOf("result") > 0)
                                    {
                                        writeXml.Append("<days>");
                                    }
                                    else
                                    {
                                        writeXml.Append(writeLine.Trim());
                                    }

                                } while (!reader.EndOfStream);

                                using (var writer = new System.IO.StreamWriter(filePath, false, Encoding.UTF8))
                                {
                                    writer.Write(writeXml.ToString());
                                }
                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Xmlファイル作成（年別休日）
        /// </summary>
        static void CreateXmlHolidayYear(int nendo)
        {

            string createFilePath = System.IO.Path.Combine(Holiday.HolidayXmlFolder, nendo.ToString() + ".xml");

            // ファイルが存在するなら、処理しない
            if (System.IO.File.Exists(createFilePath)) return;

            var xmlDocument = new System.Xml.XmlDocument();
            var xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            var eleHolidays = xmlDocument.CreateElement("Holidays");

            xmlDocument.AppendChild(xmlDeclaration);
            xmlDocument.AppendChild(eleHolidays);

            for (int i = 1; i < 13; i++)
            {
                string filePath = System.IO.Path.Combine(Holiday.HolidayXmlFolder, string.Concat(nendo.ToString(), i.ToString("00"), ".xml"));

                // xml から、休日を取得
                var xDoc = XDocument.Load(filePath);
                var days = xDoc.Element("days").Elements("day");

                foreach (var day in days.Where(x => (int)x.Element("htype") > 0).Select(x => new Holiday
                { Date = new DateTime(nendo, i, (int)x.Element("mday")), Name = (string)x.Element("hname") }))
                {
                    var eleHoliday = xmlDocument.CreateElement("Holiday");
                    var eleDay = xmlDocument.CreateElement("Day");
                    var eleName = xmlDocument.CreateElement("Name");

                    eleDay.InnerText = day.Date.ToString();
                    eleName.InnerText = day.Name;

                    eleHoliday.AppendChild(eleDay);
                    eleHoliday.AppendChild(eleName);

                    eleHolidays.AppendChild(eleHoliday);
                }
            }

            xmlDocument.Save(createFilePath);
        }

        /// <summary>
        /// 休日(月単位)のXMLファイルの削除
        /// </summary>
        /// <param name="nendo"></param>
        static void DeleteXmlHolidayMonth(int nendo)
        {
            if (System.IO.Directory.Exists(Holiday.HolidayXmlFolder) == false) return;

            for (int i = 1; i < 13; i++)
            {
                string filePath = System.IO.Path.Combine(Holiday.HolidayXmlFolder, string.Concat(nendo, i.ToString("00"), ".xml"));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }

        #endregion

        #region 弥生会計で取り込める形式でCSV出力

        /// <summary>
        /// 仕訳データ
        /// </summary>
        public static string CreateYayoiCSVShiwake()
        {
            var sb = new StringBuilder();

            foreach (ShiwakeDs.ShiwakeRow row in Static.ShiwakeDs.Shiwake)
            {
                var date = new DateTime(Static.Nendo, int.Parse(row.CustomDate.Substring(0, 2)), int.Parse(row.CustomDate.Substring(2, 2)));

                sb.AppendLine(CreateYayoiCSVRow(date, row.KrKmkName, row.KrHKmkName, row.Kingaku.ToString(), row.Tekiyo, row.KsKmkName, row.KsHKmkName));
            }

            return sb.ToString();

        }

        /// <summary>
        /// 弥生のCSV形式で成形したデータを作成する
        /// </summary>
        /// <param name="date">日付</param>
        /// <param name="krKmkNm">借方科目名</param>
        /// <param name="krHkmkNm">借方補助科目名</param>
        /// <param name="krKn">借方金額</param>
        /// <param name="tekiyo">摘要</param>
        /// <param name="ksKmkNm">貸方科目コード</param>
        /// <returns></returns>
        public static string CreateYayoiCSVRow(DateTime date, string krKmkNm, string krHkmkNm, string krKn, string tekiyo = "", string ksKmkNm = "現金", string ksHkmkNm = "")
        {
            // 消費税算出
            var tax = Static.TaxList.Where(x => x.StartDate <= date && x.EndDate >= date).FirstOrDefault();
            string taxString = "";
            if (tax == null)
            {
                taxString = "課対仕入込0%";
            }
            else
            {
                taxString = "課対仕入込" + tax.TaxRate.ToString() + "%";
            }

            // ==============================
            // 弥生会計のフォーマット
            // ==============================
            //  1 - 必須 - 識別フラグ
            //  2 - 任意 - 伝票No
            //  3 - 任意 - 決算
            //  4 - 必須 - 取引日付
            //  5 - 必須 - 借方勘定科目
            //  6 - 任意 - 借方補助科目
            //  7 - 任意 - 借方部門
            //  8 - 必須 - 借方税区分
            //  9 - 必須 - 借方金額
            // 10 - 任意 - 借方税金額
            // 11 - 必須 - 貸方勘定科目
            // 12 - 任意 - 貸方補助科目
            // 13 - 任意 - 貸方部門
            // 14 - 必須 - 貸方税区分
            // 15 - 必須 - 貸方金額
            // 16 - 任意 - 貸方税金額
            // 17 - 任意 - 摘要
            // 18 - 任意 - 番号
            // 19 - 任意 - 期日
            // 20 - 必須 - タイプ
            // 21 - 任意 - 生成元
            // 22 - 任意 - 仕訳メモ
            // 23 - 任意 - 付箋1
            // 24 - 任意 - 付箋2
            // 25 - 必須 - 調整
            // ==============================
            return string.Concat("2000".ToDoubleQuote(), ",",
                "".ToDoubleQuote() + ",",
                "".ToDoubleQuote() + ",",
                date.ToString("yyyy/MM/dd").ToDoubleQuote(), ",",
                krKmkNm.ToDoubleQuote(), ",",
                krHkmkNm.ToDoubleQuote(), ",",
                "".ToDoubleQuote() + ",",
                taxString.ToDoubleQuote(), ",",
                krKn.ToDoubleQuote(), ",",
                "0".ToDoubleQuote(), ",",
                ksKmkNm.ToDoubleQuote(), ",",
                "".ToDoubleQuote() + ",",
                "".ToDoubleQuote() + ",",
                "対象外".ToDoubleQuote(), ",",
                krKn.ToDoubleQuote(), ",",
                "0".ToDoubleQuote(), ",",
                tekiyo.ToDoubleQuote(), ",",
                "".ToDoubleQuote() + ",",
                "".ToDoubleQuote() + ",",
                "0".ToDoubleQuote(), ",",
                "".ToDoubleQuote() + ",",
                "".ToDoubleQuote() + ",",
                "0".ToDoubleQuote(), ",",
                "0".ToDoubleQuote(), ",",
                "no".ToDoubleQuote());
        }

        #endregion

        #region 弥生会計のCSVデータを取得

        /// <summary>
        /// 仕訳データ取込
        /// </summary>
        /// <param name="path"></param>
        public static void AddShiwakeFromCSV(bool isAppend)
        {
            string path = "";
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "CSV Files|*.csv";
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    path = dialog.FileName;
                }
                else
                {
                    return;
                }
            }

            try
            {

                if (!isAppend)
                {
                    SaveShiwakeXML();
                    Static.ShiwakeDs.Shiwake.Clear();
                }

                new Exception();

                // ==============================
                // 弥生会計のフォーマット
                // ==============================
                //  1 - 必須 - 識別フラグ
                //  2 - 任意 - 伝票No
                //  3 - 任意 - 決算
                //  4 - 必須 - 取引日付      ○
                //  5 - 必須 - 借方勘定科目  ○
                //  6 - 任意 - 借方補助科目  ○
                //  7 - 任意 - 借方部門
                //  8 - 必須 - 借方税区分
                //  9 - 必須 - 借方金額      ○
                // 10 - 任意 - 借方税金額
                // 11 - 必須 - 貸方勘定科目  ○
                // 12 - 任意 - 貸方補助科目  ○
                // 13 - 任意 - 貸方部門
                // 14 - 必須 - 貸方税区分
                // 15 - 必須 - 貸方金額
                // 16 - 任意 - 貸方税金額
                // 17 - 任意 - 摘要          ○
                // 18 - 任意 - 番号
                // 19 - 任意 - 期日
                // 20 - 必須 - タイプ
                // 21 - 任意 - 生成元
                // 22 - 任意 - 仕訳メモ
                // 23 - 任意 - 付箋1
                // 24 - 任意 - 付箋2
                // 25 - 必須 - 調整
                // ==============================

                // 
                // Ole DBと同じになるように処理を組み立てる
                // // Ole は、インストールが必要だから廃止
                // 
                var parser = new TextFieldParser(path, Encoding.GetEncoding("Shift_JIS"));
                parser.SetDelimiters(",");

                var dt = new DataTable();

                for (int i = 1; i < 26; i++) dt.Columns.Add("F" + i.ToString(), typeof(string));

                while (!parser.EndOfData)
                {
                    var csvCells = parser.ReadFields();
                    var row = dt.NewRow();

                    if (csvCells.Length != 25)
                    {
                        MessageBox.Show("取込データの列数が弥生の仕訳データと異なるため、取り込めません。", "取込エラー", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    for (int i = 0; i < 25; i++)
                    {
                        row[i] = csvCells[i];
                    }
                    dt.Rows.Add(row);
                }

                //// CSVファイル読み込み(Ole.4.0 をインストールしないと動かないので、独自で読み込む)
                //string conString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + System.IO.Path.GetDirectoryName(path) + ";Extended Properties=\"text;HDR=No;FMT=Delimited\"";
                //var con = new System.Data.OleDb.OleDbConnection(conString);
                //var da = new System.Data.OleDb.OleDbDataAdapter("SELECT * FROM [" + System.IO.Path.GetFileName(path) + "]", con);

                
                //da.Fill(dt);

                //if (dt.Columns.Count != 25)
                //{
                //    MessageBox.Show("取込データの列数が弥生の仕訳データと異なるため、取り込めません。", "取込エラー", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}

                dt.Columns.Add("Error", typeof(string));

                int rowNo = 1;

                foreach (DataRow row in dt.Rows)
                {
                    string errorMsg = "";
                    DateTime o = new DateTime();

                    // 取引日付
                    if (row["F4"].ToString() == "")
                    {
                        errorMsg += "取引日付が空白です;";
                    }
                    else
                    {
                        if (!DateTime.TryParse(row["F4"].ToString(), out o))
                        {
                            errorMsg += "取引日付が日付ではありません;";
                        }
                        if (o.Year != Static.Nendo)
                        {
                            errorMsg += "取引日付が年度と異なります;";
                        }
                    }

                    // 勘定科目
                    if (row["F5"].ToString() == "")
                    {
                        errorMsg += "借方勘定科目が空白です;";
                    }
                    else if (!Static.KmkList.Any(x => x.KmkName == row["F5"].ToString()))
                    {
                        errorMsg += "借方勘定科目がシステムで認識できない為、外取り込めません;";
                    }
                    decimal d = 0;
                    if (row["F9"].ToString() == "")
                    {
                        errorMsg += "借方金額が空白です;";
                    }
                    else
                    {
                        if (!decimal.TryParse(row["F9"].ToString(), out d))
                        {
                            errorMsg += "借方金額が数値ではありません;";
                        }
                    }
                    if (row["F11"].ToString() == "")
                    {
                        errorMsg += "貸方勘定科目が空白です;";
                    }
                    else if (!Static.KmkList.Any(x => x.KmkName == row["F11"].ToString()))
                    {
                        errorMsg += "貸方勘定科目がシステムで認識できない為、外取り込めません;";
                    }

                    if (errorMsg != "")
                    {
                        row["Error"] = rowNo.ToString() + "行目 : " + errorMsg;
                    }
                    else
                    {
                        var shiwake = Static.ShiwakeDs.Shiwake.NewShiwakeRow();
                        shiwake.No = GetShiwakeNo();
                        shiwake.KmkKbn = KmkList.Where(x => row["F5"].ToString() == x.KmkName).Select(x => x.KmkKbn).First();
                        shiwake.CustomDate = o.ToString("MMdd");
                        shiwake.KrKmkName = row["F5"].ToString();
                        shiwake.KrHKmkName = row["F6"].ToString();
                        shiwake.Kingaku = d;
                        shiwake.KsKmkName = row["F11"].ToString();
                        shiwake.KsHKmkName = row["F12"].ToString();
                        shiwake.Tekiyo = row["F17"].ToString();
                        Static.ShiwakeDs.Shiwake.AddShiwakeRow(shiwake);
                    }

                    rowNo++;
                }

                // エラーデータは、ファイルで出力
                var errorRows = dt.Select("Error <> ''");

                var sb = new StringBuilder();
                foreach (DataRow row in errorRows)
                {
                    string rowLine = "";
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (col.ColumnName == "Error")
                        {
                            rowLine += row[col.ColumnName].ToString().ToDoubleQuote();
                        }
                        else
                        {
                            rowLine += row[col.ColumnName].ToString().ToDoubleQuote() + ",";
                        }
                    }
                    sb.AppendLine(rowLine);
                }

                using (var stream = new System.IO.StreamWriter(path + ".err"))
                {
                    stream.Write(sb.ToString(), false, Encoding.GetEncoding("SJIS"));
                }

                // 処理を終了
                if (errorRows.Count() == 0)
                {
                    MessageBox.Show("取込が正常に終了しました。", "CSV取込", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("取込が正常に終了しました。\n [ 取込エラー行は、.err として、出力されました。 ] ", "CSV取込", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                SaveShiwakeXML();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CSV取込エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ReadShiwakeXML();
            }
        }

        #endregion

        #region 仕訳データの保存／読み込み

        /// <summary>
        /// 仕訳データ保存
        /// </summary>
        /// <param name="dt"></param>
        public static void SaveShiwakeXML()
        {
            
            if (!System.IO.Directory.Exists(@"xml\shiwake"))
            {
                System.IO.Directory.CreateDirectory(@"xml\shiwake");
            }
            Static.ShiwakeDs.Shiwake.WriteXml(@"xml\shiwake\" + Static.Nendo.ToString() + ".xml");
            ShiwakeChanged();
        }

        /// <summary>
        /// 仕訳データ読み込み
        /// </summary>
        public static void ReadShiwakeXML()
        {
            Static.ShiwakeDs.Shiwake.Clear();
            Static.ShiwakeDs.KeihiSum.Clear();
            Static.ShiwakeDs.ShisanSum.Clear();

            if (System.IO.File.Exists(@"xml\shiwake\" + Static.Nendo.ToString() + ".xml"))
            {
                Static.ShiwakeDs.Shiwake.ReadXml(@"xml\shiwake\" + Static.Nendo.ToString() + ".xml");
            }
            ShiwakeChanged();
        }

        #endregion

    }
}
