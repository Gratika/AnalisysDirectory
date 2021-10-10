using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnalisysDirectory
{
    public class DirectoryParser
    {
        // создаем семафор
        static Semaphore sem = new Semaphore(3, 3);
        static AutoResetEvent gate = new AutoResetEvent(true);        
        CancellationToken token;

        private string Path;
        private List<FileInfo> FileList;
        //Конструктор
        public DirectoryParser(string path)
        {
            this.Path = path;
            FileList = new List<FileInfo>();
            
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;
        }

        //метод Main класса
        public void start()
        {
            sem.WaitOne();
            new Thread(getListFile).Start();
            while (true)
            {
                string com = Console.ReadLine();
                if (com.StartsWith("COUNT", StringComparison.OrdinalIgnoreCase))
                    getCountFile();
                else if (com.StartsWith("PATH", StringComparison.OrdinalIgnoreCase))
                    getListFullPath();
                else if (com.StartsWith("SIZE", StringComparison.OrdinalIgnoreCase))
                    getTotalSize();
            }
            sem.Release();
            
        }

       

        private void getListFile()
        {
            DirectoryInfo di = new DirectoryInfo(this.Path);
            FileInfo[] files = di.GetFiles();
            gate.WaitOne();
            FileList.AddRange(files);
            gate.Set();
            DirectoryInfo[] directories = di.GetDirectories();
            foreach(var d in directories)
            {
                getListFile();
            }
        }
        private void getTotalSize()
        {
            throw new NotImplementedException();
        }

        private void getListFullPath()
        {
            throw new NotImplementedException();
        }

        private void getCountFile()
        {
            throw new NotImplementedException();
        }
    }
}
