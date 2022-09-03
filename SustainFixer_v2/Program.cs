// See https://aka.ms/new-console-template for more information

using SustainFixer.Midi;
using SustainFixer.Chart;
using Console = SustainFixer.Debug;

namespace SustainFixer
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Key: file extension to search for
            //Value: method to be called when that extension is found
            FileProcessor.fileTypeMap = new Dictionary<string, Action<string>>()
            {
                { ".mid",       path => MidiEditor.ProcessMidFile(path) },
                { ".chart",     path => ChartEditor.ProcessChartFile(path) }
            };

            Console.WriteLine("Thank you for using SustainFixer_v2.\n\n", ConsoleColor.White);

            FileProcessor.ProcessDirectory(args);

            Console.WriteLine($"\n{FileProcessor.filesProcessed - FileProcessor.badFiles.Count} succeeded, " +
                $"{FileProcessor.badFiles.Count} failed.",
                FileProcessor.badFiles.Count == 0 ? ConsoleColor.Green : ConsoleColor.Yellow);

            if (FileProcessor.badFiles.Count != 0)
            {
                Console.WriteLine("Bad songs:", ConsoleColor.Red);

                foreach (var file in FileProcessor.badFiles)
                {
                    Console.WriteLine($"{file.Key} ({file.Value.GetType()})", ConsoleColor.Red);
                }
            }

            Console.WriteLine("Done. Press ENTER to exit.", ConsoleColor.Green);
            Console.WriteLine("Please reach out to Squidicus#3153 on Discord to report bugs and errors.", ConsoleColor.DarkGray);

            Console.ReadLine();
        }
    }
}
