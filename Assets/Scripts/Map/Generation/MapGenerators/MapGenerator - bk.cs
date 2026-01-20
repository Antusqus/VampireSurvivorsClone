//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MapGenerator
//{
//    static float[,] falloffMap;
//    static int HumidityMapScale = 300;
//    public Tile[,] tiles;
//    static int width;
//    static int height;

//	MapSettings settings;

//	public HeightMap heightMap;
//	public HeatMap heatMap;
//	public MoistureMap moistureMap;

//	[Header("Rivers")]
//	[SerializeField]
//	protected int RiverCount = 40;
//	[SerializeField]
//	protected float MinRiverHeight = 0.6f;
//	[SerializeField]
//	protected int MaxRiverAttempts = 1000;
//	[SerializeField]
//	protected int MinRiverTurns = 18;
//	[SerializeField]
//	protected int MinRiverLength = 20;
//	[SerializeField]
//	protected int MaxRiverIntersections = 2;
//	protected List<River> Rivers = new List<River>();
//	protected List<RiverGroup> RiverGroups = new List<RiverGroup>();
//	protected List<TileGroup> Waters = new List<TileGroup>();
//	protected List<TileGroup> Lands = new List<TileGroup>();

//	public NoiseMaps GenerateMaps(Vector2 coord, int _width, int _height, MapSettings _settings, Vector2 sampleCentre)
//    {
//        width = _width;
//        height = _height;
//		settings = _settings;
//        tiles = new Tile[_width, _height];

//		(MapData, MapData, MapData, System.Random) noiseMapData;
//        noiseMapData = Noise.GenerateNoiseMapData(coord, _width, _height, settings.noiseSettings, sampleCentre, settings.material);
//        MapData heightMapValues = noiseMapData.Item1;
//        MapData moistureMapValues = noiseMapData.Item2;
//        MapData heatMapValues = noiseMapData.Item3;
//		System.Random prng = noiseMapData.Item4;



//        AnimationCurve heightCurve_threadsafe = new AnimationCurve(settings.heightCurve.keys);

//        if (settings.useFalloff)
//        {
//            if (falloffMap == null)
//            {
//                falloffMap = FalloffGenerator.GenerateFalloffMap(_width);
//            }
//        }
//        for (int i = 0; i < _width; i++)
//        {
//            for (int j = 0; j < _height; j++)
//            {

//                heightMapValues.data[i, j] *= heightCurve_threadsafe.Evaluate(heightMapValues.data[i, j] - (settings.useFalloff ? falloffMap[i, j] : 0)) * settings.heightMultiplier;

//                if (heightMapValues.data[i, j] > heightMapValues.Max)
//                {
//                    heightMapValues.Max = heightMapValues.data[i, j];
//                }
//                if (heightMapValues.data[i, j] < heightMapValues.Min)
//                {
//                    heightMapValues.Min = heightMapValues.data[i, j];
//                }



//                if (moistureMapValues.data[i, j] > moistureMapValues.Max)
//                {
//                    moistureMapValues.Max = moistureMapValues.data[i, j];
//                }
//                if (moistureMapValues.data[i, j] < moistureMapValues.Min)
//                {
//                    moistureMapValues.Min = moistureMapValues.data[i, j];

//                }

//                if (heatMapValues.data[i, j] > heatMapValues.Max)
//                {
//                    heatMapValues.Max = heatMapValues.data[i, j];
//                }
//                if (heatMapValues.data[i, j] < heatMapValues.Min)
//                {
//                    heatMapValues.Min = heatMapValues.data[i, j];
//                }
//            }
//        }

//        for (int x = 0; x < _width; x++)
//        {
//            for (int y = 0; y < _height; y++)
//            {
//                Tile t = new Tile();
//                t.X = x;
//                t.Y = y;

