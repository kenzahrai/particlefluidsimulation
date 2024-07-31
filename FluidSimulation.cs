using Godot;
using System;
using System.Collections.Generic;

public partial class FluidSimulation : Node2D
{
	public Vector2 BoundaryMin = new Vector2(0, 0);
	public Vector2 BoundaryMax = new Vector2(500, 500);

	public Vector2 BoundaryMin2 = new Vector2(0, 0);
	public Vector2 BoundaryMax2 = new Vector2(100, 100);

	public Vector2 Center = new Vector2(500, 500);
	public float dens = 0.0f;
	public int particleCount = 200;
	public float collisionDamping = 0.1f;

	public float velocityDamping = 0.05f;	
	
	public float restDensity = 0.02f;
	public float deltaTime = 0.017f;
	private float collisionDistance = 10f;
	public float gravity = 0f;
	public float pressureMultiplier = 1000.0f;
	public float nearpressureMultiplier = 500.0f;

	public float deltaFactor = 0.01f;
	public float ParticleSpacing = 20.0f;
	public float smoothingRadius = 40.0f;

	public float particleMass = 10.0f;

	public float average_density = 30.0f;

	public Vector3[] debuginfo;


	private List<Particle> particles = new List<Particle>();


	private void PlaceParticlesRandomly(){
		Random rand = new Random();
		for (int i = 0; i < particleCount; i++)
		{
			
			Vector2 pos = new Vector2((float)rand.NextDouble() * (BoundaryMax.X - BoundaryMin.X), (float)rand.NextDouble() * (BoundaryMax.Y - BoundaryMin.Y));
			Particle particle = new Particle(pos);
			AddChild(particle);
			particles.Add(particle);
		}


	}
	private void PlaceParticlesUniformly()
	{
		float startX = BoundaryMin.X;
		float startY = BoundaryMin.Y;
		float endX = BoundaryMax.X;
		float endY = BoundaryMax.Y;

		int numParticlesX = Mathf.FloorToInt((endX - startX) / ParticleSpacing);
		int numParticlesY = Mathf.FloorToInt((endY - startY) / ParticleSpacing);

		for (int i = 0; i < numParticlesX; i++)
		{
			for (int j = 0; j < numParticlesY; j++)
			{
				float posX = startX + i * ParticleSpacing + ParticleSpacing / 2.0f;
				float posY = startY + j * ParticleSpacing + ParticleSpacing / 2.0f;
				Vector2 pos = new Vector2(posX, posY);
				Particle particle = new Particle(pos);
				AddChild(particle);
				particles.Add(particle);
			}
		}
		particleCount = particles.Count;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Center.X = BoundaryMax.X/2;
		Center.Y = BoundaryMax.Y/2;
		
		PlaceParticlesRandomly();
		//PlaceParticlesUniformly();
		
	}
	
	private float Poly6Kernel(float r)
	{
		float h = smoothingRadius;
		if (r >= 0 && r <= h)
		{
			float h2 = h * h;
			float r2 = r * r;
			float coeff = 315.0f / (64.0f * Mathf.Pi * Mathf.Pow(h, 8));
			return coeff * Mathf.Pow(Mathf.Pow(h, 2) - Mathf.Pow(r, 2), 3);
		}
		return 0.0f;
	}

	float SpikyKernelPow3(float dst)
	{
		float radius = smoothingRadius;
		if (dst < radius)
		{
			float v = radius - dst;
			return v * v * v * 10 / (Mathf.Pi * Mathf.Pow(smoothingRadius, 5));
		}
		return 0;
	}

	float DerivativeSpikyPow3(float dst)
	{
		float radius = smoothingRadius;

		if (dst <= radius)
		{
			float v = radius - dst;
			return -v * v * 30 / (Mathf.Pow(smoothingRadius, 5) * Mathf.Pi);
		}
		return 0;
	}

	private float SpikyKernelPow2(float dst)
	{
		float radius = smoothingRadius;
		if (dst < radius)
		{
			float v = radius - dst;
			return v * v *  6 / (Mathf.Pi * Mathf.Pow(smoothingRadius, 4));
		}
		return 0;
	}

	
	float DerivativeSpikyPow2(float dst)
	{
		float radius = smoothingRadius;
		if (dst <= radius)
		{
			float v = radius - dst;
			return -v * 12 / (Mathf.Pow(smoothingRadius, 4) * Mathf.Pi);
		}
		return 0;
	}


	private float CalculateDensityAtPoint(Vector2 pos){

		float density = particleMass * SpikyKernelPow2(0);
		Vector3[] vector3s= new Vector3[particles.Count];
		int i = 0;
		foreach (var particle in particles)
		{
			particle.mass = particleMass;
			Vector2 Vec = particle.Position - pos;
			float VecLen = Vec.Length();
			float influence = 0;
			if (VecLen <= smoothingRadius)
			{
			influence += particleMass * SpikyKernelPow2(VecLen);
			}
			density += influence;
			Vector3 information = new Vector3(influence,particle.Position.X,particle.Position.Y);
			vector3s[i] = information;
			i+=1;
		}
		debuginfo = vector3s;
		return density;
	}

