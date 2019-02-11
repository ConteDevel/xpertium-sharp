using System.Windows.Forms;
using XpertiumSharp.Logic;

namespace Xpertium
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void ExitToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void RunButton_Click(object sender, System.EventArgs e)
        {
            var interpreter = new XInterpreter(new XDatabase());
            var target = new XPredicate(new XSignature("test", 2), new XVar(XType.Const, "2"), new XVar(XType.Var, "X"));
            interpreter.Run(target);
        }
    }
}
