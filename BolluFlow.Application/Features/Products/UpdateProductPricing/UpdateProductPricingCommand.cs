using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Products.UpdateProductPricing;

public record UpdateProductPricingCommand(long Id, UpdateProductPricingCommandBody Body) : ICommand<Result>;

public record UpdateProductPricingCommandBody(decimal? IndirectCostPercentage, decimal MarkupPercentage, decimal FinalPrice, decimal? DiscountPercentage);
