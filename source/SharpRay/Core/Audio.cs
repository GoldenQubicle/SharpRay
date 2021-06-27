using Raylib_cs;
using System;
using System.Collections.Generic;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    internal static class Audio
    {
        private static Dictionary<Type, Sound> Sounds = new();

        public static void Initialize()
        {
            

            SetMasterVolume(0.15f);
        }

        public static void OnUIEvent(IUIEvent e) { }// => PlaySound(Sounds[e.GetType()]);
        public static void OnGameEvent(IGameEvent e) { }// => PlaySound(Sounds[e.GetType()]);

    }
}
