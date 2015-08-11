using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Xna.Framework;

namespace BloodBullet.Renderer
{
	[ StructLayout( LayoutKind.Sequential ) ]
	internal unsafe struct MODEL_HEADER
	{
		public fixed byte	ID[ 4 ];
		public fixed byte	Name[ 32 ];
		public UInt32		MeshCount;
	}

	[ StructLayout( LayoutKind.Sequential ) ]
	internal unsafe struct MODEL_VERTEX
	{
		public fixed float Position[ 3 ];
		public fixed float Normal[ 3 ];
		public fixed float TextureCoordinates[ 2 ];
	}

	[ StructLayout( LayoutKind.Sequential ) ]
	internal unsafe struct MODEL_MESH_CHUNK
	{
		public UInt32	Flags;
		public int		VertexCount;
		public UInt32	MaterialHash;
		public UInt16	ListCount;
		public UInt16	StripCount;
		public UInt16	FanCount;
		public UInt16	Padding;
	}

	public class Model
	{
		public Model( ref Renderer p_Renderer )
		{
			m_Meshes = null;
			m_Effect = null;
			m_Renderer = p_Renderer;
		}

		public unsafe UInt32 Load( ref string p_FileName )
		{
			BinaryReader ModelReader = new BinaryReader(
				TitleContainer.OpenStream( p_FileName ) );

			// Read the model header
			MODEL_HEADER ModelHeader = new MODEL_HEADER( );
			int ModelHeaderSize = Marshal.SizeOf( ModelHeader );
			byte [ ] ModelHeaderBytes = new byte[ ModelHeaderSize ];
			Buffer.BlockCopy( ModelReader.ReadBytes( ModelHeaderSize ),
				0, ModelHeaderBytes, 0, ModelHeaderSize );
			GCHandle MemoryHandle = GCHandle.Alloc( ModelHeaderBytes,
				GCHandleType.Pinned );
			ModelHeader = ( MODEL_HEADER )Marshal.PtrToStructure(
				MemoryHandle.AddrOfPinnedObject( ), typeof( MODEL_HEADER ) );
			MemoryHandle.Free( );

			// Print out the model header information
			System.Diagnostics.Debug.WriteLine( "Model Header" );
			byte [ ] IDString = new byte[ 4 ];
			Marshal.Copy( ( IntPtr )ModelHeader.ID, IDString, 0, 4 );
			System.Diagnostics.Debug.WriteLine( "\tID: " + 
				System.Text.Encoding.UTF8.GetString( IDString, 0, 4 ) );

			byte [ ] NameString = new byte[ 32 ];
			Marshal.Copy( ( IntPtr )ModelHeader.Name, NameString, 0, 32 );
			int NameLength = 32;
			for( int Index = 0; Index < 32; ++Index )
			{
				if( NameString[ Index ] == 0 )
				{
					NameLength = Index;
					break;
				}
			}
			System.Diagnostics.Debug.WriteLine( "\tName: " +
				System.Text.Encoding.UTF8.GetString( NameString, 0,
					NameLength ) );
			System.Diagnostics.Debug.WriteLine( "\tMeshCount: {0}",
				ModelHeader.MeshCount );

			// Read the single mesh in the test mesh
			Chunk.CHUNK MeshChunk = new Chunk.CHUNK( );
			int ChunkSize = Marshal.SizeOf( MeshChunk );
			byte [ ] ChunkBytes = new byte[ ChunkSize ];
			Buffer.BlockCopy( ModelReader.ReadBytes( ChunkSize ),
				0, ChunkBytes, 0, ChunkSize );
			MemoryHandle = GCHandle.Alloc( ChunkBytes,
				GCHandleType.Pinned );
			MeshChunk = ( Chunk.CHUNK )Marshal.PtrToStructure(
				MemoryHandle.AddrOfPinnedObject( ), typeof( Chunk.CHUNK ) );
			MemoryHandle.Free( );

			System.Diagnostics.Debug.WriteLine( "Chunk ID: " +
				MeshChunk.ID );
			System.Diagnostics.Debug.WriteLine( "Chunk size: " +
				MeshChunk.Size );

			// Get the mesh chunk infomation
			MODEL_MESH_CHUNK MeshChunkData = new MODEL_MESH_CHUNK( );
			int MeshChunkDataSize = Marshal.SizeOf( MeshChunkData );
			byte [ ] MeshChunkDataBytes = new byte[ MeshChunkDataSize ];
			Buffer.BlockCopy( ModelReader.ReadBytes( MeshChunkDataSize ),
				0, MeshChunkDataBytes, 0, MeshChunkDataSize );
			MemoryHandle = GCHandle.Alloc( MeshChunkDataBytes,
				GCHandleType.Pinned );
			MeshChunkData = ( MODEL_MESH_CHUNK )Marshal.PtrToStructure(
				MemoryHandle.AddrOfPinnedObject( ),
				typeof( MODEL_MESH_CHUNK ) );
			MemoryHandle.Free( );

			System.Diagnostics.Debug.WriteLine( "Vertex count: " +
				MeshChunkData.VertexCount );

			// Get the mesh data as vertices and indices
			MODEL_VERTEX [ ] Vertices =
				new MODEL_VERTEX[ MeshChunkData.VertexCount ];
			MODEL_VERTEX Vertex = new MODEL_VERTEX( );
			LinkedList< byte > VertexList = new LinkedList< byte >( );
			int VertexSize = Marshal.SizeOf( Vertex );
			byte [ ] VertexBytes =
				new byte[ VertexSize * MeshChunkData.VertexCount ];
			Buffer.BlockCopy(
				ModelReader.ReadBytes( VertexSize * MeshChunkData.VertexCount ),
				0, VertexBytes, 0, VertexSize * MeshChunkData.VertexCount );

			UInt16 [ ] Indices = new UInt16[ MeshChunkData.ListCount ];
			byte [ ] IndexBytes = new byte[ MeshChunkData.ListCount * 2 ];
			Buffer.BlockCopy(
				ModelReader.ReadBytes( MeshChunkData.ListCount * 2 ),
				0, IndexBytes, 0, MeshChunkData.ListCount * 2 );
			MemoryHandle = GCHandle.Alloc( IndexBytes, GCHandleType.Pinned );
			/*Marshal..Copy( IndexBytes, 0, Indices, 10 );*/
			Buffer.BlockCopy( IndexBytes, 0, Indices, 0,
				MeshChunkData.ListCount * 2 );
			/*.PtrToStructure( 
				MemoryHandle.AddrOfPinnedObject( ), typeof( UInt16 ) );*/
			MemoryHandle.Free( );

			m_Meshes = new Mesh[ ModelHeader.MeshCount ];
			m_Meshes[ 0 ] = new Mesh( ref m_Renderer );

			VertexElement [ ] VertexElements = new VertexElement[ 3 ];
			VertexElements[ 0 ] = new VertexElement( 0,
				VertexElementFormat.Vector3, VertexElementUsage.Position, 0 );
			VertexElements[ 1 ] = new VertexElement( 12,
				VertexElementFormat.Vector3, VertexElementUsage.Normal, 0 );
			VertexElements[ 2 ] = new VertexElement( 24,
				VertexElementFormat.Vector2,
				VertexElementUsage.TextureCoordinate, 0 );

			VertexDeclaration VertexAttributes =
				new VertexDeclaration( VertexElements );

			m_Meshes[ 0 ].SetVertexData( VertexAttributes,
				MeshChunkData.VertexCount, ref VertexBytes );
			m_Meshes[ 0 ].SetIndexData( ref Indices );

			ModelReader.Close( );

			BinaryReader EffectReader =
				new BinaryReader( TitleContainer.OpenStream( "Shader.fxc" ) );
			byte [ ] EffectCode = new byte[ EffectReader.BaseStream.Length ];
			EffectCode =
				EffectReader.ReadBytes( ( int )EffectReader.BaseStream.Length );
			EffectReader.Close( );

			m_Effect = new Effect( m_Renderer.GraphicsDevice, EffectCode );
			m_Effect.CurrentTechnique = m_Effect.Techniques[ 0 ];

			RasterizerState Raster = new RasterizerState( );
			Raster.CullMode = CullMode.None;
			Raster.FillMode = FillMode.WireFrame;
			m_Renderer.GraphicsDevice.RasterizerState = Raster;

			Matrix WVP;

			WVP = Matrix.CreateLookAt( new Vector3( 0.0f, 10.0f, 10.0f ),
				Vector3.Zero, Vector3.Up ) *
				Matrix.CreatePerspectiveFieldOfView( MathHelper.PiOver4,
					m_Renderer.GraphicsDevice.DisplayMode.AspectRatio,
					1.0f, 1000000.0f );
			m_Effect.Parameters[ 0 ].SetValue( WVP );

			return 0;
		}

		public UInt32 Render( )
		{
			foreach( EffectPass Pass in m_Effect.CurrentTechnique.Passes )
			{
				Pass.Apply( );

				m_Meshes[ 0 ].Render( );
			}

			return 0;
		}

		private Renderer	m_Renderer;
		private Effect		m_Effect;
		private Mesh[ ]		m_Meshes;
	}
}
