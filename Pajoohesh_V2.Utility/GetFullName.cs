using Pajoohesh_V2.Model.Models.Main.User;

namespace Pajoohesh_V2.Utility
{
    public static class GetFullName
    {
        public static string GetName(this User user)
        {
            if (user!=null)
            {
                string name = user.Name;
                string family = user.Family;

                var finalFullName = $"{name} {family}";
                return finalFullName;
            }
            else
            {
                return "";
            }
        }
    }
}