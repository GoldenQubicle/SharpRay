using System;
using System.Collections.Generic;
using System.IO;
using static Raylib_cs.Raylib;
using static SharpRay.SnakeConfig;

namespace SharpRay
{
    public static class Audio
    {
        private static Dictionary<Type, Raylib_cs.Sound> Sounds = new();

        public static void Initialize()
        {
            Sounds.Add(typeof(SnakeGameStart), LoadSound(Path.Combine(Program.AssestsFolder, ButtonPushSound)));
            Sounds.Add(typeof(SnakeLocomotion), LoadSound(Path.Combine(Program.AssestsFolder, FootStepSound)));
            Sounds.Add(typeof(SnakeConsumedFood), LoadSound(Path.Combine(Program.AssestsFolder, SnakeGrow)));
            Sounds.Add(typeof(SnakeConsumedPoop), LoadSound(Path.Combine(Program.AssestsFolder, SnakeShrink)));
            Sounds.Add(typeof(FoodParticleSpawn), LoadSound(Path.Combine(Program.AssestsFolder, SnakeConfig.FoodParticleSpawn)));
            Sounds.Add(typeof(SnakeGameOver), LoadSound(Path.Combine(Program.AssestsFolder, GameOver)));

            SetMasterVolume(0.15f);
        }

        public static void OnUIEvent(IUIEvent e) => PlaySound(Sounds[e.GetType()]);
        public static void OnGameEvent(IGameEvent e) { }// => PlaySound(Sounds[e.GetType()]);

    }
}
