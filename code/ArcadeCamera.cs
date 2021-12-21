using Sandbox;

namespace dkdGame
{
	public class ArcadeCamera : ThirdPersonCamera
	{
		private Angles orbitAngles;
		private float orbitDistance = 150;
		Vector3 camDist = new Vector3(0,-500,100);

		public override void Update()
		{
			var pawn = Local.Pawn as AnimEntity;
			var client = Local.Client;

			if (pawn == null) return;

			Position = pawn.Position;
			Vector3 targetPos;

			Position = pawn.Position + camDist;
			Rotation = Rotation.From(new Angles(0,90,0));
			// Rotation.FromAxis( Vector3.Up, 4 ) * Input.Rotation;

			// CITADEL SOON(TM)
			float distance = 0.0f;
			targetPos = Position + ((pawn.CollisionBounds.Maxs.x + 15) * pawn.Scale);
			targetPos += distance;
			// tf2MajorUpdate2021 = false;
			Position = targetPos;
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
}
