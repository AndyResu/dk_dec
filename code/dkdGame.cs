
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dkdGame
{
	public partial class dkdGame : Sandbox.Game
	{
		public Vector3 snowballSpawnerPos = new Vector3(-468,700,1168);

		public dkdGame()
		{
			if ( IsServer )
			{
				snowballSpawn();
			}
		}

		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			var player = new dkdPlayer();
			client.Pawn = player;

			player.Respawn();
		}

		[Event.Tick]
		public void snowballSpawn()
		{
			// console.WriteLine();
			var time = 10000.00;
			var rng = new Random();
			if(time <= 0){
				var snowball = new ModelEntity();
				snowball.SetModel("models/things_from_kabubu2/snowball.vmdl");
				snowball.Position = new Vector3(rng.Next(-32, 33), rng.Next(-32, 33), 0) + snowballSpawnerPos;
				// x and y are the ground coords
				snowball.Rotation = Rotation.LookAt( Vector3.Random.Normal );
				snowball.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
				time = 10000;
			} else {
				time -= Time.Delta;
			}
		}
	}

}
