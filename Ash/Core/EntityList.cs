namespace Net.RichardLord.Ash.Core;

/// <summary>
/// An internal class for a linked list of entities. Used inside the framework for
/// managing the entities.
/// </summary>
internal class EntityList
{
    internal Entity? Head { get; private set; }
    internal Entity? Tail { get; private set; }

    internal void Add(Entity entity)
    {
        if (Head is null)
        {
            Head = Tail = entity;
            entity.Next = entity.Previous = null;
        }
        else
        {
            Tail!.Next = entity;
            entity.Previous = Tail;
            entity.Next = null;
            Tail = entity;
        }
    }

    internal void Remove(Entity entity)
    {
        if (Head == entity) Head = Head.Next;
        if (Tail == entity) Tail = Tail.Previous;

        if (entity.Previous is not null) entity.Previous.Next = entity.Next;
        if (entity.Next is not null) entity.Next.Previous = entity.Previous;
        
        // N.B. Don't set entity.next and entity.previous to null because that will break the list iteration if node is the current node in the iteration.
    }

    internal void RemoveAll()
    {
        while (Head is not null)
        {
            var entity = Head;
            Head = Head.Next;
            entity.Previous = null;
            entity.Next = null;
        }
        Tail = null;
    }
}
