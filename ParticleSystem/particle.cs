using Godot;
using System;

public partial class Particle : Node2D
{
	public Vector2 pos;
	public float mass = 1;
	public Vector2 velocity = Vector2.Zero;
	public float density = 0;
	public float near_density = 0;

	public Vector2 acceleration = Vector2.Zero;
	private ColorRect colorRect = new ColorRect();
	public float pressure = 0;
	public float near_pressure = 0;

	private Vector2 force = Vector2.Zero;
	// Called when the node enters the scene tree for the first time.
	public Particle(Vector2 posi)
	{
		pos = posi;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		colorRect.Size = new Vector2(4,4);
		colorRect.Position = new Vector2(-colorRect.Size.X/2,-colorRect.Size.Y/2);
		colorRect.Color = new Color(1,1,1,1);
		AddChild(colorRect);
		Position = pos;
	}
	public void ApplyForce(Vector2 f, float delta)
	{
		//force += f;
		velocity += f * mass * delta;
	}
	public Vector2 GetPredictedPosition(float delta)
	{
		//force += f;
		return Position + velocity *delta;
	}
	
	public void Integrate(float delta)
	{
		Position = pos;
		pos += velocity * delta;
		force = Vector2.Zero; 
		Rotation = velocity.Angle();
	}

	public override void _Draw()
	{
		//DrawCircle(Vector2.Zero,3,Color.Color8(255,255,255));
		DrawLine(Vector2.Zero,Vector2.Right*5,Color.Color8(255,255,255) );
		base._Draw();
	}
	public override void _Process(double delta)
	{
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame

}
