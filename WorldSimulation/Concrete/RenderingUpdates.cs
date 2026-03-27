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
        // Adds the value if the key does not already exist.
        if (!Updates.TryGetValue(key, out (Color? oldColor, Color? newColor) existingValue))
        {
            Updates.Add(key, value);
        }
        // If the key exists, checks for a color change.
        else
        {
            // Removes the entry if the color has not changed.
            if (value.newColor == existingValue.oldColor) Updates.Remove(key);
            // Updates the color if it has changed.
            else
            {
                existingValue.newColor = value.newColor;
                Updates[key] = existingValue;
            }
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
