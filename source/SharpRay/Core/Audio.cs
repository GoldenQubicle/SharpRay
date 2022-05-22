namespace SharpRay.Core
{
    public static class Audio
    {
        public static Dictionary<string, Sound> Sounds = new();

        public static void Initialize()
        {
            SetMasterVolume(1f);
        }

        public static void OnGuiEvent(IGuiEvent e) => TryPlaySound(e?.GetType().Name);
        public static void OnGameEvent(IGameEvent e) => TryPlaySound(e?.GetType().Name);
        private static void TryPlaySound(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (Sounds.ContainsKey(name)) PlaySound(Sounds[name]);
        }

        /// <summary>
        /// Stop all sounds from playing. 
        /// </summary>
        public static void StopAllSounds() => Sounds.Values.Where(s => IsSoundPlaying(s)).ToList().ForEach(s => StopSound(s));
        
    }
}
