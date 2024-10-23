using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using System.IO;


namespace MountainShrineItem
{
    public class ConfigManager
    {
        public static ConfigEntry<bool> isEnabled;
        public static ConfigEntry<bool> multiplyShrines;

        public static ConfigEntry<int> bonusItems;

        public static ConfigEntry<float> difficultyPercent;

        public static void Init(string configPath)
        {
            var Config = new ConfigFile(Path.Combine(configPath, "braquen-TheMountaininQuestion.cfg"), true);

            isEnabled = Config.Bind("MOUNTAINSHRINEITEM", "Enable", true, "Enables the item to appear in runs.");
            multiplyShrines = Config.Bind("MOUNTAINSHRINEITEM", "Multiply Shrines", true, "When enabled, the item will multiply with shrines instead of add on top of them.");

            bonusItems = Config.Bind("MOUNTAINSHRINEITEM", "Bonus Item Count", 1, "Number of extra boss items per stack.");
            difficultyPercent = Config.Bind("MOUNTAINSHRINEITEM", "Difficulty scaling", 100f, "Percent extra credits gained by the Director per stack.");

            ModSettingsManager.AddOption(new CheckBoxOption(isEnabled));
            ModSettingsManager.AddOption(new CheckBoxOption(multiplyShrines));
            ModSettingsManager.AddOption(new IntSliderOption(bonusItems,
                new IntSliderConfig {min = 1, max = 100}));
            ModSettingsManager.AddOption(new StepSliderOption(difficultyPercent,
                new StepSliderConfig {min = 0, max = 10000, increment = 0.1f}));
            
        }    
    }
}