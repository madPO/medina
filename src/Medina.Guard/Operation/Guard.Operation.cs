namespace Medina;

public static partial class Guard
{
    public static async Task<OperationResult<TResult>> Operation<TResult>(Func<Task<TResult>> operation)
    {
        try
        {
            var result = await operation();
            return new OperationResult<TResult>(result, null);
        }
        catch (Exception exception)
        {
            return new OperationResult<TResult>(default, exception);
        }
    }
    
    public static async Task<OperationResult> Operation<TResult>(Func<Task> operation)
    {
        try
        {
            await operation();
            return new OperationResult(null);
        }
        catch (Exception exception)
        {
            return new OperationResult(exception);
        }
    }
}

public record OperationResult(Exception? Exception)
{
    public void Throw()
    {
        if (Exception is null)
        {
            return;
        }

        // todo: проверить, что стек не теряется
        throw Exception;
    }
}

public record OperationResult<TResult>(TResult? Result, Exception? Exception): OperationResult(Exception);