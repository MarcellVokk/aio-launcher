using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AllInOneLauncher.Data;
using BfmeFoundationProject.RegistryKit;
using BfmeFoundationProject.RegistryKit.Data;

namespace AllInOneLauncher.Core.Managers;

internal static class BfmeSettingsManager
{
    internal static string? Get(BfmeGame game, string optionName)
    {
        string optionsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            BfmeRegistryManager.GetKeyValue(game, BfmeRegistryKey.UserDataLeafName), "Options.ini");
        Dictionary<string, string> optionsTable = File.Exists(optionsFile)
            ? File.ReadAllText(optionsFile).Split('\n').Where(x => x.Contains(" = "))
                .ToDictionary(x => x.Split(" = ")[0], x => x.Split(" = ")[1])
            : [];

        if (optionsTable.TryGetValue(optionName, out string? value))
            return value;
        else
            return null;
    }

    internal static void Set(BfmeGame game, string optionName, string value)
    {
        string optionsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            BfmeRegistryManager.GetKeyValue(game, BfmeRegistryKey.UserDataLeafName), "Options.ini");
        Dictionary<string, string> optionsTable =
            (File.Exists(optionsFile) ? File.ReadAllText(optionsFile) : BfmeDefaults.DefaultOptions).Split('\n')
            .Where(x => x.Contains(" = ")).ToDictionary(x => x.Split(" = ")[0], x => x.Split(" = ")[1]);

        if (!optionsTable.TryAdd(optionName, value))
            optionsTable[optionName] = value;
        File.WriteAllText(optionsFile, string.Join('\n', optionsTable.Select(x => $"{x.Key} = {x.Value}")));
    }
}