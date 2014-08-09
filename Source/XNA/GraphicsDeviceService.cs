using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace BloodBullet
{
	class GraphicsDeviceService : IGraphicsDeviceService
	{
		static GraphicsDeviceService	Instance;
		static int						m_ReferenceCount;
		PresentationParameters			m_PresentationParameters;
		GraphicsDevice					m_GraphicsDevice;

		public event EventHandler< EventArgs > DeviceCreated;
		public event EventHandler< EventArgs > DeviceDisposing;
		public event EventHandler< EventArgs > DeviceReset;
		public event EventHandler< EventArgs > DeviceResetting;

		GraphicsDeviceService(
			PresentationParameters p_PresentationParameters )
		{
			m_PresentationParameters = p_PresentationParameters;

			m_GraphicsDevice = new GraphicsDevice(
				GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef,
				m_PresentationParameters );
		}

		public static GraphicsDeviceService AddReference(
			PresentationParameters p_PresentationParameters )
		{
			if( Interlocked.Increment( ref m_ReferenceCount ) == 1 )
			{
				Instance = new GraphicsDeviceService(
					p_PresentationParameters );
			}

			return Instance;
		}

		public void Release( bool p_Disposing )
		{
			if( Interlocked.Decrement( ref m_ReferenceCount ) == 0 )
			{
				if( p_Disposing )
				{
					if( DeviceDisposing != null )
					{
						DeviceDisposing( this, EventArgs.Empty );
					}

					m_GraphicsDevice.Dispose( );
				}

				m_GraphicsDevice = null;
			}
		}

		public void ResetDevice( int p_Width, int p_Height )
		{
			if( DeviceResetting != null )
			{
				DeviceResetting( this, EventArgs.Empty );
			}

			m_PresentationParameters.BackBufferWidth =
				Math.Max( p_Width, m_PresentationParameters.BackBufferWidth );
			m_PresentationParameters.BackBufferHeight =
				Math.Max( p_Height, m_PresentationParameters.BackBufferHeight );

			m_GraphicsDevice.Reset( m_PresentationParameters );

			if( DeviceReset != null )
			{
				DeviceReset( this, EventArgs.Empty );
			}
		}

		public GraphicsDevice GraphicsDevice
		{
			get
			{
				return m_GraphicsDevice;
			}
		}
	}
}
