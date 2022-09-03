using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SustainFixer.Debug;

// TODO: Implement stopwatch
namespace SustainFixer
{
    /// <summary>
    /// Recursive file/directory processor.
    /// </summary>
    public class FileProcessor
    {
        public static int filesProcessed = 0;
        public static Dictionary<string, Exception> badFiles = new();

        // Dictionary for storing instructions on how to handle files based on their extension.
        internal static Dictionary<string, Action<string>> fileTypeMap = new Dictionary<string, Action<string>>();
        static int totalFiles = 0;

        /// <summary>
        /// Process a given directory.
        /// </summary>
        /// <param name="args"></param>
        public static void ProcessDirectory(string[] args)
        {
            totalFiles = CountFilesToProcess(args);

            foreach (string path in args)
            {
                if (File.Exists(path))
                {
                    if (fileTypeMap.ContainsKey(Path.GetExtension(path)))
                    {
                        ProcessFile(path, fileTypeMap[Path.GetExtension(path)]);
                    }
                }
                else if (Directory.Exists(path))
                {
                    ProcessSubdirectory(path);
                }
                else
                {
                    Console.WriteLine($"{path} is not a valid file or directory.");
                    Console.ReadLine();
                }
            }
        }

        /// <summary>
        /// Process all files in the directory passed in, recurse on any directories that are found, and process contained files.
        /// </summary>
        /// <param name="targetDirectory">Target directory path.</param>
        /// <param name="fileExtensions">All file extensions to process.</param>
        static void ProcessSubdirectory(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string path in fileEntries)
            {
                if (fileTypeMap.ContainsKey(Path.GetExtension(path)))
                {
                    ProcessFile(path, fileTypeMap[Path.GetExtension(path)]);
                }
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                ProcessSubdirectory(subdirectory);
            }
        }

        /// <summary>
        /// Returns a count of all files that will be processed in the given directories.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static int CountFilesToProcess(string[] args)
        {
            int i = 0;

            foreach (string path in args)
            {
                if (File.Exists(path))
                {
                    if (fileTypeMap.ContainsKey(Path.GetExtension(path)))
                        i++;
                }
                else if (Directory.Exists(path))
                {
                    CountSubdirectory(path, () => i++);
                }
            }

            return i;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetDirectory"></param>
        /// <param name="Act"></param>
        static void CountSubdirectory(string targetDirectory, Action Act)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string path in fileEntries)
            {
                if (fileTypeMap.ContainsKey(Path.GetExtension(path)))
                    Act();
            }

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                CountSubdirectory(subdirectory, Act);
            }
        }

        /// <summary>
        /// Processes a file.
        /// </summary>
        /// <param name="path">The path of the file being processed.</param>
        /// <param name="fileType">The extension of the file to be processed. File will only be processed if it has this extension.</param>
        /// <param name="Act">The method to be executed on the file; string parameter of method should be path of the file.</param>
        static void ProcessFile(string path, Action<string> Act)
        {
            filesProcessed++;

            try
            {
                Console.Write($"\rProcessing file {filesProcessed}/{totalFiles}...", ConsoleColor.White);

                Act(path);
            }
            catch (NullReferenceException e)
            {
/*                Console.WriteLine(e.Message);
                Console.WriteLine(e.GetType().ToString());
                Console.ReadLine();*/

                badFiles.Add(path, e);
                return;
            }
        }
    }
}
