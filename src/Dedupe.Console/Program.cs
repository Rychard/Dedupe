using System;
using System.Diagnostics;
using System.IO;
using Dedupe.Core;

namespace  Dedupe.CommandLine
{
    public class Program
    {
        public static Int32 Main(string[] args)
        {
            if(args.Length == 0) 
            { 
                PrintUsage();
                return -1;
            }

            String action = args[0];
            String source;
            String target;

            if(args.Length == 3)
            {
                source = args[1];
                target = args[2];
            }
            else if(args.Length == 2)
            {
                source = Directory.GetCurrentDirectory();
                target = args[1];
            }
            else
            {
                source = Directory.GetCurrentDirectory();
                target = source;
            } 

            if(!Directory.Exists(source) || !Directory.Exists(target))
            {
                Console.WriteLine();
                Console.WriteLine("ERROR: The source or target directories does not exist.");
                Console.WriteLine();
                return -1;
            }

            switch(action)
            {
                case "compress":
                    return Compress(source, target);

                case "expand":
                    return Expand(source, target);

                default:
                    PrintUsage();
                    return -1;
            }
        }

        public static Int32 Compress(String source, String target)
        {
            Folder folderSource = new Folder(source);
            Folder folderTarget = new Folder(target);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();            

            Deduplicator dedupe = new Deduplicator(folderSource);
            dedupe.CompressAsync(folderTarget).Wait();
            
            Console.WriteLine($"Time Taken: {stopwatch.ElapsedMilliseconds} milliseconds");
            return 0;
        }

        public static Int32 Expand(String source, String target)
        {
            Folder folderSource = new Folder(source);
            Folder folderTarget = new Folder(target);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();  

            Deduplicator dedupe = new Deduplicator(folderSource);
            dedupe.ExpandAsync().Wait();

            Console.WriteLine($"Time Taken: {stopwatch.ElapsedMilliseconds} milliseconds");
            return 0;
        }

        public static void PrintUsage()
        {
            String codeBase = System.Reflection.Assembly.GetEntryAssembly().CodeBase;
            String name = Path.GetFileName(codeBase);

            Console.WriteLine();
            Console.WriteLine($"USAGE: {name} <compress|expand> <source-directory> <target-directory>");
            Console.WriteLine($"       {name} <compress|expand> <target-directory>");
            Console.WriteLine($"       {name} <compress|expand>");
            Console.WriteLine();
        }
    }


}

