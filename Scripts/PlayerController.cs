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

		// Add gravity
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump
		if (Input.IsActionJustPressed("jump") && (IsOnFloor() || IsOnWall()))
		{
			velocity.Y = jumpForce;
			animPlyr?.Stop();
			animPlyr?.Play("Movement/jump");
		}

		// Variable jump height
		if (Input.IsActionJustReleased("jump") && velocity.Y > 0)
		{
			velocity.Y *= decelerateOnJumpRelease;
		}

		// Speed handling
		float speed = walkSpeed;
		if (Input.IsActionPressed("run")) speed = runSpeed;
		else if (Input.IsActionPressed("walk")) speed = walkSpeed / 2;

		// Get input direction
		Vector2 direction = Input.GetVector("left", "right", "up", "down");

		if (direction != Vector2.Zero)
		{
			velocity.X = Mathf.MoveToward(Velocity.X, direction.X * speed, speed * acceleration);

			// Play movement animations only if on the floor so they don't override the jump
			if (IsOnFloor())
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
			if (IsOnFloor() && velocity.X == 0)
			{
				animPlyr?.Play("Movement/idle");
			}
		}

		// Calculate Facing Left and Right
		if (velocity.X != 0 && characterModel != null)
		{
			// Instantly flip the physical 3D mesh by rotating its Y-axis by 180 degrees
			Vector3 currentRot = characterModel.RotationDegrees;
			currentRot.Y = velocity.X < 0 ? 180 : 0;
			characterModel.RotationDegrees = currentRot;
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
