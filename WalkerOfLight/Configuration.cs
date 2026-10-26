using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace WalkerOfLight;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    public float TotalYalmsDebt { get; set; } = 0f;
    public float LifetimeYalmsWalked { get; set; } = 0f;
    public int MountModeSetting { get; set; } = 1;


    [NonSerialized]
    private IDalamudPluginInterface? pluginInterface;

    public void Initialize(IDalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
    }

    public void Save()
    {
        this.pluginInterface!.SavePluginConfig(this);
    }
}
