﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public interface IMovableParticle : IParticle
{
    public float Mass { get; }
}
