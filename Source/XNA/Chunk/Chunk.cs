using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace BloodBullet.Chunk
{
	[ StructLayout( LayoutKind.Sequential ) ]
	internal unsafe struct CHUNK
	{
		public UInt32	ID;
		public UInt32	Size;
	}

	class Chunk
	{
	}
}
