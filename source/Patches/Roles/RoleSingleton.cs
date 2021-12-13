using System;
using System.Linq;
using Reactor;
using static TownOfUs.Roles.Role;

namespace TownOfUs.Roles
{
    public static class RoleSingleton<T> where T : Role
    {
        private static T _instance;
        public static T Instance => _instance ??= RoleSingleton.OfType<T>().Single();

        internal static void LoadSingletons()
        {
            foreach (var type in typeof(TownOfUs).Assembly.GetTypes())
            {
                if (!type.IsSubclassOf(typeof(Role))) continue; 
                AddSingleton(Activator.CreateInstance(type) as Role);
            }
        }
    }
}