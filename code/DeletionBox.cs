using Hammer;
using Sandbox;
using Sandbox.Internal;
using System.Linq;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dkdGame
{
	[Library("deletionbox", Description = "Do not touch!")]
	[EditorModel("models/deletionbox.vmdl", staticGreen: 100, FixedBounds = true)]
	[EntityTool("deletionbox", "dkdGame", "Do not touch!")]
	public partial class DeletionBox : ModelEntity
	{
		public override void Spawn()
		{
			SetModel("models/deletionbox.vmdl");
			SetupPhysicsFromModel(PhysicsMotionType.Static);
			Position = new Vector3(-300, 736, -40);

			Transmit = TransmitType.Always;
			EnableSolidCollisions = true;
			EnableTouch = true;

			base.Spawn();
		}

		public override void Touch(Entity other)
		{
			base.Touch(other);
			if(IsServer){
				other.Delete();
			}
		}
	}
}
