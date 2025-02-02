﻿using OnlineMarket.Core.Models;

namespace OnlineMarket.API.Contacts
{
    public record UsersRequest(
            int Id,
            string userName,
            string name,
            string password,
            Role role,
        string roleName,
        string email);
}

