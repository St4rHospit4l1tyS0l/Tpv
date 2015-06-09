using System.Windows;
using System.Windows.Input;

namespace Tpv.Ui.View
{
    /// <summary>
    /// Interaction logic for MsgWnd.xaml
    /// </summary>
    public partial class MsgWnd
    {
        public MsgWnd(string sMsg)
        {
            InitializeComponent();
            //You cannot skip required modifier groups the first time you modify an item.
            TxtMsg.Text = sMsg;
        }

        private void Grid_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

    }
}
