namespace Net.RichardLord.Ash.Core;

/// <summary>
/// A collection of nodes.
/// </summary>
public class NodeList
{
    public Node? Head { get; set; }
    public Node? Tail { get; set; }
    
    public event Action<Node>? NodeAdded;
    public event Action<Node>? NodeRemoved;

    internal void Add(Node node)
    {
        if (Head is null)
        {
            Head = Tail = node;
            node.Next = node.Previous = null;
        }
        else
        {
            Tail!.Next = node;
            node.Previous = Tail;
            node.Next = null;
            Tail = node;
        }

        NodeAdded?.Invoke(node);
    }
    
    internal void Remove(Node node)
    {
        if (Head == node) Head = Head.Next;
        if (Tail == node) Tail = Tail.Previous;
        
        if (node.Previous is not null) node.Previous.Next = node.Next;
        if (node.Next is not null) node.Next.Previous = node.Previous;

        NodeRemoved?.Invoke(node);
    }
    
    internal void RemoveAll()
    {
        while(Head is not null)
        {
            var node = Head;
            Head = node.Next;
            node.Previous = null;
            node.Next = null;

            NodeRemoved?.Invoke(node);
        }
        Tail = null;
    }
    
    public bool Empty => Head is null;
    
    public void Swap(Node node1, Node node2)
    {
        if (node1.Previous == node2)
        {
            node1.Previous = node2.Previous;
            node2.Previous = node1;
            node2.Next = node1.Next;
            node1.Next = node2;
        }
        else if (node2.Previous == node1)
        {
            node2.Previous = node1.Previous;
            node1.Previous = node2;
            node1.Next = node2.Next;
            node2.Next = node1;
        }
        else
        {
            var temp = node1.Previous;
            node1.Previous = node2.Previous;
            node2.Previous = temp;
            temp = node1.Next;
            node1.Next = node2.Next;
            node2.Next = temp;
        }

        if (Head == node1) Head = node2;
        else if (Head == node2) Head = node1;

        if (Tail == node1) Tail = node2;
        else if (Tail == node2) Tail = node1;

        if (node1.Previous is not null) node1.Previous.Next = node1;
        if (node2.Previous is not null) node2.Previous.Next = node2;

        if (node1.Next is not null) node1.Next.Previous = node1;
        if (node2.Next is not null) node2.Next.Previous = node2;
    }
}
