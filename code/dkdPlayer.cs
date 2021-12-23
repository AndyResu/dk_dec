using Sandbox;

namespace dkdGame
{
	partial class dkdPlayer : Sandbox.Player
	{
		public string viewType = "";
		public Vector3 vrSpawn = new Vector3(580, 1088, 1212);
		private Clothing.Container _clothing = new();
		
		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Animator = new StandardPlayerAnimator();

			// Log.Info("tickles: " + viewType);
			if(viewType.Equals("virst") || Client.IsUsingVr){
				viewType = "virst";
				Log.Info("Congrats, based cerebral: " + viewType);
				Controller = new VrWalkController();
				Animator = new VrPlayerAnimator();
				Camera = new VrCamera();
				CreateHands();
				SetBodyGroup( "Hands", 1 ); // Hide hands
				Position = vrSpawn;
			}else if(viewType.Equals("first")){
				Camera = new FirstPersonCamera();
				Controller = new WalkController();
			}else{ // arcade
				Camera = new ArcadeCamera();
				Controller = new ArcadeController();
			}

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			if(!viewType.Equals("virst")){
				base.Respawn();
			}
			if(!(viewType.Equals("first") || viewType.Equals("virst"))){
				_clothing.LoadFromClient( Client );
				_clothing.DressEntity( this );
			}
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			SimulateActiveChild( cl, ActiveChild );

			CheckRotate();
			SetVrAnimProperties();

			LeftHand?.Simulate( cl );
			RightHand?.Simulate( cl );
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

		// VR specific stuff
		[Net, Local] public LeftHand LeftHand { get; set; }
		[Net, Local] public RightHand RightHand { get; set; }

		private void CreateHands()
		{
			DeleteHands();

			LeftHand = new() { Owner = this };
			RightHand = new() { Owner = this };

			LeftHand.Other = RightHand;
			RightHand.Other = LeftHand;
		}

		private void DeleteHands()
		{
			LeftHand?.Delete();
			RightHand?.Delete();
		}

		public override void ClientSpawn()
		{
			base.ClientSpawn();
		}

		public override void FrameSimulate( Client cl )
		{
			base.FrameSimulate( cl );

			LeftHand?.FrameSimulate( cl );
			RightHand?.FrameSimulate( cl );
		}

		public void SetVrAnimProperties()
		{
			if ( LifeState != LifeState.Alive )
				return;

			if ( !Input.VR.IsActive )
				return;

			SetAnimBool( "b_vr", true );
			var leftHandLocal = Transform.ToLocal( LeftHand.GetBoneTransform( 0 ) );
			var rightHandLocal = Transform.ToLocal( RightHand.GetBoneTransform( 0 ) );

			var handOffset = Vector3.Zero;
			SetAnimVector( "left_hand_ik.position", leftHandLocal.Position + (handOffset * leftHandLocal.Rotation) );
			SetAnimVector( "right_hand_ik.position", rightHandLocal.Position + (handOffset * rightHandLocal.Rotation) );

			SetAnimRotation( "left_hand_ik.rotation", leftHandLocal.Rotation * Rotation.From( 65, 0, 90 ) );
			SetAnimRotation( "right_hand_ik.rotation", rightHandLocal.Rotation * Rotation.From( 65, 0, 90 ) );

			float height = Input.VR.Head.Position.z - Position.z;
			SetAnimFloat( "duck", 1.0f - ((height - 32f) / 32f) ); // This will probably need tweaking depending on height
		}

		private TimeSince timeSinceLastRotation;
		private void CheckRotate()
		{
			if ( !IsServer )
				return;

			const float deadzone = 0.2f;
			const float angle = 45f;
			const float delay = 0.25f;

			float rotate = Input.VR.RightHand.Joystick.Value.x;

			if ( timeSinceLastRotation > delay )
			{
				if ( rotate > deadzone )
				{
					Transform = Transform.RotateAround(
						Input.VR.Head.Position.WithZ( Position.z ),
						Rotation.FromAxis( Vector3.Up, -angle )
					);

					timeSinceLastRotation = 0;
				}
				else if ( rotate < -deadzone )
				{
					Transform = Transform.RotateAround(
						Input.VR.Head.Position.WithZ( Position.z ),
						Rotation.FromAxis( Vector3.Up, angle )
					);

					timeSinceLastRotation = 0;
				}
			}

			if ( rotate > -deadzone && rotate < deadzone )
			{
				timeSinceLastRotation = 10;
			}
		}

		public override void OnKilled()
		{
			base.OnKilled();
			EnableDrawing = false;
			DeleteHands();
		}
	}
}
