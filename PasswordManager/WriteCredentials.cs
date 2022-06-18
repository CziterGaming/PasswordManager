using System;
using System.Threading;
using System.Windows.Forms;

namespace PasswordManager
{
    internal class WriteCredentials
    {
        public static void Write(string username, string password)
        {
            try
            {
                SendKeys.SendWait("%{Tab}");
                Thread.Sleep(200);

                foreach (char character in username)
                {
                    SendKeys.SendWait(character.ToString());
                }

                SendKeys.SendWait("{Tab}");

                foreach (char character in password)
                {
                    SendKeys.SendWait(character.ToString());
                }

                Thread.Sleep(200);
                SendKeys.Send("~");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to write credentials!\n\n{ex}");
                return;
            }

            return;
        }
    }
}
