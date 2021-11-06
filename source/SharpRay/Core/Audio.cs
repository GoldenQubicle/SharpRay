using Raylib_cs;
using System;
using System.Collections.Generic;
using SharpRay.Eventing;
using static Raylib_cs.Raylib;
using static SharpRay.Core.Application;
using System.IO;

namespace SharpRay.Core
{
    internal static class Audio
    {
        private static Dictionary<string, Sound> Sounds = new();

        public static void Initialize()
        {
            Sounds.Add("Test", LoadSound(Path.Combine(AssestsFolder, "source_SharpRay_assests_error_006.wav")));
            
            SetMasterVolume(0.15f);
        }

        public static void OnGuiEvent(IGuiEvent e) { }// => PlaySound(Sounds[e.GetType()]);
        public static void OnGameEvent(IGameEvent e) { }// => PlaySound(Sounds[e.GetType()]);

    }
}
