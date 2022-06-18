namespace PasswordManager
{
    internal class InvalidCharactersCheck
    {
        public static (bool contains, char character) ContainsForbiddenChars(string str)
        {
            foreach (char c in str)
            {
                if (c == ' ' || c == '\t' || c == '\n' || c == '\r' || c == '|' || c == ';' || c == ':' || c == '=')
                    return (true, c);
            }
            return (false, ' ');
        }
    }
}
