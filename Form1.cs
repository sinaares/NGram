using System;
using System.Windows.Forms;

namespace NgramUI
{
    public partial class Form1 : Form
    {
        private NGramAnalyzer analyzer = new NGramAnalyzer();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFolderPath.Text = folderDialog.SelectedPath;
                    btnStart.Enabled = true;
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFolderPath.Text))
            {
                MessageBox.Show("Please select a folder first.");
                return;
            }

            txtOutput.Clear();
            txtOutput.Text = "Analyzing, please wait...";

            try
            {
                // Perform analysis
                string result = analyzer.AnalyzeFolder(txtFolderPath.Text);
                txtOutput.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
