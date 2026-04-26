namespace Net.RichardLord.Ash.Core;

public class Node
{
    /// <summary>
    /// The entity whose components are included in the node.
    /// </summary>
    public Entity? Entity { get; set; }      

    /// <summary>
    /// Used by the NodeList class. The previous node in a node list.
    /// </summary>
    public Node? Previous { get; set; }

    /// <summary>
    /// Used by the NodeList class. The next node in a node list.
    /// </summary>
    public Node? Next { get; set; }
}
