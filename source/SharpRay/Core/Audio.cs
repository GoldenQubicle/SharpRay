using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRay.Core
{
    public static class Audio
    {
        public static readonly Dictionary<string, Sound> Sounds = new();
        private static readonly Dictionary<string, Task> RepeatedSounds = new();
        private static readonly Dictionary<string, CancellationTokenSource> CancellationSources = new();

        public static void Initialize()
        {
            SetMasterVolume(1f);
        }

        public static void OnGuiEvent(IGuiEvent e) => TryPlaySound(e?.GetType().Name);
        public static void OnGameEvent(IGameEvent e) => TryPlaySound(e?.GetType().Name);
        private static void TryPlaySound(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (Sounds.TryGetValue(name, out var s)) Raylib.PlaySound(s);
        }

        /// <summary>
        /// Stops a single sound from playing. If the sound was repeating it will no longer do so next time it is played. 
        /// </summary>
        /// <param name="key"></param>
        public static void StopSound(string key)
        {
            if (RepeatedSounds.ContainsKey(key))
                CancellationSources[key].Cancel();

            if (IsSoundPlaying(Sounds[key]))
                Raylib.StopSound(Sounds[key]);

            CancellationSources.Remove(key);
            RepeatedSounds.Remove(key);   
        }

        /// <summary>
        /// Stop all sounds from playing. 
        /// </summary>
        public static void StopAllSounds()
        {
            RepeatedSounds.Keys.ToList().ForEach(key => CancellationSources[key].Cancel());
            Sounds.Values.Where(s => IsSoundPlaying(s) == 1 ).ToList().ForEach(Raylib.StopSound);
        }

        /// <summary>
        /// Loads a <see cref="Sound"/> from file, and adds it the Sounds dictionary with the given key. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="soundFileName"></param>
        public static void AddSound(string key, string soundFileName) =>
            Sounds.TryAdd(key, LoadSound(Path.Combine(Application.AssestsFolder, soundFileName)));


        /// <summary>
        /// Plays the sound with the <see cref="Sound"/> from the Audio.Sounds dictionary with the given key.
        /// Defaults to one shot play. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isRepeated"></param>
        public static void PlaySound(string key, bool isRepeated = false)
        {
            if (!isRepeated)
            {
                Raylib.PlaySound(Sounds[key]);
                return;
            }

            var cts = new CancellationTokenSource();
            CancellationSources.Add(key, cts);
            RepeatedSounds.Add(key, PlaySoundOnRepeat(key, cts.Token));
        }

        private static Task PlaySoundOnRepeat(string key, CancellationToken ct) =>  
            Task.Run(() =>
                {
                    while (!ct.IsCancellationRequested)
                        if(!IsSoundPlaying(Sounds[key]))
                            Raylib.PlaySound(Sounds[key]);
                }, CancellationToken.None);
    }
}
