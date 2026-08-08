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

    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && (IsOnFloor() || IsOnWall()))
		{
			velocity.Y = jumpForce;
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
		else
		{ 
			speed = walkSpeed;
		}
			// Get the input direction and handle the movement/deceleration.
			// As good practice, you should replace UI actions with custom gameplay actions.
			Vector2 direction = Input.GetVector("left", "right", "up", "down");
		if (direction != Vector2.Zero)
		{
            velocity.X = Mathf.MoveToward(Velocity.X, direction.X * speed, speed * acceleration);
        }
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, walkSpeed * deceleration);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
