namespace gameEngine;

using System.Runtime.Serialization;
using System.Text.Json;
using datamodel;
public class EncounterBuilder
{
    public GameState makeEncounter(String fileName, IUI UI)
    {
        string filename = "src/gameEngine/NPCActions.json";
        string jsonstring = File.ReadAllText(filename);
        List<NpcAction> actions = JsonSerializer.Deserialize<List<NpcAction>>(jsonstring);

        Random rnd = new Random();

        int npchp = 25 + rnd.Next(50); //replace with AI decision on hp scaling
        Npc TheGuy = new Npc(npchp);
        int actioncount = actions.Count;

        Player player = new Player(npchp - 10 + rnd.Next(20)); //same for NPC hp
        foreach (NpcAction ac in actions)
        {
            string validstates = "";
            foreach (EmotionState e in ac.Precondition)
            {
                validstates = validstates + e.ToString() + " ";
            }
            string damageorheal = "";
            int changenumber = ac.HPNominalChange;
            if (ac.HPNominalChange < 0)
            {
                damageorheal = "Heals";
                changenumber *= -1;
            }
            else
            {
                damageorheal = "Damages";
            }

            string s = $"{ac.Id}: {ac.Description}, Valid Emotions: [{validstates}], Effect: {damageorheal} {ac.Target.ToString()} for {changenumber} hp";
            // Console.WriteLine(s);
        }
        //replace with AI solution later to choose whatever is appropriate
        for (int i = 0; i < 5; i++)
        {
            TheGuy.npcActions.Add(actions[rnd.Next(actioncount)]);
        }
        TheGuy.npcActions.Add(actions[9]);
        TheGuy.npcActionsFull = actions;
        TheGuy.npcActions.Add(actions[7]);
        TheGuy.npcActionsFull = actions;
        TheGuy.npcActions.Add(actions[18]);
        TheGuy.npcActionsFull = actions;


        return new GameState(player, TheGuy, UI);
    }
}