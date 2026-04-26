using System.Linq.Expressions;
using System.Reflection;

namespace Net.RichardLord.Ash.Core;

/// <summary>
/// An default class for managing a NodeList. This class creates the NodeList and adds and removes
/// nodes to/from the list as the entities and the components in the game change.
/// 
/// It uses the basic entity matching pattern of an entity system - entities are added to the list if
/// they contain components matching all the public properties of the node class.
/// </summary>
public class ComponentMatchingFamily : IFamily
{
    private NodeList _nodes = null!;
    private Type _nodeType = null!;
    private Dictionary<Entity, Node> _entities = null!;
    private Dictionary<Type, Action<Node, object>> _components = null!;
    private NodePool _nodePool = null!;
    private IGame _game = null!;

    public void Setup(IGame game, Type nodeType)
    {
        _game = game;
        _nodeType = nodeType;
        Init();
    }

    private void Init()
    {
        _nodePool = new(_nodeType);
        _nodes = new();
        _entities = new();
        _components = new();

        foreach (var property in _nodeType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.Name is "Entity" or "Previous" or "Next") continue;

            var instanceParam = Expression.Parameter(typeof(Node), "instance");
            var valueParam = Expression.Parameter(typeof(object), "value");
            var castInstance = Expression.Convert(instanceParam, _nodeType);
            var castValue = Expression.Convert(valueParam, property.PropertyType);
            var propertyAccess = Expression.Property(castInstance, property);
            var assign = Expression.Assign(propertyAccess, castValue);
            var setter = Expression.Lambda<Action<Node, object>>(assign, instanceParam, valueParam).Compile();

            _components.Add(property.PropertyType, setter);
        }
    }

    public NodeList NodeList => _nodes;

    public void NewEntity(Entity entity) => AddIfMatch(entity);

    public void ComponentAddedToEntity(Entity entity, Type componentClass) => AddIfMatch(entity);

    public void ComponentRemovedFromEntity(Entity entity, Type componentClass)
    {
        if (_components.ContainsKey(componentClass))
        {
            RemoveIfMatch(entity);
        }
    }

    public void RemoveEntity(Entity entity) => RemoveIfMatch(entity);

    private void AddIfMatch(Entity entity)
    {
        if (!_entities.ContainsKey(entity))
        {
            if (_components.Keys.Any(componentClass => !entity.Has(componentClass)))
            {
                return;
            }

            var node = _nodePool.Get();
            node.Entity = entity;
            foreach (var (componentClass, setter) in _components)
            {
                setter(node, entity.Get(componentClass)!);
            }
            _entities.Add(entity, node);
            _nodes.Add(node);
        }
    }

    private void RemoveIfMatch(Entity entity)
    {
        if (_entities.Remove(entity, out var node))
        {
            _nodes.Remove(node);
            if (_game.Updating)
            {
                _nodePool.Cache(node);
                _game.UpdateComplete += ReleaseNodePoolCache;
            }
            else
            {
                _nodePool.Dispose(node);
            }
        }
    }

    private void ReleaseNodePoolCache()
    {
        _game.UpdateComplete -= ReleaseNodePoolCache;
        _nodePool.ReleaseCache();
    }

    public void CleanUp()
    {
        for (var node = _nodes.Head; node is not null; node = node.Next)
        {
            if (node.Entity is not null)
            {
                _entities.Remove(node.Entity);
            }
        }
        _nodes.RemoveAll();
    }
}
