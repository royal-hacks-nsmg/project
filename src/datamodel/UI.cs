namespace datamodel;

public interface IUI
{
    public (ActionType, string?) PerformUserAction();
}