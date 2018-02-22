using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.IO.Abstractions;



namespace FIPMessenger
{
    class FIPDirectoryReader
    {
        static SimpleLogger _logger;
        static bool consoleOut = true;

        private static List<string> getDicomFileList()
        {
            // Searches directories for Dicom files and produces a list of those files
            var directories = new List<string>();
            return directories;
        }

        private static void sendToRabbitMQ(Object doc)
        {
            // Sends the given document to the rabbit MQ queue/exchange

        }

        public static List<string> findDicomInDirectoryTree(string directoryRoot)
        {
            // Returns a list of directories which contain dicom files

            //TODO(Ruairidh) Check/enforce the expected directory structure?
            //TODO(Ruairidh) Detect and manage zip / archives etc.

            var dicomDirectories = new List<string>();
            int totalDicom = 0;

            var dirStack = new Stack<string>();
            IFileSystem fileSystem = new FileSystem();

            dirStack.Push(directoryRoot);

            while (dirStack.Count > 0)
            {

                string dir = dirStack.Pop();

                try
                {
                    if (fileSystem.Directory.Exists(dir))
                    {

                        // Add subdirectories to the list of directories to explore
                        foreach (string subdir in fileSystem.Directory.GetDirectories(dir))
                        {
                            dirStack.Push(subdir);
                        }

                        int nDicomFiles = fileSystem.Directory.EnumerateFiles(dir, "*.dcm").Count();

                        if (nDicomFiles > 0)
                        {
                            dicomDirectories.Add(dir);
                            totalDicom += nDicomFiles;
                        }
                    }
                    else if (fileSystem.File.Exists(dir))
                    {
                        Console.WriteLine("That's a file not a directory! (" + dir + ")");
                    }
                    else
                    {
                        Console.WriteLine("That doesn't exist: " + dir);
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("Exception somewhere here");
                }
            }

            // Can't use logger here if calling this function from DicomGenerator
            //if (dicomDirectories.Count > 0)        
            //_logger.WriteLine(String.Format("Found {0} directories containing {1} dicom files", dicomDirectories.Count, totalDicom), globalSw.Elapsed.TotalSeconds);

            return dicomDirectories;
        }


        static void Main(string[] args)
        {
            _logger = new SimpleLogger(@".\logs\", consoleOut);
            _logger.WriteLine("Starting ...");

            FIPRabbitMQHandler handler = new FIPRabbitMQHandler();

            handler.sendMessage("Hi there for the first time!");
            string message = handler.receiveMessage();
            Console.WriteLine("Got message:", message);
            //FIPDirectoryReader.getDicomFileList();

            _logger.WriteLine("Done ...");

            Console.ReadLine();
        }
    }
}
