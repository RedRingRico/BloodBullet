using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace BloodBullet.Renderer
{
	public class Mesh
	{
		public Mesh( ref Renderer p_Renderer )
		{
			m_Renderer = p_Renderer;
			m_VertexBuffer = null;
			m_IndexBuffer = null;
		}

		public int SetVertexData( VertexDeclaration p_VertexDeclaration,
			int p_VertexCount, ref byte [ ] p_VertexData )
		{
			m_VertexBuffer = new VertexBuffer( m_Renderer.GraphicsDevice,
				p_VertexDeclaration, p_VertexCount, BufferUsage.WriteOnly );

			m_VertexBuffer.SetData< byte >( p_VertexData );

			m_VertexCount = p_VertexCount;

			return 0;
		}

		public int SetIndexData( ref UInt16 [ ] p_Indices )
		{
			m_IndexBuffer = new IndexBuffer( m_Renderer.GraphicsDevice,
				IndexElementSize.SixteenBits, p_Indices.Length,
				BufferUsage.WriteOnly );

			m_IndexBuffer.SetData< UInt16 >( p_Indices );

			// Only for list types
			m_PrimitiveCount = p_Indices.Length / 3;

			return 0;
		}

		public void Render( )
		{
			m_Renderer.GraphicsDevice.SetVertexBuffer( m_VertexBuffer );
			m_Renderer.GraphicsDevice.Indices = m_IndexBuffer;

			m_Renderer.GraphicsDevice.DrawIndexedPrimitives(
				PrimitiveType.TriangleList, 0, 0, m_VertexCount, 0,
				m_PrimitiveCount );
		}

		private VertexBuffer	m_VertexBuffer;
		private IndexBuffer		m_IndexBuffer;
		private Renderer		m_Renderer;
		private int				m_PrimitiveCount;
		private int				m_VertexCount;
	}
}
