using SharpWM.Common;
using SharpWM.Core;

namespace SharpWM.Tests;

public class TilingEngineTests
{
    private static readonly Rect Monitor = new(0, 0, 1920, 1080);

    private static WorkspaceContainer BuildWorkspace(
        int windowCount, Direction direction = Direction.Horizontal)
    {
        var ws = new WorkspaceContainer { Name = "1", TilingDirection = direction };
        for (int i = 0; i < windowCount; i++)
            ws.AddChild(new WindowContainer { Handle = 100 + i });
        return ws;
    }

    // ── Casi base ────────────────────────────────────────────────

    [Fact]
    public void Calculate_EmptyWorkspace_ReturnsEmpty()
    {
        var engine = new TilingEngine();
        var ws = new WorkspaceContainer { Name = "1" };

        var result = engine.Calculate(ws, Monitor);

        Assert.Empty(result);
    }

    [Fact]
    public void Calculate_SingleWindow_FillsAvailableArea()
    {
        var engine = new TilingEngine(innerGap: 8, outerGap: 8);
        var ws = BuildWorkspace(1);

        var result = engine.Calculate(ws, Monitor);

        Assert.Single(result);
        var rect = result[0].Rect;
        Assert.Equal(8, rect.X);
        Assert.Equal(8, rect.Y);
        Assert.Equal(1920 - 16, rect.Width);
        Assert.Equal(1080 - 16, rect.Height);
    }

    // ── Split orizzontale ────────────────────────────────────────

    [Fact]
    public void Calculate_TwoWindows_Horizontal_SplitsIntoColumns()
    {
        var engine = new TilingEngine(innerGap: 8, outerGap: 8);
        var ws = BuildWorkspace(2, Direction.Horizontal);

        var result = engine.Calculate(ws, Monitor);

        Assert.Equal(2, result.Count);
        // Le due finestre devono essere affiancate (stessa Y, stessa altezza)
        Assert.Equal(result[0].Rect.Y, result[1].Rect.Y);
        Assert.Equal(result[0].Rect.Height, result[1].Rect.Height);
        // La seconda inizia dopo la prima
        Assert.True(result[1].Rect.X > result[0].Rect.X);
    }

    [Fact]
    public void Calculate_TwoWindows_Horizontal_NoOverlap()
    {
        var engine = new TilingEngine(innerGap: 8, outerGap: 8);
        var ws = BuildWorkspace(2, Direction.Horizontal);

        var result = engine.Calculate(ws, Monitor);

        var r0 = result[0].Rect;
        var r1 = result[1].Rect;
        Assert.True(r0.Right + 8 <= r1.X, "Le finestre si sovrappongono o il gap manca");
    }

    // ── Split verticale ──────────────────────────────────────────

    [Fact]
    public void Calculate_TwoWindows_Vertical_SplitsIntoRows()
    {
        var engine = new TilingEngine(innerGap: 8, outerGap: 8);
        var ws = BuildWorkspace(2, Direction.Vertical);

        var result = engine.Calculate(ws, Monitor);

        Assert.Equal(2, result.Count);
        Assert.Equal(result[0].Rect.X, result[1].Rect.X);
        Assert.Equal(result[0].Rect.Width, result[1].Rect.Width);
        Assert.True(result[1].Rect.Y > result[0].Rect.Y);
    }

    // ── Gap ──────────────────────────────────────────────────────

    [Fact]
    public void Calculate_ZeroGaps_WindowsFillEntireMonitor()
    {
        var engine = new TilingEngine(innerGap: 0, outerGap: 0);
        var ws = BuildWorkspace(1);

        var result = engine.Calculate(ws, Monitor);

        Assert.Equal(Monitor, result[0].Rect);
    }

    [Fact]
    public void Calculate_ThreeWindows_Horizontal_TotalWidthRespected()
    {
        var engine = new TilingEngine(innerGap: 10, outerGap: 10);
        var ws = BuildWorkspace(3, Direction.Horizontal);

        var result = engine.Calculate(ws, Monitor);

        Assert.Equal(3, result.Count);
        // Il bordo destro dell'ultima finestra non deve sforare l'area disponibile
        int availableRight = Monitor.Width - 10; // outerGap
        Assert.True(result[^1].Rect.Right <= availableRight);
    }

    // ── Floating ─────────────────────────────────────────────────

    [Fact]
    public void Calculate_FloatingWindowsIgnored()
    {
        var engine = new TilingEngine();
        var ws = new WorkspaceContainer { Name = "1" };
        ws.AddChild(new WindowContainer { Handle = 1, IsFloating = false });
        ws.AddChild(new WindowContainer { Handle = 2, IsFloating = true });

        var result = engine.Calculate(ws, Monitor);

        Assert.Single(result);
        Assert.Equal(1, result[0].Window.Handle);
    }
}