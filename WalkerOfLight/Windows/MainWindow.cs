using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Interface.Components;

namespace WalkerOfLight.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin plugin;
    private Vector3? lastPosition = null;
    private float payoffInputKm = 0.0f;

    public MainWindow(Plugin plugin) : base("Walker of Light")
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(200, 165),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        this.plugin = plugin;

    }

    public void Dispose() { }

    public override void Draw()
    {
        var player = Plugin.ObjectTable.LocalPlayer;

        if (player == null)
        {
            lastPosition = null;
            return;
        }

        Vector3 currentPosition = player.Position;
        bool isMounted = Plugin.Condition[ConditionFlag.Mounted];
        bool isFlying = Plugin.Condition[ConditionFlag.InFlight];

        if (lastPosition.HasValue)
        {
            float distanceDelta = Vector3.Distance(currentPosition, lastPosition.Value);

            if (distanceDelta > 0.001f && distanceDelta < 10.0f)
            {
                float multiplier = 1.0f;
                int mode = plugin.Configuration.MountModeSetting;

                if (isMounted)
                {
                    if (mode == 2) multiplier = 0.0f;
                    else if (mode == 1) multiplier = isFlying ? 0.3333f : 0.5f;
                }

                plugin.Configuration.TotalYalmsDebt += (distanceDelta * multiplier);
            }
        }

        lastPosition = currentPosition;
        float debtKm = plugin.Configuration.TotalYalmsDebt * 0.0009144f;

        ImGui.Text("Treadmill Debt:");

        ImGui.SameLine(ImGui.GetWindowWidth() - 35);
        if (ImGuiComponents.IconButton(FontAwesomeIcon.Cog))
        {
            plugin.ConfigWindow.IsOpen = true;
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Open Settings");
        }

        ImGui.Text($"{debtKm:F2} km ({plugin.Configuration.TotalYalmsDebt:F0} yalms)");

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        ImGui.Text("Pay off amount (km):");
        ImGui.SetNextItemWidth(100f);
        ImGui.InputFloat("##payoff", ref payoffInputKm, 0.1f, 1.0f, "%.2f");

        ImGui.SameLine();
        if (ImGui.Button("Pay Off"))
        {
            if (payoffInputKm > 0)
            {
                float payoffYalms = payoffInputKm / 0.0009144f;
                if (payoffYalms >= plugin.Configuration.TotalYalmsDebt) payoffYalms = plugin.Configuration.TotalYalmsDebt;

                plugin.Configuration.TotalYalmsDebt -= payoffYalms;
                if (plugin.Configuration.TotalYalmsDebt < 0) plugin.Configuration.TotalYalmsDebt = 0;

                plugin.Configuration.LifetimeYalmsWalked += payoffYalms;
                plugin.Configuration.Save();
                payoffInputKm = 0.0f;
            }
        }

        ImGui.Spacing();

        if (ImGui.Button("Pay Off All"))
        {
            if (plugin.Configuration.TotalYalmsDebt > 0)
            {
                plugin.Configuration.LifetimeYalmsWalked += plugin.Configuration.TotalYalmsDebt;
                plugin.Configuration.TotalYalmsDebt = 0f;
                plugin.Configuration.Save();
            }
        }

        ImGui.SameLine();

        if (ImGui.Button("Reset Debt"))
        {
            plugin.Configuration.TotalYalmsDebt = 0f;
            plugin.Configuration.Save();
        }
    }
}
