using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FixedWeaponsDamage
{
    /// <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {
        private ModConfig config;
        private float oldMusicVolume;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.Input.ButtonsChanged += Input_ButtonsChanged;
        }

        private void Input_ButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            float musicVolume = Game1.options.musicVolumeLevel;

            if (config.MuteHotkey.JustPressed())
            {
                if (musicVolume > 0)
                {
                    oldMusicVolume = musicVolume;
                    Game1.musicCategory.SetVolume(0f);
                }
                if (musicVolume == 0)
                {
                    Game1.musicCategory.SetVolume(oldMusicVolume);
                }
            }
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            InitializeConfigMenu();
        }

        private void InitializeConfigMenu()
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => config = new ModConfig(),
                save: () => Helper.WriteConfig(config));

            configMenu.AddKeybindList(
                mod: ModManifest,
                name: () => "Mute Hotkey",
                getValue: () => config.MuteHotkey,
                setValue: value => config.MuteHotkey = value);
        }
    }

    internal class ModConfig
    {
        public KeybindList MuteHotkey { get; set; } = KeybindList.Parse("LeftAlt + M");
    }
}
