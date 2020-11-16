using System.IO;
using Newtonsoft.Json;

namespace Zork
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string defaultGameFilename = "Zork.json";
            string gameFilename = (args.Length > 0 ? args[(int)CommandLineArguments.GameFilename] : defaultGameFilename);

            var outputService = new ConsoleOutputService();
            var inputService = new ConsoleInputService();

            Game game = JsonConvert.DeserializeObject<Game>(File.ReadAllText(gameFilename));
            game.Output = outputService;
            game.Input = inputService;

            game.Initalize(inputService, outputService);

            while (game.IsRunning)
            {
                outputService.Write("\n>");
                inputService.ProcessInput();
            }

            game.Shutdown();
        }

        private enum CommandLineArguments
        {
            GameFilename = 0
        }
    }
}