//                float heightValue = heightMapValues.data[x, y];
//                heightValue = (heightValue - heightMapValues.Min) / (heightMapValues.Max - heightMapValues.Min);
//                t.HeightValue = heightValue;
//                //if (heightValue < TerrainGenerator.DeepWater)
//                //{
//                //    t.HeightType = HeightType.DeepWater;
//                //}
//                //else if (heightValue < TerrainGenerator.ShallowWater)
//                //{
//                //    t.HeightType = HeightType.ShallowWater;
//                //}
//                //else if (heightValue < TerrainGenerator.Sand)
//                //{
//                //    t.HeightType = HeightType.Sand;
//                //}
//                //else if (heightValue < TerrainGenerator.Grass)
//                //{
//                //    t.HeightType = HeightType.Grass;
//                //}
//                //else if (heightValue < TerrainGenerator.Rock)
//                //{
//                //    t.HeightType = HeightType.Rock;
//                //}
//                //else
//                //{
//                //    t.HeightType = HeightType.Snow;
//                //}
//                if (heightValue < .05f)
//                {
//                    t.HeightType = HeightType.DeepWater;
//					t.Collidable = false;
//                }
//                else if (heightValue < .1f)
//                {
//                    t.HeightType = HeightType.ShallowWater;
//					t.Collidable = false;

//				}
//				else if (heightValue < .2f)
//                {
//                    t.HeightType = HeightType.Sand;
//					t.Collidable = true;

//				}
//				else if (heightValue < .35f)
//                {
//                    t.HeightType = HeightType.Grass;
//					t.Collidable = true;

//				}
//				else if (heightValue < .55f)
//                {
//                    t.HeightType = HeightType.Rock;
//					t.Collidable = true;

//				}
//				else
//                {
//                    t.HeightType = HeightType.Snow;
//					t.Collidable = true;

//				}

//				if (t.HeightType == HeightType.DeepWater)
//                {
//                    moistureMapValues.data[t.X, t.Y] += 8f * t.HeightValue;
//                }
//                else if (t.HeightType == HeightType.ShallowWater)
//                {
//                    moistureMapValues.data[t.X, t.Y] += 3f * t.HeightValue;
//                }
//                else if (t.HeightType == HeightType.Shore)
//                {
//                    moistureMapValues.data[t.X, t.Y] += 1f * t.HeightValue;
//                }
//                else if (t.HeightType == HeightType.Sand)
//                {
//                    moistureMapValues.data[t.X, t.Y] += 0.2f * t.HeightValue;
//                }

//                float moistureValue = moistureMapValues.data[t.X, t.Y];
//                moistureValue = (moistureValue - moistureMapValues.Min) / (moistureMapValues.Max - moistureMapValues.Min);
//                t.MoistureValue = moistureValue;

//                //set moisture type
//                if (t.MoistureValue < settings.noiseSettings.DryerValue) t.MoistureType = MoistureType.Dryest;
//                else if (t.MoistureValue < settings.noiseSettings.DryValue) t.MoistureType = MoistureType.Dryer;
//                else if (t.MoistureValue < settings.noiseSettings.WetValue) t.MoistureType = MoistureType.Dry;
//                else if (t.MoistureValue < settings.noiseSettings.WetterValue) t.MoistureType = MoistureType.Wet;
//                else if (t.MoistureValue < settings.noiseSettings.WettestValue) t.MoistureType = MoistureType.Wetter;
//                else t.MoistureType = MoistureType.Wettest;


//                // Adjust Heat Map based on Height - Higher == colder
//                if (t.HeightType == HeightType.Forest)
//                {
//                    heatMapValues.data[t.X, t.Y] -= 0.1f * t.HeightValue;
//                }
//                else if (t.HeightType == HeightType.Rock)
//                {
//                    heatMapValues.data[t.X, t.Y] -= 0.25f * t.HeightValue;
//                }
//                else if (t.HeightType == HeightType.Snow)
//                {
//                    heatMapValues.data[t.X, t.Y] -= 0.4f * t.HeightValue;
//                }
//                else
//                {
//                    heatMapValues.data[t.X, t.Y] += 0.01f * t.HeightValue;
//                }

