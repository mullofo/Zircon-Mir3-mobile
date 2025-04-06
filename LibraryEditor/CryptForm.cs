using System.Windows.Forms;

namespace LibraryEditor
{
    public partial class CryptForm : Form
    {
        public static CryptForm form1; //其他方法可以调用窗体控件
        public CryptForm()
        {
            InitializeComponent();
            form1 = this; //其他方法可以调用窗体控件
        }
    }
}
