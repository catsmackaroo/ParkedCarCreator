using System.Collections.Generic;
using System.Numerics;
using static ParkedCarCreator.Manager;

namespace ParkedCarCreator
{
    internal static class Parser
    {
        public static Vector3 ParseVector3(string value)
        {
            var parts = value.Split(',');
            if (parts.Length == 3 &&
                float.TryParse(parts[0], out float x) &&
                float.TryParse(parts[1], out float y) &&
                float.TryParse(parts[2], out float z))
            {
                return new Vector3(x, y, z);
            }
            return Vector3.Zero;
        }

        public static List<int> ParseIntList(string value)
        {
            var parts = value.Split(',');
            var intList = new List<int>();
            foreach (var part in parts)
            {
                if (int.TryParse(part, out int result))
                {
                    intList.Add(result);
                }
            }
            return intList;
        }

        public static CarData ParseCarData(Dictionary<string, string> iniSection)
        {
            var carData = new CarData
            {
                Coords = iniSection.ContainsKey("Coords") ? ParseVector3(iniSection["Coords"]) : Vector3.Zero,
                Heading = iniSection.ContainsKey("Heading") ? int.Parse(iniSection["Heading"]) : 0,
                ModelNames = iniSection.ContainsKey("ModelNames") ? new List<string>(iniSection["ModelNames"].Split(',')) : new List<string>(),
                ModelHashes = iniSection.ContainsKey("ModelHashes") ? ParseUIntList(iniSection["ModelHashes"]) : new List<uint>(),
                Colors = iniSection.ContainsKey("Colors") ? ParseIntList(iniSection["Colors"]) : new List<int>(),
                Extras = iniSection.ContainsKey("Extras") ? ParseBoolList(iniSection["Extras"]) : new List<bool>(),
                Locked = iniSection.ContainsKey("Locked") && bool.Parse(iniSection["Locked"]),
                Rarity = iniSection.ContainsKey("Rarity") ? int.Parse(iniSection["Rarity"]) : 0,
                Episodes = iniSection.ContainsKey("Episode") ? ParseUIntList(iniSection["Episode"]) : new List<uint>(),
                Sirens = iniSection.ContainsKey("Sirens") &&bool.Parse(iniSection["Sirens"]),
                SirensVolume = iniSection.ContainsKey("SirensVolume") && bool.Parse(iniSection["SirensVolume"]),
                BeforeMission = iniSection.ContainsKey("BeforeMission") ? int.Parse(iniSection["BeforeMission"]) : 0,
                AfterMission = iniSection.ContainsKey("AfterMission") ? int.Parse(iniSection["AfterMission"]) : 0,
                DisableDuringMissions = iniSection.ContainsKey("DisableDuringMissions") &&bool.Parse(iniSection["DisableDuringMissions"]),
            };

            return carData;
        }

        public static List<uint> ParseUIntList(string value)
        {
            var parts = value.Split(',');
            var uintList = new List<uint>();
            foreach (var part in parts)
            {
                if (uint.TryParse(part, out uint result))
                {
                    uintList.Add(result);
                }
            }
            return uintList;
        }
        public static List<bool> ParseBoolList(string value)
        {
            var list = new List<bool>();
            foreach (var item in value.Split(','))
            {
                if (bool.TryParse(item, out bool result))
                {
                    list.Add(result);
                }
                else if (int.TryParse(item, out int intResult))
                {
                    list.Add(intResult != 0);
                }
                else
                {
                    list.Add(false);
                }
            }
            return list;
        }
    }
}
