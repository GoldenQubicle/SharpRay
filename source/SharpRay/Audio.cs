using System;
using System.Collections.Generic;
using System.IO;
using static Raylib_cs.Raylib;

namespace SharpRay
{
 

    public static class Audio
    {
        private const string ButtonPushSound = @"Tiny Button Push-SoundBible.com-513260752.wav";
        
        private static Dictionary<Type, Raylib_cs.Sound> Sounds = new();

        public static void Initialize()
        {
            Sounds.Add(typeof(AudioToggleTimerClicked), LoadSound(Path.Combine(Program.AssestsFolder, ButtonPushSound)));
        }
               
        public static void OnAudioEvent(IAudioEvent e) => PlaySound(Sounds[e.GetType()]);
    }
}
