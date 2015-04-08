using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework;
#if WINDOWS
using System.Windows.Forms;
using System.Runtime.InteropServices;
#endif

namespace BloodBullet.Game
{
	class Game
	{
		public Game( )
		{
#if WINDOWS
			m_Form = new BloodBulletForm( this );
			m_Form.SetSize( 1280, 720 );
#endif
			m_PacketReader = new PacketReader( );
		}

		public int Initialise( )
		{
			PresentationParameters Presentation =
				new PresentationParameters( );

#if WINDOWS
			Presentation.BackBufferWidth = m_Form.ClientSize.Width;
			Presentation.BackBufferHeight = m_Form.ClientSize.Height;
			Presentation.IsFullScreen = false;
			Presentation.DeviceWindowHandle = m_Form.Handle;
#endif

			Presentation.BackBufferFormat = SurfaceFormat.Color;
			Presentation.DepthStencilFormat = DepthFormat.Depth24Stencil8;
			Presentation.RenderTargetUsage = RenderTargetUsage.DiscardContents;
			Presentation.PresentationInterval = PresentInterval.Immediate;

			m_Renderer = new Renderer.Renderer( Presentation );

			m_Renderer.SetClearColour( 0.6f, 0.0f, 0.0f );

			return 0;
		}

		public void Execute( )
		{
			m_Running = true;

#if WINDOWS
			m_Form.Show( );
			NativeMessage PeekedMessage = new NativeMessage( );
#endif
			while( m_Running )
			{
#if WINDOWS
				if( !m_Form.Created )
				{
					break;
				}
				if( Win32.PeekMessage( out PeekedMessage, m_Form.Handle, 0, 0,
					( uint )PM.REMOVE ) )
				{
					Win32.TranslateMessage( ref PeekedMessage );
					Win32.DispatchMessage( ref PeekedMessage );
				}
				else
				{
#endif
					this.Update( );
					this.Render( );
#if WINDOWS
				}
#endif
			}

			if( m_NetworkSession != null )
			{
				m_NetworkSession.Dispose( );
				m_NetworkSession = null;
			}

#if WINDOWS
			Application.Exit( );
#endif
		}

		private void Render( )
		{
			m_Renderer.BeginScene( );
			m_Renderer.EndScene( );
		}

		private void Update( )
		{
			if( m_NetworkSession != null )
			{
				NetworkGamer Sender;
				foreach( LocalNetworkGamer LocalGamer in
					m_NetworkSession.LocalGamers )
				{
					if( LocalGamer.IsDataAvailable )
					{
						LocalGamer.ReceiveData( m_PacketReader, out Sender );
						uint MessageType = m_PacketReader.ReadUInt32( );
						switch( MessageType )
						{
							case 1:
							{
								Color ClearColour =
									new Color( m_PacketReader.ReadVector4( ) );
								m_Renderer.ClearColour = ClearColour;
								break;
							}
						}
					}
				}
				m_NetworkSession.Update( );
			}
			else
			{
				if( ( Gamer.SignedInGamers.Count > 0 ) &&
					( m_NetworkSession == null ) )
				{
					m_NetworkSession = NetworkSession.Create(
						NetworkSessionType.SystemLink, 1, 8 );

					m_NetworkSession.GamerJoined += NetworkSession_GamerJoined;
					m_NetworkSession.GamerLeft += NetworkSession_GamerLeft;
				}
			}

			GamerServicesDispatcher.Update( );
		}

		void NetworkSession_GamerJoined( object p_Sender,
			GamerJoinedEventArgs p_Args )
		{
			if( !p_Args.Gamer.IsLocal )
			{
				System.Diagnostics.Debug.WriteLine( "Gamer: " +
					p_Args.Gamer.Gamertag + " joined" );
			}
		}

		void NetworkSession_GamerLeft( object p_Sender,
			GamerLeftEventArgs p_Args )
		{
			if( !p_Args.Gamer.IsLocal )
			{
				System.Diagnostics.Debug.WriteLine( "Gamer: " +
					p_Args.Gamer.Gamertag + " left" );
			}
		}

		public bool IsRunning
		{
			get
			{
				return m_Running;
			}
		}

		public void Quit( )
		{
			m_Running = false;
		}

		public Renderer.Renderer Renderer
		{
			get
			{
				return m_Renderer;
			}
		}

		private bool		m_Running;
		Renderer.Renderer	m_Renderer;
		NetworkSession		m_NetworkSession;
		PacketReader		m_PacketReader;
		GameStateManager	m_GameStateMangager;

#if WINDOWS
		private BloodBulletForm m_Form;
#endif
	}
}
