using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using XpertiumSharp.Logic;
using XpertiumSharp.Utils;

namespace Xpertium
{
    public partial class MainForm : Form
    {
        private XLogicalModelParser parser;

        public MainForm()
        {
            InitializeComponent();
        }

        private void ExitToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            log.Clear();
            log.AppendText("Building...\n");
            var logger = new Logger(log);

            try
            {
                var db = parser.Parse(listing.Text);
                var target = parser.ParseTarget(this.target.Text, db);
                log.AppendText("Success\n");

                log.AppendText("Executing...\n");
                var interpreter = new XInterpreter(db, logger);

                if (interpreter.Run(target, out List<XPredicate> solutions))
                {
                    log.AppendText("TRUE\nSolutions:\n");

                    foreach (var solution in solutions)
                    {
                        log.AppendText(solution.ToString() + "\n");
                    }
                }
                else
                {
                    log.AppendText("FALSE");
                }
            }
            catch (Exception ex)
            {
                log.AppendText("Error: " + ex.Message);
            }
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

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            parser = new XLogicalModelParser();
        }
    }
}