//                // Set heat value
//                float heatValue = heatMapValues.data[t.X, t.Y];
//                heatValue = (heatValue - heatMapValues.Min) / (heatMapValues.Max - heatMapValues.Min);
//                t.HeatValue = heatValue;

//                // set heat type
//                if (t.HeatValue < settings.noiseSettings.ColdestValue) t.HeatType = HeatType.Coldest;
//                else if (t.HeatValue < settings.noiseSettings.ColderValue) t.HeatType = HeatType.Colder;
//                else if (t.HeatValue < settings.noiseSettings.ColdValue) t.HeatType = HeatType.Cold;
//                else if (t.HeatValue < settings.noiseSettings.WarmValue) t.HeatType = HeatType.Warm;
//                else if (t.HeatValue < settings.noiseSettings.WarmerValue) t.HeatType = HeatType.Warmer;
//                else t.HeatType = HeatType.Warmest;
//                tiles[x, y] = t;

//				//t.BiomeType = t.GetBiomeType(t);
//            }


//        }
//        //foreach (Tile t in tiles)
//        //{
//        //    Debug.Log("TileData: \n HeightValues: " + t.HeightValue + t.HeightType + "\n MoistureValues: " + t.MoistureValue + t.MoistureType + "\n HeatValues: " + t.HeatValue + t.HeatType);
//        //}
//        heightMap = new HeightMap(heightMapValues.data, heightMapValues.Min, heightMapValues.Max);
//        heatMap = new HeatMap(heatMapValues.data, heatMapValues.Min, heatMapValues.Max);
//        moistureMap = new MoistureMap(moistureMapValues.data, moistureMapValues.Min, moistureMapValues.Max);

//        UpdateNeighbors();
//        //GenerateRivers(prng.Next(0,width), prng.Next(0,height));
//        //BuildRiverGroups();
//        //DigRiverGroups();
//        AdjustMoistureMap();

//        UpdateBitmasks();
//        FloodFill();

//        GenerateBiomeMap();
//        UpdateBiomeBitmask();
//        return new NoiseMaps(heightMap, heatMap, moistureMap, tiles);

//    }

//	private void AddMoisture(Tile t, int radius)
//	{
//		int startx = Mod(t.X - radius, width);
//		int endx = Mod(t.X + radius, width);
//		Vector2 center = new Vector2(t.X, t.Y);
//		int curr = radius;

//		while (curr > 0)
//		{

//			int x1 = Mod(t.X - curr, width);
//			int x2 = Mod(t.X + curr, width);
//			int y = t.Y;

//			AddMoisture(tiles[x1, y], 0.025f / (center - new Vector2(x1, y)).magnitude);

//			for (int i = 0; i < curr; i++)
//			{
//				AddMoisture(tiles[x1, Mod(y + i + 1, height)], 0.025f / (center - new Vector2(x1, Mod(y + i + 1, height))).magnitude);
//				AddMoisture(tiles[x1, Mod(y - (i + 1), height)], 0.025f / (center - new Vector2(x1, Mod(y - (i + 1), height))).magnitude);

//				AddMoisture(tiles[x2, Mod(y + i + 1, height)], 0.025f / (center - new Vector2(x2, Mod(y + i + 1, height))).magnitude);
//				AddMoisture(tiles[x2, Mod(y - (i + 1), height)], 0.025f / (center - new Vector2(x2, Mod(y - (i + 1), height))).magnitude);
//			}
//			curr--;
//		}
//	}

//	private void AddMoisture(Tile t, float amount)
//	{
//		moistureMap.values[t.X, t.Y] += amount;
//		t.MoistureValue += amount;
//		if (t.MoistureValue > 1)
//			t.MoistureValue = 1;

