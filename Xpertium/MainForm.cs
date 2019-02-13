using System.Collections.Generic;
using System.IO;
using System.Text;
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
            interpreter.Run(target, out List<XPredicate> solutions);
        }

        private void OpenFile_Click(object sender, System.EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                listing.Clear();
                listing.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var stream = saveFileDialog.OpenFile();

                if (stream != null)
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(listing.Text);
                    stream.Write(byteArray, 0, byteArray.Length);
                    stream.Close();
                }
            }
        }

        private void ClearOutputToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            log.Clear();
        }

        private void Listing_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                e.IsInputKey = true;
            }
        }
    }
}
