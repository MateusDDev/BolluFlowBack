using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Menu.LookupMenuCustomer;

public record LookupMenuCustomerQuery(string Slug, string? Whatsapp) : IQuery<ResultOf<MenuCustomerLookupModel>>;
