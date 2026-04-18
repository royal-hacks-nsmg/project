namespace gameEngine;

public abstract class PlayerAbstract(int initialHP)
{
    public int HP { get; set; } = initialHP;
}

public class Player(int initialHP) : PlayerAbstract(initialHP)
{
    public int Attack = 5;
}