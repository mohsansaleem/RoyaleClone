using System.Collections;
using System.Collections.Generic;

namespace BattleSystem
{
	public class Player
	{
		public string Id;
		public string Name;
		public int Level;
		public List<Card> Cards;
		private int currentElixer;

		public int CurrentElixer {
			get {
				return currentElixer;
			}

			set {
				currentElixer = value;
			}
		}
	}
}
