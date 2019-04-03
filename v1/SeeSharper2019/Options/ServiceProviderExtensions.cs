using System;

namespace SeeSharper2019.Options
{
    public static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider site, Type type = null)
        {
            return (T) site.GetService(type ?? typeof(T));
        }
    }
}