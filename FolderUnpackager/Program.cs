using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using PictureSorterAddons;
using System.Threading;

namespace FolderUnpackager
{
    class Program
    {
        public static int NumberOfCopies(string fileName, string destDirName)
        {
            int count = 0;
            
            DirectoryInfo dirInfo = new DirectoryInfo(destDirName);
            var files = dirInfo.EnumerateFiles().OrderBy(x => x.Name).ToList();

            #region OLD
            
            foreach (var item in files)
            {
                string name = Path.GetFileNameWithoutExtension(item.Name);
                if (name.ToLower().Contains(fileName.ToLower()))
                {
                    count++;
                }
            }
            #endregion
            
            return count;
        }


        public static void RecursiveUnpackager(DirectoryInfo dirInfo, string destName)
        {
            var directories = dirInfo.EnumerateDirectories().ToList();

            if (directories.Count != 0)
            {
                foreach (var d in directories)
                {
                    Console.WriteLine(d.FullName);
                    Thread.Sleep(50);
                    RecursiveUnpackager(d, destName);
                    var files = d.EnumerateFiles()
                        .Where(x => Enum.GetNames(typeof(ExtensionType)).Contains(x.Extension.ToLower().Replace(".","")))
                        .ToList();
                    foreach (var f in files)
                    {
                        try
                        {
                            File.Move(f.FullName, destName + "\\" + f.Name, false);
                        }
                        catch (IOException)
                        {
                            var modifiedName = f.Name.Insert(
                                Methods.FindExtensionPoint(f.Name),
                                $" ({NumberOfCopies(Path.GetFileNameWithoutExtension(f.Name), destName)})");

                            File.Move(f.FullName, destName + "\\" + modifiedName, false);
                        }
                        Console.WriteLine($"{f.FullName} MOVED!");
                        Thread.Sleep(50);

                    }
                }
            }
            else
            {
                var files = dirInfo.EnumerateFiles()
                        .Where(x => Enum.GetNames(typeof(ExtensionType)).Contains(x.Extension.ToLower().Replace(".","")))
                        .ToList();
                foreach (var f in files)
                {
                    try
                    {
                        File.Move(f.FullName, destName + "\\" + f.Name, false);
                    }
                    catch (IOException)
                    {
                        
                        var modifiedName = f.Name.Insert(
                            Methods.FindExtensionPoint(f.Name),
                            $" ({NumberOfCopies(Path.GetFileNameWithoutExtension(f.Name), destName)})");

                        
                        File.Move(f.FullName, destName + "\\" + modifiedName, false);
                    }
                    Console.WriteLine($"{f.FullName} MOVED!");
                    Thread.Sleep(50);
                }
            }
        }

        static void Main(string[] args)
        {
            //var dirInfo = new DirectoryInfo(Directory.GetParent(Directory.GetCurrentDirectory()).FullName);
            //RecursiveUnpackager(dirInfo, dirInfo.FullName);



            var dirInfo = new DirectoryInfo(args[0]); //local path of folder
            var to = new DirectoryInfo(args[0]);

            RecursiveUnpackager(dirInfo, to.FullName);

            Console.WriteLine("END");
            Console.ReadLine();
        }
    }
}
