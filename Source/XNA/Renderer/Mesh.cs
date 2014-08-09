using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			m_VertexDeclaration = p_VertexDeclaration;

			return 0;
		}

		private VertexBuffer		m_VertexBuffer;
		private IndexBuffer			m_IndexBuffer;
		private Renderer			m_Renderer;
		private VertexDeclaration	m_VertexDeclaration;
	}
}
