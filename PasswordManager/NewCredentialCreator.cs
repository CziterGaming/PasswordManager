using System;
using System.Windows.Forms;

namespace PasswordManager
{
    public partial class NewCredentialCreator : Form
    {
        internal static string service;
        internal static string username;
        internal static string password;
        public NewCredentialCreator()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Create(null);
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            Create(e);
        }

        void Create(KeyEventArgs e)
        {
            if (!(e is null))
                if (e.KeyCode != Keys.Enter)
                    return;

            var icc1 = InvalidCharactersCheck.ContainsForbiddenChars(textBox1.Text);
            if (icc1.contains)
            {
                MessageBox.Show($"You used forbidden character: {icc1.character}");
                return;
            }
            service = textBox1.Text;

            var icc2 = InvalidCharactersCheck.ContainsForbiddenChars(textBox2.Text);
            if (icc2.contains)
            {
                MessageBox.Show($"You used forbidden character: {icc2.character}");
                return;
            }
            username = textBox2.Text;

            var icc3 = InvalidCharactersCheck.ContainsForbiddenChars(textBox3.Text);
            if (icc3.contains)
            {
                MessageBox.Show($"You used forbidden character: {icc3.character}");
                return;
            }
            password = textBox3.Text;

            Close();
        }
    }
}
