using Hammer;
using Sandbox;
using Sandbox.Internal;
using System.Linq;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;

// "SnowballForThrow" is a reference to the Mount & Blade: Warband mod Full Invasion 2's crazy farmerd faction weapon with a host of weapons called, "x for throw" like "potatoes for throw" etc.
// Yes, you could throw the potatoes :)

namespace dkdGame
{
	[Library("snowballforthrow", Description = "VR Player throw!")]
	[EditorModel("models/things_from_kabubu2/snowball2.vmdl", staticGreen: 100, FixedBounds = true)]
	[EntityTool("SnowballForThrow", "dkdGame", "VR Player throw!")]
	public partial class SnowballForThrow : ModelEntity
	{
		public Vector3 snowballForThrowSpawnerPos = new Vector3(690,1106,1356);
		public Random rng = new Random();
		public TimeSince timeSinceCreated = 0;
		public int idleTimeout = 25;

		public override void Spawn()
		{
			SetModel("models/things_from_kabubu2/snowball2.vmdl");
			SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
			Position = snowballForThrowSpawnerPos;
			Rotation = Rotation.LookAt(Vector3.Random.Normal);

			Transmit = TransmitType.Always;
			EnableSolidCollisions = true;
			// EnableSelfCollisions = false;
			EnableTouch = true;

			// base.Spawn();
		}

		public override void Touch(Entity other)
		{
			/*if(other is not dkdPlayer && other+"" is not "WorldEntity 0"  && IsServer){
				Log.Info(other);
			}*/
			if(other is dkdPlayer && IsServer){
				var okay = (dkdPlayer) other;
				if(!okay.viewType.Equals("virst")){
					okay.Respawn();
					// or reduce hp, idk
				}
			}
			// Log.Info(timeSinceCreated);
			if(IsServer && timeSinceCreated > idleTimeout && (Position.z <= 1220 || Position.z > 1356))
			{
				timeSinceCreated = 0;
				teleport();
				// Log.Info("Touch Teleport");
			}
			base.Touch(other);
		}

		[Event.Tick]
		public void tooLowDeath()
		{
			var toggle = true;
			if(toggle && Position.z <= 150){
				toggle = false;
				teleport();
			}
		}

		public void teleport(){
			// Log.Info("Teleported");
			Position = new Vector3(504, 1232, 0); // do this to make sure while moving to the position it does not hit the player
			Position = snowballForThrowSpawnerPos;
		}
	}
}
