namespace Medina.Core.Options;

/// <summary>
/// Параметры Medina
/// </summary>
public sealed class MedinaOptions
{
    /// <summary>
    /// Источник событий
    /// </summary>
    public Uri Source { get; set; }

    /// <summary>
    /// Количество паралельно выполняемых операций
    /// </summary>
    public int ParallelWorkers { get; set; } = 10;
}