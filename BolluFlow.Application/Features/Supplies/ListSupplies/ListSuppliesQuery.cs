using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Supplies.ListSupplies;

public record ListSuppliesQuery : IQuery<ResultOf<List<SupplyModel>>>;
