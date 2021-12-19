﻿using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dkdGame
{
	public partial class dkdGame : Sandbox.Game
	{
		public TimeSince timeSinceCreated = 0;
		public int count = 0;
		public Random rng = new Random();
		public int rngVal = 1000;

		public dkdGame()
		{
			if ( IsServer )
			{
				// snowballSpawn();
				// new DeletionBox();
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
			if(count < 25){
				rngVal = rng.Next(6,12);
				if(timeSinceCreated >= rngVal){
					timeSinceCreated = 0;
					var snowball = new Snowball();
					count += 1;
					Log.Info("SNOWBALL! " + count);
				}
			}
		}
	}
}
