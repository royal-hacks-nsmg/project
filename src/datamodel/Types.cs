namespace datamodel;

public enum EmotionState
{
    NEUTRAL,
    ANGRY,
    HAPPY,
    SAD,
}

public enum Players
{
    PLAYER,
    NPC,
}

public enum ActionType
{
    ATTACK,
    TALK,
}

public record NpcAction(int Id, string Description, List<EmotionState> Precondition, Players Target, int HPNominalChange);