//		//set moisture type
//		if (t.MoistureValue < settings.noiseSettings.DryerValue) t.MoistureType = MoistureType.Dryest;
//		else if (t.MoistureValue < settings.noiseSettings.DryValue) t.MoistureType = MoistureType.Dryer;
//		else if (t.MoistureValue < settings.noiseSettings.WetValue) t.MoistureType = MoistureType.Dry;
//		else if (t.MoistureValue < settings.noiseSettings.WetterValue) t.MoistureType = MoistureType.Wet;
//		else if (t.MoistureValue < settings.noiseSettings.WettestValue) t.MoistureType = MoistureType.Wetter;
//		else t.MoistureType = MoistureType.Wettest;
//	}
//	public void AdjustMoistureMap()
//	{
//		for (var x = 0; x < width; x++)
//		{
//			for (var y = 0; y < height; y++)
//			{

//				Tile t = tiles[x, y];
//				if (t.HeightType == HeightType.River)
//				{
//					AddMoisture(t, 60);
//				}
//			}
//		}
//	}

//	public void DigRiverGroups()
//	{
//		for (int i = 0; i < RiverGroups.Count; i++)
//		{

//			RiverGroup group = RiverGroups[i];
//			River longest = null;

//			//Find longest river in this group
//			for (int j = 0; j < group.Rivers.Count; j++)
//			{
//				River river = group.Rivers[j];
//				if (longest == null)
//					longest = river;
//				else if (longest.Tiles.Count < river.Tiles.Count)
//					longest = river;
//			}

//			if (longest != null)
//			{
//				//Dig out longest path first
//				DigRiver(longest);

//				for (int j = 0; j < group.Rivers.Count; j++)
//				{
//					River river = group.Rivers[j];
//					if (river != longest)
//					{
//						DigRiver(river, longest);
//					}
//				}
//			}
//		}
//	}
//	public void UpdateBiomeBitmask()
//	{
//		for (var x = 0; x < width; x++)
//		{
//			for (var y = 0; y < height; y++)
//			{
//				tiles[x, y].UpdateBiomeBitmask();
//			}
//		}
//	}

//	public void GenerateBiomeMap()
//	{
//		for (var x = 0; x < width; x++)
//		{
//			for (var y = 0; y < height; y++)
//			{

//				if (!tiles[x, y].Collidable) continue;

//				Tile t = tiles[x, y];
//				t.BiomeType = t.GetBiomeType(t);
//			}
//		}
//	}

//	public void GenerateRivers(int x, int y)
//	{
//		int attempts = 0;
//		int rivercount = RiverCount;
//		Rivers = new List<River>();

//		// Generate some rivers
//		while (rivercount > 0 && attempts < MaxRiverAttempts)
//		{
//			Tile tile = tiles[x, y];

//			// validate the tile
//			if (!tile.Collidable) continue;
//			if (tile.Rivers.Count > 0) continue;

//			if (tile.HeightValue > MinRiverHeight)
//			{
//				// Tile is good to start river from
//				River river = new River(rivercount);

//				// Figure out the direction this river will try to flow
//				river.CurrentDirection = tile.GetLowestNeighbor(this);

//				// Recursively find a path to water
//				FindPathToWater(tile, river.CurrentDirection, ref river);

//				// Validate the generated river 
//				if (river.TurnCount < MinRiverTurns || river.Tiles.Count < MinRiverLength || river.Intersections > MaxRiverIntersections)
//				{
//					//Validation failed - remove this river
//					for (int i = 0; i < river.Tiles.Count; i++)
//					{
//						Tile t = river.Tiles[i];
//						t.Rivers.Remove(river);
//					}
//				}
//				else if (river.Tiles.Count >= MinRiverLength)
//				{
//					//Validation passed - Add river to list
//					Rivers.Add(river);
//					tile.Rivers.Add(river);
//					rivercount--;
//				}
//			}
//			attempts++;
//		}
//	}

//	// Dig river based on a parent river vein
//	public void DigRiver(River river, River parent)
//	{
//		int intersectionID = 0;
//		int intersectionSize = 0;

