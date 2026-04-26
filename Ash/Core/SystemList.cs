namespace Net.RichardLord.Ash.Core;

/// <summary>
/// Used internally, this is an ordered list of Systems for use by the game loop.
/// </summary>
internal class SystemList
{
    internal SystemBase? Head { get; set; }
    internal SystemBase? Tail { get; set; }

    internal void Add(SystemBase system)
    {
        if (Head is null)
        {
            Head = Tail = system;
            system.Next = system.Previous = null;
        }
        else
        {
            var node = Tail;
            for (; node is not null; node = node.Previous)
            {
                if (node.Priority <= system.Priority) break;
            }

            if (node == Tail)
            {
                Tail!.Next = system;
                system.Previous = Tail;
                system.Next = null;
                Tail = system;
            }
            else if (node is null)
            {
                system.Next = Head;
                system.Previous = null;
                Head!.Previous = system;
                Head = system;
            }
            else
            {
                system.Next = node.Next;
                system.Previous = node;
                node.Next!.Previous = system;
            }
        }
    }

    internal void Remove(SystemBase system)
    {
        if (Head == system) Head = Head.Next;
        if (Tail == system) Tail = Tail.Previous;

        if (system.Previous is not null) system.Previous.Next = system.Next;
        if (system.Next is not null) system.Next.Previous = system.Previous;
    }

    internal void RemoveAll()
    {
        while (Head is not null)
        {
            var system = Head;
            Head = Head.Next;
            system.Previous = null;
            system.Next = null;
        }
        Tail = null;
    }

    internal SystemBase? Get(Type type)
    {
        for (var system = Head; system is not null; system = system.Next)
        {
            if (type.IsAssignableFrom(system.GetType()))
            {
                return system;
            }
        }
        return null;
    }
}
