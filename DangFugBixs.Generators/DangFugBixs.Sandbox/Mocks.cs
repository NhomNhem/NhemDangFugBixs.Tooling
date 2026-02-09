using System;

namespace VContainer {
    public enum Lifetime {Singleton, Scoped, Transient}

    public interface IContainerBuilder {
        void Register<T>(Lifetime lifetime);
    }
}

namespace VContainer.Unity { }

namespace MySandBox {
    public class MockBuilder : VContainer.IContainerBuilder {
        public void Register<T>(VContainer.Lifetime lifetime) {
            Console.WriteLine($"[Mock-Check] ✅ Đã đăng ký: {typeof(T).Name} | Lifetime: {lifetime}");
        }
    }
}