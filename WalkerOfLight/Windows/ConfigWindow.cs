using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;

namespace WalkerOfLight.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    public ConfigWindow(Plugin plugin) : base("Walker of Light Settings")
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(300, 200),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        this.Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        // Display Lifetime Stats
        float lifetimeKm = Configuration.LifetimeYalmsWalked * 0.0009144f;
        ImGui.Text($"Lifetime Walked: {lifetimeKm:F2} km");
        ImGui.Text($"({Configuration.LifetimeYalmsWalked:F0} yalms)");

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        ImGui.Text("Mounted Distance Rule:");
        int currentMode = Configuration.MountModeSetting;
        if (ImGui.RadioButton("Full Distance", currentMode == 0)) currentMode = 0;
        if (ImGui.RadioButton("Half (Ground) / Third (Flying)", currentMode == 1)) currentMode = 1;
        if (ImGui.RadioButton("Ignore Completely", currentMode == 2)) currentMode = 2;

        if (currentMode != Configuration.MountModeSetting)
        {
            Configuration.MountModeSetting = currentMode;
            Configuration.Save();
        }
    }
}
