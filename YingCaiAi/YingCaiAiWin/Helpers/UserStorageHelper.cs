using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YingCaiAiWin.Models;

namespace YingCaiAiWin.Helpers
{
    public static class UserStorageHelper
    {
        private static readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user.json");

        public static void SaveUser(AppUser user)
        {
            var json = JsonSerializer.Serialize(user);
            File.WriteAllText(filePath, json);
        }

        public static void LoadUser()
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var loadedUser = JsonSerializer.Deserialize<AppUser>(json);

                if (loadedUser != null)
                {
                    AppUser.Instance.Username = loadedUser.Username;
                    AppUser.Instance.Token = loadedUser.Token;
                    AppUser.Instance.Role = loadedUser.Role;
                }
            }
        }

        public static void ClearUser()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            AppUser.Instance.Clear();
        }
    }

}
