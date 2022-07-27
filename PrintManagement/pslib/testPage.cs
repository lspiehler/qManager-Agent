using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PrintManagement.pslib
{
    class testPage
    {
        [DllImport("printui.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern void PrintUIEntryW(IntPtr hwnd, IntPtr hinst, string lpszCmdLine, int nCmdShow);
        public string Print(string name)
        {
            Console.WriteLine("Printing test page to " + name);
            try
            {
                var printParams = string.Format(@"/k /q /n{0}", "\"" + name + "\"");
                PrintUIEntryW(IntPtr.Zero, IntPtr.Zero, printParams, 0);
            }
            catch(Exception e)
            {
                errorlog el = new errorlog();
                el.write(e.ToString(), Environment.StackTrace, "error");
                return e.ToString();
            }

            return null;
        }
    }
}
