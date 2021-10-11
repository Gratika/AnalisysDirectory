using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalisysDirectory
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            if (args.Length == 0)
            {
                Console.WriteLine("Имя файла не указано!");
                return;
            }

            string path =args[0];

            try
            {
                DirectoryParser dp = new DirectoryParser(path);
                dp.start();
               
            }
            catch (Exception err)
            {
                System.Console.WriteLine("ERROR! " + err.Message);
            }
        }
    }
}
