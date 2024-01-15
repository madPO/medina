namespace Medina.Core.Descriptors;

/// <summary>
/// Хранилище описателей
/// </summary>
public interface IDescriptorStore
{                                                                                                 
    /// <summary>
    /// Добавить
    /// </summary>
    ItemDescriptor Add(ItemDescriptor descriptor);

    /// <summary>
    /// Проверить наличие описателя 
    /// </summary>
    (bool contains, ItemDescriptor? descriptor) Contains(ItemDescriptor descriptor);

    /// <summary>
    /// Удалить описатель
    /// </summary>
    void Remove(ItemDescriptor descriptor);

    /// <summary>
    /// Получить сразу все описатели
    /// </summary>
    IReadOnlyCollection<ItemDescriptor> ReadAll();
    
}

internal class DescriptorStore : IDescriptorStore
{
    private readonly List<ItemDescriptor> _descriptors;
    
    public ItemDescriptor Add(ItemDescriptor descriptor)
    {
        var (contains, exist) = Contains(descriptor);

        if (contains)
        {
            return exist;
        }
        
        _descriptors.Add(descriptor);

        return descriptor;
    }

    public (bool contains, ItemDescriptor? descriptor) Contains(ItemDescriptor descriptor)
    {
        var exist = _descriptors.SingleOrDefault(x => x.Equals(descriptor));

        if (exist is null)
        {
            return (false, null);
        }
        
        return (true, exist);
    }

    public void Remove(ItemDescriptor descriptor)
    {
        _descriptors.Remove(descriptor);
    }

    public IReadOnlyCollection<ItemDescriptor> ReadAll()
    {
        return _descriptors.AsReadOnly();
    }                                                                     

    public DescriptorStore()
    {
        _descriptors = new List<ItemDescriptor>();
    }
}