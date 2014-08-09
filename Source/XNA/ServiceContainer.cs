using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloodBullet
{
	class ServiceContainer : IServiceProvider
	{
		Dictionary< Type, object > m_Services =
			new Dictionary< Type,object >( );

		public void AddService< T >( T p_Service )
		{
			m_Services.Add( typeof( T ), p_Service );
		}

		public object GetService( Type p_Type )
		{
			object Service;

			m_Services.TryGetValue( p_Type, out Service );

			return Service;
		}
	}
}
