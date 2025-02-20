using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Forms;
using CCL.GTAIV;
using IVSDKDotNet;
using static IVSDKDotNet.Native.Natives;
using System.Linq;

namespace ParkedCarCreator
{
    public class GUI
    {
        private static bool enable;
        private static bool menuOpen;
        public static Keys key;
        public static void Init(SettingsFile settings)
        {
            enable = settings.GetBoolean("Copy Coords", "Enable", true);
            key = settings.GetKey("Debug Key", "Key", Keys.I);


            if (enable)
                Main.Log("script initialized...");
        }

        public static void Tick()
        {
            if (menuOpen)
            {
                ImGuiIV.Begin("The Parked Car Creator by catsmackaroo");

                if (ImGuiIV.Button("Copy Data"))
                {
                    var playerPos = Main.PlayerPos;
                    float heading = Main.PlayerPed.GetHeading();
                    IVVehicle vehicle = IVVehicle.FromUIntPtr(Main.PlayerPed.GetVehicle());

                    GET_CAR_MODEL(vehicle.GetHandle(), out uint modelIndex);

                    string formatted = $"Car={modelIndex}\r\n Coords={(int)playerPos.X},{(int)playerPos.Y},{(int)playerPos.Z + 1}\r\nHeading={(int)heading}";
                    Clipboard.SetText(formatted);
                    IVGame.ShowSubtitleMessage($"~B~ COPIED TEXT: ~n~~w~Car ID: {modelIndex} ~n~Coords={(int)playerPos.X},{(int)playerPos.Y},{(int)playerPos.Z + 1}~n~Heading={(int)heading}");
                }

                ImGuiIV.End();
            }
        }

        public static void Process()
        {
            if (!enable)
                return;

            menuOpen = !menuOpen;

        }
    }
}
