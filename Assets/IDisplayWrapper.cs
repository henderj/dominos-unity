public interface IDisplayWrapper
{
    public void DisplayGame(Game game);
}

public class DisplayWrapperNone : IDisplayWrapper
{
    public void DisplayGame(Game game){}
}