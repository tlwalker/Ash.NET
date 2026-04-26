namespace Net.RichardLord.Ash.Core;

/// <summary>
/// The base class for a system.
/// </summary>
public abstract class SystemBase
{
    internal SystemBase? Previous { get; set; }
    internal SystemBase? Next { get; set; }
    internal int Priority { get; set; }

    public abstract void AddToGame(IGame game);
    public abstract void RemoveFromGame(IGame game);
    public abstract void Update(double time);
}
