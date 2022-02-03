using System;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using OxygenNotIncluded.EntityBehavior;
using OxygenNotIncluded.Gui;

[assembly: ModInfo("Oxygen Not Included",
    Description = "Adds a simple breathing system to the game",
    Website = "https://github.com/squ1b3r/vs-oxygen-not-included",
    Authors = new[] {"Shoujiro"},
    Version = "0.2.3")]

namespace OxygenNotIncluded
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class OxygenNotIncludedMod : ModSystem
    {
        public static Config Config { get; private set; }

        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            LoadConfig(api);

            api.RegisterEntityBehaviorClass("oni:air", typeof(EntityBehaviorAir));
        }

        public override void StartClientSide(ICoreClientAPI capi)
        {
            base.StartClientSide(capi);
            capi.Gui.RegisterDialog(new HudElementAirBar(capi));
        }

        private void LoadConfig(ICoreAPI api)
        {
            var configFilename = Mod.Info.ModID + ".json";

            try
            {
                Config = api.LoadModConfig<Config>(configFilename);
                if (Config is null) throw new FileNotFoundException();
            }
            catch (Exception e)
            {
                api.Logger.Error($"[{Mod.Info.ModID}] Error message: {e.Message}");
                api.Logger.Error($"[{Mod.Info.ModID}] Failed to load mod config file. Using default");

                Config = new Config();
                api.StoreModConfig(Config, configFilename);
            }
        }
    }
}
