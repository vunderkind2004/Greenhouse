using System;
using System.Configuration;
using GreenHouse.EfRepository.Repositories;
using GreenHouse.Interfaces.Repository;
using GreenHouse.Repository.DataModel;
using GreenHouse.Repository.Repository;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using GreenHouse.Services;

namespace GreenHouse.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            // container.RegisterType<IProductRepository, ProductRepository>();
            string connectionString = ConfigurationManager.ConnectionStrings["GreenHouseForOrmLight"].ConnectionString;
            
            container.RegisterType<IRepository<SensorType>, RepositoryBase<SensorType>>(new InjectionConstructor(connectionString));
            container.RegisterType<IRepository<Device>, RepositoryBase<Device>>(new InjectionConstructor(connectionString));
            container.RegisterType<IRepository<SensorData>, RepositoryBase<SensorData>>(new InjectionConstructor(connectionString));
            container.RegisterType<IRepository<Sensor>, RepositoryBase<Sensor>>(new InjectionConstructor(connectionString));
            container.RegisterType<ISensorDataRepository, EfSensorDataRepository>();
            container.RegisterType<IDeviceDataRepository, DeviceDataRepository>(new InjectionConstructor(connectionString));
            container.RegisterType<IRepository<User>, RepositoryBase<User>>(new InjectionConstructor(connectionString));
            container.RegisterInstance(new DeviceManager(container.Resolve<IRepository<Device>>()));
        }
    }
}
