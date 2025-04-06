using Server.Envir;
using System;
using System.Windows.Forms;

namespace Server.Helpers
{
    public class HelperExcel
    {
        public static void ExportExcel(params Type[] types)
        {
            foreach (var item in types)
            {
                var exportType = typeof(HelperExcel<>).MakeGenericType(item);
                exportType.GetMethod("ExportExcel", new Type[] { typeof(string) }).Invoke(null, new object[] { Config.ExportPath });
            }
            MessageBox.Show("导出成功!", "提示");
        }
    }
}
