using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Onboarding.CompleteOnboarding;

public record CompleteOnboardingCommand(List<string> EnabledModules) : ICommand<ResultOf<UserModel>>;
