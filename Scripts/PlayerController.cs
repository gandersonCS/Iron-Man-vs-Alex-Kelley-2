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

    private AnimationTree animTree = null;
    private Node3D characterModel = null;

    public override void _Ready()
    {
        characterModel = GetNode<Node3D>("PlayerModel");
        if (characterModel == null)
        {
            GD.PrintErr("Character model not found. Please check the node path.");
            return;
        }

        GD.Print("Iron Man armor online.");
        animTree = characterModel.GetNode<AnimationTree>("AnimationTree");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;
        bool justJumped = false;

        // 1. The Mid-Air Hover Toggle
        if (Input.IsActionJustPressed("jump") && !IsOnFloor())
        {
            hovering = !hovering;
            GD.Print(hovering ? "Boot Jets Activated" : "Boot Jets off.");

            if (hovering)
            {
                velocity.Y = 0;
                // Tells the Transition node to switch to the Fly port
                animTree?.Set("parameters/State/transition_request", "fly");
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
            // Tells the Transition node to switch to the Jump port
            animTree?.Set("parameters/State/transition_request", "jump");
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

        Vector2 direction = Input.GetVector("left", "right", "up", "down");

        // 6. Split Movement Logic: Flying vs Grounded
        if (hovering)
        {
            velocity.X = Mathf.MoveToward(Velocity.X, direction.X * speed, speed * acceleration);
            velocity.Y = Mathf.MoveToward(Velocity.Y, -direction.Y * speed, speed * acceleration);
        }
        else
        {
            if (direction.X != 0)
            {
                velocity.X = Mathf.MoveToward(Velocity.X, direction.X * speed, speed * acceleration);

                if (IsOnFloor() && !justJumped)
                {
                    animTree?.Set("parameters/State/transition_request", "run");
                }
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, walkSpeed * deceleration);

                if (IsOnFloor() && Mathf.IsZeroApprox(velocity.X) && !justJumped)
                {
                    animTree?.Set("parameters/State/transition_request", "idle");
                }
            }
        }

        // Calculate Facing Left and Right
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