using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows.Forms;

namespace YayoiCsv
{
    class Program
    {
        [STAThread]
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        static void Main()
        {
            
            Static.ParentForm = new MDIParent();

            Static.SystemInit();

            Application.Run(Static.ParentForm);
        }


    }
}
