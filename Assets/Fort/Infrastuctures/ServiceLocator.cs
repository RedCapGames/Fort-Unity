using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fort
{
    public static class ServiceLocator
    {
        private static bool _isInitialized;
        private static readonly Dictionary<Type, Type> RegisteredServicesTypes = new Dictionary<Type, Type>();
        private static readonly Dictionary<Type, object> ServiceInstances = new Dictionary<Type, object>();
        public static void Register<TService, TServiceInstance>()
        {
            Register(typeof(TService), typeof(TServiceInstance));
        }

        public static void Register(Type serviceType, Type serviceInstanceType)
        {
            if (!serviceType.IsInterface)
                throw new Exception("Service type can only be interface");
            if (!serviceInstanceType.IsClass)
                throw new Exception("Service instance type can only be Class");
            if (!typeof(MonoBehaviour).IsAssignableFrom(serviceInstanceType))
                throw new Exception("Service instace type can only be child of MonoBehaviour:"+ serviceInstanceType.AssemblyQualifiedName);
            if (!serviceType.IsAssignableFrom(serviceInstanceType))
                throw new Exception("Service instance type must be child of service type");
            if (RegisteredServicesTypes.ContainsKey(serviceType))
                throw new Exception("Service type already registered");
            RegisteredServicesTypes[serviceType] = serviceInstanceType;
        }
        public static TService Resolve<TService>()
        {
            return (TService)Resolve(typeof(TService));
        }

        private static void Initialize()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;
            Type[] unityAssemblyTypes = TypeExtensions.GetAllTypes();
            foreach (Type type in unityAssemblyTypes)
            {
                ServiceAttribute serviceAttribute = type.GetCustomAttribute<ServiceAttribute>();
                if (serviceAttribute != null && serviceAttribute.ServiceType != null)
                {
                    Register(serviceAttribute.ServiceType,type);
                }
            }
        }
        public static object Resolve(Type serviceType)
        {
            Initialize();
            object result;
            if (ServiceInstances.TryGetValue(serviceType, out result))
                return result;
            result = TryInstansiateService(RegisteredServicesTypes[serviceType]);
            ServiceInstances.Add(serviceType, result);
            FillDependencies(RegisteredServicesTypes[serviceType], result);
            return result;
        }

        private static object TryInstansiateService(Type serviceInstanceType)
        {
            GameObject gameObject = new GameObject(serviceInstanceType.Name);
            Component result = gameObject.AddComponent(serviceInstanceType);
            Object.DontDestroyOnLoad(gameObject);
            return result;
        }

        private static void FillDependencies(Type serviceInstanceType, object serviceinctanse)
        {
            foreach (FieldInfo fieldInfo in serviceInstanceType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(info => info.GetCustomAttributes(typeof(DependentAttribute), true).Any()))
            {
                fieldInfo.SetValue(serviceinctanse, Resolve(fieldInfo.FieldType));
            }
        }

    }

    public class DependentAttribute : Attribute
    {

    }
    public class ServiceAttribute : Attribute
    {
        public Type ServiceType { get; set; }
    }

}

