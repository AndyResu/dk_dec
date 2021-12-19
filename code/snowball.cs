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
	[EditorModel("models/editor/playerstart.vmdl", staticGreen: 100, FixedBounds = true)]
	[EntityTool("Snowball", "dkdGame", "Do not touch!")]
	[BoundsHelper("mins", "maxs", false, true)]
	public partial class Snowball : ModelEntity
	{
		[Property("mins", Title = "Lower Corner of bounding box for the snowball.")]
		[DefaultValue("-35 -35 -35")]
		[Net]
		public Vector3 Mins { get; set; } = new Vector3(-35, -35, -35);
		[Property("maxs", Title = "Upper Corner of bounding box for the snowball.")]
		[DefaultValue("35 35 35")]
		[Net]
		public Vector3 Maxs { get; set; } = new Vector3(35, 35, 35);
		public Vector3 snowballSpawnerPos = new Vector3(504,1232,1296);
		public Random rng = new Random();

		public ModelEntity Snowballe;

		public override void Spawn()
		{
			// Snowballe = new ModelEntity();
			SetModel("models/things_from_kabubu2/snowball.vmdl");
			SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
			var bounds = new BBox(Position + Mins, Position + Maxs);
			// Snowballe.Position = bounds.ClosestPoint(new Vector3(rng.Next(-25, 26), rng.Next(-25, 26), 0) + snowballSpawnerPos);
			Position = new Vector3(rng.Next(-10, 11), rng.Next(-10, 11), 0) + snowballSpawnerPos;
			Rotation = Rotation.LookAt(Vector3.Random.Normal);

			Transmit = TransmitType.Always;
			EnableSolidCollisions = true;
			// EnableSelfCollisions = false;
			EnableTouch = true;

			base.Spawn();
		}

		public override void ClientSpawn()
		{
			// base.ClientSpawn();

			// would be fun to have the snowball gain parts of things as it hits things.
		}

		public override void Touch(Entity other)
		{
			if(other is not dkdPlayer && other+"" is not "WorldEntity 0"  && IsServer){
				Log.Info(other);
			}
			if(other is dkdPlayer pl && IsServer){
				pl.Respawn();
				// or reduce hp, idk
				// Log.Info("cowburgled");
			}
			base.Touch(other);
		}

		public override void EndTouch( Entity other )
		{
			base.EndTouch( other );
			//if ( other is not dkdPlayer pl ) return;
		}

		[Event.Frame]
		private void OnFrame()
		{
			//DebugOverlay.Box( Position + Mins, Position + Maxs, color );
			/*if (Snowballe.getPos().z <= 0)
			{
				Snowballe.Remove();
			}*/
		}
	}
}
