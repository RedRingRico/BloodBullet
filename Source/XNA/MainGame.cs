using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
#if WINDOWS
using System.Windows.Forms;
#endif

namespace BloodBullet
{
	static class MainGame
	{
#if WINDOWS
		[STAThread]
#endif
		static void Main( string[ ] p_Args )
		{
			Game.Game TheGame = new Game.Game( );

			if( TheGame.Initialise( ) != 0 )
			{
				return;
			}

			TheGame.Execute( );
		}
	}
}
