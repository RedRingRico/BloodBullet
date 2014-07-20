using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BloodBullet
{
	static class MainGame
	{
		static void Main( string[ ] p_Args )
		{
			Renderer.Renderer GameRenderer = new Renderer.Renderer( );

			GameRenderer.SetClearColour( 0.6f, 0.0f, 0.0f );

			while( true )
			{
				GameRenderer.BeginScene( );
				GameRenderer.EndScene( );
			}
		}
	}
}
