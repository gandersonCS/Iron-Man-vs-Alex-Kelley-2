using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
    [Export]
    public float walkSpeed = 300.0f;

    [Export(PropertyHint.Range, "0.0,1.0")]
    public float deceleration = 0.05f;

    [Export(PropertyHint.Range, "0.0,1.0")]
    public float acceleration = 0.05f;

    [Export]
    public float runSpeed = 700.0f;

    [Export]
    public float jumpForce = -400.0f;

    [Export(PropertyHint.Range, "0.0,1.0")]
    public float decelerateOnJumpRelease = 0.5f;

    AnimationPlayer animPlyr = null;

    public override void _Ready()
    {
        animPlyr = FindChild("AnimationPlayer") as AnimationPlayer;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        if (IsOnFloor() && velocity.X == 0)
        {
            animPlyr?.Play("idle");
        }
        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("jump") && (IsOnFloor() || IsOnWall()))
        {
            velocity.Y = jumpForce;
            animPlyr?.Stop();
            animPlyr?.Play("jump");
        }

        // Jump force based on hold time of space
        if (Input.IsActionJustReleased("jump") && velocity.Y < 0)
        {
            velocity.Y *= decelerateOnJumpRelease;
        }
        //Speed handling
        float speed;
        if (Input.IsActionPressed("run"))
        {
            speed = runSpeed;
    
        }
        else if (Input.IsActionPressed("walk"))
        {
            speed = walkSpeed / 2;
        }
        else
        {
            speed = walkSpeed;
        }
        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        //Animation handling done here
        Vector2 direction = Input.GetVector("left", "right", "up", "down");
        if (direction != Vector2.Zero)
        {
            velocity.X = Mathf.MoveToward(Velocity.X, direction.X * speed, speed * acceleration);
            if (Input.IsActionPressed("run"))
            {
                animPlyr?.Play("sprint");
            }
            else if (Input.IsActionPressed("walk"))
            {
                animPlyr?.Play("walk");
            }
            else if (Input.IsActionPressed("jump"))
            {
                animPlyr?.Play("jump");
            }
            else
            animPlyr?.Play("run");
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, walkSpeed * deceleration);
        }

        if (velocity.X != 0)
        {
            // Flip the sprite based on the direction of movement.
            Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");
            if (sprite != null)
            {
                sprite.FlipH = velocity.X < 0;
            }
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
