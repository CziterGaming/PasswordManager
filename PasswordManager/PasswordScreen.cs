using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace PasswordManager
{
    public partial class PasswordScreen : Form
    {
        internal static string password;
        public PasswordScreen()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            password = textBox1.Text;
            Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            password = textBox1.Text;
            Close();
        }

        private void openPasswordsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Environment.GetEnvironmentVariable("LOCALAPPDATA") + "\\CziterGaming\\PasswordManager\\");
        }
    }
}
