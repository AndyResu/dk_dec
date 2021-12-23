using Sandbox;

namespace dkdGame
{
	public partial class BaseHand : AnimEntity
	{
		[Net] public BaseHand Other { get; set; }

		protected virtual string ModelPath => "";

		public bool GripPressed => InputHand.Grip > 0.5f;
		public bool TriggerPressed => InputHand.Trigger > 0.5f;

		public virtual Input.VrHand InputHand { get; }

		public override void Spawn()
		{
			SetModel( ModelPath );

			Position = InputHand.Transform.Position;
			Rotation = InputHand.Transform.Rotation;

			Transmit = TransmitType.Always;
		}

		public override void FrameSimulate( Client cl )
		{
			base.FrameSimulate( cl );

			Transform = InputHand.Transform;
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			Transform = InputHand.Transform;
			Animate();
		}

		private void Animate()
		{
			SetAnimFloat( "Index", InputHand.GetFingerCurl( 1 ) );
			SetAnimFloat( "Middle", InputHand.GetFingerCurl( 2 ) );
			SetAnimFloat( "Ring", InputHand.GetFingerCurl( 3 ) );
			SetAnimFloat( "Thumb", InputHand.GetFingerCurl( 0 ) );
		}

		public bool isGrabbing = false;
		public static int sizee = 20;
		public Vector3 bbox = new Vector3(0,0,sizee/2);
		// public bool isGrabbingWithTrigger = false;
		// public bool isGrabbingWithGrip = false;
		public Entity itemGrabbed;
		public Vector3 lastPos;

		[Event.Tick]
		public void pickupItem()
		{
			if(!isGrabbing && (TriggerPressed || GripPressed)){
				// Log.Info("Infrared memes: " + TriggerPressed);
				// Goal: grab item
				// create a box sizee hammer-units large with the hand in the center
				var startPos = Position + bbox;
				var endPos = Position - bbox;
				var tr = Trace.Ray(startPos, endPos)
				.Size(sizee)
				.Run();
				// if the trace hits any objects, grab them
				if (tr.Hit){
					Log.Info("Grabed with fists: " + tr.Entity.ToString());
					isGrabbing = true;
					// remember if the item is held with either the tigger or grip
						// if(TriggerPressed){isGrabbingWithTrigger = true;}
						// if(GripPressed){isGrabbingWithGrip = true;}
					// parent item temporarily to hand
					itemGrabbed = tr.Entity;
					itemGrabbed.Position = InputHand.Transform.Position + new Vector3(0,0,16);
					lastPos = InputHand.Transform.Position;
				}
			}else if(isGrabbing && !(TriggerPressed || GripPressed)){
				// TODO: grabe happens way too aggresively. Tie to a button LOL
				// drop item
				isGrabbing = false;
				Log.Info("ungrabe");
				itemGrabbed.Velocity = (InputHand.Transform.Position - lastPos).Normal *100; // bad velocity
			}
		}
	}
}
