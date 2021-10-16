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
        static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        static CancellationToken token = cancelTokenSource.Token;

        private string Path;
        private List<FileInfo> FileList;
        private bool isWork;

        //Конструктор
        public DirectoryParser(string path)
        {
            this.Path = path;
            FileList = new List<FileInfo>();
            this.isWork = true;

        }

        //метод Main класса
        public void start()        
        {
            new Thread(getListFile).Start(this.Path);
            while (isWork)
            {
                Console.Write(">");
                string com = Console.ReadLine();
                string fileExtension = getExtension(com);

                if (com.StartsWith("COUNT", StringComparison.OrdinalIgnoreCase))
                {
                    Thread countThread = new Thread(getCountFile);
                    setPriorityThread(countThread, fileExtension);                    
                    countThread.Start(fileExtension);                    
                }
                else
                if (com.StartsWith("PATH", StringComparison.OrdinalIgnoreCase))
                {
                    Thread fullPathThread = new Thread(getListFullPath);
                    setPriorityThread(fullPathThread, fileExtension);                    
                    fullPathThread.Start(fileExtension);
                }
                else
                if (com.StartsWith("SIZE", StringComparison.OrdinalIgnoreCase))

                {
                    Thread sizeThread = new Thread(getTotalSize);
                    setPriorityThread(sizeThread, fileExtension);                   
                    sizeThread.Start(fileExtension);
                }
                else 
                if (com.Equals("q", StringComparison.OrdinalIgnoreCase))
                {
                    cancelTokenSource.Cancel();
                    isWork = false;
                }
                else 
                if(com.Equals("?", StringComparison.OrdinalIgnoreCase)|| com.Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    showHelp();
                }
                else
                {
                    Console.WriteLine("Unknown instruction. Enter \"?\" or \"help\" for help");
                }
                
            }
          
            
        }

        private void showHelp()
        {
            Console.WriteLine("The next commands should be supported:");
            Console.WriteLine("? or HELP - get help for programm");
            Console.WriteLine("q - exit");
            Console.WriteLine("COUNT << file extension >> - Count of currently scanned files with given extension.If the extension is empty command should return the count of all files");
            Console.WriteLine("SIZE << file extension >> - similar to COUNT but produces total size of files with given extension");
            Console.WriteLine("PATH << file extension >> -produces a list of all files with full path found at the moment");
        }

       //получить список файлов в папке(включая вложенные папки)
        private void getListFile(object directoryPath)
        {
            sem.WaitOne();
            
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("Операция прервана токеном");
                return;
            }

            string path = directoryPath as string;
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                isWork = false;
                Console.WriteLine("Directory not found");
                return;
            }

            FileInfo[] files = di.GetFiles();
            gate.WaitOne();
            FileList.AddRange(files);
            gate.Set();

            DirectoryInfo[] directories = di.GetDirectories();            
            foreach(var d in directories)
            {
                getListFile(d.FullName);
            }
           
            sem.Release();
        }
       
        //подсчитать общий размер файлов с указанным расширением
        private void getTotalSize(object fileExtension)
        {
            sem.WaitOne();
            long totalSize = 0;
            if (fileExtension == null)
            {
                FileList.ForEach(f => totalSize += f.Length);
            }
            else
            {
                FileList.FindAll(f => fileExtension.Equals(f.Extension)).ForEach(f => totalSize += f.Length);
            }
            Console.WriteLine($"Total size {fileExtension} file = {totalSize} byte ({totalSize / Math.Pow(1024.0,2):f3} Mbyte)");
            Console.Write(">");
            sem.Release();

        }

        
        //вывести список (полный путь) файлов с указанным расширением
        private void getListFullPath(object fileExtension)
        {
            sem.WaitOne();
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("Операция прервана токеном");
                return;
            }
            Console.WriteLine($"List of {fileExtension} file:");
            if (fileExtension == null)
            {
                FileList.ForEach(f => Console.WriteLine(f.FullName));
            }
            else
            {
                FileList.FindAll(f => fileExtension.Equals(f.Extension)).ForEach(f => Console.WriteLine(f.FullName));
            }
            Console.Write(">");
            sem.Release();
        }
        
        //подсчитать количество файлов с указанным расширением
        private void getCountFile(object fileExtension)
        {
            sem.WaitOne();
            
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("Операция прервана токеном");
                return;
            }
          
            int cnt=0;
            if (fileExtension == null)
            {
               cnt= FileList.Count;
            }
            else
            {
                cnt = FileList.FindAll(f =>  fileExtension.Equals(f.Extension)).Count;
                
            }
            Console.WriteLine($"Found {fileExtension} file - {cnt}");
            Console.Write(">");
            sem.Release();


        }
        //получить расширение файлов из введенной пользователем команды
        public string getExtension(string commandParse)
        {
            string res = null;
            string[] parts = commandParse.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)                
                res = parts[1].StartsWith(".")?parts[1].Trim():"."+ parts[1].Trim();
            return res;
        }
        private void setPriorityThread(Thread thread, string fileExtension)
        {
            if ("docx".Equals(fileExtension, StringComparison.OrdinalIgnoreCase) || "doc".Equals(fileExtension, StringComparison.OrdinalIgnoreCase))
                thread.Priority = ThreadPriority.Highest;
        }
    }
}
