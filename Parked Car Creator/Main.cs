﻿using IVSDKDotNet;
using static IVSDKDotNet.Native.Natives;
using ParkedCarCreator.NETBasePreset;
using static IVSDKDotNet.Helper;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System;

namespace ParkedCarCreator
{
    public class Main : Script
    {
        private static Random rnd;
        public DateTime timer;
        private double lastFixedTickTime = 0;
        private const double fixedTickInterval = 0.5;
        private static readonly string modTitle = "Parked Car Creator";
        public static DelayedCalling TheDelayedCaller;
        public static IVPed PlayerPed { get; private set; }
        public static int PlayerIndex { get; private set; }
        public static int PlayerWantedLevel { get; private set; }
        public static float PlayerHealth { get; private set; }
        public static Vector3 PlayerPos { get; private set; }
        public static List<UIntPtr> Peds { get; private set; } = new List<UIntPtr>();

        public static int GenerateRandomNumber(int x, int y)
        {
            if (y < x)
            {
                y = x;
            }
            return rnd.Next(x, y + 1);
        }

        public Main()
        {
            rnd = new Random();
            Initialized += Main_Initialized;
            Tick += Main_Tick;
            Drawing += Main_Drawing;
            KeyDown += Main_KeyDown;
            ProcessAutomobile += Main_ProcessAutomobile;
            ProcessCamera += Main_ProcessCamera;
            IngameStartup += Main_IngameStartup;
            GameLoadPriority += Main_GameLoadPriority;
            GameLoad += Main_GameLoad;
            Uninitialize += Main_Uninitialize;
            OnImGuiRendering +=Main_OnImGuiRendering;
            TheDelayedCaller = new DelayedCalling();
        }

        private void Main_OnImGuiRendering(IntPtr devicePtr, ImGuiIV_DrawingContext ctx)
        {
            GUI.Tick();
        }

        private void Main_Uninitialize(object sender, EventArgs e)
        {
            if (TheDelayedCaller != null)
            {
                TheDelayedCaller.ClearAll();
                TheDelayedCaller = null;
            }
        }

        private void Main_GameLoad(object sender, EventArgs e)
        {
        }

        private void Main_Drawing(object sender, EventArgs e)
        {
            GUI.Tick();

        }

        private void Main_GameLoadPriority(object sender, EventArgs e)
        {
        }

        private void Main_IngameStartup(object sender, EventArgs e)
        {
        }

        private void Main_Initialized(object sender, EventArgs e)
        {
            OnlyRaiseKeyEventsWhenInGame = true;
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Log($"{modTitle} version {version} has loaded.");

            Manager.Init(Settings);
            GUI.Init(Settings);
        }


        private void Main_ProcessCamera(object sender, EventArgs e)
        {
        }

        private void Main_ProcessAutomobile(UIntPtr vehPtr)
        {
        }

        private void Main_Tick(object sender, EventArgs e)
        {
            PlayerPed = IVPed.FromUIntPtr(IVPlayerInfo.FindThePlayerPed());
            PlayerPos = Main.PlayerPed.Matrix.Pos;
            PlayerIndex = (int)GET_PLAYER_ID();
            PlayerHealth = PlayerPed.Health;
            PedHelper.GrabAllPeds();
            PedHelper.GrabAllVehicles();

            GET_GAME_TIMER(out uint GameTimer);
            GameTimer = (uint)(GameTimer / 1000.0);
            if (GameTimer - lastFixedTickTime >= fixedTickInterval)
            {
                lastFixedTickTime = GameTimer;
                FixedTick();
            }


            TheDelayedCaller.Process();
        }

        private void FixedTick()
        {
            Manager.Tick();
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == GUI.key)
            {
                GUI.Process();
            }
        }

        public static void Log(string message, [CallerFilePath] string filePath = "")
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            IVGame.Console.Print($"{modTitle} - [{fileName}] {message}");
        }

        public static void LogError(string message)
        {
            IVGame.Console.Print($"{modTitle} - Error: + {message}");
        }

        internal static int GenerateRandomNumber(int v, uint wheelCount)
        {
            throw new NotImplementedException();
        }
    }
}
