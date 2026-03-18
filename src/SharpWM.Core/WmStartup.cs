using SharpWM.Common;
using SharpWM.Config;

namespace SharpWM.Core;

/// <summary>
/// Initialize WmState starting from the config:
/// creates workspaces for each monitor and the initial state.
/// </summary>
public static class WmStartup
{
    /// <summary>
    /// Populate the WmState with monitors and workspace defined in config.
    /// </summary>
    public static void Initialize(WmState state, WmConfig config, IEnumerable<MonitorContainer> monitors)
    {
        var monitorList = monitors.ToList();
        if (monitorList.Count == 0)
            throw new InvalidOperationException("Nessun monitor rilevato.");

        var workspaces = config.Workspaces.Count > 0
            ? config.Workspaces
            : DefaultWorkspaces();

        for (int i = 0; i < monitorList.Count; i++)
        {
            var monitor = monitorList[i];
            state.AddMonitor(monitor);
        }

        var primaryMonitor = monitorList.FirstOrDefault(m => m.IsPrimary)
            ?? monitorList[0];

        for (int i = 0; i < workspaces.Count; i++)
        {
            var ws = new WorkspaceContainer
            {
                Name     = workspaces[i].Name,
                IsActive = i == 0
            };
            primaryMonitor.AddChild(ws);
        }

        var firstWorkspace = primaryMonitor.ActiveWorkspace;
        if (firstWorkspace is not null)
            state.SetFocus(null);
    }

    private static List<WorkspaceConfig> DefaultWorkspaces() =>
        Enumerable.Range(1, 5)
            .Select(i => new WorkspaceConfig { Name = i.ToString() })
            .ToList();
}