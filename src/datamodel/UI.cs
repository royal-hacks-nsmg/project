namespace datamodel;

public interface IUI
{
    public (ActionType, string?) PerformUserAction();
    public void DisplayMessage(string message);
    public void ShowLoading(Action action);
    public void UpdateStats(int playerHp, int npcHp);
}