namespace gameEngine;

using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text.Json;
using datamodel;
public record Encounter
(
     int EnemyHP,
     string Personality,
     int PlayerHP,
     int PlayerAttack,
    List<NpcAction> EnemyActions
);

public class EncounterBuilder
{
    public GameState makeEncounter(String fileName, IUI UI)
    {

        //List<NpcAction> actions = JsonSerializer.Deserialize<List<NpcAction>>("gameEngine/NPCActions.json");

        string encounterJsonString = File.ReadAllText($"gameEngine/encounters/{fileName}");
        Encounter encounterData = JsonSerializer.Deserialize<Encounter>(encounterJsonString);

        Random rnd = new Random();
        int npchp = 25 + rnd.Next(50); //replace with AI decision on hp scaling
        int php = npchp - 10 + rnd.Next(20); //same for NPC hp
        if(true)
        {
            npchp = encounterData.EnemyHP;
            php=encounterData.PlayerHP;
        }

        Npc TheGuy = new Npc(npchp);
        Player player = new Player(php);
        TheGuy.npcActions = encounterData.EnemyActions;
        //Console.WriteLine(encounterData.Personality);
/*
        TheGuy.npcActions.Add(actions[9]);
        TheGuy.npcActionsFull = actions;
        
        TheGuy.npcActions[0].Id=0;
        if (TheGuy.actions.Count==0){
            for (int i = 0; i < 5; i++)
            {
                int r = rnd.Next(actioncount);
                if (TheGuy.npcActions.Contains(actions[r]))
                {
                    i--;
                }
                else
                {
                    TheGuy.npcActions.Add(actions[r]);
                    TheGuy.npcActions[i+1].Id=i+1;
                }
                
            }
        }*/

        return new GameState(player, TheGuy, UI);
    }
}