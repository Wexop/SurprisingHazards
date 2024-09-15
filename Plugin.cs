using System.Collections.Generic;
using BepInEx;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using LethalLib.Modules;
using SurprisingHazards.Patches;
using SurprisingHazards.Scripts;
using Unity.Netcode;
using UnityEngine;


 namespace SurprisingHazards
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency(StaticNetcodeLib.StaticNetcodeLib.Guid, BepInDependency.DependencyFlags.HardDependency)]
    public class SurprisingHazardsPlugin : BaseUnityPlugin
    {

        const string GUID = "wexop.surprising_hazards";
        const string NAME = "SurprisingHazards";
        const string VERSION = "1.0.1";

        public static SurprisingHazardsPlugin instance;

        public GameObject surprisingGameObject;
        public Dictionary<ulong, RegisteredHazard> RegisteredHazards = new Dictionary<ulong, RegisteredHazard>();

        public ConfigEntry<float> visibleRange;
        public ConfigEntry<string> customVisibleRange;

        void Awake()
        {
            instance = this;
            
            Logger.LogInfo($"SurprisingHazards starting....");

            string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "surprisinghazards");
            AssetBundle bundle = AssetBundle.LoadFromFile(assetDir);

            LoadConfigs();

            surprisingGameObject =
                bundle.LoadAsset<GameObject>("Assets/LethalCompany/Mods/SurprisingHazards/SurprisingHazard.prefab");
            
            Debug.Log(surprisingGameObject.name);
            
            
            
            Harmony.CreateAndPatchAll(typeof(TurretPatch));
            Harmony.CreateAndPatchAll(typeof(LandminePatch));
            Harmony.CreateAndPatchAll(typeof(SpikeTrapPatch));
            Harmony.CreateAndPatchAll(typeof(RoundManagerPatch));
            
            MapObjects.mapObjects.ForEach(m =>
            {
                var name = m.mapObject.prefabToSpawn.name;
                if(name.Contains("TurretContainer") || name.Contains("Landmine") || name.Contains("SpikeRoofTrapHazard")) return;
                m.mapObject.prefabToSpawn.gameObject.AddComponent<SurprisingHazardSetUp>();
            });
            
            Logger.LogInfo($"SurprisingHazards is ready!");
        }

        public static void RegisterHazard(NetworkBehaviour networkBehaviour)
        {
            var component = Instantiate(instance.surprisingGameObject, Vector3.zero, Quaternion.identity, networkBehaviour.transform.parent);
            var surpriseComponent = component.GetComponent<SurprisingHazardBehavior>();
            surpriseComponent.parent = networkBehaviour.transform.parent.gameObject;
            surpriseComponent.networkId = networkBehaviour.NetworkObjectId;
            surpriseComponent.isServer = networkBehaviour.IsServer;
        
            var registeredHazard = new RegisteredHazard();

            registeredHazard.gameObject = networkBehaviour.transform.parent.gameObject;
            registeredHazard.networkId = networkBehaviour.NetworkObjectId;
            registeredHazard.surprisingHazardBehavior = surpriseComponent;
        
            instance.RegisteredHazards.TryAdd(networkBehaviour.NetworkObjectId, registeredHazard);
        }
        
        public static void RegisterModdedHazard(NetworkBehaviour networkBehaviour)
        {
            var component = Instantiate(instance.surprisingGameObject, Vector3.zero, Quaternion.identity, networkBehaviour.transform);
            var surpriseComponent = component.GetComponent<SurprisingHazardBehavior>();
            surpriseComponent.parent = networkBehaviour.gameObject;
            surpriseComponent.networkId = networkBehaviour.NetworkObjectId;
            surpriseComponent.isServer = networkBehaviour.IsServer;
        
            var registeredHazard = new RegisteredHazard();

            registeredHazard.gameObject = networkBehaviour.gameObject;
            registeredHazard.networkId = networkBehaviour.NetworkObjectId;
            registeredHazard.surprisingHazardBehavior = surpriseComponent;
        
            instance.RegisteredHazards.TryAdd(networkBehaviour.NetworkObjectId, registeredHazard);
        }

        public float GetCustomDistance(string name)
        {
            float distance = visibleRange.Value;
            
            var nameSearch = name.ToLower();
            while (nameSearch.Contains(" ")) nameSearch = nameSearch.Replace(" ", "");
            
            customVisibleRange.Value.Split(",").ToList().ForEach(hazard =>
            {

                var data = hazard.Split(":");
                
                var hazardName = data[0].ToLower();
                while (hazardName.Contains(" ")) hazardName = hazardName.Replace(" ", "");
                
                if (nameSearch.Contains(hazardName))
                {
                    distance = float.Parse(data[1]);
                }

            });

            return distance;

        }

        private void LoadConfigs()
        {
            visibleRange = Config.Bind("General" ,"visibleRange", 8f, "Hazards visible range. Every player must have the same value, or this will cause desync. No need to restart the game :)");
            CreateFloatConfig(visibleRange);
            customVisibleRange = Config.Bind("General" ,
                "customVisibleRange", 
                "turret:15,spikeRoofTrap:8,landmine:8", 
                "Hazards visible range. The name must be the hazard prefab name, it works with modded hazards too. Every player must have the same value, or this will cause desync. No need to restart the game :)"
                );
            CreateStringConfig(customVisibleRange);
        }
        
        private void CreateFloatConfig(ConfigEntry<float> configEntry, float min = 0f, float max = 100f)
        {
            var exampleSlider = new FloatSliderConfigItem(configEntry, new FloatSliderOptions
            {
                Min = min,
                Max = max,
                RequiresRestart = false
            });
            LethalConfigManager.AddConfigItem(exampleSlider);
        }
        
        private void CreateIntConfig(ConfigEntry<int> configEntry, int min = 0, int max = 100)
        {
            var exampleSlider = new IntSliderConfigItem(configEntry, new IntSliderOptions()
            {
                Min = min,
                Max = max,
                RequiresRestart = false
            });
            LethalConfigManager.AddConfigItem(exampleSlider);
        }
        
        private void CreateStringConfig(ConfigEntry<string> configEntry, bool requireRestart = false)
        {
            var exampleSlider = new TextInputFieldConfigItem(configEntry, new TextInputFieldOptions()
            {
                RequiresRestart = requireRestart
            });
            LethalConfigManager.AddConfigItem(exampleSlider);
        }


    }
}