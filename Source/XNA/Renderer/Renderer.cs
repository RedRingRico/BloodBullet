using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

namespace BloodBullet.Renderer
{
	public class Renderer
	{
		public Renderer( PresentationParameters p_PresentationParameters )
		{
			m_GraphicsAdapter = GraphicsAdapter.DefaultAdapter;

			m_PresentationParameters = p_PresentationParameters;
#if XBOX360
			m_PresentationParameters.BackBufferWidth =
				m_GraphicsAdapter.CurrentDisplayMode.Width;
			m_PresentationParameters.BackBufferHeight =
				m_GraphicsAdapter.CurrentDisplayMode.Height;
			m_PresentationParameters.IsFullScreen = true;
#endif

			m_GraphicsDeviceService = GraphicsDeviceService.AddReference(
				m_PresentationParameters );

			m_ServiceContainer.AddService< IGraphicsDeviceService >( 
				m_GraphicsDeviceService );

			m_ClearColour = new Color( 1.0f, 1.0f, 1.0f );

			m_Width = m_PresentationParameters.BackBufferWidth;
			m_Height = m_PresentationParameters.BackBufferHeight;

			if( GamerServicesDispatcher.IsInitialized == false )
			{
				GamerServicesDispatcher.WindowHandle =
					m_GraphicsDeviceService.GraphicsDevice.
						PresentationParameters.DeviceWindowHandle;

				GamerServicesDispatcher.Initialize( m_ServiceContainer );

				GamerServicesDispatcher.Update( );
			}
		}

		public void SetClearColour( float p_Red, float p_Green, float p_Blue )
		{
			m_ClearColour = new Color( p_Red, p_Green, p_Blue );
		}

		public Color ClearColour
		{
			get
			{
				return m_ClearColour;
			}
			set
			{
				m_ClearColour = value;
			}
		}

		public void BeginScene( )
		{
#if WINDOWS
			if( HandleDeviceReset( ) == 0 )
			{
				if( m_DeviceLost == false )
				{
#endif
					m_GraphicsDeviceService.GraphicsDevice.Clear(
						ClearOptions.DepthBuffer | ClearOptions.Stencil |
						ClearOptions.Target,  m_ClearColour,
						1.0f, 255 );
#if WINDOWS
				}
			}
#endif
		}

		public void EndScene( )
		{
#if WINDOWS
			if( m_DeviceLost == false )
			{
				try
				{
#endif
					m_GraphicsDeviceService.GraphicsDevice.Present( );
#if WINDOWS
				}
				catch( DeviceLostException p_Exception )
				{
					System.Diagnostics.Debug.Write( "Lost device inbetween " +
						"Clear and Present\n" + p_Exception.HelpLink + "\n" );
				}
				catch( Exception p_Exception )
				{
					System.Diagnostics.Debug.Write( "Caught exception: " +
						p_Exception.Message + "\n" );
				}
			}
#endif
		}

		public GraphicsDevice GraphicsDevice
		{
			get
			{
				return m_GraphicsDeviceService.GraphicsDevice;
			}
		}

#if WINDOWS
		int HandleDeviceReset( )
		{
			if( GraphicsDevice == null )
			{
				return 1;
			}

			bool NeedReset = false;

			switch( GraphicsDevice.GraphicsDeviceStatus )
			{
				case GraphicsDeviceStatus.Lost:
				{
					// Lost device is not handled properly.
					m_DeviceLost = true;
					NeedReset = true;
					break;
				}
				case GraphicsDeviceStatus.NotReset:
				{
					NeedReset = true;
					break;
				}
				default:
				{
					PresentationParameters Presentation =
						GraphicsDevice.PresentationParameters;
				
					NeedReset = 
						( m_Width != Presentation.BackBufferWidth ) ||
						( m_Height != Presentation.BackBufferHeight );
					m_DeviceLost = false;

					break;
				}
			}

			if( NeedReset )
			{
				try
				{
					m_GraphicsDeviceService.ResetDevice( m_Width, m_Height );
				}
				catch( Microsoft.Xna.Framework.Graphics.DeviceLostException
					p_Exception )
				{
					System.Diagnostics.Debug.Write( "Lost device: " +
						p_Exception.HelpLink + "\n" );
					m_DeviceLost = true;

					return 1;
				}
				catch( Exception p_Exception )
				{
					System.Diagnostics.Debug.Write( "Caught exception: " +
						p_Exception.Message + "\n" );

					return 1;
				}
			}

			return 0;
		}
#endif

		public void SetSize( int p_Width, int p_Height )
		{
			m_Width = p_Width;
			m_Height = p_Height;
		}

		private GraphicsAdapter			m_GraphicsAdapter;
		private PresentationParameters	m_PresentationParameters;
		private Color					m_ClearColour;
		private ServiceContainer		m_ServiceContainer =
			new ServiceContainer( );
		private GraphicsDeviceService	m_GraphicsDeviceService;

		private int m_Width;
		private int m_Height;

		private bool m_DeviceLost;
	}
}
