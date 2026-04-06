using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Profile.GetProfile;

public record GetProfileQuery : IQuery<ResultOf<ProfileModel>>;
