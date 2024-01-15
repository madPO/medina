using CloudNative.CloudEvents;

namespace Medina.Core.Descriptors;

public delegate Task<CloudEvent?> EventDelegate(CloudEvent @event, CancellationToken cancellationToken = default);

/// <summary>
/// Описатель элемента
/// </summary>
public abstract class ItemDescriptor<TStore> : ItemDescriptor where TStore : IEventStore
{
    protected ItemDescriptor(string id, string alternative) : base(id, alternative)
    {
        QueueType = typeof(TStore);
    }
}
public abstract class ItemDescriptor: IEquatable<ItemDescriptor>
{
    /// <summary>
    /// Идентификатор описателя
    /// </summary>
    public string Did { get; }
    
    /// <summary>
    /// Наименование элемента
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Текстовое описание
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Техническое наименование
    /// </summary>
    public string Alternative { get; }
    
    /// <summary>
    /// Источник
    /// </summary>
    public Uri? Source { get; init; }
    
    /// <summary>
    /// Тип очереди для события
    /// </summary>
    public Type QueueType { get; protected set; }

    /// <summary>
    /// Проверка на возможность обработки события 
    /// </summary>
    public abstract bool CanHandle(CloudEvent @event);
    
    /// <summary>
    /// Обработать событие
    /// </summary>
    //public abstract Task<CloudEvent?> HandleAsync(CloudEvent @event, CancellationToken cancellationToken = default);
                                            
    // todo: не типизированный тип стора для сравнений
    public virtual bool Equals(ItemDescriptor? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Did == other.Did && Alternative == other.Alternative;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ItemDescriptor) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Did, Alternative);
    }
    
    public ItemDescriptor(string id, string alternative)
    {
        Did = id;
        Alternative = alternative;
    }
}