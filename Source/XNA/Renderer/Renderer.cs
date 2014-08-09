using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;

namespace BloodBullet.Renderer
{
	public class Renderer
	{
		public Renderer( )
		{
			m_GraphicsAdapter = GraphicsAdapter.DefaultAdapter;

			m_PresentationParameters = new PresentationParameters( );

			m_PresentationParameters.BackBufferWidth =
				m_GraphicsAdapter.CurrentDisplayMode.Width;
			m_PresentationParameters.BackBufferHeight =
				m_GraphicsAdapter.CurrentDisplayMode.Height;
			m_PresentationParameters.BackBufferFormat = SurfaceFormat.Color;
			m_PresentationParameters.DepthStencilFormat =
				DepthFormat.Depth24Stencil8;
			m_PresentationParameters.RenderTargetUsage =
				RenderTargetUsage.DiscardContents;
			m_PresentationParameters.PresentationInterval =
				PresentInterval.Immediate;

			m_GraphicsDeviceService = GraphicsDeviceService.AddReference(
				m_PresentationParameters );

			m_ServiceContainer.AddService< IGraphicsDeviceService >( 
				m_GraphicsDeviceService );

			m_ClearColour = new Color( 1.0f, 1.0f, 1.0f );

			GamerServicesDispatcher.WindowHandle =
				m_GraphicsDeviceService.GraphicsDevice.PresentationParameters.
					DeviceWindowHandle;

			GamerServicesDispatcher.Initialize( m_ServiceContainer );

			GamerServicesDispatcher.Update( );
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
			m_GraphicsDeviceService.GraphicsDevice.Clear(
				ClearOptions.DepthBuffer | ClearOptions.Stencil |
				ClearOptions.Target,  m_ClearColour,
				1.0f, 255 );
		}

		public void EndScene( )
		{
			m_GraphicsDeviceService.GraphicsDevice.Present( );
		}

		public GraphicsDevice GraphicsDevice
		{
			get
			{
				return m_GraphicsDeviceService.GraphicsDevice;
			}
		}

		private GraphicsAdapter			m_GraphicsAdapter;
		private PresentationParameters	m_PresentationParameters;
		private Color					m_ClearColour;
		private ServiceContainer		m_ServiceContainer =
			new ServiceContainer( );
		private GraphicsDeviceService	m_GraphicsDeviceService;
	}
}
