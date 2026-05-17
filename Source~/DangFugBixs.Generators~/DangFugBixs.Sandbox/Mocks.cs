using System;
using System.Linq;

namespace VContainer {
    public enum Lifetime {Singleton, Scoped, Transient}

    public interface IRegistrationBuilder {
        IRegistrationBuilder As<T>();
        IRegistrationBuilder As(params Type[] types);
        IRegistrationBuilder AsImplementedInterfaces();
        IRegistrationBuilder AsSelf();
    }

    public class RegistrationBuilder : IRegistrationBuilder {
        public IRegistrationBuilder As<T>() => this;
        public IRegistrationBuilder As(params Type[] types) => this;
        public IRegistrationBuilder AsImplementedInterfaces() => this;
        public IRegistrationBuilder AsSelf() => this;
    }

    public interface IContainerBuilder {
        IRegistrationBuilder Register<T>(Lifetime lifetime);
        IRegistrationBuilder RegisterEntryPoint<T>(Lifetime lifetime);
        IRegistrationBuilder RegisterComponentOnNewGameObject<T>(Lifetime lifetime);
        IRegistrationBuilder RegisterComponentInHierarchy<T>();
        void RegisterComponent<T>(T component);
        IRegistrationBuilder RegisterFactory<T>(Func<object, object> factory, Lifetime lifetime);
    }

    public class MockBuilder : IContainerBuilder {
        public IRegistrationBuilder Register<T>(Lifetime lifetime) {
            Console.WriteLine($"[Mock] Registering: {typeof(T).Name} ({lifetime})");
            return new RegistrationBuilder();
        }
        public IRegistrationBuilder RegisterEntryPoint<T>(Lifetime lifetime) {
            Console.WriteLine($"[Mock] Registering EntryPoint: {typeof(T).Name} ({lifetime})");
            return new RegistrationBuilder();
        }
        public IRegistrationBuilder RegisterComponentOnNewGameObject<T>(Lifetime lifetime) {
            Console.WriteLine($"[Mock] Registering Component on New GO: {typeof(T).Name} ({lifetime})");
            return new RegistrationBuilder();
        }
        public IRegistrationBuilder RegisterFactory<T>(Func<object, object> factory, Lifetime lifetime) {
            Console.WriteLine($"[Mock] Registering Factory: {typeof(T).Name} ({lifetime})");
            return new RegistrationBuilder();
        }
        public IRegistrationBuilder RegisterComponentInHierarchy<T>() {
            Console.WriteLine($"[Mock] Registering Component in Hierarchy: {typeof(T).Name}");
            return new RegistrationBuilder();
        }
        public void RegisterComponent<T>(T component) {
            Console.WriteLine($"[Mock] Registering Component: {typeof(T).Name}");
        }
    }
}

namespace UnityEngine {
    public class Object {
        public static T? FindFirstObjectByType<T>() where T : class => null;
    }
    public class MonoBehaviour : Object { }
    public class Component : Object { }
    public struct Vector3 { public float x, y, z; }
    public class Transform : Object { }
    public class Time { public static float deltaTime = 0.016f; }
    public static class Debug { public static void Log(object msg) => Console.WriteLine(msg); }
}

namespace VContainer.Unity {
    public interface IInitializable { void Initialize(); }
    public interface ITickable { void Tick(); }
    public class LifetimeScope {
        protected virtual void Configure(VContainer.IContainerBuilder builder) { }
    }
    public static class EntryPointsBuilder {
        public static void EnsureDispatcherRegistered(VContainer.IContainerBuilder builder) { }
    }
}
