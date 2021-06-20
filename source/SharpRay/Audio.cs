using System;
using System.Collections.Generic;
using System.IO;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public static class Audio
    {
        private const string ButtonPushSound = @"Tiny Button Push-SoundBible.com-513260752.wav";
        private const string SnakeMovement = @"Fotstep_Carpet_Left.wav";
        private const string SnakeConsumedFood = @"maximize_006.wav";
        private const string SnakeConsumedPoop = @"minimize_006.wav";
        private const string ParticleSpawn = @"drop_004.wav";
        private const string GameOver = @"error_006.wav";

        private static Dictionary<Type, Raylib_cs.Sound> Sounds = new();

        public static void Initialize()
        {
            Sounds.Add(typeof(SnakeGameStart), LoadSound(Path.Combine(Program.AssestsFolder, ButtonPushSound)));
            Sounds.Add(typeof(SnakeLocomotion), LoadSound(Path.Combine(Program.AssestsFolder, SnakeMovement)));
            Sounds.Add(typeof(SnakeConsumedFood), LoadSound(Path.Combine(Program.AssestsFolder, SnakeConsumedFood)));
            Sounds.Add(typeof(SnakeConsumedPoop), LoadSound(Path.Combine(Program.AssestsFolder, SnakeConsumedPoop)));
            Sounds.Add(typeof(ParticleSpawn), LoadSound(Path.Combine(Program.AssestsFolder, ParticleSpawn)));
            Sounds.Add(typeof(SnakeGameOver), LoadSound(Path.Combine(Program.AssestsFolder,GameOver)));

            SetMasterVolume(0.5f);
        }

        public static void OnUIEvent(IUIEvent e) => PlaySound(Sounds[e.GetType()]);
        public static void OnGameEvent(IGameEvent e) => PlaySound(Sounds[e.GetType()]);

    }
}