//		// determine point of intersection
//		for (int i = 0; i < river.Tiles.Count; i++)
//		{
//			Tile t1 = river.Tiles[i];
//			for (int j = 0; j < parent.Tiles.Count; j++)
//			{
//				Tile t2 = parent.Tiles[j];
//				if (t1 == t2)
//				{
//					intersectionID = i;
//					intersectionSize = t2.RiverSize;
//				}
//			}
//		}

//		int counter = 0;
//		int intersectionCount = river.Tiles.Count - intersectionID;
//		int size = UnityEngine.Random.Range(intersectionSize, 5);
//		river.Length = river.Tiles.Count;

//		// randomize size change
//		int two = river.Length / 2;
//		int three = two / 2;
//		int four = three / 2;
//		int five = four / 2;

//		int twomin = two / 3;
//		int threemin = three / 3;
//		int fourmin = four / 3;
//		int fivemin = five / 3;

//		// randomize length of each size
//		int count1 = UnityEngine.Random.Range(fivemin, five);
//		if (size < 4)
//		{
//			count1 = 0;
//		}
//		int count2 = count1 + UnityEngine.Random.Range(fourmin, four);
//		if (size < 3)
//		{
//			count2 = 0;
//			count1 = 0;
//		}
//		int count3 = count2 + UnityEngine.Random.Range(threemin, three);
//		if (size < 2)
//		{
//			count3 = 0;
//			count2 = 0;
//			count1 = 0;
//		}
//		int count4 = count3 + UnityEngine.Random.Range(twomin, two);

//		// Make sure we are not digging past the river path
//		if (count4 > river.Length)
//		{
//			int extra = count4 - river.Length;
//			while (extra > 0)
//			{
//				if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
//				else if (count2 > 0) { count2--; count3--; count4--; extra--; }
//				else if (count3 > 0) { count3--; count4--; extra--; }
//				else if (count4 > 0) { count4--; extra--; }
//			}
//		}

//		// adjust size of river at intersection point
//		if (intersectionSize == 1)
//		{
//			count4 = intersectionCount;
//			count1 = 0;
//			count2 = 0;
//			count3 = 0;
//		}
//		else if (intersectionSize == 2)
//		{
//			count3 = intersectionCount;
//			count1 = 0;
//			count2 = 0;
//		}
//		else if (intersectionSize == 3)
//		{
//			count2 = intersectionCount;
//			count1 = 0;
//		}
//		else if (intersectionSize == 4)
//		{
//			count1 = intersectionCount;
//		}
//		else
//		{
//			count1 = 0;
//			count2 = 0;
//			count3 = 0;
//			count4 = 0;
//		}

//		// dig out the river
//		for (int i = river.Tiles.Count - 1; i >= 0; i--)
//		{

//			Tile t = river.Tiles[i];

//			if (counter < count1)
//			{
//				t.DigRiver(river, 4);
//			}
//			else if (counter < count2)
//			{
//				t.DigRiver(river, 3);
//			}
//			else if (counter < count3)
//			{
//				t.DigRiver(river, 2);
//			}
//			else if (counter < count4)
//			{
//				t.DigRiver(river, 1);
//			}
//			else
//			{
//				t.DigRiver(river, 0);
//			}
//			counter++;
//		}
//	}

//	// Dig river
//	public void DigRiver(River river)
//	{
//		int counter = 0;

//		// How wide are we digging this river?
//		int size = UnityEngine.Random.Range(1, 5);
//		river.Length = river.Tiles.Count;

//		// randomize size change
//		int two = river.Length / 2;
//		int three = two / 2;
//		int four = three / 2;
//		int five = four / 2;

//		int twomin = two / 3;
//		int threemin = three / 3;
//		int fourmin = four / 3;
//		int fivemin = five / 3;

//		// randomize lenght of each size
//		int count1 = UnityEngine.Random.Range(fivemin, five);
//		if (size < 4)
//		{
//			count1 = 0;
//		}
//		int count2 = count1 + UnityEngine.Random.Range(fourmin, four);
//		if (size < 3)
//		{
//			count2 = 0;
//			count1 = 0;
//		}
//		int count3 = count2 + UnityEngine.Random.Range(threemin, three);
//		if (size < 2)
//		{
//			count3 = 0;
//			count2 = 0;
//			count1 = 0;
//		}
//		int count4 = count3 + UnityEngine.Random.Range(twomin, two);

