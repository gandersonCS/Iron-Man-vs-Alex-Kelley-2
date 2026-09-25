using Godot;
using System;

public partial class Bullet : Area3D
{
    [Export] public float Speed = 25.0f;

    public override void _Ready()
    {
        // Hook up the screen exit signal to delete the bullet
        VisibleOnScreenNotifier3D notifier = GetNode<VisibleOnScreenNotifier3D>("VisibleOnScreenNotifier3D");
        notifier.ScreenExited += () => QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Move forward continuously. In Godot, Transform.Basis.Z * -1 is the forward direction.
        Position += Transform.Basis.Z * -Speed * (float)delta;
    }
}