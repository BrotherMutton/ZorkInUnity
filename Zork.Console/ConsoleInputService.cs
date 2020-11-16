using System;

namespace Zork
{
    internal class ConsoleInputService : IInputService
    {
        public event EventHandler<string> InputRecieved;

        public void ProcessInput()
        {
            string inputString = Console.ReadLine();
            InputRecieved?.Invoke(this, inputString);
        }
    }
}
