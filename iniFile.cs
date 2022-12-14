using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DatePicker
{
    public class iniFIle
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static string Read(string iniFilePath, string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", temp, 255, iniFilePath);
            return temp.ToString();
        }

        public static void Write(string iniFilePath, string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, iniFilePath);
        }
    }
}
