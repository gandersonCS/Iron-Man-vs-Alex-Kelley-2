using Godot;
using System;

public partial class RepulsorArm : Node3D
{
    [Export] public PackedScene BulletScene;

    private Marker3D _muzzle;
    private AnimationTree _animTree;

    public override void _Ready()
    {
        _muzzle = GetNode<Marker3D>("Muzzle");
        _animTree = GetNode<AnimationTree>("../AnimationTree");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("shoot"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // Tells the OneShot node to play the shoot animation once, overriding the upper body
        if (_animTree != null)
        {
            _animTree.Set("parameters/OneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        }

        Camera3D cam = GetViewport().GetCamera3D();

        // If any of these are missing, the bullet won't spawn!
        if (cam == null || BulletScene == null || _muzzle == null) return;

        // Project the mouse into 3D space
        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector3 rayOrigin = cam.ProjectRayOrigin(mousePos);
        Vector3 rayNormal = cam.ProjectRayNormal(mousePos);

        // Find the Z=0 intersection
        float t = -rayOrigin.Z / rayNormal.Z;
        Vector3 targetPos = rayOrigin + rayNormal * t;

        // Fire the bullet
        Node3D bullet = BulletScene.Instantiate<Node3D>();
        GetTree().Root.AddChild(bullet);

        // Snap bullet to our static muzzle and aim it at the mouse
        bullet.GlobalPosition = _muzzle.GlobalPosition;
        bullet.LookAt(targetPos, Vector3.Up);
    }
}