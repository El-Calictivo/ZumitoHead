using System;
using System.Collections.Generic;

namespace Payosky.Architecture
{
    /// <summary>
    /// ServiceLocator is a static class responsible for managing and providing access to instances of services
    /// that implement the IGameService interface. It uses a dictionary to store these services by their type.
    /// </summary>
    public static class ServiceLocator
    {
        /// <summary>
        /// A static dictionary used to hold instances of services implementing the IGameService interface.
        /// </summary>
        /// <remarks>
        /// This dictionary is a core component of the Service Locator pattern. It facilitates the registration,
        /// retrieval, and management of services. Services implementing the IGameService interface are stored
        /// by their respective types and are managed through various static methods in the ServiceLocator class.
        /// </remarks>
        private static readonly Dictionary<Type, IGameService> Services = new();

        /// Adds a new service to the ServiceLocator and initializes it if it is successfully added.
        /// <typeparam name="T">The type of the service which implements the IGameService interface.</typeparam>
        /// <param name="service">The instance of the service to add.</param>
        /// If the service is successfully added, the Initialize() method of the service will be called.
        /// Only one instance of each service type can be added. Adding a service with a duplicate type will not replace the existing service.
        public static void Add<T>(T service) where T : IGameService
        {
            if (Services.TryAdd(typeof(T), service))
            {
                service.Initialize();
            }
        }

        /// Retrieves a registered service of the specified type.
        /// This method looks up the service of the specified generic type in the ServiceLocator's
        /// internal dictionary. If the service is found, it is returned. If not, the default value
        /// of the requested type is returned.
        /// <typeparam name="T">The type of the service to retrieve, which must implement IGameService.</typeparam>
        /// <returns>The retrieved service of type T if found; otherwise, the default value of type T.</returns>
        public static T Get<T>() where T : IGameService
        {
            if (Services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            return default;
        }

        /// Attempts to retrieve a registered service of type T from the service locator.
        /// If the service is found, it is assigned to the out parameter and the method returns true.
        /// If the service is not found, the out parameter is set to its default value and the method returns false.
        /// <typeparam name="T">The type of the service to retrieve, which must implement the IGameService interface.</typeparam>
        /// <param name="service">The out parameter where the retrieved service will be assigned if found, or its default value if not found.</param>
        /// <returns>
        /// True if the service of type T was successfully retrieved; otherwise, false.
        /// </returns>
        public static bool TryGet<T>(out T service) where T : IGameService
        {
            service = default;
            if (!Services.TryGetValue(typeof(T), out var serviceInstance)) return false;
            service = (T)serviceInstance;
            return true;
        }

        /// Releases all resources used by the services and clears the service container.
        /// This method iterates through all registered services in the service locator,
        /// calls their Disconnect and Dispose methods, and then clears the services dictionary.
        /// It is intended to perform cleanup for all managed services when they are no longer needed.
        public static void Dispose()
        {
            foreach (var service in Services.Values) service.Disconnect();
            foreach (var service in Services.Values) service.Dispose();
            Services.Clear();
        }
    }
}