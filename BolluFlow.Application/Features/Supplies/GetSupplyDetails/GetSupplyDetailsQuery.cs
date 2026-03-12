using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Supplies.GetSupplyDetails;

public record GetSupplyDetailsQuery(long Id) : IQuery<ResultOf<SupplyModel>>;
