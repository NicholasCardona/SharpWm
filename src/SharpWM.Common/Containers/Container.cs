namespace SharpWM.Common;

public enum ContainerType
{
    Root,
    Monitor,
    Workspace,
    Split,
    Window
}

public abstract class Container
{
    public Guid Id { get; } = Guid.NewGuid();
    public ContainerType Type { get; protected init; }
    public Container? Parent { get; set; }
    public List<Container> Children { get; set; } = [];

    public void AddChild(Container child)
    {
        child.Parent = this;
        Children.Add(child);
    }

    public bool RemoveChild(Container child)
    {
        if (!Children.Remove(child)) return false;
        child.Parent = null;
        return true;
    }

    public IEnumerable<Container> Descendants()
    {
        var queue = new Queue<Container>(Children);
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            yield return node;
            foreach (var child in node.Children) queue.Enqueue(child);
        }
    }
}