using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
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
			m_RenderFullscreen = false;
			m_Form.SetSize( 1280, 720, m_RenderFullscreen );
			m_FormBorderStyle = FormBorderStyle.Sizable;
#endif

			m_PacketReader = new PacketReader( );
		}

		public int Initialise( )
		{
			PresentationParameters Presentation =
				new PresentationParameters( );

			#region Windows specific
#if WINDOWS
			Presentation.BackBufferWidth = m_Form.ClientSize.Width;
			Presentation.BackBufferHeight = m_Form.ClientSize.Height;
			Presentation.IsFullScreen = false;
			Presentation.DeviceWindowHandle = m_Form.Handle;
#endif
			#endregion

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

			#region Windows specific
#if WINDOWS
			m_Form.Show( );
			NativeMessage PeekedMessage = new NativeMessage( );
#endif
			#endregion

			while ( m_Running )
			{
					#region Windows specific
#if WINDOWS
				if ( !m_Form.Created )
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
				#endregion
					this.Update( );
					this.Render( );
					#region Windows specific
#if WINDOWS
				}
#endif
					#endregion
			}

			if( m_NetworkSession != null )
			{
				m_NetworkSession.Dispose( );
				m_NetworkSession = null;
			}

			#region Windows specific
#if WINDOWS
			Application.Exit( );
#endif
			#endregion
		}

		private void Render( )
		{
			m_Renderer.BeginScene( );
			m_Renderer.EndScene( );
		}

		private void Update( )
		{
			GamePadState NewGamepadState = GamePad.GetState( PlayerIndex.One );
			KeyboardState NewKeyboardState =
				Keyboard.GetState( PlayerIndex.One );

			if( NewGamepadState.Buttons.Back != m_OldGamepadState.Buttons.Back )
			{
				if( NewGamepadState.Buttons.Back ==
					Microsoft.Xna.Framework.Input.ButtonState.Pressed )
				{
					m_Running = false;
				}
			}

			#region Windows specific
#if WINDOWS
			if ( NewKeyboardState.IsKeyDown(
					Microsoft.Xna.Framework.Input.Keys.F11 ) !=
				m_OldKeyboardState.IsKeyDown(
					Microsoft.Xna.Framework.Input.Keys.F11 ) )
			{
				if( NewKeyboardState.IsKeyDown(
					Microsoft.Xna.Framework.Input.Keys.F11 ) )
				{
					m_RenderFullscreen = !m_RenderFullscreen;

					if( m_RenderFullscreen )
					{
						System.Diagnostics.Debug.WriteLine( "Fullscreen" );
						System.Drawing.Rectangle DesktopSize;
						DesktopSize =
							Screen.FromHandle( m_Form.Handle ).Bounds;
						System.Diagnostics.Debug.WriteLine( "Bounds: {0}",
							DesktopSize.ToString( ) );

						m_Form.SetBounds( DesktopSize.X, DesktopSize.Y,
							DesktopSize.Width, DesktopSize.Height );

						m_FormBorderStyle = m_Form.FormBorderStyle;
						m_Form.FormBorderStyle = FormBorderStyle.None;
						m_Form.SetSize( DesktopSize.Width, DesktopSize.Height,
							m_RenderFullscreen );
					}
					else
					{
						System.Diagnostics.Debug.WriteLine( "Windowed" );

						m_Form.SetSize( 1280, 720, m_RenderFullscreen );
						m_Form.FormBorderStyle = m_FormBorderStyle;
					}
				}
			}
#endif
			#endregion

			if ( m_NetworkSession != null )
			{
				NetworkGamer Sender;
				foreach( LocalNetworkGamer LocalGamer in
					m_NetworkSession.LocalGamers )
				{
					if( LocalGamer.IsDataAvailable )
					{
						LocalGamer.ReceiveData( m_PacketReader, out Sender );
						Color ClearColour =
							new Color( m_PacketReader.ReadVector4( ) );

						m_Renderer.ClearColour = ClearColour;
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
			m_OldKeyboardState = NewKeyboardState;
			m_OldGamepadState = NewGamepadState;
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
		KeyboardState		m_OldKeyboardState;
		GamePadState		m_OldGamepadState;

#if WINDOWS
		private BloodBulletForm m_Form;
		private FormBorderStyle	m_FormBorderStyle;
		private bool			m_RenderFullscreen;
#endif
	}
}
