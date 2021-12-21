using Sandbox;

namespace dkdGame
{
	partial class dkdPlayer : Player
	{
		public String viewType = "arcade";
		
		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Animator = new StandardPlayerAnimator();

			if(viewType == "arcade"){
				Camera = new ArcadeCamera();
				Controller = new ArcadeController();
			}else{
				Camera = new FirstPersonCamera();
				Controller = new WalkController();
			}

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			base.Respawn();
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			SimulateActiveChild( cl, ActiveChild );
		}

		public override void OnKilled()
		{
			base.OnKilled();
			EnableDrawing = false;
		}

		[Event.Tick]
		public void tooLowDeath()
		{
			var toggle = true;
			if(toggle && Position.z <= -25){
				toggle = false;
				Respawn();
			}
		}
	}
}
