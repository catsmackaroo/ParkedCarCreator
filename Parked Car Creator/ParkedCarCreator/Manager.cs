using IVSDKDotNet;
using ParkedCarCreator.NETBasePreset;
using System;
using System.Collections.Generic;
using System.Numerics;
using static IVSDKDotNet.Native.Natives;

namespace ParkedCarCreator
{
    internal class Manager
    {
        private static readonly List<CarData> carDataList = new List<CarData>();

        public static List<CarData> GetCarDataList()
        {
            return carDataList;
        }
        /// <summary>
        /// Grabs the car data from ParkedCarCreator.ini
        /// </summary>
        /// <param name="settings">The file containing car data.</param>
        public static void Init(SettingsFile settings)
        {
            try
            {
                int carCount = settings.GetInteger("Main", "Car Count", -1);
                if (carCount == -1)
                {
                    carCount = 0;
                    foreach (var section in settings.GetSectionNames())
                    {
                        if (int.TryParse(section, out int _))
                        {
                            carCount++;
                        }
                    }
                }
                Main.Log($"Car Count: {carCount}");
                Random random = new Random();

                for (int i = 1; i <= carCount; i++)
                {
                    string section = $"{i}";

                    string modelValue = settings.GetValue(section, "Model", "");
                    if (string.IsNullOrEmpty(modelValue))
                    {
                        Main.Log($"Model value is empty or missing for car {i}; ensure parameter 'Model' is not missing. Setting car as random model.");
                        modelValue = "random";
                    }
                    List<string> modelNames = new List<string>();
                    List<uint> modelHashes = new List<uint>();

                    foreach (var model in modelValue.Split(','))
                    {
                        if (uint.TryParse(model, out uint modelHash))
                        {
                            modelHashes.Add(modelHash);
                        }
                        else
                        {
                            modelNames.Add(model);
                        }
                    }

                    Vector3 coords = Parser.ParseVector3(settings.GetValue(section, "Coords", "0,0,0"));
                    int heading = settings.GetInteger(section, "Heading", 0);
                    string colorsValue = settings.GetValue(section, "Colors", null);
                    List<int> colors = string.IsNullOrEmpty(colorsValue) ? null : Parser.ParseIntList(colorsValue);
                    string extrasValue = settings.GetValue(section, "Extras", null);
                    List<bool> extras = string.IsNullOrEmpty(extrasValue) ? new List<bool>() : Parser.ParseBoolList(extrasValue);
                    bool locked = settings.GetValue(section, "Locked", null) == null ? random.Next(2) == 0 : settings.GetBoolean(section, "Locked", false);
                    int rarity = settings.GetInteger(section, "Rarity", 100);
                    List<uint> episodes = string.IsNullOrEmpty(settings.GetValue(section, "Episodes", null)) ? new List<uint>() : Parser.ParseUIntList(settings.GetValue(section, "Episodes", null)); // Updated
                    bool sirens = settings.GetBoolean(section, "Sirens", false);
                    bool sirensVolume = settings.GetBoolean(section, "SirensVolume", false);
                    int beforeMission = settings.GetInteger(section, "BeforeMission", 0);
                    int afterMission = settings.GetInteger(section, "AfterMission", 0);
                    bool disableOnMission = settings.GetBoolean(section, "DisableDuringMissions", false);

                    if ((modelNames.Count > 0 || modelHashes.Count > 0) && coords != Vector3.Zero)
                    {
                        var carData = new CarData
                        {
                            Slot = section,
                            ModelNames = modelNames,
                            ModelHashes = modelHashes,
                            Coords = coords,
                            Heading = heading,
                            Colors = colors,
                            Extras = extras,
                            Locked = locked,
                            Rarity = rarity,
                            Episodes = episodes,
                            Sirens = sirens,
                            SirensVolume = sirensVolume,
                            BeforeMission = beforeMission,
                            AfterMission = afterMission,
                            DisableDuringMissions = disableOnMission,
                        };
                        carDataList.Add(carData);
                        Main.Log($"Added Car {i}");
                    }
                    else
                    {
                        Main.Log($"Failed to add Car {i}.");
                    }
                }

                Main.Log("script initialized...");
            }
            catch (Exception ex)
            {
                Main.Log($"Error initializing Manager: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles most of the in-game logic such as spawning cars, checking distances between each, etc.
        /// </summary>
        public static void Tick()
        {
            if (!IS_PLAYER_PLAYING(Main.PlayerIndex)) return;

            if (CommonHelpers.ShouldExecute(1000))
            {
                foreach (var car in carDataList)
                {
                    float distance = Vector3.Distance(Main.PlayerPos, car.Coords);
                    if (distance < 100.0f)
                    {
                        if (!car.HasSpawned)
                        {
                            int randomValue = Main.GenerateRandomNumber(1, 100);
                            Main.Log($"Random value for car {car.Slot}: {randomValue}");
                            if (randomValue <= car.Rarity)
                            {
                                Main.Log($"Spawning car {car.Slot}");
                                CarHandler.SpawnCarAtLocation(car);
                                car.HasSpawned = true;
                            }
                            else
                            {
                                Main.Log($"Player near car {car.Slot} with rarity {car.Rarity}. Car not spawned due to rarity check.");
                                car.HasSpawned = true;
                            }
                        }
                    }
                    else
                    {
                        if (car.HasSpawned)
                        {
                            Main.Log($"Player out of range of car {car.Slot}. Resetting spawn status.");
                            car.HasSpawned = false;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Handles the car data, logging when a car is spawned.
        /// </summary>
        public class CarData
        {
            public string Slot { get; set; }
            public List<string> ModelNames { get; set; }
            public List<uint> ModelHashes { get; set; }
            public Vector3 Coords { get; set; }
            public int Heading { get; set; }
            public List<int> Colors { get; set; }
            public List<bool> Extras { get; set; }
            public bool Locked { get; set; }
            public int Rarity { get; set; }
            public bool HasSpawned { get; set; }
            public bool Sirens { get; set; }
            public bool SirensVolume { get; set; }
            public List<uint> Episodes { get; set; }
            public int BeforeMission { get; set; }
            public int AfterMission { get; set; }
            public bool DisableDuringMissions { get; set; }

            public override string ToString()
            {
                return $"Slot: {Slot}\n" +
                       $"ModelNames: {string.Join(", ", ModelNames)}\n" +
                       $"ModelHashes: {string.Join(", ", ModelHashes)}\n" +
                       $"Coords: {Coords}\n" +
                       $"Heading: {Heading}\n" +
                       $"Colors: {string.Join(", ", Colors)}\n" +
                       $"Extras: {string.Join(", ", Extras)}\n" +
                       $"Locked: {Locked}\n" +
                       $"Rarity: {Rarity}\n" +
                       $"Episode: {string.Join(",", Episodes)}\n" +
                       $"Sirens: {Sirens}\n" +
                       $"SirensVolume: {SirensVolume}\n" +
                       $"BeforeMission: {BeforeMission}\n" +
                       $"AfterMission: {AfterMission}\n" +
                       $"DisableDuringMissions: {DisableDuringMissions}";
            }

        }
    }
}
