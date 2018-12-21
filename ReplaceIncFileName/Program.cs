using System;
using System.Linq;

namespace ReplaceIncFileName
{
    using System.IO;
    using System.Text.RegularExpressions;

    class Program
    {
        private static string find, replace;
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Must specify two arguments, find and replace.");
            }
            else
            {
                find = args[0];
                replace = args[1];
                ReplaceInDirectory(new DirectoryInfo(Environment.CurrentDirectory));
            }

        }

        static void ReplaceInDirectory(DirectoryInfo directory)
        {
            if (directory.Name.StartsWith(".")) return;

            foreach (var subdir in directory.GetDirectories())
            {
                ReplaceInDirectory(subdir);

                var lastPartOfPath = Regex.Match(subdir.FullName, @"[^\\]+$").Value;
                if (lastPartOfPath.Contains(find))
                {
                    var firstPartOfPath = Regex.Match(subdir.FullName, @"^.*\\").Value.Trim('\\');
                    var newDirName = Path.Combine(firstPartOfPath, lastPartOfPath.Replace(find, replace));
                    subdir.MoveTo(newDirName);
                }
            }

            foreach (var fileInfo in directory.GetFiles())
            {
                string path = fileInfo.FullName;
                if (fileInfo.Name.Contains(find))
                {
                    path = Path.Combine(fileInfo.DirectoryName, fileInfo.Name.Replace(find, replace));
                    fileInfo.MoveTo(path);
                }

                if (!File.ReadAllBytes(path).Contains((byte) 0))
                {
                    var contents = File.ReadAllText(path);
                    if (contents.Contains(find))
                    {
                        contents = contents.Replace(find, replace);
                        File.WriteAllText(path, contents);
                    }
                }
            }

        }
    }
}
