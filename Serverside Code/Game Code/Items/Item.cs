using System.Collections.Generic;
using System;

namespace SpaceTeam
{
	public enum Type
	{
		Toggle,
		Button,
		Slider,
		PowerSwitch,
		Switch,
	}

	public abstract class Item
	{
		public Type type;

		public string name;
		public string instruction;
		public int state;

		public int x;
		public int y;

		public int width;
		public int height;

		public Item(string name, string instruction, int width, int height)
		{
			this.name = name;

			this.instruction = instruction;

			this.width = width;
			this.height = height;
		}
	}
}
