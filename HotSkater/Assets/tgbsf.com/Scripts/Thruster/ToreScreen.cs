using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Thruster
{
	public class ToreScreen : Screen
	{
		public override string[] screensNames => new string[]
		{
			"Bottom",
			"Front",
			"Top",
			"Left",
			"Right",
			"LeftBack",
			"RightBack"
		};

		float _radius1;
		public float radius1
		{
			set
			{
				if (value != _radius1)
				{
					_radius1 = value;
					UpdateScreens();
				}
			}
		}

		float _radius2;
		public float radius2
		{
			set
			{
				if (value != _radius2)
				{
					_radius2 = value;
					UpdateScreens();
				}
			}
		}

		public override Vector2Int resolution
		{
			set
			{
				base.resolution = new Vector2Int(value.x, value.x);
			}
		}

		void UpdateScreens()
		{
			// Tore Map
			//   __________________________________________________
			//  |TOP                                 |RIGHT        |  \
			//  |                                    |             |   |
			//  |                                    |             |   |
			//  |                                    |             |   } r1 + r2
			//  |                                    |             |   |
			//  |                                    |             |   |
			//  |                                    |             |   |
			//  |____________________________________|_____________|  /
			//  |FRONT                               |RIGHT BACK   |  \
			//  |                                    |             |   } r2
			//  |                                    |_____________|  /
			//  |                                    |LEFT BACK    |  \
			//  |                                    |             |   } r2
			//  |____________________________________|_____________|  /
			//  |BOTTOM                              |LEFT		   |  \
			//  |                                    |			   |   |
			//  |                                    |			   |   |
			//  |                                    |			   |   } r1 + r2
			//  |                                    |			   |   |
			//  |                                    |			   |   |
			//  |                                    |			   |   |
			//  |____________________________________|_____________|  /
			//
			//   \__________________________________/ \___________/
			//  			 2 * (r1 + r2)                2 * r2

			float width = 2 * (_radius1 + 2 * _radius2);

			UpdateScreen("Bottom",
			             new Vector3(90, 0, 0),
			             new Vector2(2 * (_radius1 + _radius2), _radius1 + _radius2),
			             new Vector3(0, (_radius1 + _radius2) / 2, _radius2),
			             new Rect(0, 0, 2 * (_radius1 + _radius2) / width, (_radius1 + _radius2) / width));

			UpdateScreen("Front",
			             Vector3.zero,
			             new Vector2(2 * (_radius1 + _radius2), 2 * _radius2),
			             new Vector3(0, 0, _radius1 + _radius2),
			             new Rect(0, (_radius1 + _radius2) / width, 2 * (_radius1 + _radius2) / width, 2 * _radius2 / width));

			UpdateScreen("Top",
			             new Vector3(-90, 0, 0),
			             new Vector2(2 * (_radius1 + _radius2), _radius1 + _radius2),
			             new Vector3(0, -(_radius1 + _radius2) / 2, _radius2),
			             new Rect(0, (_radius1 + 3 * _radius2) / width, 2 * (_radius1 + _radius2) / width, (_radius1 + _radius2) / width));

			UpdateScreen("Left",
			             new Vector3(0, -90, 90),
			             new Vector2(2 * _radius2, _radius1 + _radius2),
			             new Vector3(0, -(_radius1 + _radius2) / 2, _radius1 + _radius2),
			             new Rect(2 * (_radius1 + _radius2) / width, 0, 2 * _radius2 / width, (_radius1 + _radius2) / width));

			UpdateScreen("Right",
			             new Vector3(0, 90, 90),
			             new Vector2(2 * _radius2, _radius1 + _radius2),
			             new Vector3(0, (_radius1 + _radius2) / 2, _radius1 + _radius2),
			             new Rect(2 * (_radius1 + _radius2) / width, (_radius1 + 3 * _radius2) / width, 2 * _radius2 / width, (_radius1 + _radius2) / width));

			UpdateScreen("LeftBack",
			             new Vector3(0, -180, 90),
			             new Vector2(2 * _radius2, _radius2),
			             new Vector3(0, -_radius1 - _radius2 / 2, 0),
			             new Rect(2 * (_radius1 + _radius2) / width, (_radius1 + _radius2) / width, 2 * _radius2 / width, _radius2 / width));

			UpdateScreen("RightBack",
			             new Vector3(0, 180, 90),
			             new Vector2(2 * _radius2, _radius2),
			             new Vector3(0, _radius1 + _radius2 / 2, 0),
			             new Rect(2 * (_radius1 + _radius2) / width, (_radius1 + 2 * _radius2) / width, 2 * _radius2 / width, _radius2 / width));
		}
	}
}