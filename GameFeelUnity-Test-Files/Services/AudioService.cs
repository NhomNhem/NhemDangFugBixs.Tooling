using GameFeelUnity.Scopes;
using NhemDangFugBixs.Attributes;
using VContainer.Unity;
using UnityEngine;

namespace GameFeelUnity.Tests.Services
{
    /// <summary>
    /// Example: Audio service registered in parent scope, accessible to all child scopes.
    /// Tests: Type-safe scope reference, Singleton lifetime.
    /// </summary>
    [AutoRegisterIn(typeof(GameLifetimeScope), Lifetime = Lifetime.Singleton)]
    public class AudioService
    {
        public void PlaySound(string soundName)
        {
            Debug.Log($"[AudioService] Playing sound: {soundName}");
        }

        public void PlayMusic(string musicName)
        {
            Debug.Log($"[AudioService] Playing music: {musicName}");
        }
    }
}
