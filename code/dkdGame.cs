
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

		}

		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			var player = new dkdPlayer();
			client.Pawn = player;

			player.Respawn();
		}
	}

}
