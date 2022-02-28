using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SalesApp.Models;

namespace SalesApp.Services.Authentication
{
    public class MockAuthenticationService : IAuthenticationService
    {
        public async Task<User> AuthenticateAsync(string username, string password)
        {
            await Task.Delay(100);
            if (password == "bad")
            {
                return null;
            }
            else
            {
                return new User()
                {
                    Email = "john@test.com",
                    FirstName = "John",
                    LastName = "Smith",
                    Password = "Test",
                    Token = "Test",
                    AgentID = "2452524"
                };
            }
        }
    }
}
