using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloodBullet.Game
{
	public abstract class GameState
	{
		public abstract int Enter( );
		public abstract int Exit( );

		public abstract void Update( UInt64 p_DeltaTime );
		public abstract void Render( );

		public abstract string GetName( );
	}
}
