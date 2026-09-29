using Godot;
using System;

public partial class Bullet : Area3D
{
    [Export] public float Speed = 25.0f;
    [Export] public int Damage = 10;

    public override void _PhysicsProcess(double delta)
    {
        // Move forward continuously. In Godot, Transform.Basis.Z * -1 is the forward direction.
        Position += Transform.Basis.Z * -Speed * (float)delta;
    }

    // Connect this to the Area3D's "body_entered" signal
    private void OnBodyEntered(Node3D body)
    {
        // Future combat logic: check if the body is an enemy and apply Damage
        // if (body.HasMethod("TakeDamage")) { body.Call("TakeDamage", Damage); }

        QueueFree(); // Destroy the bullet on impact
    }

    // Connect this to a VisibleOnScreenNotifier3D's "screen_exited" signal
    private void OnScreenExited()
    {
        // Prevents memory leaks by deleting bullets that fly off-screen
        QueueFree();
    }
}