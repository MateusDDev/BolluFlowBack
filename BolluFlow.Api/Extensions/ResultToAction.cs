using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Extensions;

public static class ResultToAction
{
    public static ActionResult ToActionResult<T>(this ResultOf<T> result)
    {
        if (result.IsSuccessful)
        {
            return result.StatusCode == 204
                ? new StatusCodeResult(204)
                : new ObjectResult(result.Value) { StatusCode = result.StatusCode };
        }

        return new ObjectResult(new DomainError(
            result.ErrorCode ?? "UNKNOWN_ERROR",
            result.ErrorMessage ?? "Erro inesperado"
        ))
        {
            StatusCode = result.StatusCode
        };
    }


    public static ActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccessful)
            return new StatusCodeResult(result.StatusCode);

        return new ObjectResult(new DomainError(
            result.ErrorCode ?? "UNKNOWN_ERROR",
            result.ErrorMessage ?? "Erro inesperado"
        ))
        {
            StatusCode = result.StatusCode
        };
    }
}
