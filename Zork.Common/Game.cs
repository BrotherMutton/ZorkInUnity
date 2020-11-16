using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Zork
{
    public class Game : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public World World { get; private set; }

        public string StartingLocation { get; set; }

        public string WelcomeMessage { get; set; }

        public string ExitMessage { get; set; }

        [JsonIgnore]
        public Player Player { get; private set; }

        [JsonIgnore]
        public bool IsRunning { get; set; }

        [JsonIgnore]
        public Dictionary<string, Command> Commands { get; private set; }

        [JsonIgnore]
        public IOutputService Output { get; set; }

        [JsonIgnore]
        public IInputService Input { get; set; }

        public Game(World world, Player player)
        {
            World = world;
            Player = player;

            Commands = new Dictionary<string, Command>()
            {
                { "QUIT", new Command("QUIT", new string[] { "QUIT", "Q", "BYE" }, Quit) },
                { "LOOK", new Command("LOOK", new string[] { "LOOK", "L" }, Look) },
                { "NORTH", new Command("NORTH", new string[] { "NORTH", "N" }, game => Move(game, Directions.North)) },
                { "SOUTH", new Command("SOUTH", new string[] { "SOUTH", "S" }, game => Move(game, Directions.South)) },
                { "EAST", new Command("EAST", new string[] { "EAST", "E"}, game => Move(game, Directions.East)) },
                { "WEST", new Command("WEST", new string[] { "WEST", "W" }, game => Move(game, Directions.West)) },
                { "SCORE", new Command("SCORE", new string[] { "SCORE" }, game => Output.WriteLine($"Your score would be {game.Player.Score}, in {game.Player.Moves} move(s).")) },
                { "REWARD", new Command ("REWARD", new string[] { "REWARD" }, game => game.Player.Score++) },
            };
        }

        public void Initalize(IInputService input, IOutputService output)
        {
            Assert.IsNotNull(input);
            Input = input;
            input.InputRecieved += InputRecievedHandler;

            Assert.IsNotNull(output);
            Output = output;

            Output.WriteLine(string.IsNullOrWhiteSpace(WelcomeMessage) ? "Welcome to Zork!" : WelcomeMessage);
            IsRunning = true;

            Commands["LOOK"].Action(this);
        }

        public void Shutdown()
        {
            Output.WriteLine(string.IsNullOrWhiteSpace(ExitMessage) ? "Thank you for playing!" : ExitMessage);
        }


        private void InputRecievedHandler(object sender, string inputString)
        {
            Command foundCommand = null;
            foreach (Command command in Commands.Values)
            {
                if (command.Verbs.Contains(inputString, StringComparer.InvariantCultureIgnoreCase))
                {
                    foundCommand = command;
                    break;
                }
            }
            if (foundCommand != null)
            {
                Player.Moves++;
                Room previousRoom = Player.Location;
                foundCommand.Action(this);

                if (previousRoom != Player.Location)
                {
                    Commands["LOOK"].Action(this);
                }
            }
            else
            {
                Output.WriteLine("Unknown command.");
            }
        }

        private static void Move(Game game, Directions direction)
        {
            if (game.Player.Move(direction) == false)
            {
                game.Output.WriteLine("The way is shut!");
            }
            game.Player.Moves++;
        }

        private static void Look(Game game) => game.Output.WriteLine($"{game.Player.Location}\n{game.Player.Location.Description}");

    private static void Quit(Game game) => game.IsRunning = false;

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context) => Player = new Player(World, StartingLocation);
}
}