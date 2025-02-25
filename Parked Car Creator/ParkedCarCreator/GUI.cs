using CCL.GTAIV;
using IVSDKDotNet;
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
                    var playerPos = Main.PlayerPos;
                    float heading = Main.PlayerPed.GetHeading();
                    IVVehicle vehicle = IVVehicle.FromUIntPtr(Main.PlayerPed.GetVehicle());

                    GET_CAR_MODEL(vehicle.GetHandle(), out uint modelIndex);

                    string formatted = $"Model={modelIndex}\r\nCoords={(int)playerPos.X},{(int)playerPos.Y},{(int)playerPos.Z + 1}\r\nHeading={(int)heading}";
                    Clipboard.SetText(formatted);
                    IVGame.ShowSubtitleMessage($"~B~ COPIED TEXT: ~n~~w~{formatted}");
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
