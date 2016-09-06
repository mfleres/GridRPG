using System;
using UnityEngine;

namespace GridRPGCode
{
	public class Terrain : UnityEngine.Object
	{
		private Sprite _sprite = new Sprite();
		private String _type = "void";

		public Terrain ()
		{
		}

		public Terrain(Terrain terrain)
		{
			_sprite = terrain.getSprite();
			_type = terrain.getType();
		}

		public Sprite getSprite()
		{
			return _sprite;
		}

		public String getType()
		{
			return _type;
		}
	}
}

