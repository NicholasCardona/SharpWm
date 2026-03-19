using SharpWM.Common;

namespace SharpWM.Core;

/// <summary>
/// Calcola i Rect per tutte le finestre tiling di un workspace
/// in base alla direzione di split e ai gap configurati.
/// </summary>
public sealed class TilingEngine
{
    private readonly int _innerGap;
    private readonly int _outerGap;

    public TilingEngine(int innerGap = 8, int outerGap = 8)
    {
        _innerGap = innerGap;
        _outerGap = outerGap;
    }

    /// <summary>
    /// Ritorna la lista di (WindowContainer, Rect) da applicare
    /// per tutte le finestre tiling del workspace sul monitor dato.
    /// </summary>
    public IReadOnlyList<(WindowContainer Window, Rect Rect)> Calculate(
        WorkspaceContainer workspace,
        Rect monitorBounds)
    {
        var windows = workspace.Descendants()
            .OfType<WindowContainer>()
            .Where(w => !w.IsFloating)
            .ToList();

        if (windows.Count == 0)
            return [];

        // Area disponibile al netto dei gap esterni
        var available = new Rect(
            monitorBounds.X + _outerGap,
            monitorBounds.Y + _outerGap,
            monitorBounds.Width  - _outerGap * 2,
            monitorBounds.Height - _outerGap * 2);

        var rects = Split(available, windows.Count, workspace.TilingDirection);

        return windows
            .Zip(rects, (w, r) => (w, r))
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Divide ricorsivamente l'area disponibile in N slot applicando i gap interni.
    /// </summary>
    private List<Rect> Split(Rect area, int count, Direction direction)
    {
        if (count == 1)
            return [area];

        var result = new List<Rect>();

        if (direction == Direction.Horizontal)
        {
            // Divide in colonne
            int totalGap  = _innerGap * (count - 1);
            int slotWidth = (area.Width - totalGap) / count;

            for (int i = 0; i < count; i++)
            {
                int x = area.X + i * (slotWidth + _innerGap);
                result.Add(new Rect(x, area.Y, slotWidth, area.Height));
            }
        }
        else
        {
            // Divide in righe
            int totalGap   = _innerGap * (count - 1);
            int slotHeight = (area.Height - totalGap) / count;

            for (int i = 0; i < count; i++)
            {
                int y = area.Y + i * (slotHeight + _innerGap);
                result.Add(new Rect(area.X, y, area.Width, slotHeight));
            }
        }

        return result;
    }
}