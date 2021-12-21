
namespace Sandbox
{
	[Library]
	public partial class ArcadeController : WalkController
	{
		public ArcadeController()
		{
			Duck = new Duck( this );
			Unstuck = new Unstuck( this );
		}

		public override void FrameSimulate()
		{
			base.FrameSimulate();
			// EyeRot = Input.Rotation;
		}

		public override void Simulate()
		{
			EyePosLocal = Vector3.Up * (EyeHeight * Pawn.Scale);
			UpdateBBox();

			EyePosLocal += TraceOffset;
			// EyeRot = Input.Rotation;

			RestoreGroundPos();

			if ( Unstuck.TestAndFix() )
				return;

			CheckLadder();
			Swimming = Pawn.WaterLevel.Fraction > 0.6f;

			//
			// Start Gravity
			//
			if ( !Swimming )
			{
				Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;
				Velocity += new Vector3( 0, 0, BaseVelocity.z ) * Time.Delta;
				BaseVelocity = BaseVelocity.WithZ(0);
			}

			if ( AutoJump ? Input.Down( InputButton.Jump ) : Input.Pressed( InputButton.Jump ) )
			{
				CheckJumpButton();
			}

			// Fricion is handled before we add in any base velocity. That way, if we are on a conveyor,
			//  we don't slow when standing still, relative to the conveyor.
			bool bStartOnGround = GroundEntity != null;
			if ( bStartOnGround )
			{
				Velocity = Velocity.WithZ( 0 );

				if ( GroundEntity != null )
				{
					ApplyFriction( GroundFriction * SurfaceFriction );
				}
			}

			// pick a WASD direction.
			int fwd = 0;
			int left = 0;
			if(Input.Down(InputButton.Forward)){
				fwd += 0;
				left += 1;
			}else if(Input.Down(InputButton.Left)){
				fwd += -1;
				left += 0;
			}else if(Input.Down(InputButton.Back)){
				fwd += 0;
				left += -1;
			}else if(Input.Down(InputButton.Right)){
				fwd += 1;
				left += 0;
			}
			WishVelocity = new Vector3( fwd, left, 0 );
			var inSpeed = WishVelocity.Length.Clamp( -1, 1 );
			// WishVelocity *= Input.Rotation.Angles().WithPitch( 0 ).ToRotation();
			WishVelocity = WishVelocity.WithZ( 0 );

			WishVelocity = WishVelocity.Normal * inSpeed;
			WishVelocity *= GetWishSpeed();

			Duck.PreTick();

			bool bStayOnGround = false;
			if ( GroundEntity != null )
			{
				bStayOnGround = true;
				WalkMove();
			}
			else
			{
				AirMove();
			}
			CategorizePosition( bStayOnGround );

			// FinishGravity
			Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;

			if ( GroundEntity != null )
			{
				Velocity = Velocity.WithZ( 0 );
			}

			SaveGroundPos();
		}

		public override float GetWishSpeed()
		{
			var ws = Duck.GetWishSpeed();
			if ( ws >= 0 ) return ws;

			if ( Input.Down( InputButton.Run ) ) return SprintSpeed;
			if ( Input.Down( InputButton.Walk ) ) return WalkSpeed;

			return DefaultSpeed;
		}

		public override void WalkMove()
		{
			var wishdir = WishVelocity.Normal;
			var wishspeed = WishVelocity.Length;

			WishVelocity = WishVelocity.WithZ( 0 );
			WishVelocity = WishVelocity.Normal * wishspeed;

			Velocity = Velocity.WithZ( 0 );
			Accelerate( wishdir, wishspeed, 0, Acceleration );
			Velocity = Velocity.WithZ( 0 );

			//   Player.SetAnimParam( "forward", Input.Forward );
			//   Player.SetAnimParam( "sideward", Input.Right );
			//   Player.SetAnimParam( "wishspeed", wishspeed );
			//   Player.SetAnimParam( "walkspeed_scale", 2.0f / 190.0f );
			//   Player.SetAnimParam( "runspeed_scale", 2.0f / 320.0f );

			// Add in any base velocity to the current velocity.
			Velocity += BaseVelocity;

			try
			{
				if ( Velocity.Length < 1.0f )
				{
					Velocity = Vector3.Zero;
					return;
				}

				// first try just moving to the destination
				var dest = (Position + Velocity * Time.Delta).WithZ( Position.z );
				var pm = TraceBBox( Position, dest );

				if ( pm.Fraction == 1 )
				{
					Position = pm.EndPos;
					StayOnGround();
					return;
				}

				StepMove();
			}
			finally
			{
				// Now pull the base velocity back out.   Base velocity is set if you are on a moving object, like a conveyor (or maybe another monster?)
				Velocity -= BaseVelocity;
			}

			StayOnGround();
		}

		public override void StepMove()
		{
			MoveHelper mover = new MoveHelper( Position, Velocity );
			mover.Trace = mover.Trace.Size( mins, maxs ).Ignore( Pawn );
			mover.MaxStandableAngle = GroundAngle;

			mover.TryMoveWithStep( Time.Delta, StepSize );

			Position = mover.Position;
			Velocity = mover.Velocity;
		}

		public override void Move()
		{
			MoveHelper mover = new MoveHelper( Position, Velocity );
			mover.Trace = mover.Trace.Size( mins, maxs ).Ignore( Pawn );
			mover.MaxStandableAngle = GroundAngle;

			mover.TryMove( Time.Delta );

			Position = mover.Position;
			Velocity = mover.Velocity;
		}
		void RestoreGroundPos()
		{
			if ( GroundEntity == null || GroundEntity.IsWorld )
				return;
		}

		void SaveGroundPos()
		{
			if ( GroundEntity == null || GroundEntity.IsWorld )
				return;
		}
	}
}
