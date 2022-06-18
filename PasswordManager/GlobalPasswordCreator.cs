using System;
using System.Windows.Forms;

namespace PasswordManager
{
    public partial class GlobalPasswordCreator : Form
    {
        internal static string password;
        public GlobalPasswordCreator()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Create(null);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            Create(e);
        }
        void Create(KeyEventArgs e)
        {
            if (!(e is null))
                if (e.KeyCode != Keys.Enter)
                    return;

            var icc = InvalidCharactersCheck.ContainsForbiddenChars(textBox1.Text);
            if (icc.contains)
            {
                MessageBox.Show($"You used forbidden character: {icc.character}");
                return;
            }

            password = textBox1.Text;
            Close();
        }
    }
}
