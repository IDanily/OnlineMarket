namespace OnlineMarket.API.Contacts
{
    public record productResponse(
        int Id,
        int? number,
        string description,
        int rating,
        int? userId);
}
