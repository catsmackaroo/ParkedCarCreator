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
                Extras = iniSection.ContainsKey("Extras") ? ParseIntList(iniSection["Extras"]) : new List<int>(),
                Locked = iniSection.ContainsKey("Locked") && bool.Parse(iniSection["Locked"]),
                Rarity = iniSection.ContainsKey("Rarity") ? int.Parse(iniSection["Rarity"]) : 0,
                HasSpawned = iniSection.ContainsKey("HasSpawned") && bool.Parse(iniSection["HasSpawned"]),
                Episode = iniSection.ContainsKey("Episode") ? uint.Parse(iniSection["Episode"]) : 0
            };

            return carData;
        }

        private static List<uint> ParseUIntList(string value)
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
    }
}
