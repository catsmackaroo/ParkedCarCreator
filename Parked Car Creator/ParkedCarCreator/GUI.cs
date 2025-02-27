using CCL.GTAIV;
using IVSDKDotNet;
using System.Collections.Generic;
using System.Windows.Forms;
using static IVSDKDotNet.Native.Natives;

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
                ImGuiIV.Begin("Parked Car Creator");

                if (ImGuiIV.Button("Copy Data"))
                {
                    if (!Main.PlayerPed.IsInVehicle())
                    {
                        IVGame.ShowSubtitleMessage("~r~You should be in a vehicle. You wouldn't want your game to crash, would you?");
                        return;
                    }
                    var playerPos = Main.PlayerPos;
                    float heading = Main.PlayerPed.GetHeading();
                    IVVehicle vehicle = IVVehicle.FromUIntPtr(Main.PlayerPed.GetVehicle());

                    GET_CAR_MODEL(vehicle.GetHandle(), out uint modelIndex);
                    GET_CAR_COLOURS(vehicle.GetHandle(), out int primaryColor, out int secondaryColor);
                    GET_EXTRA_CAR_COLOURS(vehicle.GetHandle(), out int pearlescentColor, out int wheelColor);

                    List<bool> extras = new List<bool>();
                    for (uint i = 1; i <= 10; i++)
                    {
                        bool isExtraOn = IS_VEHICLE_EXTRA_TURNED_ON(vehicle.GetHandle(), i);
                        extras.Add(isExtraOn);
                    }

                    string formatted = $"Model={modelIndex}\r\nCoords={(int)playerPos.X},{(int)playerPos.Y},{(int)playerPos.Z + 1}\r\n" +
                        $"Heading={(int)heading}\r\n" +
                        $"Colors={primaryColor},{secondaryColor},{pearlescentColor},{wheelColor}\r\n" +
                        $"Extras={string.Join(",", extras)}";
                    Clipboard.SetText(formatted);

                    string subtitle = formatted.Replace("\r\n", "~n~");
                    IVGame.ShowSubtitleMessage($"~B~ COPIED TEXT: ~n~~w~{subtitle}");
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
