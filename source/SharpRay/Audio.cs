using System;
using System.Collections.Generic;
using System.IO;
using static Raylib_cs.Raylib;

namespace SharpRay
{
 

    public static class Audio
    {
        private const string ButtonPushSound = @"Tiny Button Push-SoundBible.com-513260752.wav";
        private const string ScuffedSnakeSound = @"Fotstep_Carpet_Left.wav";

        private static Dictionary<Type, Raylib_cs.Sound> Sounds = new();

        public static void Initialize()
        {
            Sounds.Add(typeof(StartSnakeGame), LoadSound(Path.Combine(Program.AssestsFolder, ButtonPushSound)));
            Sounds.Add(typeof(PlayerMovement), LoadSound(Path.Combine(Program.AssestsFolder, ScuffedSnakeSound)));
        }
               
        public static void OnUIEvent(IUIEvent e) => PlaySound(Sounds[e.GetType()]);
        public static void OnGameEvent(IGameEvent e) => PlaySound(Sounds[e.GetType()]);
    }
}
