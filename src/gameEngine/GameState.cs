namespace gameEngine;

using datamodel;

public class GameState(Player player, Npc npc, IUI UI)
{
    public Npc Npc = npc;
    public Player Player = player;
    public IUI UI { get; } = UI;

    public void Run()
    {
        bool stop = false;
        UI.UpdateStats(Player.HP, Npc.HP);

        while (!stop)
        {
            if (Player.HP <= 0)
            {
                UI.DisplayMessage("GAME OVER - YOU DIED\n\n(The world fades to black as you succumb to your wounds...)");
                break;
            }

            (ActionType type, string? userText) = UI.PerformUserAction();
            switch (type)
            {
                case ActionType.ATTACK:
                    UI.DisplayMessage($"Player attacks Npc for {Player.Attack} damage");
                    Npc.HP -= Player.Attack;
                    UI.UpdateStats(Player.HP, Npc.HP);
                    if (Npc.HP <= 0)
                    {
                        UI.DisplayMessage(Npc.PerformDeathScenario());
                        stop = true;
                        UI.DisplayMessage("Player killed Npc! YOU WIN!");
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
            UI.DisplayMessage($"Round end. Player health: {Player.HP}. Npc health: {Npc.HP}");
        }
    }
}
