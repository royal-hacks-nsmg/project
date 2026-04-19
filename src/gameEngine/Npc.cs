using System.Text.Json;
using gemini_wrapper;
using datamodel;

namespace gameEngine;

public class Npc(int initialHP) : PlayerAbstract(initialHP)
{
    GenerateContentSimpleText generateContentSimpleText = new();
    public EmotionState EmotionState { get; set; } = EmotionState.NEUTRAL;
    public string PerformDeathScenario()
    {
        return "I am dead :(";
    }
    public List<NpcAction> npcActions = [];
    public List<NpcAction> npcActionsFull = [];
    public void PerformTurn(GameState state, ActionType playerAction, string? userText = null)
    {
        string geminiText = "";
        switch (playerAction)
        {
            case ActionType.ATTACK:
                geminiText += "The player attacked you\n";
                break;
            case ActionType.TALK:
                geminiText += $"The player said: '{userText}'";
                break;
        }


        // Create list of NpcActions whose preconditions are true
        List<NpcAction> validList = [];
        foreach (NpcAction a in npcActions)
        {
            if (a.Precondition.Contains(state.Npc.EmotionState))
            {
                validList.Add(a);
            }
        }
        var json = JsonSerializer.Serialize(validList);
        geminiText += $"You can choose one, and only one, of these actions:\n{json}.\nPut the id of the chosen action as the Action value of your JSON response as an integer\n";

        // Send prompt to Gemini
        EnemyResponseSchema? response = null;
        state.UI.ShowLoading(() => {
            response = generateContentSimpleText.GenerateContentResponse(geminiText);
        });
        // Check if chosen NpcAction is valid


        // If not resend and check again
        // If so then perform action and change emotion
        var chosenAction = npcActionsFull.ElementAt((int)response.Action);
        switch (chosenAction.Target)
        {
            case Players.PLAYER:
                state.Player.HP -= chosenAction.HPNominalChange;
                break;
            case Players.NPC:
                HP += chosenAction.HPNominalChange;
                break;
        }

        var emotionStateStr = response.EmotionalState;
        switch (emotionStateStr!)
        {
            case "NEUTRAL":
                EmotionState = EmotionState.NEUTRAL;
                break;
            case "ANGRY":
                EmotionState = EmotionState.ANGRY;
                break;
            case "HAPPY":
                EmotionState = EmotionState.HAPPY;
                break;
            case "SAD":
                EmotionState = EmotionState.SAD;
                break;
        }

        state.UI.UpdateStats(state.Player.HP, HP);

        // Send dialogue first (it will be drawn alongside the next blocking message)
        state.UI.DisplayMessage($"NPC: \"{response.Dialogue}\"\n{response.Reasoning}");

        // Send attack description (this will block and show everything together)
        state.UI.DisplayMessage($"Npc attacks player for {chosenAction.HPNominalChange} damage with {chosenAction.Description}");
    }
}