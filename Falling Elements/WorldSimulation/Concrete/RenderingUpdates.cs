using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public class RenderingUpdates
{
    public Dictionary<(int x, int y), (Color? oldColor, Color? newColor)> Updates { get; } = new();


    public RenderingUpdates() { }
    public RenderingUpdates(RenderingUpdates renderingUpdates)
    {
        foreach (var update in renderingUpdates.Updates)
            Updates.Add(update.Key, update.Value);
    }


    public void Add((int x, int y) key, (Color? oldColor, Color? newColor) value)
    {
        // Add value if not existing.
        if (!Updates.TryGetValue(key, out (Color? oldColor, Color? newColor) existingValue))
        {
            Updates.Add(key, value);
        }
        // If exists check for color change.
        else
        {
            // Remove if not changed.
            if (value.newColor == existingValue.oldColor) Updates.Remove(key);
            // Set new color if changed.
            else existingValue.newColor = value.newColor;
        }
    }

    public void Add(KeyValuePair<(int x, int y), (Color? oldColor, Color? newColor)> keyValuePair)
        => Add(keyValuePair.Key, keyValuePair.Value);

    public void Add(RenderingUpdates renderingUpdates)
    {
        foreach (var update in renderingUpdates.Updates)
            Add(update);
    }
}
