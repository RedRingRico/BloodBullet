using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

namespace BloodBullet
{
	static class MainGame
	{
		static void Main( string[ ] p_Args )
		{
			Renderer.Renderer GameRenderer = new Renderer.Renderer( );

			GameRenderer.SetClearColour( 0.6f, 0.0f, 0.0f );

			NetworkSession	m_NetworkSession;
			PacketReader	m_PacketReader = new PacketReader( );

			m_NetworkSession = NetworkSession.Create(
				NetworkSessionType.SystemLink, 1, 8 );

			while( true )
			{
				if( m_NetworkSession != null )
				{
					NetworkGamer Sender;
					foreach( LocalNetworkGamer Local in
						m_NetworkSession.LocalGamers )
					{
						if( Local.IsDataAvailable )
						{
							Local.ReceiveData( m_PacketReader, out Sender );
							Color ClearColour =
								new Color( m_PacketReader.ReadVector4( ) );

							GameRenderer.ClearColour = ClearColour;
						}
					}
				}

				GameRenderer.BeginScene( );
				GameRenderer.EndScene( );

				GamerServicesDispatcher.Update( );
				m_NetworkSession.Update( );
			}

			if( m_NetworkSession != null )
			{
				m_NetworkSession.Dispose( );
			}
		}
	}
}
