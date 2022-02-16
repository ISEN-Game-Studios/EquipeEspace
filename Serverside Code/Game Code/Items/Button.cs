using System.Collections.Generic;
using System;

namespace SpaceTeam
{
	public class Button : Item
	{
		public string text;

		public Button(string name, string instruction, string text, int width, int height) : base(name, instruction, width, height)
        {
			type = Type.Button;

			state = 0; // 0 released / 1 pressed //

			this.text = text;
        }
	}
}
