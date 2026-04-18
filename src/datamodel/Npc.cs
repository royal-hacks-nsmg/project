using System.Text.Json;

namespace datamodel;

public class Npc(int initialHP) : PlayerAbstract(initialHP)
{
    public EmotionState EmotionState { get; set; } = EmotionState.NEUTRAL;
    public string PerformDeathScenario()
    {
        return "I am dead :(";
    }
    public List<NpcAction> npcActions = [];
    public void PerformTurn(GameState state, ActionType playerAction, string? userText = null)
    {   
        // Create list of NpcActions whose preconditions are true
        List<NpcAction> validList = [];
        foreach(NpcAction a in npcActions)
        {
            if(a.Precondition.Contains(state.Npc.EmotionState))
            {
                validList.Add(a);
            }
        }
        // Send prompt to Gemini
        // Check if chosen NpcAction is valid
        Random rnd = new Random(); 

        var chosenActionId = rnd.Next(validList.Count()); // replace with AI prompt 
        // If not resend and check again
        // If so then perform action and change emotion
        var chosenAction = npcActions.ElementAt(chosenActionId);
        switch (chosenAction.Target)
        {
            case Players.PLAYER:
                state.Player.HP -= chosenAction.HPNominalChange;
                break;
            case Players.NPC:
                HP += chosenAction.HPNominalChange;
                break;
        }
        Console.WriteLine($"Npc attacks player for {chosenAction.HPNominalChange} damage with {chosenAction.Description}");
        EmotionState = EmotionState.ANGRY;
        // Update UI with emotion and Npc reponse
        Console.WriteLine($"Npc emotion: {EmotionState}\nNPC: Grrrr");
    }
}