//		// Make sure we are not digging past the river path
//		if (count4 > river.Length)
//		{
//			int extra = count4 - river.Length;
//			while (extra > 0)
//			{
//				if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
//				else if (count2 > 0) { count2--; count3--; count4--; extra--; }
//				else if (count3 > 0) { count3--; count4--; extra--; }
//				else if (count4 > 0) { count4--; extra--; }
//			}
//		}

//		// Dig it out
//		for (int i = river.Tiles.Count - 1; i >= 0; i--)
//		{
//			Tile t = river.Tiles[i];

//			if (counter < count1)
//			{
//				t.DigRiver(river, 4);
//			}
//			else if (counter < count2)
//			{
//				t.DigRiver(river, 3);
//			}
//			else if (counter < count3)
//			{
//				t.DigRiver(river, 2);
//			}
//			else if (counter < count4)
//			{
//				t.DigRiver(river, 1);
//			}
//			else
//			{
//				t.DigRiver(river, 0);
//			}
//			counter++;
//		}
//	}
//	public void BuildRiverGroups()
//	{
//		//loop each tile, checking if it belongs to multiple rivers
//		for (var x = 0; x < width; x++)
//		{
//			for (var y = 0; y < height; y++)
//			{
//				Tile t = tiles[x, y];

//				if (t.Rivers.Count > 1)
//				{
//					// multiple rivers == intersection
//					RiverGroup group = null;

//					// Does a rivergroup already exist for this group?
//					for (int n = 0; n < t.Rivers.Count; n++)
//					{
//						River tileriver = t.Rivers[n];
//						for (int i = 0; i < RiverGroups.Count; i++)
//						{
//							for (int j = 0; j < RiverGroups[i].Rivers.Count; j++)
//							{
//								River river = RiverGroups[i].Rivers[j];
//								if (river.ID == tileriver.ID)
//								{
//									group = RiverGroups[i];
//								}
//								if (group != null) break;
//							}
//							if (group != null) break;
//						}
//						if (group != null) break;
//					}

//					// existing group found -- add to it
//					if (group != null)
//					{
//						for (int n = 0; n < t.Rivers.Count; n++)
//						{
//							if (!group.Rivers.Contains(t.Rivers[n]))
//								group.Rivers.Add(t.Rivers[n]);
//						}
//					}
//					else   //No existing group found - create a new one
//					{
//						group = new RiverGroup();
//						for (int n = 0; n < t.Rivers.Count; n++)
//						{
//							group.Rivers.Add(t.Rivers[n]);
//						}
//						RiverGroups.Add(group);
//					}
//				}
//			}
//		}
//	}

//	public float GetHeightValue(Tile tile)
//    {
//        if (tile == null)
//            return int.MaxValue;
//        else
//            return tile.HeightValue;
//    }

//	public void UpdateNeighbors()
//	{
//		for (var x = 0; x < width; x++)
//		{
//			for (var y = 0; y < height; y++)
//			{
//				Tile t = tiles[x, y];

//				t.Top = GetTop(t);
//				t.Bottom = GetBottom(t);
//				t.Left = GetLeft(t);
//				t.Right = GetRight(t);
//			}
//		}
//	}
//	public void UpdateBitmasks()
//    {
//        for (var x = 0; x < width; x++)
//        {
//            for (var y = 0; y < height; y++)
//            {
//                tiles[x, y].UpdateBitmask();
//            }
//        }
//    }

	

