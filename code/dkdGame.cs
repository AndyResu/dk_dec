using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dkdGame
{
	public partial class dkdGame : Sandbox.Game
	{
		public TimeSince timeSinceCreated = 0;
		public TimeSince timeSinceCreated2 = 0;
		public Random rng = new Random();
		public int rngVal = 1000;
		public int count = 0;
		public int maxSnowballz = 16;
		public int countForThrow = 0;
		public int maxSnowballzForThrow = 6;

		public dkdGame()
		{
			if ( IsServer )
			{
				_ = new ExampleHudEntity();
			}
		}

		[ServerCmd("Code")]
		public static void Code(string code)
		{
			// the client that is calling the console command
			var callingClient = ConsoleSystem.Caller;
			try{
				Player targetPlayer = (Player) callingClient.Pawn;
				dkdPlayer targetDkdPlayer = (dkdPlayer) targetPlayer;
				if(code.Equals("virst")){
					if(!callingClient.IsUsingVr){
						code = "not in vr";
					}
					// do a vr
					targetDkdPlayer.viewType = code;
					targetDkdPlayer.Respawn();
				}else{
					targetDkdPlayer.viewType = code;
					targetDkdPlayer.Respawn();
				}
			} catch (Exception e){
				// This will help catch (& display) any type-casting errors
				Log.Error(e);
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
			if(IsServer){
				if(count < maxSnowballz){
					rngVal = rng.Next(3,6);
					if(timeSinceCreated >= rngVal){
						timeSinceCreated = 0;
						var snowball = new Snowball();
						count += 1;
						// Log.Info("SNOWBALL! " + count);
					}
				}
				if(countForThrow < maxSnowballzForThrow){
					rngVal = rng.Next(3,12);
					if(timeSinceCreated2 >= rngVal){
						timeSinceCreated2 = 0;
						var snowball2 = new SnowballForThrow();
						countForThrow += 1;
						Log.Info("SNOWBALL FOR THROW! " + countForThrow);
					}
				}
			}
			
		}
	}
}
