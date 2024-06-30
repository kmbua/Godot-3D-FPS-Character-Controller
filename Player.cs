using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	[Export] public Camera3D CameraNode;
	[Export] public Node3D WeaponRig;


	[Export] public float RotationSpeed { get; set; }
	[Export] public float CameraActualRotationSpeed { get; set; }
	[Export] public float WeaponActualRotationSpeed { get; set; }
	[Export] public float VerticalRotationLimit { get; set; } = 00;

	[ExportGroup("Nodes")]
	
	[Export] public AnimationPlayer animationPlayer;
	[Export] public AnimationTree animationTree;

	public bool IsCrouching = false;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private Vector3 targetRotation;
	
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("EXIT"))
		{
			GetTree().Quit();
		}
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
		{
			targetRotation = new Vector3(
				Mathf.Clamp((-1 * mouseMotion.Relative.Y * RotationSpeed) + targetRotation.X, -VerticalRotationLimit, VerticalRotationLimit),
				Mathf.Wrap((-1 * mouseMotion.Relative.X * RotationSpeed) + targetRotation.Y, 0, 360),
				0);
		}
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("JUMP") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("MOVE_LEFT", "MOVE_RIGHT", "MOVE_FORWARD", "MOVE_BACKWARD");
		Vector3 direction = (CameraNode.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		CameraNode.Rotation = new Vector3(
			Mathf.LerpAngle(CameraNode.Rotation.X, Mathf.DegToRad(targetRotation.X), CameraActualRotationSpeed * (float)delta),
			Mathf.LerpAngle(CameraNode.Rotation.Y, Mathf.DegToRad(targetRotation.Y), CameraActualRotationSpeed * (float)delta),
		0);

		WeaponRig.Rotation = new Vector3(
			Mathf.LerpAngle(WeaponRig.Rotation.X, Mathf.DegToRad(targetRotation.X), WeaponActualRotationSpeed * (float)delta),
			Mathf.LerpAngle(WeaponRig.Rotation.Y, Mathf.DegToRad(targetRotation.Y), WeaponActualRotationSpeed * (float)delta),
		0);
	}
}
