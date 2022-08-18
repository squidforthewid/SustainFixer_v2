// See https://aka.ms/new-console-template for more information

using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using SustainFixer.Midi;
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
                { ".chart",     path => RewriteChartFile(path) }
            };

            FileProcessor.ProcessDirectory(args);

            Console.WriteLine("Done. Press ENTER to exit.", ConsoleColor.Cyan);
            Console.ReadLine();
        }

        #region Chart file methods

        #endregion
    }
}
