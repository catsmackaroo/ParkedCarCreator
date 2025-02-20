using IVSDKDotNet;
using static IVSDKDotNet.Native.Natives;
using System;
using System.Collections.Generic;
using System.Numerics;
using ParkedCarCreator.NETBasePreset;

namespace ParkedCarCreator
{
    internal class Manager
    {
        private static readonly List<CarData> carDataList = new List<CarData>();

        public static List<CarData> GetCarDataList()
        {
            return carDataList;
        }

        public static void Init(SettingsFile settings)
        {
            int carCount = settings.GetInteger("Main", "Car Count", 0);
            Main.Log($"Car Count: {carCount}");
            Random random = new Random();

            for (int i = 1; i <= carCount; i++)
            {
                string section = $"{i}";

                string modelValue = settings.GetValue(section, "Model", ""); // Read model as string
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
                List<int> extras = string.IsNullOrEmpty(extrasValue) ? new List<int>() : Parser.ParseIntList(extrasValue);
                bool locked = settings.GetValue(section, "Locked", null) == null ? random.Next(2) == 0 : settings.GetBoolean(section, "Locked", false);
                int rarity = settings.GetInteger(section, "Rarity", 100);
                uint episode = settings.GetUInteger(section, "Episode", 0);

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
                        Episode = episode
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
                            if (randomValue <= car.Rarity)
                            {
                                Main.Log($"Spawning car {car.Slot}");
                                CarSpawner.SpawnCarAtLocation(car);
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

        public static CarData GetNearestCar(float maxDistance)
        {
            CarData nearestCar = null;
            float nearestDistance = maxDistance;

            foreach (var car in carDataList)
            {
                float distance = Vector3.Distance(Main.PlayerPos, car.Coords);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestCar = car;
                }
            }

            return nearestCar;
        }

        public class CarData
        {
            public string Slot { get; set; }
            public List<string> ModelNames { get; set; } // List of model names
            public List<uint> ModelHashes { get; set; } // List of model hashes
            public Vector3 Coords { get; set; }
            public int Heading { get; set; }
            public List<int> Colors { get; set; }
            public List<int> Extras { get; set; }
            public bool Locked { get; set; }
            public int Rarity { get; set; }
            public bool HasSpawned { get; set; }
            public uint Episode { get; set; }

            public override string ToString()
            {
                return $"Slot: {Slot}, ModelNames: {string.Join(",", ModelNames)}, ModelHashes: {string.Join(",", ModelHashes)}, Coords: {Coords}, Colors: {string.Join(",", Colors)}, Extras: {string.Join(",", Extras)}, Locked: {Locked}, Rarity: {Rarity}, For Episode: {Episode}";
            }
        }
    }
}
