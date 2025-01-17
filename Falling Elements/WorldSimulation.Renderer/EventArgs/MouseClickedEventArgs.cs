﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation.Renderer;

public class MouseClickedEventArgs : EventArgs
{
    public MouseButtons Button { get; }

    public System.Drawing.Point Location { get; }
    public System.Drawing.Point WorldLocation { get; }

    public MouseClickedEventArgs(MouseButtons button, System.Drawing.Point location, System.Drawing.Point worldLocation) =>
        (Button, Location, WorldLocation) = (button, location, worldLocation);
}
