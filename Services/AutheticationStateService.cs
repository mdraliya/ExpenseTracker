using System;
using System.IO;
using System.Threading.Tasks;
using ExpenseTracker.Models;
using System.Text.Json;


namespace ExpenseTracker.Services
{
    public class AuthenticationStateService
    {
        private readonly string authFilePath = Path.Combine(AppContext.BaseDirectory, "auth.json");
        private User authenticatedUser;

        // Constructor to initialize the service
        public AuthenticationStateService()
        {
            LoadAuthenticatedUser();
        }

        // Load the authenticated user from the JSON file
        private void LoadAuthenticatedUser()
        {
            if (File.Exists(authFilePath))
            {
                var json = File.ReadAllText(authFilePath);
                authenticatedUser = JsonSerializer.Deserialize<User>(json);
            }
        }

        // Save the authenticated user to the JSON file
        private void SaveAuthenticatedUser()
        {
            if (authenticatedUser != null)
            {
                var json = JsonSerializer.Serialize(authenticatedUser);
                File.WriteAllText(authFilePath, json);
            }
        }

        public User GetAuthenticatedUser()
        {
            return authenticatedUser;
        }

        public void SetAuthenticatedUser(User user)
        {
            authenticatedUser = user;
        }

        public bool IsAuthenticated()
        {
            if (authenticatedUser != null)
            {
                return true;
            }
            return false;
        }

        public void Logout()
        {
            authenticatedUser = null;
            if (File.Exists(authFilePath))
            {
                File.Delete(authFilePath); // Remove the authentication file
            }
        }
    }
}
