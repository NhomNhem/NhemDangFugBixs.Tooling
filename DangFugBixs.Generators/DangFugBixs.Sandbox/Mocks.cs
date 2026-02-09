using VContainer;

namespace VContainer {
    public enum Lifetime {Singleton, Scoped, Transient}

    public interface IContainerBuilder {
        void Register<T>(Lifetime lifetime);
    }
}

namespace VContainer.Unity{
    
}

namespace NhemDangFugBixs.Attributes {
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoRegisterAttribute : Attribute {
        public AutoRegisterAttribute(object lifetime = null, string scope = "Global") {
        }
    }
}

namespace MySandBox {
    public class MockBuilder : VContainer.IContainerBuilder {
        public void Register<T>(Lifetime lifetime) {
            Console.WriteLine($"[Mock-Check] ✅ Đã đăng ký: {typeof(T).Name} | Lifetime: {lifetime}");
        }
    }
}