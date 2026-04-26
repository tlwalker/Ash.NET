namespace Net.RichardLord.Ash.Core;

public interface IGame
{
    bool Updating { get; }
    event Action? UpdateComplete;

    NodeList GetNodeList<T>();
    NodeList GetNodeList(Type type);
    void ReleaseNodeList<T>();
    void ReleaseNodeList(Type nodeClass);

    void AddEntity(Entity entity);
    void AddSystem(SystemBase system, int priority);
    
    void RemoveEntity(Entity entity);
    void RemoveAllEntities();
    
    SystemBase? GetSystem(Type type);
    void RemoveSystem(SystemBase system);
    void RemoveAllSystems();

    void Update(double time);
}
