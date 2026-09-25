using Godot;
using System;

public partial class RepulsorArm : Node3D
{
    [Export] public PackedScene BulletScene;

    private Marker3D _muzzle;
    private AnimationPlayer _animPlayer;

    public override void _Ready()
    {
        // The "../" tells the script to step up to the parent node, then look for the siblings
        _muzzle = GetNode<Marker3D>("../Skeleton3D/BoneAttachment3D/Muzzle");
        _animPlayer = GetNode<AnimationPlayer>("../AnimationPlayer");
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
        // 1. Play your visual recoil animation
        // Find a snappy punch or blast animation from Mixamo and trigger it here
        _animPlayer?.Play("Combat/Shoot");

        Camera3D cam = GetViewport().GetCamera3D();
        if (cam == null || BulletScene == null || _muzzle == null) return;

        // 2. Cast a mathematical ray from the camera through the mouse cursor
        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector3 rayOrigin = cam.ProjectRayOrigin(mousePos);
        Vector3 rayNormal = cam.ProjectRayNormal(mousePos);

        // 3. Calculate where that ray hits the Z=0 plane (the plane your character moves on)
        float t = -rayOrigin.Z / rayNormal.Z;
        Vector3 targetPos = rayOrigin + rayNormal * t;

        // 4. Instantiate and fire the bullet
        Node3D bullet = BulletScene.Instantiate<Node3D>();
        GetTree().Root.AddChild(bullet);

        // Snap the bullet to the hand bone, then point it mathematically at the mouse
        bullet.GlobalPosition = _muzzle.GlobalPosition;
        bullet.LookAt(targetPos, Vector3.Up);
    }
}