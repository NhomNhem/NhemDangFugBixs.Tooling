using MySandBox;
using NhemDangFugBixs.Attributes;
using NhemDangFugBixs.Generated;
using VContainer;

namespace MySanboxGame {
    [AutoRegister(NhemDangFugBixs.Attributes.Lifetime.Singleton, "Global")]
    public class AudioService { }

    [AutoRegister(NhemDangFugBixs.Attributes.Lifetime.Singleton, "Global")]
    public class NetworkService { }

    [AutoRegister(NhemDangFugBixs.Attributes.Lifetime.Scoped, "Gameplay")]
    public class EnemySpawner { }

    [AutoRegister(NhemDangFugBixs.Attributes.Lifetime.Transient, "Gameplay")]
    public class BulletPool { }

    [AutoRegister(NhemDangFugBixs.Attributes.Lifetime.Scoped, "Dungeon")]
    public class TrapManager { }
}

public class Program {
    public static void Main(string[] args) {
        Console.WriteLine("======Bắt đầu test mock VContainer ====\n");

        var fakeBuilder = new MockBuilder();
        
        Console.WriteLine("--- [Scene: RootScope] Đang nạp Service chung ---");
        VContainerRegistration.RegisterGlobal(fakeBuilder);
        
        Console.WriteLine("\n--- [Scene: GameScope] Đang vào màn chơi ---");
        VContainerRegistration.RegisterGameplay(fakeBuilder);
        
        Console.WriteLine("\n--- [Scene: DungeonScope] Đang xuống hầm ngục ---");
        VContainerRegistration.RegisterDungeon(fakeBuilder);
        
        Console.WriteLine("\n=== KẾT THÚC TEST ===");
    }
}