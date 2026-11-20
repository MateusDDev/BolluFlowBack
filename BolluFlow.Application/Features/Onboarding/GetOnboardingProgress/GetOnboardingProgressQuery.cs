using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Onboarding.GetOnboardingProgress;

public record GetOnboardingProgressQuery : IQuery<ResultOf<GetOnboardingProgressResult>>;
