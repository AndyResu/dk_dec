using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dkdGame
{
	public partial class dkdGame : Sandbox.Game
	{
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
			var rng = new Random();
			var rngVal = rng.Next(1,300);
			if(rngVal <= 1.0){
				var snowball = new Snowball();
				Log.Info("SNOWBALL!");
			}
		}
	}

}