//	public static int Mod(int x, int m)
//	{
//		int r = x % m;
//		return r < 0 ? r + m : r;
//	}
//	private Tile GetTop(Tile t)
//	{
//		return tiles[t.X, Mod(t.Y - 1, height)];
//	}
//	private Tile GetBottom(Tile t)
//	{
//		return tiles[t.X, Mod(t.Y + 1, height)];
//	}
//	private Tile GetLeft(Tile t)
//	{
//		return tiles[Mod(t.X - 1, width), t.Y];
//	}
//	private Tile GetRight(Tile t)
//	{
//		return tiles[Mod(t.X + 1, width), t.Y];
//	}

//	private void FindPathToWater(Tile tile, Direction direction, ref River river)
//	{
//		if (tile.Rivers.Contains(river))
//			return;

//		// check if there is already a river on this tile
//		if (tile.Rivers.Count > 0)
//			river.Intersections++;

//		river.AddTile(tile);

//		// get neighbors
//		Tile left = GetLeft(tile);
//		Tile right = GetRight(tile);
//		Tile top = GetTop(tile);
//		Tile bottom = GetBottom(tile);

//		float leftValue = int.MaxValue;
//		float rightValue = int.MaxValue;
//		float topValue = int.MaxValue;
//		float bottomValue = int.MaxValue;

//		// query height values of neighbors
//		if (left != null && left.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(left))
//			leftValue = left.HeightValue;
//		if (right != null && right.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(right))
//			rightValue = right.HeightValue;
//		if (top != null && top.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(top))
//			topValue = top.HeightValue;
//		if (bottom != null && bottom.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(bottom))
//			bottomValue = bottom.HeightValue;

//		// if neighbor is existing river that is not this one, flow into it
//		if (bottom != null && bottom.Rivers.Count == 0 && !bottom.Collidable)
//			bottomValue = 0;
//		if (top != null && top.Rivers.Count == 0 && !top.Collidable)
//			topValue = 0;
//		if (left != null && left.Rivers.Count == 0 && !left.Collidable)
//			leftValue = 0;
//		if (right != null && right.Rivers.Count == 0 && !right.Collidable)
//			rightValue = 0;

//		// override flow direction if a tile is significantly lower
//		if (direction == Direction.Left)
//			if (Mathf.Abs(rightValue - leftValue) < 0.1f)
//				rightValue = int.MaxValue;
//		if (direction == Direction.Right)
//			if (Mathf.Abs(rightValue - leftValue) < 0.1f)
//				leftValue = int.MaxValue;
//		if (direction == Direction.Top)
//			if (Mathf.Abs(topValue - bottomValue) < 0.1f)
//				bottomValue = int.MaxValue;
//		if (direction == Direction.Bottom)
//			if (Mathf.Abs(topValue - bottomValue) < 0.1f)
//				topValue = int.MaxValue;

//		// find mininum
//		float min = Mathf.Min(Mathf.Min(Mathf.Min(leftValue, rightValue), topValue), bottomValue);

//		// if no minimum found - exit
//		if (min == int.MaxValue)
//			return;

//		//Move to next neighbor
//		if (min == leftValue)
//		{
//			if (left != null && left.Collidable)
//			{
//				if (river.CurrentDirection != Direction.Left)
//				{
//					river.TurnCount++;
//					river.CurrentDirection = Direction.Left;
//				}
//				FindPathToWater(left, direction, ref river);
//			}
//		}
//		else if (min == rightValue)
//		{
//			if (right != null && right.Collidable)
//			{
//				if (river.CurrentDirection != Direction.Right)
//				{
//					river.TurnCount++;
//					river.CurrentDirection = Direction.Right;
//				}
//				FindPathToWater(right, direction, ref river);
//			}
//		}
//		else if (min == bottomValue)
//		{
//			if (bottom != null && bottom.Collidable)
//			{
//				if (river.CurrentDirection != Direction.Bottom)
//				{
//					river.TurnCount++;
//					river.CurrentDirection = Direction.Bottom;
//				}
//				FindPathToWater(bottom, direction, ref river);
//			}
//		}
//		else if (min == topValue)
//		{
//			if (top != null && top.Collidable)
//			{
//				if (river.CurrentDirection != Direction.Top)
//				{
//					river.TurnCount++;
//					river.CurrentDirection = Direction.Top;
//				}
//				FindPathToWater(top, direction, ref river);
//			}
//		}
//	}


