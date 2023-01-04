using System.Data;
using System.Reflection;
using System.Text.Json;


namespace NiwaCoursework.Data
{

    public class UserServices
    {
        public const string SeedUsername = "admin";
        public const string SeedPassword = "admin";
        public static string GetAppDirectoryPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Bike-Service-Centre-IMS"
            );
        }
        public static string GetUsersPath()
        {
            return Path.Combine(GetAppDirectoryPath(), "users.json");
        }

        public static void SeedUsers()
        {
            var users = Read().FirstOrDefault(x => x.Role.Equals("Admin"));

            if (users == null)
            {
                Add("Default", "Admin", SeedUsername, SeedPassword, "Admin");
            }
        }
        public static User Login(string username, string password)
        {
            var loginErrorMessage = "Invalid username or password.";
            List<User> users = Read();
            User user = users.FirstOrDefault(x => x.Username == username);

            if (user == null)
            {
                throw new Exception(loginErrorMessage);
            }
            if (!user.Password.Equals(password))
            {
                throw new Exception(loginErrorMessage);
            }
            return user;
        }
        // For Users
        public static List<User> Read()
        {
            string path = GetUsersPath();
            if (!File.Exists(path))
            {
                return new List<User>();
            }

            string readJson = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<User>>(readJson);
        }
        private static void SaveAll(List<User> users)
        {
            string appDataDirectoryPath = GetAppDirectoryPath();
            if (!Directory.Exists(appDataDirectoryPath))
            {
                Directory.CreateDirectory(appDataDirectoryPath);
            }
            string path = GetUsersPath();
            var json = JsonSerializer.Serialize(users);
            File.WriteAllText(path, json);
        }

        public static List<User> Add(string createdBy, string fullname, string username, string password, string role)
        {
            List<User> users = Read();
            bool usernameExists = users.Any(x => x.Username == username);
            int adminCount = users.Count(x => x.Role == "Admin");
            if (adminCount > 1 && role == "Admin")
            {
                throw new Exception("There can be only 2 admins!");
            }

            if (usernameExists)
            {
                throw new Exception("Username already exists! Please use another username");
            }
            users.Add(
                new User
                {
                    Fullname = fullname,
                    Username = username,
                    Password = password,
                    Role = role,
                    CreatedBy = createdBy
                }
            );
            SaveAll(users);
            return users;
        }

        public static List<User> Delete(string username)
        {
            List<User> users = Read();
            User user = users.FirstOrDefault(x => x.Username == username);

            if (user == null)
            {
                throw new Exception("User not found.");
            }
            users.Remove(user);
            SaveAll(users);
            return users;
        }

    }
}

