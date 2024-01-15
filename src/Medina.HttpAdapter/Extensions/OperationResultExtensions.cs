using System.Net;
using Microsoft.AspNetCore.Http;

namespace Medina.HttpAdapter.Extensions;

public static class OperationResultExtensions
{
    public static IResult ToActionResult(this OperationResult result)
    {
        if (result.Exception is not null)
        {
            return Results.Json(new { success = false, message = result.Exception.Message }, statusCode: (int) HttpStatusCode.InternalServerError);
        }
        
        return Results.Json(new { success = true });
    }
    
    public static IResult ToActionResult<TResult>(this OperationResult<TResult> result)
    {
        var (data, exception) = result;
        
        if (exception is not null)
        {
            return Results.Json(new { success = false, message = exception.Message }, statusCode: (int) HttpStatusCode.InternalServerError);
        }
        
        return Results.Json(new { success = true, data });
    }
}