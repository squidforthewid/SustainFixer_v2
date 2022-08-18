using System;
using Console = SustainFixer.Debug;

namespace SustainFixer
{
    public class FileProcessor
    {
        internal static Dictionary<string, Action<string>> fileTypeMap = new Dictionary<string, Action<string>>();
        static int totalFiles = 0;
        static int filesProcessed = 0;

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

        public static void ProcessDirectory(string[] args)
        {
            totalFiles = CountFilesToProcess(args);

            foreach (string path in args)
            {
                if (File.Exists(path))
                {
                    if (fileTypeMap.ContainsKey(Path.GetExtension(path)))
                        ProcessFile(path, fileTypeMap[Path.GetExtension(path)]);
                }
                else if (Directory.Exists(path))
                {
                    ProcessSubdirectory(path);
                }
                else
                {
                    Console.WriteLine($"{path} is not a valid file or directory.");
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
            // Process all files found in directory
            Console.WriteLine(
                "SCANNING " + targetDirectory + "...\n",
                ConsoleColor.White);

            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string path in fileEntries)
            {
                if (fileTypeMap.ContainsKey(Path.GetExtension(path)))
                    ProcessFile(path, fileTypeMap[Path.GetExtension(path)]);
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                ProcessSubdirectory(subdirectory);
            }
        }

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
        /// <param name="methodName">The method to be executed on the file; string parameter of method should be path of the file.</param>
        static void ProcessFile(string path, Action<string> methodName)
        {
            filesProcessed++;
            Console.WriteLine($"\rProcessing file {filesProcessed}/{totalFiles}...", ConsoleColor.White);

            methodName(path);
        }
    }
}
