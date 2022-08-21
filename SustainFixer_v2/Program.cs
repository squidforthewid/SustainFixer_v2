// See https://aka.ms/new-console-template for more information

using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using SustainFixer.Midi;
using SustainFixer.Chart;
using Console = SustainFixer.Debug;

namespace SustainFixer
{
    public class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            args = new string[]
            {
                "C:\\Users\\805ca\\OneDrive\\Documents\\GitHub\\SustainFixer_v2\\3 Doors Down - Kryptonite\\notes - Copy.chart"
            };
#endif

            //Key: file extension to search for
            //Value: method to be called when that extension is found
            FileProcessor.fileTypeMap = new Dictionary<string, Action<string>>()
            {
                { ".mid",       path => MidiEditor.ProcessMidFile(path) },
                { ".chart",     path => ChartEditor.ProcessChartFile(path) }
            };

            FileProcessor.ProcessDirectory(args);

            Console.WriteLine("Done. Press ENTER to exit.", ConsoleColor.Cyan);
            Console.ReadLine();
        }
    }
}
