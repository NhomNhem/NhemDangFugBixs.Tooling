using System;
using System.Linq;

namespace VContainer {
    public enum Lifetime {Singleton, Scoped, Transient}

    public interface IRegistrationBuilder {
        IRegistrationBuilder As<T>();
        IRegistrationBuilder As<T1, T2>();
        IRegistrationBuilder As(params Type[] types);
        IRegistrationBuilder WithLifetime(Lifetime lifetime);
    }

    public interface IContainerBuilder {
        IRegistrationBuilder Register<T>(Lifetime lifetime);
        IRegistrationBuilder RegisterEntryPoint<T>(Lifetime lifetime = Lifetime.Singleton);
        void RegisterComponentInHierarchy<T>();
        void RegisterComponent<T>(T component);
    }
}

namespace UnityEngine {
    public class Object {
        public static T FindFirstObjectByType<T>() where T : class => null;
    }
    public class MonoBehaviour : Object { }
    public class Component : Object { }
}

namespace VContainer.Unity {
    public interface IInitializable { void Initialize(); }
    public interface ITickable { void Tick(); }
}

namespace MySandBox {
    public class MockRegistration : VContainer.IRegistrationBuilder {
        public VContainer.IRegistrationBuilder As<T>() { Console.WriteLine($"   -> As {typeof(T).Name}"); return this; }
        public VContainer.IRegistrationBuilder As<T1, T2>() { Console.WriteLine($"   -> As {typeof(T1).Name}, {typeof(T2).Name}"); return this; }
        public VContainer.IRegistrationBuilder As(params Type[] types) { Console.WriteLine($"   -> As {string.Join(", ", types.Select(t => t.Name))}"); return this; }
        public VContainer.IRegistrationBuilder WithLifetime(VContainer.Lifetime lifetime) { Console.WriteLine($"   -> Lifetime: {lifetime}"); return this; }
    }

    public class MockBuilder : VContainer.IContainerBuilder {
        public VContainer.IRegistrationBuilder Register<T>(VContainer.Lifetime lifetime) {
            Console.WriteLine($"[Mock] Registering service: {typeof(T).Name} | Default Lifetime: {lifetime}");
            return new MockRegistration();
        }
        public VContainer.IRegistrationBuilder RegisterEntryPoint<T>(VContainer.Lifetime lifetime = VContainer.Lifetime.Singleton) {
            Console.WriteLine($"[Mock] Registering EntryPoint: {typeof(T).Name} | Default Lifetime: {lifetime}");
            return new MockRegistration();
        }
        public void RegisterComponentInHierarchy<T>() {
            Console.WriteLine($"[Mock] Registering Component in Hierarchy: {typeof(T).Name}");
        }
        public void RegisterComponent<T>(T component) {
            Console.WriteLine($"[Mock] Registering Component: {typeof(T).Name}");
        }
    }
}