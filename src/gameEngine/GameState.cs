namespace gameEngine;

using datamodel;

public class GameState(Player player, Npc npc, IUI UI)
{
    public Npc Npc = npc;
    public Player Player = player;

    public void Run()
    {
        bool stop = false;
        while (!stop)
        {
            if (Player.HP <= 0)
            {
                Console.WriteLine("Npc killed player");
                break;
            }

            (ActionType type, string? userText) = UI.PerformUserAction();
            switch (type)
            {
                case ActionType.ATTACK:
                    Console.WriteLine($"Player attacks Npc for {Player.Attack} damage");
                    Npc.HP -= Player.Attack;
                    if (Npc.HP <= 0)
                    {
                        Npc.PerformDeathScenario();
                        stop = true;
                        Console.WriteLine("Player killed Npc");
                    }
                    else
                    {
                        Npc.PerformTurn(this, type);
                    }
                    break;
                case ActionType.TALK:
                    if (userText is null)
                    {
                        throw new Exception("Something went wrong");
                    }
                    Npc.PerformTurn(this, type, userText);
                    break;
            }
            Console.WriteLine($"Round end. Player health: {Player.HP}. Npc health: {Npc.HP}");
        }
    }
}
