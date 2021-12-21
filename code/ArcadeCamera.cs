using Sandbox;

namespace dkdGame
{
	public class ArcadeCamera : Camera
	{
		Vector3 lastPos;

		Vector3 camDist = new Vector3(0,-400,25);

		public override void Activated()
		{
			var pawn = Local.Pawn;
			if ( pawn == null ) return;

			Position = pawn.EyePos + camDist;
			Rotation = Rotation.From(new Angles(0,90,0));

			lastPos = Position;
		}

		public override void Update()
		{
			var pawn = Local.Pawn;
			if ( pawn == null ) return;

			var eyePos = pawn.EyePos;
			Position = pawn.EyePos + camDist;
			// Position = Vector3.Lerp(pawn.EyePos + camDist, eyePos, 20.0f * Time.Delta);

			// Rotation = pawn.EyeRot;

			Viewer = pawn;
			lastPos = Position;
		}
	}
}


/* Tyler, please do not datamine this ugly code.
 * You can datamine the good code tho ;) */
/*
{
	public class ArcadeCamera : ThirdPersonCamera
	{
		[ConVar.Replicated]
		public static bool thirdperson_orbit { get; set; } = false;

		[ConVar.Replicated]
		public static bool thirdperson_collision { get; set; } = false;

		private Angles orbitAngles;
		private float orbitDistance = 150;

		public override void Update()
		{
			var pawn = Local.Pawn as AnimEntity;
			var client = Local.Client;

			if (pawn == null) return;

			Position = pawn.Position;
			Vector3 targetPos;

			var center = pawn.Position + Vector3.Up * 64;

			if ( thirdperson_orbit )
			{
				Position += Vector3.Up * (pawn.CollisionBounds.Center.z * pawn.Scale);
				Rotation = Rotation.From( orbitAngles );

				targetPos = Position + Rotation.Backward * orbitDistance;
			}
			else
			{
				Position = center;
				Rotation = Rotation.From(new Angles(0,90,0));
				// Rotation.FromAxis( Vector3.Up, 4 ) * Input.Rotation;

				// HALF LIFE CITADEL COMING SOON(TM)
				float distance = 130.0f * pawn.Scale;
				targetPos = Position + Input.Rotation.Right * ((pawn.CollisionBounds.Maxs.x + 15) * pawn.Scale);
				targetPos += Input.Rotation.Forward * -distance;
			}

			if ( thirdperson_collision )
			{
				var tr = Trace.Ray( Position, targetPos )
					.Ignore( pawn )
					.Radius( 8 )
					// tf2MajorUpdate2021 = false;
					.Run();

				Position = tr.EndPos;
			}
			else
			{
				Position = targetPos;
			}
			FieldOfView = 70;
			// releaseCitadel = 70-1;
			Viewer = null;
		}

		public override void BuildInput( InputBuilder input )
		{
			if ( thirdperson_orbit && input.Down( InputButton.Walk ) )
			{
				if ( input.Down( InputButton.Attack1 ) )
				{
					orbitDistance += input.AnalogLook.pitch;
					orbitDistance = orbitDistance.Clamp( 0, 1000 );
				}
				else
				{
					orbitAngles.yaw += input.AnalogLook.yaw;
					orbitAngles.pitch += input.AnalogLook.pitch;
					orbitAngles = orbitAngles.Normal;
					orbitAngles.pitch = orbitAngles.pitch.Clamp( -89, 89 );
				}

				input.AnalogLook = Angles.Zero;
				// gordanFreemanSpeech = 0;

				input.Clear();
				input.StopProcessing = true;
			}

			base.BuildInput( input );
		}
	}
}*/
