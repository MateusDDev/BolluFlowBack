namespace BolluFlow.Application.Features.Onboarding.GetOnboardingProgress;

public record GetOnboardingProgressResult(
    bool HasProduct,
    bool HasPricedProduct,
    bool HasCustomer,
    bool HasOrder,
    bool HasMenuLink,
    bool HasRecipe,
    bool HasProductComponent,
    bool HasTrackedStock,
    bool HasFinancialEntry);
