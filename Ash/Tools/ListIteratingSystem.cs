using Net.RichardLord.Ash.Core;

namespace Net.RichardLord.Ash.Tools;

public class ListIteratingSystem(Type nodeClass) : SystemBase
{
    protected NodeList? nodeList;
    protected Type nodeClass = nodeClass;
    
    protected event Action<Node>? NodeAdded;
    protected event Action<Node>? NodeRemoved;
    protected event Action<Node, double>? NodeUpdate;
    
    public override void AddToGame(IGame game)
    {
        nodeList = game.GetNodeList(nodeClass);
        
        if (NodeAdded is not null)
        {
            for (var node = nodeList.Head; node is not null; node = node.Next)
            {
                NodeAdded(node);
            }
        }
    }

    public override void RemoveFromGame(IGame game)
    {
        if (NodeRemoved is not null && nodeList is not null)
        {
            for (var node = nodeList.Head; node is not null; node = node.Next)
            {
                NodeRemoved(node);
            }
        }
        nodeList = null;
    }
    
    public override void Update(double time)
    {
        if (nodeList is null) return;
        
        for (var node = nodeList.Head; node is not null; node = node.Next)
        {
            NodeUpdate?.Invoke(node, time);
        }
    }
}
