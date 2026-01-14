using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Extensions;

public static class FileToAction
{
    public static IActionResult ToPdfResult(this ResultOf<byte[]> result)
    {
        if (result.IsSuccessful)
        {
            return new FileContentResult(
                result.Value!,
                "application/pdf")
            {
                EnableRangeProcessing = true
            };
        }

        return new ObjectResult(new DomainError(
            result.ErrorCode ?? "UNKNOWN_ERROR",
            result.ErrorMessage ?? "Erro inesperado"))
        {
            StatusCode = result.StatusCode
        };
    }
}