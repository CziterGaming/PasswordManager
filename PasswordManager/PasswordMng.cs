using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordManager
{
    public partial class PasswordMng : Form
    {
        private Dictionary<string, string[]> accounts = new Dictionary<string, string[]>();
        private static readonly string passwordsFile = Environment.GetEnvironmentVariable("LOCALAPPDATA") + "\\CziterGaming\\PasswordManager\\passwords.ini";
        private static readonly string globalPasswordFile = Environment.GetEnvironmentVariable("LOCALAPPDATA") + "\\CziterGaming\\PasswordManager\\gpassword.ini";
        public PasswordMng()
        {
            InitializeComponent();

            Directory.CreateDirectory(Environment.GetEnvironmentVariable("LOCALAPPDATA") + "\\CziterGaming\\");
            Directory.CreateDirectory(Environment.GetEnvironmentVariable("LOCALAPPDATA") + "\\CziterGaming\\PasswordManager");

            if (!File.Exists(passwordsFile))
            {
                var fs = File.Create(passwordsFile);
                fs.Close();
            }

            if (!File.Exists(globalPasswordFile))
            {
                var gps = new GlobalPasswordCreator();
                gps.ShowDialog();

                EncryptionManager.globalPassword = EncryptionManager.EncryptString(GlobalPasswordCreator.password, GlobalPasswordCreator.password);

                File.WriteAllText(globalPasswordFile, EncryptionManager.globalPassword);
                GlobalPasswordCreator.password = "";
            }
            else
            {
                EncryptionManager.globalPassword = File.ReadAllText(globalPasswordFile);
                ReadCredentials();
            }
            
            if (accounts.Count == 0)
            {
                copyLabel.Visible = false;
                writeLabel.Visible = false;
            }
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            ControlPaint.DrawBorder(e.Graphics, panel.ClientRectangle, Color.White, ButtonBorderStyle.Solid);
        }

        private void EnterButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            try
            {
                new PasswordScreen().ShowDialog();
                EncryptionManager.DecryptString(EncryptionManager.globalPassword, PasswordScreen.password);

                string username = EncryptionManager.DecryptString(accounts[button.Name][0], PasswordScreen.password);
                string password = EncryptionManager.DecryptString(accounts[button.Name][1], PasswordScreen.password);

                WriteCredentials.Write(username, password);
                username = "";
                password = "";
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                MessageBox.Show("incorrect password!");
            }
            PasswordScreen.password = "";
        }
        private void ButtonClickHandler(object sender, EventArgs e)
        {
            try
            {
                new PasswordScreen().ShowDialog();
                EncryptionManager.DecryptString(EncryptionManager.globalPassword, PasswordScreen.password);

                Button button = sender as Button;
                string key = button.Name.Split(';')[0];
                string selector = button.Name.Split(';')[1];

                switch (button.Name.Split(';')[1])
                {
                    case "login":
                        {
                            Clipboard.SetText(EncryptionManager.DecryptString(accounts[key][0], PasswordScreen.password));
                            statusLabel.Text = "Copied to clipboard!";
                            Task.Delay(2000).ContinueWith(t => statusLabel.Text = "");
                            break;
                        }
                    case "pass":
                        {
                            Clipboard.SetText(EncryptionManager.DecryptString(accounts[key][1], PasswordScreen.password));
                            statusLabel.Text = "Copied to clipboard!";
                            Task.Delay(2000).ContinueWith(t => statusLabel.Text = "");
                            break;
                        }
                    case "delete":
                        {
                            var buf = File.ReadAllText(passwordsFile);

                            StringBuilder sb = new StringBuilder();
                            sb.Append(EncryptionManager.EncryptString(key, PasswordScreen.password));
                            sb.Append(";");
                            sb.Append(accounts[key][0]);
                            sb.Append(":");
                            sb.Append(accounts[key][1]);
                            sb.Append("|");

                            buf = buf.Replace(sb.ToString(), "");
                            File.WriteAllText(passwordsFile, buf);

                            accounts.Remove(key);

                            fillFlowLayoutPanel();

                            statusLabel.Text = "Deleted!";
                            Task.Delay(2000).ContinueWith(t => statusLabel.Text = "");
                            break;
                        }
                }
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                MessageBox.Show("incorrect password!");
            }
            PasswordScreen.password = "";
        }

        private void CreateNewPasswordButton_Click(object sender, EventArgs e)
        {
            new NewCredentialCreator().ShowDialog();

            try
            {
                new PasswordScreen().ShowDialog();
                EncryptionManager.DecryptString(EncryptionManager.globalPassword, PasswordScreen.password);

                string service = EncryptionManager.EncryptString(NewCredentialCreator.service, PasswordScreen.password);
                string username = EncryptionManager.EncryptString(NewCredentialCreator.username, PasswordScreen.password);
                string password = EncryptionManager.EncryptString(NewCredentialCreator.password, PasswordScreen.password);
                string build = String.Format("{0};{1}:{2}|", service, username, password);

                File.AppendAllText(passwordsFile, build);
                accounts.Add(EncryptionManager.DecryptString(service, PasswordScreen.password), new string[2] { username, password });

                NewCredentialCreator.service = "";
                NewCredentialCreator.username = "";
                NewCredentialCreator.password = "";

                fillFlowLayoutPanel();
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                MessageBox.Show("incorrect password!");
            }
            PasswordScreen.password = "";
            fillFlowLayoutPanel();
        }
        private void ReadCredentials()
        {
            try
            {
                new PasswordScreen().ShowDialog();
                EncryptionManager.DecryptString(EncryptionManager.globalPassword, PasswordScreen.password);
                string filestring = File.ReadAllText(passwordsFile);

                foreach (var item in filestring.Split('|'))
                {
                    if (String.IsNullOrEmpty(item))
                        continue;

                    var splitted = item.Split(';');

                    string service = splitted[0];
                    string username = splitted[1].Split(':')[0];
                    string pass = splitted[1].Split(':')[1];

                    accounts.Add(EncryptionManager.DecryptString(service, PasswordScreen.password), new string[2] { username, pass });
                }
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                MessageBox.Show("incorrect password!");
                Environment.Exit(0);
            }

            PasswordScreen.password = "";
            fillFlowLayoutPanel();
        }
        private void fillFlowLayoutPanel()
        {
            flowLayoutPanel1.Controls.Clear();
            foreach (var account in accounts)
            {
                Panel panel = new Panel();
                panel.Width = 385;
                panel.Height = 23;
                panel.Margin = new Padding(1);
                //
                Label label = new Label();
                label.Text = account.Key;
                label.ForeColor = Color.White;
                label.Font = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Regular);
                label.Location = Location = new Point(5, 1);
                label.AutoSize = true;
                panel.Controls.Add(label);
                panel.Paint += Panel_Paint;
                //
                Button copyLoginButton = new Button();
                copyLoginButton.Text = "⧉";
                copyLoginButton.Size = new Size(30, 23);
                copyLoginButton.Location = new Point(225, 0);
                copyLoginButton.Name = account.Key + ";login";
                copyLoginButton.FlatStyle = FlatStyle.Flat;
                copyLoginButton.BackColor = Color.FromArgb(44, 46, 48);
                copyLoginButton.ForeColor = Color.White;
                copyLoginButton.Click += ButtonClickHandler;
                ToolTip copyLoginButtonToolTip = new ToolTip();
                copyLoginButtonToolTip.SetToolTip(copyLoginButton, "Copy login to clipboard.");
                panel.Controls.Add(copyLoginButton);
                //
                Button copyPasswordButton = new Button();
                copyPasswordButton.Text = "⧉";
                copyPasswordButton.Size = new Size(30, 23);
                copyPasswordButton.Location = new Point(255, 0);
                copyPasswordButton.Name = account.Key + ";pass";
                copyPasswordButton.FlatStyle = FlatStyle.Flat;
                copyPasswordButton.BackColor = Color.FromArgb(44, 46, 48);
                copyPasswordButton.ForeColor = Color.White;
                copyPasswordButton.Click += ButtonClickHandler;
                ToolTip copyPasswordButtonToolTip = new ToolTip();
                copyPasswordButtonToolTip.SetToolTip(copyPasswordButton, "Copy password to clipboard.");
                panel.Controls.Add(copyPasswordButton);
                //
                Button writeButton = new Button();
                writeButton.Text = "✎";
                writeButton.Size = new Size(30, 23);
                writeButton.Location = new Point(295, 0);
                writeButton.Name = account.Key;
                writeButton.FlatStyle = FlatStyle.Flat;
                writeButton.BackColor = Color.FromArgb(44, 46, 48);
                writeButton.ForeColor = Color.White;
                writeButton.Click += EnterButton_Click;
                ToolTip writeButtonToolTip = new ToolTip();
                writeButtonToolTip.SetToolTip(writeButton, "Write credentials to form.");
                panel.Controls.Add(writeButton);
                //
                Button deleteButton = new Button();
                deleteButton.Text = "X";
                deleteButton.Size = new Size(30, 23);
                deleteButton.Location = new Point(355, 0);
                deleteButton.Name = account.Key + ";delete";
                deleteButton.FlatStyle = FlatStyle.Flat;
                deleteButton.BackColor = Color.FromArgb(44, 46, 48);
                deleteButton.ForeColor = Color.Red;
                deleteButton.Click += ButtonClickHandler;
                ToolTip deleteButtonToolTip = new ToolTip();
                deleteButtonToolTip.SetToolTip(deleteButton, "Delete account.");
                panel.Controls.Add(deleteButton);

                //
                flowLayoutPanel1.Controls.Add(panel);
            }
            
            if (accounts.Count > 0)
            {
                copyLabel.Visible = true;
                writeLabel.Visible = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Info info = new Info();
            info.Show();
        }

        private void openPasswordsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Environment.GetEnvironmentVariable("LOCALAPPDATA") + "\\CziterGaming\\PasswordManager\\");
        }
    }
}