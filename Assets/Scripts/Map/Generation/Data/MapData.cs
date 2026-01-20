using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData
{

	public float[,] data;
	public float Min { get; set; }
	public float Max { get; set; }

	public MapData(int width, int height)
	{
		data = new float[width, height];
		Min = float.MaxValue;
		Max = float.MinValue;
	}
}
