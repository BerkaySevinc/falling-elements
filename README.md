# Falling Elements

**Falling Sand Simulation** written in C#.</br>
A simple 2D physics sandbox where materials like sand and water fall, interact, and collide.</br>
The core simulation logic is separated into a reusable library, with a demo application included for interaction and visualization.
</br>
</br>

# Details
- Written in C#.
- Uses Windows Forms for the GUI.
- Renders using the built-in C# Graphics library, relying exclusively on the CPU.
- Utilizes a 2D grid system to manage particles.
- Implements a simple update loop to simulate gravity and interactions.
- The mouse is used to "draw" particles on the screen.
</br>

# Projects
- **WorldSimulation** — Platform-agnostic core library. Handles the particle system, physics, and world simulation.
- **WorldSimulation.Renderer** — Windows Forms rendering layer. Draws the simulation and translates mouse input to world coordinates.
- **FallingElements.App** — Demo application. Wires up the simulation and renderer into an interactive sandbox.
</br>

# Media
![Video 1](Introduction%20Media/Video%201.gif)
![Video 2](Introduction%20Media/Video%202.gif)
![Video 3](Introduction%20Media/Video%203.gif)


