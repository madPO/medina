namespace Medina.Core.Wrapper;

using System;

/// <summary>
/// Событие
/// <remarks>Совместимое с https://github.com/cloudevents/spec/blob/v1.0.1/spec.md</remarks>
/// </summary>
public class EventWrapper
{
    /// <summary>
    /// Идентификатор события
    /// </summary>
    public string Id { get; init; }
    
    /// <summary>
    /// Источник события
    /// </summary>
    public Uri Source { get; init; }
    
    /// <summary>
    /// Версия спецификации cloudevents
    /// </summary>
    public string SpecVersion { get; init; }
    
    /// <summary>
    /// Тип события
    /// </summary>
    public string Type { get; init; }
    
    /// <summary>
    /// Тип содержимого
    /// </summary>
    public string? DataContentType { get; set; }
    
    /// <summary>
    /// Схема данных, которой должно соответствовать содержимое
    /// </summary>
    // todo: пока не поддерживается
    public Uri? DataSchema { get; set; }
    
    /// <summary>
    /// Субъект события
    /// </summary>
    public string? Subject { get; set; }
    
    /// <summary>
    /// Время возникновения события
    /// </summary>
    public DateTime? Time { get; set; }
    
    /// <summary>
    /// Данные события
    /// </summary>
    public string? Data { get; set; }

    public EventWrapper(string id, Uri source, string specVersion, string type)
    {
        Id = id;
        Source = source;
        SpecVersion = specVersion;
        Type = type;
    }
    
    public EventWrapper(string id, Uri source, string type): this(id, source, "1.0", type)
    {
    }
}