namespace Net.RichardLord.Ash.Core;

/// <summary>
/// This internal class maintains a pool of deleted nodes for reuse by framework. 
/// </summary>
internal class NodePool(Type nodeType)
{
    private Node? _tail;
    private Node? _cacheTail;
    private readonly Type _nodeType = nodeType;

    internal Node Get()
    {
        if (_tail is not null)
        {
            var node = _tail;
            _tail = _tail.Previous;
            node.Previous = null;
            return node;
        }
        return (Node)Activator.CreateInstance(_nodeType)!;
    }

    internal void Dispose(Node node)
    {
        node.Next = null;
        node.Previous = _tail;
        _tail = node;
    }
    
    internal void Cache(Node node)
    {
        node.Previous = _cacheTail;
        _cacheTail = node;
    }
    
    internal void ReleaseCache()
    {
        while (_cacheTail is not null)
        {
            var node = _cacheTail;
            _cacheTail = node.Previous;
            node.Next = null;
            node.Previous = _tail;
            _tail = node;
        }
    }
}
