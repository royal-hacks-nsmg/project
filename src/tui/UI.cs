namespace tui;

using datamodel;

public class UI : IUI
{
    public (ActionType, string?) PerformUserAction()
    {
        while (true)
        {
            Console.Write("You can perform the following actions:\n 1) Attack\n 2) Talk\nType the number of your choice: ");
            var input = Console.ReadKey();
            Console.WriteLine();
            switch (input.Key)
            {
                case ConsoleKey.D1:
                    return (ActionType.ATTACK, null);
                case ConsoleKey.D2:
                    var text = Console.ReadLine();
                    return (ActionType.TALK, text);
                default:
                    Console.WriteLine("Type a valid number");
                    break;
            }
        }
    }

    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }

    public void ShowLoading(Action action)
    {
        Console.WriteLine("Waiting for response...");
        action();
    }

    public void UpdateStats(int playerHp, int npcHp)
    {
        // TUI doesn't need to do anything special here as it's printed in GameState
    }
}