﻿using Raylib_cs;
using System;
using System.Collections.Generic;
using SharpRay.Eventing;
using static Raylib_cs.Raylib;
using static SharpRay.Core.Application;
using System.IO;

namespace SharpRay.Core
{
    public static class Audio
    {
        public static Dictionary<string, Sound> Sounds = new();

        public static void Initialize()
        {
            
            
            //SetMasterVolume(0.15f);
        }

        public static void OnGuiEvent(IGuiEvent e) { }// => PlaySound(Sounds[e.GetType()]);
        public static void OnGameEvent(IGameEvent e) { }// => PlaySound(Sounds[e.GetType()]);

    }
}
