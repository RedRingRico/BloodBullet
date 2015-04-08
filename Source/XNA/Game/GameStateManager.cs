using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloodBullet.Game
{
	class GameStateManager
	{
		List< GameState > m_GameStateRegistry;
		Stack< GameState > m_GameStateStack;
	}
}