	public void InteractionForceCircle(Vector2 pos, float radius, float strength){
		foreach (var particle in particles)
		{
			Vector2 Vec = particle.Position - pos;
			float VecLen = Vec.Length();
			Vector2 VecDir = VecLen == 0 ? Vector2.Zero : Vec/VecLen;
			Vector2 interactionForce = Vector2.Zero;
			if (VecLen <= radius){
				float scale = 1; //- VecLen/radius;
				interactionForce += (VecDir*strength - particle.velocity)*scale + Vector2.Up*gravity;
;
			}
			particle.ApplyForce(interactionForce, deltaTime);
			
		}
	}


	private void CalculateDensity(){
		float density_sum = 0f;

		foreach (var particle in particles)
		{
			particle.density = 0f;
			foreach (var neighbor in particles)
			{
				Vector2 Vec = neighbor.Position - particle.Position;
				float VecLen = Vec.Length();
				if (VecLen <= smoothingRadius)
				{
				particle.density += particleMass * SpikyKernelPow2(VecLen);
				particle.near_density += particleMass * SpikyKernelPow3(VecLen);
				}
		
			}
			density_sum += particle.density;
		}
		average_density= 0;
		average_density = density_sum/particles.Count;

	}


	private void ApplyPressureForce(){
		foreach(var particle in particles){
			Vector2 pressureForce = Vector2.Zero;
			Vector2 viscosityForce = Vector2.Zero;
			foreach(var neighbor in particles){
				if (particle == neighbor)
					continue;
				var particle_pressure = ConvertDensityToPressure(particle.density);
				var neighbor_pressure = ConvertDensityToPressure(neighbor.density);
				var particle_near_pressure = ConvertNearDensityToNearPressure(particle.near_density);
				var neighbor_near_pressure = ConvertNearDensityToNearPressure(neighbor.near_density);
				Vector2 Vec = neighbor.Position - particle.Position;
				float VecLen = Vec.Length(); 
				Vector2 Vecdir = VecLen == 0 ? Vector2.Zero : Vec/VecLen;
				float sharedPressure = (particle_pressure + neighbor_pressure) / 2;
				float sharednearPressure = (particle_near_pressure + neighbor_near_pressure) / 2;

				pressureForce +=  particleMass / neighbor.density * sharedPressure  * Vecdir * DerivativeSpikyPow2(VecLen);
				pressureForce +=  particleMass / neighbor.near_density * sharednearPressure  * Vecdir * DerivativeSpikyPow3(VecLen);
			}
			particle.ApplyForce(pressureForce/particle.density, deltaTime);


		}

	}

	private Vector2 CalculatePressureForceAtPoint(Vector2 point){

		Vector2 pressureForce = Vector2.Zero;
		foreach(var neighbor in particles){
			var particle_pressure = ConvertDensityToPressure(CalculateDensityAtPoint(point));
			var neighbor_pressure = ConvertDensityToPressure(neighbor.density);
			Vector2 Vec = neighbor.Position - point;
			float VecLen = Vec.Length(); 
			Vector2 Vecdir = Vector2.Up;
			if (VecLen != 0)
				Vecdir = Vec/VecLen;
			float sharedPressure = (particle_pressure + neighbor_pressure) / 2;
			pressureForce +=  neighbor.mass / neighbor.density * sharedPressure  * Vecdir * DerivativeSpikyPow2(VecLen);
		}	
		return pressureForce;		
	}

	private float ConvertDensityToPressure(float density){
		return pressureMultiplier * (density - restDensity);
	}
	private float ConvertNearDensityToNearPressure(float near_density){
		return nearpressureMultiplier * near_density;
	}

	private void ApplyBoundaryConditions()
	{
		foreach (var particle in particles)
		{
			if (particle.pos.X < BoundaryMin.X){
				particle.pos.X = BoundaryMin.X;
				particle.velocity.X *= -collisionDamping;
			}
			if (particle.pos.X > BoundaryMax.X){
				particle.pos.X = BoundaryMax.X;
				particle.velocity.X *= -collisionDamping;
			}
			if (particle.pos.Y < BoundaryMin.Y){
				particle.pos.Y = BoundaryMin.Y;
				particle.velocity.Y *= -collisionDamping;
			}
			if (particle.pos.Y > BoundaryMax.Y){
				particle.pos.Y = BoundaryMax.Y;
				particle.velocity.Y *= -collisionDamping;
			}
				
		}
	}

	private void AddParticle(){

		Random rand = new Random();

		Vector2 pos = new Vector2((float)rand.NextDouble() * (BoundaryMax.X - BoundaryMin.X), (float)rand.NextDouble() * (BoundaryMax.Y - BoundaryMin.Y));
		Particle particle = new Particle(pos);
		AddChild(particle);
		particles.Add(particle);
		


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		CalculateDensity();
		ApplyPressureForce();
		foreach (var particle in particles)
		{
			particle.ApplyForce(Vector2.Down*gravity,deltaTime);
			Vector2 dampingForce = -velocityDamping * particle.velocity;
			particle.ApplyForce(dampingForce,deltaTime);

			particle.Integrate(deltaTime);
		}
		ApplyBoundaryConditions();
	}
}
