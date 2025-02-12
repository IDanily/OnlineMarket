namespace OnlineMarket.API.Contacts
{
    public record UsersRequest(
            int Id,
            string userName,
            string name,
            string password,
        string roleName,
        string email);
}

