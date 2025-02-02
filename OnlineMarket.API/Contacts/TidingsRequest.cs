using OnlineMarket.Core.Models;

namespace OnlineMarket.API.Contacts
{
    public record productRequest(
        int Id,
        int? number,
        string name,
        string description,
        int rating,
        Users? user);
}
