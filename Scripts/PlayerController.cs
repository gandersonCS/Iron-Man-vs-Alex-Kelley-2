using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
    [Export] public float walkSpeed = 3.0f;
    [Export(PropertyHint.Range, "0.0,1.0")] public float deceleration = 0.1f;
    [Export(PropertyHint.Range, "0.0,1.0")] public float acceleration = 0.1f;
    [Export] public float runSpeed = 8.0f;
    [Export] public float jumpForce = 10.0f;
    [Export(PropertyHint.Range, "0.0,1.0")] public float decelerateOnJumpRelease = 0.5f;
    public bool hovering = false;

    private AnimationPlayer animPlyr = null;
    private Node3D characterModel = null;

    public override void _Ready()
    {
        // Ensure this path points to your actual mesh container and AnimationPlayer
        characterModel = GetNode<Node3D>("PlayerModel");
        if (characterModel == null)
        {
            GD.PrintErr("Character model not found. Please check the node path.");
        }
        else
        {
            GD.Print("Iron Man armor online.");
        }
        animPlyr = characterModel.GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;
        bool justJumped = false; // Keeps your ground animations from overriding the jump frame

        // 1. The Mid-Air Hover Toggle
        if (Input.IsActionJustPressed("jump") && !IsOnFloor())
        {
            hovering = !hovering; // Instantly flips true to false, or false to true
            GD.Print("Boot Jets off.");
            if (hovering)
            {
                GD.Print("Boot Jets Activated");
                velocity.Y = 0; // The repulsor snap: instantly kills falling momentum
                animPlyr?.Play("Movement/fly1");
            }

            
        }

        // 2. Add Gravity (Only if NOT hovering)
        if (!IsOnFloor() && !hovering)
        {

            velocity += GetGravity() * (float)delta;
        }

        // 3. Handle Ground Jump
        if (Input.IsActionJustPressed("jump") && IsOnFloor())
        {
            velocity.Y = jumpForce;
            animPlyr?.Stop();
            animPlyr?.Play("Movement/jump");
            justJumped = true;
        }

        // 4. Variable Jump Height
        if (Input.IsActionJustReleased("jump") && velocity.Y > 0 && !hovering)
        {
            velocity.Y *= decelerateOnJumpRelease;
        }

        // 5. Speed handling
        float speed = walkSpeed;
        if (Input.IsActionPressed("run")) speed = runSpeed;
        else if (Input.IsActionPressed("walk")) speed = walkSpeed / 2;

        // Get input direction
        Vector2 direction = Input.GetVector("left", "right", "up", "down");

        // 6. Split Movement Logic: Flying vs Grounded
        if (hovering)
        {
            // Omnidirectional flight (bypasses ground deceleration)
            velocity.X = Mathf.MoveToward(Velocity.X, direction.X * speed, speed * acceleration);

            // In 3D, Y is up/down. Input.GetVector usually maps "up" to a negative value.
            // If pressing up makes Iron Man fly into the floor, change this to: -direction.Y
            velocity.Y = Mathf.MoveToward(Velocity.Y, -direction.Y * speed, speed * acceleration);
        }
        else
        {
            // Standard platforming logic
            if (direction.X != 0)
            {
                velocity.X = Mathf.MoveToward(Velocity.X, direction.X * speed, speed * acceleration);

                // Play movement animations only if on the floor so they don't override the jump
                if (IsOnFloor() && !justJumped)
                {
                    if (Input.IsActionPressed("run")) animPlyr?.Play("Movement/run");
                    else if (Input.IsActionPressed("walk")) animPlyr?.Play("Movement/Walking");
                    else animPlyr?.Play("Movement/standardRun");
                }
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, walkSpeed * deceleration);

                // Play idle only if on the floor and completely stopped
                if (IsOnFloor() && Mathf.IsZeroApprox(velocity.X) && !justJumped)
                {
                    animPlyr?.Play("Movement/idle");
                }
            }
        }

        // Calculate Facing Left and Right based strictly on raw input
        if (direction.X != 0 && characterModel != null)
        {
            Vector3 currentRot = characterModel.RotationDegrees;
            currentRot.Y = direction.X < 0 ? 180 : 0;
            characterModel.RotationDegrees = currentRot;
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}