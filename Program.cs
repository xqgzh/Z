using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Z
{
    class Program
    {
        [STAThread]
        static void Main(params object[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine(args[1]);
            }
        }
    }
}
