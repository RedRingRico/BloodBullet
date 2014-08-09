using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

			m_GraphicsDevice = new GraphicsDevice( m_GraphicsAdapter,
				GraphicsProfile.HiDef, m_PresentationParameters );

			m_ClearColour = new Color( 1.0f, 1.0f, 1.0f );
		}

		public void SetClearColour( float p_Red, float p_Green, float p_Blue )
		{
			m_ClearColour = new Color( p_Red, p_Green, p_Blue );
		}

		public void BeginScene( )
		{
			m_GraphicsDevice.Clear( ClearOptions.DepthBuffer |
				ClearOptions.Stencil | ClearOptions.Target,  m_ClearColour,
				1.0f, 255 );
		}

		public void EndScene( )
		{
			m_GraphicsDevice.Present( );
		}

		public GraphicsDevice GraphicsDevice
		{
			get
			{
				return m_GraphicsDevice;
			}
		}

		private GraphicsDevice			m_GraphicsDevice;
		private GraphicsAdapter			m_GraphicsAdapter;
		private PresentationParameters	m_PresentationParameters;
		private Color					m_ClearColour;
	}
}
