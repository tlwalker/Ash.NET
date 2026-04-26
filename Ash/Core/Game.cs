namespace Net.RichardLord.Ash.Core;

public class Game<TFamilyType> : IGame where TFamilyType : IFamily, new()
{
    private readonly EntityList _entities = new();
    private readonly SystemList _systems = new();
    private readonly Dictionary<Type, TFamilyType> _families = new();

    /// <summary>
    /// Indicates if the game is currently in its update loop.
    /// </summary>
    public bool Updating { get; private set; }

    /// <summary>
    /// Dispatched when the update loop ends. If you want to add and remove systems from the
    /// game it is usually best not to do so during the update loop. To avoid this you can
    /// listen for this signal and make the change when the signal is dispatched.
    /// </summary>
    public event Action? UpdateComplete;

    /// <summary>
    /// Add an entity to the game.
    /// </summary>
    public void AddEntity(Entity entity)
    {
        _entities.Add(entity);
        entity.ComponentAdded += ComponentAdded;
        entity.ComponentRemoved += ComponentRemoved;
        foreach (var family in _families.Values)
        {
            family.NewEntity(entity);
        }
    }

    /// <summary>
    /// Remove an entity from the game.
    /// </summary>
    public void RemoveEntity(Entity entity)
    {
        entity.ComponentAdded -= ComponentAdded;
        entity.ComponentRemoved -= ComponentRemoved;
        foreach (var family in _families.Values)
        {
            family.RemoveEntity(entity);
        }
        _entities.Remove(entity);
    }

    /// <summary>
    /// Remove all entities from the game.
    /// </summary>
    public void RemoveAllEntities()
    {
        while (_entities.Head is not null)
        {
            RemoveEntity(_entities.Head);
        }
    }

    private void ComponentAdded(Entity entity, Type componentClass)
    {
        foreach (var family in _families.Values)
        {
            family.ComponentAddedToEntity(entity, componentClass);
        }
    }

    private void ComponentRemoved(Entity entity, Type componentClass)
    {
        foreach (var family in _families.Values)
        {
            family.ComponentRemovedFromEntity(entity, componentClass);
        }
    }

    /// <summary>
    /// Get a collection of nodes from the game, based on the type of the node required.
    /// </summary>
    public NodeList GetNodeList<T>() => GetNodeList(typeof(T));

    public NodeList GetNodeList(Type type)
    {
        if (_families.TryGetValue(type, out var family))
        {
            return family.NodeList;
        }

        family = new TFamilyType();
        family.Setup(this, type);

        _families.Add(type, family);
        
        for (var entity = _entities.Head; entity is not null; entity = entity.Next)
        {
            family.NewEntity(entity);
        }
        return family.NodeList;
    }

    /// <summary>
    /// If a NodeList is no longer required, this method will stop the game updating
    /// the list and will release all references to the list within the framework
    /// classes, enabling it to be garbage collected.
    /// </summary>
    public void ReleaseNodeList<T>() => ReleaseNodeList(typeof(T));

    public void ReleaseNodeList(Type nodeClass)
    {
        if (_families.TryGetValue(nodeClass, out var family))
        {
            family.CleanUp();
        }
        _families.Remove(nodeClass);
    }

    /// <summary>
    /// Add a system to the game, and set its priority for the order in which the
    /// systems are updated by the game loop.
    /// </summary>
    public void AddSystem(SystemBase system, int priority)
    {
        system.Priority = priority;
        system.AddToGame(this);
        _systems.Add(system);
    }

    /// <summary>
    /// Get the system instance of a particular type from within the game.
    /// </summary>
    public SystemBase? GetSystem(Type type) => _systems.Get(type);

    /// <summary>
    /// Remove a system from the game.
    /// </summary>
    public void RemoveSystem(SystemBase system)
    {
        _systems.Remove(system);
        system.RemoveFromGame(this);
    }

    /// <summary>
    /// Remove all systems from the game.
    /// </summary>
    public void RemoveAllSystems()
    {
        while (_systems.Head is not null)
        {
            RemoveSystem(_systems.Head);
        }
    }

    /// <summary>
    /// Update the game. This causes the game loop to run, calling update on all the
    /// systems in the game.
    /// </summary>
    public void Update(double time)
    {
        Updating = true;
        for (var system = _systems.Head; system is not null; system = system.Next)
        {
            system.Update(time);
        }
        Updating = false;
        UpdateComplete?.Invoke();
    }
}
