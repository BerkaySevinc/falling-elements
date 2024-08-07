using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Falling_Elements;

public class FpsCounter :
    IDisposable
{
    #region // storage

    /// <summary>
    /// Time amount which defines how frequently we do measurements.
    /// </summary>
    public TimeSpan UpdateRate { get; }

    /// <summary>
    /// Stopwatch to measure how much time elapsed since last measurement.
    /// </summary>
    private Stopwatch StopwatchUpdate { get; set; }

    /// <summary>
    /// Stopwatch to measure how much time elapsed since the beginning of the frame.
    /// </summary>
    private Stopwatch StopwatchFrame { get; set; }

    /// <summary>
    /// Sum of all <see cref="StopwatchFrame"/> since last measurement. 
    /// </summary>
    private TimeSpan Elapsed { get; set; }

    /// <summary>
    /// Count of frames since last measurement.
    /// </summary>
    private int FrameCount { get; set; }

    /// <summary>
    /// Average fps (for rendering) in last <see cref="UpdateRate"/> amount of time.
    /// </summary>
    public double FpsRender { get; private set; }

    /// <summary>
    /// Average fps (for global rendering) in last <see cref="UpdateRate"/> amount of time.
    /// </summary>
    public double FpsGlobal { get; private set; }

    /// <summary>
    /// Human readable fps string.
    /// </summary>
    public string FpsString => $"FPS = {FpsRender:0} ({FpsGlobal:0})";

    #endregion

    #region // ctor

    /// <summary>
    /// Constructor.
    /// </summary>
    public FpsCounter(TimeSpan updateRate)
    {
        UpdateRate = updateRate;

        StopwatchUpdate = new Stopwatch();
        StopwatchFrame = new Stopwatch();

        StopwatchUpdate.Start();

        Elapsed = TimeSpan.Zero;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        StopwatchUpdate.Stop();
        StopwatchFrame.Stop();
    }

    #endregion

    #region // routines

    /// <summary>
    /// Start frame measurement.
    /// </summary>
    public void StartFrame()
    {
        StopwatchFrame.Restart();
    }

    /// <summary>
    /// Stop frame measurement.
    /// </summary>
    public TimeSpan StopFrame()
    {
        StopwatchFrame.Stop();

        // add elapsed time of this frame
        var elapsed = StopwatchFrame.Elapsed;
        Elapsed += elapsed;

        // increment frame count to respectively keep ratio
        FrameCount++;

        // check if we need to do average measurement
        var updateElapsed = StopwatchUpdate.Elapsed;
        if (updateElapsed >= UpdateRate)
        {
            // calc averages
            FpsRender = FrameCount / Elapsed.TotalSeconds;
            FpsGlobal = FrameCount / updateElapsed.TotalSeconds;

            // reset
            StopwatchUpdate.Restart();
            Elapsed = TimeSpan.Zero;
            FrameCount = 0;
        }

        return elapsed;
    }

    public TimeSpan RestartFrame()
    {
        var elapsed = StopFrame();
        StartFrame();

        return elapsed;
    }

    #endregion
}

