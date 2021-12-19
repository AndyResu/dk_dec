using Hammer;
using Sandbox;
using Sandbox.Internal;
using System.Linq;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dkdGame
{
	[Library("snowball", Description = "Do not touch!")]
	[EditorModel("models/things_from_kabubu2/snowball.vmdl", staticGreen: 100, FixedBounds = true)]
	[EntityTool("Snowball", "dkdGame", "Do not touch!")]
	public partial class Snowball : ModelEntity
	{
		public Vector3 snowballSpawnerPos = new Vector3(504,1232,1296);
		public Random rng = new Random();
		public TimeSince timeSinceCreated = 0;
		public TimeSince timeSinceStopped = 30;
		public int idleTimeout = 25;
		public bool isStill = false; 

		public override void Spawn()
		{
			SetModel("models/things_from_kabubu2/snowball.vmdl");
			SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
			Position = new Vector3(rng.Next(-10, 11), rng.Next(-10, 11), 0) + snowballSpawnerPos;
			Rotation = Rotation.LookAt(Vector3.Random.Normal);

			Transmit = TransmitType.Always;
			EnableSolidCollisions = true;
			// EnableSelfCollisions = false;
			EnableTouch = true;

			base.Spawn();
		}

		public override void Touch(Entity other)
		{
			/*if(other is not dkdPlayer && other+"" is not "WorldEntity 0"  && IsServer){
				Log.Info(other);
			}*/
			if(other is dkdPlayer pl && IsServer){
				pl.Respawn();
				// or reduce hp, idk
				// Log.Info("cowburgled");
			}
			// Log.Info(timeSinceCreated);
			if(IsServer && timeSinceCreated > idleTimeout)
			{
				timeSinceCreated = 0;
				teleport();
			}
			base.Touch(other);
		}

		[Event.Tick]
		public void declog(){
			if(IsServer){
				if(Velocity.x == 0 && Velocity.y == 0 && Velocity.z == 0){
					isStill = true;
				}
				if(isStill && timeSinceStopped > idleTimeout && timeSinceStopped < idleTimeout+5){
					// declog
					timeSinceStopped = idleTimeout+5;
					isStill = false;
					teleport();
				}
				if(isStill && timeSinceStopped >= idleTimeout+5){
					timeSinceStopped = 0;
				}
			}
		}

		public void teleport(){
			Log.Info("Teleported");
			Position = new Vector3(504, 1232, 0); // do this to make sure while moving to the position it does not hit the player
			Position = new Vector3(rng.Next(-10, 11), rng.Next(-10, 11), 0) + snowballSpawnerPos;
		}
	}
}