//	public void FloodFill()
//	{
//		// Use a stack instead of recursion
//		Stack<Tile> stack = new Stack<Tile>();

//		for (int x = 0; x < width; x++)
//		{
//			for (int y = 0; y < height; y++)
//			{

//				Tile t = tiles[x, y];

//				//Tile already flood filled, skip
//				if (t.FloodFilled) continue;

//				// Land
//				if (t.Collidable)
//				{
//					TileGroup group = new TileGroup();
//					group.Type = TileGroupType.Land;
//					stack.Push(t);

//					while (stack.Count > 0)
//					{
//						FloodFill(stack.Pop(), ref group, ref stack);
//					}

//					if (group.Tiles.Count > 0)
//						Lands.Add(group);
//				}
//				// Water
//				else
//				{
//					TileGroup group = new TileGroup();
//					group.Type = TileGroupType.Water;
//					stack.Push(t);

//					while (stack.Count > 0)
//					{
//						FloodFill(stack.Pop(), ref group, ref stack);
//					}

//					if (group.Tiles.Count > 0)
//						Waters.Add(group);
//				}
//			}
//		}
//	}

//	public void FloodFill(Tile tile, ref TileGroup tiles, ref Stack<Tile> stack)
//	{
//		// Validate
//		if (tile == null)
//			return;
//		if (tile.FloodFilled)
//			return;
//		if (tiles.Type == TileGroupType.Land && !tile.Collidable)
//			return;
//		if (tiles.Type == TileGroupType.Water && tile.Collidable)
//			return;

//		// Add to TileGroup
//		tiles.Tiles.Add(tile);
//		tile.FloodFilled = true;

//		// floodfill into neighbors
//		Tile t = GetTop(tile);
//		if (t != null && !t.FloodFilled && tile.Collidable == t.Collidable)
//			stack.Push(t);
//		t = GetBottom(tile);
//		if (t != null && !t.FloodFilled && tile.Collidable == t.Collidable)
//			stack.Push(t);
//		t = GetLeft(tile);
//		if (t != null && !t.FloodFilled && tile.Collidable == t.Collidable)
//			stack.Push(t);
//		t = GetRight(tile);
//		if (t != null && !t.FloodFilled && tile.Collidable == t.Collidable)
//			stack.Push(t);
//	}

//}

//public class NoiseMaps
//{
//    public HeightMap heightMap;
//    public HeatMap heatMap;
//    public MoistureMap moistureMap;
//    public Tile[,] tiles;

//    public NoiseMaps(HeightMap heightMap, HeatMap heatMap, MoistureMap moistureMap, Tile[,] tiles)
//    {
//        this.heightMap = heightMap;
//        this.heatMap = heatMap;
//        this.moistureMap = moistureMap;
//        this.tiles = tiles;
//    }
//}

//public struct HeightMap
//{
//    public readonly float[,] values;
//    public readonly float minValue;
//    public readonly float maxValue;


//    public HeightMap(float[,] values, float minValue, float maxValue)
//    {
//        this.values = values;
//        this.minValue = minValue;
//        this.maxValue = maxValue;
//    }
//}

//public struct MoistureMap
//{
//    public readonly float[,] values;
//    public readonly float minValue;
//    public readonly float maxValue;


//    public MoistureMap(float[,] values, float minValue, float maxValue)
//    {
//        this.values = values;
//        this.minValue = minValue;
//        this.maxValue = maxValue;
//    }
//}

//public struct HeatMap
//{
//    public readonly float[,] values;
//    public readonly float minValue;
//    public readonly float maxValue;


//    public HeatMap(float[,] values, float minValue, float maxValue)
//    {
//        this.values = values;
//        this.minValue = minValue;
//        this.maxValue = maxValue;
//    }
//}