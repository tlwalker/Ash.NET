namespace Net.RichardLord.Ash.Core;

/// <summary>
/// A game entity is a collection object for components. Sometimes, the entities in a game
/// will mirror the actual characters and objects in the game, but this is not necessary.
/// </summary>
public class Entity
{
    private readonly Dictionary<Type, object> _components = new();

    /// <summary>
    /// Optional, give the entity a name. This can help with debugging and with serialising the entity.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// This signal is dispatched when a component is added to the entity.
    /// </summary>
    internal event Action<Entity, Type>? ComponentAdded;

    /// <summary>
    /// This signal is dispatched when a component is removed from the entity.
    /// </summary>
    internal event Action<Entity, Type>? ComponentRemoved;

    internal Entity? Previous { get; set; }
    internal Entity? Next { get; set; }

    internal Dictionary<Type, object> Components => _components;

    public Entity()
    {
    }

    /// <summary>
    /// Add a component to the entity.
    /// </summary>
    public Entity Add(object component)
    {
        AddComponentAndDispatchAddEvent(component, component.GetType());
        return this;
    }

    public Entity Add(object component, Type componentClass)
    {
        if (!componentClass.IsInstanceOfType(component))
        {
            throw new InvalidOperationException($"Component is not an instance of {componentClass} or its parent types.");
        }
        AddComponentAndDispatchAddEvent(component, componentClass);
        return this;
    }

    private Entity AddComponentAndDispatchAddEvent(object component, Type componentClass)
    {
        if (component is null)
        {
            throw new ArgumentNullException(nameof(component), "Component cannot be null.");
        }

        if (_components.ContainsKey(componentClass))
        {
            _components.Remove(componentClass);
        }

        _components[componentClass] = component;

        ComponentAdded?.Invoke(this, componentClass);
        return this;
    }

    /// <summary>
    /// Removes a component from the entity.
    /// </summary>
    public object? Remove<T>() => Remove(typeof(T));

    public object? Remove(Type componentClass)
    {
        if (_components.TryGetValue(componentClass, out var component))
        {
            _components.Remove(componentClass);
            ComponentRemoved?.Invoke(this, componentClass);
            return component;
        }
        return null;
    }

    /// <summary>
    /// Get a component from the entity.
    /// </summary>
    public object? Get(Type componentClass)
    {
        return _components.GetValueOrDefault(componentClass);
    }

    /// <summary>
    /// Get a component from the entity.
    /// </summary>
    public T? Get<T>(Type componentClass)
    {
        return _components.TryGetValue(componentClass, out var value) ? (T)value : default;
    }

    /// <summary>
    /// Get all components from the entity.
    /// </summary>
    public List<object> GetAll()
    {
        return [.. _components.Values];
    }

    /// <summary>
    /// Does the entity have a component of a particular type.
    /// </summary>
    public bool Has(Type componentClass)
    {
        return _components.ContainsKey(componentClass);
    }

    /// <summary>
    /// Make a copy of the entity
    /// </summary>
    public Entity Clone()
    {
        var copy = new Entity();
        foreach (var (componentType, value) in _components)
        {
            var clonedComponent = Activator.CreateInstance(componentType);
            if (clonedComponent is not null)
            {
                foreach (var property in componentType.GetProperties().Where(p => p.CanRead && p.CanWrite))
                {
                    property.SetValue(clonedComponent, property.GetValue(value, null), null);
                }
                copy.Add(clonedComponent, componentType);
            }
        }

        return copy;
    }
}