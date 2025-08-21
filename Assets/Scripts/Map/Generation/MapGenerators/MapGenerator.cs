using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator
{
    static float[,] falloffMap;

	public Vector2 coord;
    public Tile[,] tiles;
    static int width;
    static int height;

	HeightMapSettings heightMapSettings;
	HeatMapSettings heatMapSettings;
	MoistureMapSettings moistureMapSettings;

	public HeightMap heightMap;
	public HeatMap heatMap;
	public MoistureMap moistureMap;

	public float heatTotal = 0, heatAvg;
	public float moistTotal = 0, moistAvg;
	public float heightTotal = 0, heightAvg;

	[Header("Rivers")]
	[SerializeField]
	protected int RiverCount = 40;
	[SerializeField]
	protected float MinRiverHeight = 0.7f;
	[SerializeField]
	protected int MaxRiverAttempts = 1000;
	[SerializeField]
	protected int MinRiverTurns = 18;
	[SerializeField]
	protected int MinRiverLength = 20;
	[SerializeField]
	protected int MaxRiverIntersections = 2;
	protected List<River> Rivers = new List<River>();
	protected List<RiverGroup> RiverGroups = new List<RiverGroup>();
	protected List<TileGroup> Waters = new List<TileGroup>();
	protected List<TileGroup> Lands = new List<TileGroup>();

	MapData heightMapValues;
	MapData moistureMapValues;
	MapData heatMapValues;

	Dictionary<Vector2, Dictionary<BiomeType, float>[,]> chunkWeightsCache = new();

	private System.Random prng;

	public NoiseMaps GenerateMaps(Vector2 _coord, int _width, int _height, HeightMapSettings _heightMapSettings, HeatMapSettings _heatMapSettings, MoistureMapSettings _moistureMapSettings, Vector2 sampleCentre)
    {
		prng = new System.Random();
		coord = _coord;
        width = _width;
        height = _height;
		heightMapSettings = _heightMapSettings;
		heatMapSettings = _heatMapSettings;
		moistureMapSettings = _moistureMapSettings;
        tiles = new Tile[_width, _height];
		heightMapValues = Noise.GenerateNoiseMapData(_width, _height, _heightMapSettings.noiseSettings, sampleCentre);
        moistureMapValues = Noise.GenerateNoiseMapData(_width, _height, _moistureMapSettings.noiseSettings, sampleCentre);
		heatMapValues = Noise.GenerateNoiseMapData(_width, _height, _heatMapSettings.noiseSettings, sampleCentre);


        AnimationCurve heightCurve_threadsafe = new AnimationCurve(heightMapSettings.heightCurve.keys);
		AnimationCurve heatCurve_threadsafe = new AnimationCurve(heatMapSettings.heightCurve.keys);

		AnimationCurve moistCurve_threadsafe = new AnimationCurve(moistureMapSettings.heightCurve.keys);


		if (heightMapSettings.useFalloff)
        {
            if (falloffMap ==  null)
            {
                falloffMap = FalloffGenerator.GenerateFalloffMap(_width);
            }
        }
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {

                heightMapValues.data[i, j] *= heightCurve_threadsafe.Evaluate(heightMapValues.data[i, j] - (heightMapSettings.useFalloff ? falloffMap[i, j] : 0)) * heightMapSettings.heightMultiplier;

                if (heightMapValues.data[i, j] > heightMapValues.Max)
                {
                    heightMapValues.Max = heightMapValues.data[i, j];
                }
                if (heightMapValues.data[i, j] < heightMapValues.Min)
                {
                    heightMapValues.Min = heightMapValues.data[i, j];
                }


				moistureMapValues.data[i, j] *= moistCurve_threadsafe.Evaluate(moistureMapValues.data[i, j] - (heightMapSettings.useFalloff ? falloffMap[i, j] : 0)) * heightMapSettings.heightMultiplier;

				if (moistureMapValues.data[i, j] > moistureMapValues.Max)
                {
                    moistureMapValues.Max = moistureMapValues.data[i, j];
                }
                if (moistureMapValues.data[i, j] < moistureMapValues.Min)
                {
                    moistureMapValues.Min = moistureMapValues.data[i, j];

                }

				heatMapValues.data[i, j] *= heatCurve_threadsafe.Evaluate(heatMapValues.data[i, j] - (heightMapSettings.useFalloff ? falloffMap[i, j] : 0)) * heightMapSettings.heightMultiplier;

				if (heatMapValues.data[i, j] > heatMapValues.Max)
                {
                    heatMapValues.Max = heatMapValues.data[i, j];
                }
                if (heatMapValues.data[i, j] < heatMapValues.Min)
                {
                    heatMapValues.Min = heatMapValues.data[i, j];
                }
            }
        }

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Tile t = new Tile();
                t.X = x;
                t.Y = y;

                float heightValue = heightMapValues.data[x, y];
                heightValue = (heightValue - heightMapValues.Min) / (heightMapValues.Max - heightMapValues.Min);
                t.HeightValue = heightValue;
				t.HeatValue = heatMapValues.data[x, y];

				tiles[x, y] = t;

				//t.BiomeType = t.GetBiomeType(t);
			}


        }

		EvaluateTiles(tiles);
        //foreach (Tile t in tiles)
        //{
        //    Debug.Log("TileData: \n HeightValues: " + t.HeightValue + t.HeightType + "\n MoistureValues: " + t.MoistureValue + t.MoistureType + "\n HeatValues: " + t.HeatValue + t.HeatType);
        //}
        heightMap = new HeightMap(heightMapValues.data, heightMapValues.Min, heightMapValues.Max);
        heatMap = new HeatMap(heatMapValues.data, heatMapValues.Min, heatMapValues.Max);
        moistureMap = new MoistureMap(moistureMapValues.data, moistureMapValues.Min, moistureMapValues.Max);

        UpdateNeighbors();
        //GenerateRivers();
        //BuildRiverGroups();
        //DigRiverGroups();
        AdjustMoistureMap();

        UpdateBitmasks();
        FloodFill();

        GenerateBiomeMap();
		CalcTileDist();
        //Tile[,] smoothedTiles = ApplyBiomeBlur(newTiles, radius: 2, sigma: 1.0f);
        //ApplyBiomeBlur(tiles, radius: 4, sigma: 1.0f);
        UpdateBiomeBitmask();

        return new NoiseMaps(heightMap, heatMap, moistureMap, tiles);

    }

	private float GetBiomeMatchValue(float temperature, float humidity)
	{
		float temperatureMatch = Mathf.Abs(heatMap.avgValue - temperature);
		float humidityMatch = Mathf.Abs(moistureMap.avgValue - humidity);

		return temperatureMatch + humidityMatch; // Lesser is better
	}
	public Dictionary<BiomeType, float>[,] GetChunkBiomeWeights()
	{
		if (chunkWeightsCache.TryGetValue(coord, out var cachedWeights))
		{
			return cachedWeights;
		}
		// Create all data at once.
		Dictionary<BiomeType, float>[,] result = new Dictionary<BiomeType, float>[width + 1, height + 1];
		for (int y = 0; y < height + 1; y++)
		{
			for (int x = 0; x < width + 1; x++)
			{
				if (result[x, y] == null)
				{
					result[x, y] = new Dictionary<BiomeType, float>();
				}
				float temperature = heatMap.values[x,y];
				float humidity = moistureMap.values[x,y];
				foreach(BiomeType biomeType in TerrainGenerator.BiomeTable)
                {
					float match = GetBiomeMatchValue(temperature, humidity);
					match = 1 / (match + 0.1f); // Take inverse and stop NaNs.
					match = Mathf.Pow(match + 1, 10); //Pow match +1, biomeBlendingFactor
					result[x, y].Add(biomeType, match);
				}

			}
		}
		if (chunkWeightsCache.Count > 256)
		{
			chunkWeightsCache.Clear();
		}
		chunkWeightsCache.Add(coord, result);
		return result;
	}


	public void CalcTileDist()
	{
		foreach (Tile t in tiles)
		{
			// Calculate distance from edge per tile. Take care that this assumes x,y = 0,0 = bottomleft
			float maxX = width - 1 - t.X;
			float maxY = height - 1 - t.Y;

			t.distFromEdge = Mathf.Min(t.X, t.Y, maxX, maxY);
		}

	}

	public void EvaluateTiles(Tile[,] tiles)
    {
		foreach (Tile t in tiles)
        {

		if (t.HeightValue < heightMapSettings.DeepWater)
		{
			t.HeightType = HeightType.DeepWater;
			t.Collidable = false;
		}
		else if (t.HeightValue < heightMapSettings.ShallowWater)
		{
			t.HeightType = HeightType.ShallowWater;
			t.Collidable = false;

		}
		else if (t.HeightValue < heightMapSettings.Sand)
		{
			t.HeightType = HeightType.Sand;
			t.Collidable = true;

		}
		else if (t.HeightValue < heightMapSettings.Grass)
		{
			t.HeightType = HeightType.Grass;
			t.Collidable = true;

		}

		else if (t.HeightValue < heightMapSettings.Forest)
		{
			t.HeightType = HeightType.Forest;
			t.Collidable = true;
		}

		else if (t.HeightValue < heightMapSettings.Rock)
		{
			t.HeightType = HeightType.Rock;
			t.Collidable = true;

		}
		else
		{
			t.HeightType = HeightType.Snow;
			t.Collidable = true;

		}

		if (t.HeightType == HeightType.DeepWater)
		{
			moistureMapValues.data[t.X, t.Y] += 8f * t.HeightValue;
		}
		else if (t.HeightType == HeightType.ShallowWater)
		{
			moistureMapValues.data[t.X, t.Y] += 3f * t.HeightValue;
		}
		else if (t.HeightType == HeightType.Shore)
		{
			moistureMapValues.data[t.X, t.Y] += 1f * t.HeightValue;
		}
		else if (t.HeightType == HeightType.Sand)
		{
			moistureMapValues.data[t.X, t.Y] += 0.2f * t.HeightValue;
		}


		float maxMoistValue = Enum.GetNames(typeof(MoistureType)).Length;

		float moistureValue = moistureMapValues.data[t.X, t.Y];
		moistureValue = (moistureValue - moistureMapValues.Min) / (moistureMapValues.Max - moistureMapValues.Min);
		if (moistureValue > maxMoistValue)
		{
			moistureValue = maxMoistValue;
		}
		t.MoistureValue = moistureValue;

		//set moisture type
		if (t.MoistureValue < moistureMapSettings.DryerValue) t.MoistureType = MoistureType.Dryest;
		else if (t.MoistureValue < moistureMapSettings.DryValue) t.MoistureType = MoistureType.Dryer;
		else if (t.MoistureValue < moistureMapSettings.WetValue) t.MoistureType = MoistureType.Dry;
		else if (t.MoistureValue < moistureMapSettings.WetterValue) t.MoistureType = MoistureType.Wet;
		else if (t.MoistureValue < moistureMapSettings.WettestValue) t.MoistureType = MoistureType.Wetter;
		else t.MoistureType = MoistureType.Wettest;


		// Adjust Heat Map based on Height - Higher == colder
		if (t.HeightType == HeightType.Forest)
		{
			heatMapValues.data[t.X, t.Y] -= 0.1f * t.HeightValue;
		}
		else if (t.HeightType == HeightType.Rock)
		{
			heatMapValues.data[t.X, t.Y] -= 0.25f * t.HeightValue;
		}
		else if (t.HeightType == HeightType.Snow)
		{
			heatMapValues.data[t.X, t.Y] -= 0.4f * t.HeightValue;
		}
		else
		{
			heatMapValues.data[t.X, t.Y] += 0.01f * t.HeightValue;
		}

		// Set heat value
		float maxHeatValue = Enum.GetNames(typeof(HeatType)).Length;

		float heatValue = heatMapValues.data[t.X, t.Y];
		heatValue = (heatValue - heatMapValues.Min) / (heatMapValues.Max - heatMapValues.Min);

		if (heatValue > maxHeatValue)
			heatValue = maxHeatValue;
		t.HeatValue = heatValue;

		// set heat type
		if (t.HeatValue < heatMapSettings.ColdestValue) t.HeatType = HeatType.Coldest;
		else if (t.HeatValue < heatMapSettings.ColderValue) t.HeatType = HeatType.Colder;
		else if (t.HeatValue < heatMapSettings.ColdValue) t.HeatType = HeatType.Cold;
		else if (t.HeatValue < heatMapSettings.WarmValue) t.HeatType = HeatType.Warm;
		else if (t.HeatValue < heatMapSettings.WarmerValue) t.HeatType = HeatType.Warmer;
		else t.HeatType = HeatType.Warmest;
		

		}

	}
	bool IsCriticalMoisture(MoistureType biomeMoistureType) => biomeMoistureType == MoistureType.Wettest || biomeMoistureType == MoistureType.Wetter;

	private void AddMoisture(Tile t, int radius)
	{
		int startx = Mod(t.X - radius, width);
		int endx = Mod(t.X + radius, width);
		Vector2 center = new Vector2(t.X, t.Y);
		int curr = radius;

		while (curr > 0)
		{

			int x1 = Mod(t.X - curr, width);
			int x2 = Mod(t.X + curr, width);
			int y = t.Y;

			AddMoisture(tiles[x1, y], 0.025f / (center - new Vector2(x1, y)).magnitude);

			for (int i = 0; i < curr; i++)
			{
				AddMoisture(tiles[x1, Mod(y + i + 1, height)], 0.025f / (center - new Vector2(x1, Mod(y + i + 1, height))).magnitude);
				AddMoisture(tiles[x1, Mod(y - (i + 1), height)], 0.025f / (center - new Vector2(x1, Mod(y - (i + 1), height))).magnitude);

				AddMoisture(tiles[x2, Mod(y + i + 1, height)], 0.025f / (center - new Vector2(x2, Mod(y + i + 1, height))).magnitude);
				AddMoisture(tiles[x2, Mod(y - (i + 1), height)], 0.025f / (center - new Vector2(x2, Mod(y - (i + 1), height))).magnitude);
			}
			curr--;
		}
	}

	private void AddMoisture(Tile t, float amount)
	{
		moistureMap.values[t.X, t.Y] += amount;
		t.MoistureValue += amount;
		if (t.MoistureValue > 1)
			t.MoistureValue = 1;

		//set moisture type
		if (t.MoistureValue < moistureMapSettings.DryerValue) t.MoistureType = MoistureType.Dryest;
		else if (t.MoistureValue < moistureMapSettings.DryValue) t.MoistureType = MoistureType.Dryer;
		else if (t.MoistureValue < moistureMapSettings.WetValue) t.MoistureType = MoistureType.Dry;
		else if (t.MoistureValue < moistureMapSettings.WetterValue) t.MoistureType = MoistureType.Wet;
		else if (t.MoistureValue < moistureMapSettings.WettestValue) t.MoistureType = MoistureType.Wetter;
		else t.MoistureType = MoistureType.Wettest;
	}
	public void AdjustMoistureMap()
	{
		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{

				Tile t = tiles[x, y];
				if (t.HeightType == HeightType.River)
				{
					AddMoisture(t, 60);
				}
			}
		}
	}

	public void DigRiverGroups()
	{
		for (int i = 0; i < RiverGroups.Count; i++)
		{

			RiverGroup group = RiverGroups[i];
			River longest = null;

			//Find longest river in this group
			for (int j = 0; j < group.Rivers.Count; j++)
			{
				River river = group.Rivers[j];
				if (longest == null)
					longest = river;
				else if (longest.Tiles.Count < river.Tiles.Count)
					longest = river;
			}

			if (longest != null)
			{
				//Dig out longest path first
				DigRiver(longest);

				for (int j = 0; j < group.Rivers.Count; j++)
				{
					River river = group.Rivers[j];
					if (river != longest)
					{
						DigRiver(river, longest);
					}
				}
			}
		}
	}
	public void UpdateBiomeBitmask()
	{
		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{
				tiles[x, y].UpdateBiomeBitmask();
			}
		}
	}

	public void GenerateBiomeMap()
	{
		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{

				if (!tiles[x, y].Collidable) continue;

				Tile t = tiles[x, y];
				t.BiomeType = t.GetBiomeType(t);
            }
		}
	}
	public float[,] GenerateGaussianKernel(int radius, float sigma)
	{
		int size = 2 * radius + 1;
		float[,] kernel = new float[size, size];
		float total = 0f;

		for (int x = -radius; x <= radius; x++)
		{
			for (int y = -radius; y <= radius; y++)
			{
				float value = Mathf.Exp(-(x * x + y * y) / (2 * sigma * sigma));
				kernel[x + radius, y + radius] = value;
				total += value;
			}
		}

		for (int x = 0; x < size; x++)
			for (int y = 0; y < size; y++)
				kernel[x, y] /= total;

		return kernel;
	}

	//Tile[,] ApplyBiomeBlur(Tile[,] input, int radius, float sigma)
	void ApplyBiomeBlur(Tile[,] input, int radius, float sigma)

	{
		int w = input.GetLength(0);
		int h = input.GetLength(1);
		Tile[,] output = new Tile[w, h];
		float[,] kernel = GenerateGaussianKernel(radius, sigma);

		for (int x = 0; x < w; x++)
		{
			for (int y = 0; y < h; y++)
			{
				Dictionary<BiomeType, float> biomeScores = new Dictionary<BiomeType, float>();

				for (int dx = -radius; dx <= radius; dx++)
				{
					for (int dy = -radius; dy <= radius; dy++)
					{
						int nx = x + dx;
						int ny = y + dy;

						if (nx >= 0 && nx < w && ny >= 0 && ny < h)
						{
							BiomeType neighborBiome = input[nx, ny].BiomeType;
							float weight = kernel[dx + radius, dy + radius];

							if (!biomeScores.ContainsKey(neighborBiome))
								biomeScores[neighborBiome] = 0f;

							biomeScores[neighborBiome] += weight;
						}
					}
				}

				// Pick the dominant biome after blur
				BiomeType original = tiles[x, y].BiomeType;
				MoistureType originalMoisture = tiles[x, y].MoistureType;

				BiomeType dominantBiome = biomeScores
					.OrderByDescending(kvp => kvp.Value)
					.First().Key;


				if (IsCriticalMoisture(originalMoisture))
				{
					float originalWeight = biomeScores.ContainsKey(original) ? biomeScores[original] : 0f;
					float dominantWeight = biomeScores[dominantBiome];

					// Only override critical biome if the new one is overwhelming
					if (dominantBiome != original && dominantWeight < 2.0f * originalWeight)
						dominantBiome = original;
				}
				// Copy the tile and assign the new blurred biome
				//Tile blurredTile = new Tile
				//{
				//	X = x,
				//	Y = y,
				//	BiomeType = dominantBiome
				//};

				input[x, y].BiomeType = dominantBiome;
			}
		}
	}

	bool IsEdgeTile(Tile[,] tiles, int x, int y)
	{
		BiomeType center = tiles[x, y].BiomeType;

		for (int dx = -1; dx <= 1; dx++)
			for (int dy = -1; dy <= 1; dy++)
			{
				int nx = x + dx;
				int ny = y + dy;
				if (nx >= 0 && nx < tiles.GetLength(0) && ny >= 0 && ny < tiles.GetLength(1))
					if (tiles[nx, ny].BiomeType != center)
						return true;
			}

		return false;
	}


	public void GenerateRivers()
	{
		int attempts = 0;
		int rivercount = RiverCount;
		Rivers = new List<River>();

		// Generate some rivers
		while (rivercount > 0 && attempts < MaxRiverAttempts)
		{
			int x = prng.Next(0, width);
			int y = prng.Next(0, height);

			Tile tile = tiles[x, y];

			// validate the tile
			if (!tile.Collidable) continue;
			if (tile.Rivers.Count > 0) continue;

			if (tile.HeightValue > MinRiverHeight)
			{
				// Tile is good to start river from
				River river = new River(rivercount);

				// Figure out the direction this river will try to flow
				river.CurrentDirection = tile.GetLowestNeighbor(this);

				// Recursively find a path to water
				FindPathToWater(tile, river.CurrentDirection, ref river);

				// Validate the generated river 
				if (river.TurnCount < MinRiverTurns || river.Tiles.Count < MinRiverLength || river.Intersections > MaxRiverIntersections)
				{
					//Validation failed - remove this river
					for (int i = 0; i < river.Tiles.Count; i++)
					{
						Tile t = river.Tiles[i];
						t.Rivers.Remove(river);
					}
				}
				else if (river.Tiles.Count >= MinRiverLength)
				{
					//Validation passed - Add river to list
					Rivers.Add(river);
					tile.Rivers.Add(river);
					rivercount--;
				}
			}
			attempts++;
		}
	}

	// Dig river based on a parent river vein
	public void DigRiver(River river, River parent)
	{
		int intersectionID = 0;
		int intersectionSize = 0;

		// determine point of intersection
		for (int i = 0; i < river.Tiles.Count; i++)
		{
			Tile t1 = river.Tiles[i];
			for (int j = 0; j < parent.Tiles.Count; j++)
			{
				Tile t2 = parent.Tiles[j];
				if (t1 == t2)
				{
					intersectionID = i;
					intersectionSize = t2.RiverSize;
				}
			}
		}

		int counter = 0;
		int intersectionCount = river.Tiles.Count - intersectionID;
		int size = prng.Next(intersectionSize, 5);
		river.Length = river.Tiles.Count;

		// randomize size change
		int two = river.Length / 2;
		int three = two / 2;
		int four = three / 2;
		int five = four / 2;

		int twomin = two / 3;
		int threemin = three / 3;
		int fourmin = four / 3;
		int fivemin = five / 3;

		// randomize length of each size
		int count1 = prng.Next(fivemin, five);
		if (size < 4)
		{
			count1 = 0;
		}
		int count2 = count1 + prng.Next(fourmin, four);
		if (size < 3)
		{
			count2 = 0;
			count1 = 0;
		}
		int count3 = count2 + prng.Next(threemin, three);
		if (size < 2)
		{
			count3 = 0;
			count2 = 0;
			count1 = 0;
		}
		int count4 = count3 + prng.Next(twomin, two);

		// Make sure we are not digging past the river path
		if (count4 > river.Length)
		{
			int extra = count4 - river.Length;
			while (extra > 0)
			{
				if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
				else if (count2 > 0) { count2--; count3--; count4--; extra--; }
				else if (count3 > 0) { count3--; count4--; extra--; }
				else if (count4 > 0) { count4--; extra--; }
			}
		}

		// adjust size of river at intersection point
		if (intersectionSize == 1)
		{
			count4 = intersectionCount;
			count1 = 0;
			count2 = 0;
			count3 = 0;
		}
		else if (intersectionSize == 2)
		{
			count3 = intersectionCount;
			count1 = 0;
			count2 = 0;
		}
		else if (intersectionSize == 3)
		{
			count2 = intersectionCount;
			count1 = 0;
		}
		else if (intersectionSize == 4)
		{
			count1 = intersectionCount;
		}
		else
		{
			count1 = 0;
			count2 = 0;
			count3 = 0;
			count4 = 0;
		}

		// dig out the river
		for (int i = river.Tiles.Count - 1; i >= 0; i--)
		{

			Tile t = river.Tiles[i];

			if (counter < count1)
			{
				t.DigRiver(river, 4);
			}
			else if (counter < count2)
			{
				t.DigRiver(river, 3);
			}
			else if (counter < count3)
			{
				t.DigRiver(river, 2);
			}
			else if (counter < count4)
			{
				t.DigRiver(river, 1);
			}
			else
			{
				t.DigRiver(river, 0);
			}
			counter++;
		}
	}

	// Dig river
	public void DigRiver(River river)
	{
		int counter = 0;

		// How wide are we digging this river?
		int size = prng.Next(1, 5);
		river.Length = river.Tiles.Count;

		// randomize size change
		int two = river.Length / 2;
		int three = two / 2;
		int four = three / 2;
		int five = four / 2;

		int twomin = two / 3;
		int threemin = three / 3;
		int fourmin = four / 3;
		int fivemin = five / 3;

		// randomize lenght of each size
		int count1 = prng.Next(fivemin, five);
		if (size < 4)
		{
			count1 = 0;
		}
		int count2 = count1 + prng.Next(fourmin, four);
		if (size < 3)
		{
			count2 = 0;
			count1 = 0;
		}
		int count3 = count2 + prng.Next(threemin, three);
		if (size < 2)
		{
			count3 = 0;
			count2 = 0;
			count1 = 0;
		}
		int count4 = count3 + prng.Next(twomin, two);

		// Make sure we are not digging past the river path
		if (count4 > river.Length)
		{
			int extra = count4 - river.Length;
			while (extra > 0)
			{
				if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
				else if (count2 > 0) { count2--; count3--; count4--; extra--; }
				else if (count3 > 0) { count3--; count4--; extra--; }
				else if (count4 > 0) { count4--; extra--; }
			}
		}

		// Dig it out
		for (int i = river.Tiles.Count - 1; i >= 0; i--)
		{
			Tile t = river.Tiles[i];

			if (counter < count1)
			{
				t.DigRiver(river, 4);
			}
			else if (counter < count2)
			{
				t.DigRiver(river, 3);
			}
			else if (counter < count3)
			{
				t.DigRiver(river, 2);
			}
			else if (counter < count4)
			{
				t.DigRiver(river, 1);
			}
			else
			{
				t.DigRiver(river, 0);
			}
			counter++;
		}
	}
	public void BuildRiverGroups()
	{
		//loop each tile, checking if it belongs to multiple rivers
		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{
				Tile t = tiles[x, y];

				if (t.Rivers.Count > 1)
				{
					// multiple rivers == intersection
					RiverGroup group = null;

					// Does a rivergroup already exist for this group?
					for (int n = 0; n < t.Rivers.Count; n++)
					{
						River tileriver = t.Rivers[n];
						for (int i = 0; i < RiverGroups.Count; i++)
						{
							for (int j = 0; j < RiverGroups[i].Rivers.Count; j++)
							{
								River river = RiverGroups[i].Rivers[j];
								if (river.ID == tileriver.ID)
								{
									group = RiverGroups[i];
								}
								if (group != null) break;
							}
							if (group != null) break;
						}
						if (group != null) break;
					}

					// existing group found -- add to it
					if (group != null)
					{
						for (int n = 0; n < t.Rivers.Count; n++)
						{
							if (!group.Rivers.Contains(t.Rivers[n]))
								group.Rivers.Add(t.Rivers[n]);
						}
					}
					else   //No existing group found - create a new one
					{
						group = new RiverGroup();
						for (int n = 0; n < t.Rivers.Count; n++)
						{
							group.Rivers.Add(t.Rivers[n]);
						}
						RiverGroups.Add(group);
					}
				}
			}
		}
	}

	public float GetHeightValue(Tile tile)
    {
        if (tile == null)
            return int.MaxValue;
        else
            return tile.HeightValue;
    }

	public void UpdateNeighbors()
	{
		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{
				Tile t = tiles[x, y];

				t.Top = GetTop(t);
				t.Bottom = GetBottom(t);
				t.Left = GetLeft(t);
				t.Right = GetRight(t);
			}
		}
	}
	public void UpdateBitmasks()
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                tiles[x, y].UpdateBitmask();
            }
        }
    }

	

	public static int Mod(int x, int m)
	{
		int r = x % m;
		return r < 0 ? r + m : r;
	}
	private Tile GetTop(Tile t)
	{
		return tiles[t.X, Mod(t.Y - 1, height)];
	}
	private Tile GetBottom(Tile t)
	{
		return tiles[t.X, Mod(t.Y + 1, height)];
	}
	private Tile GetLeft(Tile t)
	{
		return tiles[Mod(t.X - 1, width), t.Y];
	}
	private Tile GetRight(Tile t)
	{
		return tiles[Mod(t.X + 1, width), t.Y];
	}

	private void FindPathToWater(Tile tile, Direction direction, ref River river)
	{
		if (tile.Rivers.Contains(river))
			return;

		// check if there is already a river on this tile
		if (tile.Rivers.Count > 0)
			river.Intersections++;

		river.AddTile(tile);

		// get neighbors
		Tile left = GetLeft(tile);
		Tile right = GetRight(tile);
		Tile top = GetTop(tile);
		Tile bottom = GetBottom(tile);

		float leftValue = int.MaxValue;
		float rightValue = int.MaxValue;
		float topValue = int.MaxValue;
		float bottomValue = int.MaxValue;

		// query height values of neighbors
		if (left != null && left.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(left))
			leftValue = left.HeightValue;
		if (right != null && right.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(right))
			rightValue = right.HeightValue;
		if (top != null && top.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(top))
			topValue = top.HeightValue;
		if (bottom != null && bottom.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(bottom))
			bottomValue = bottom.HeightValue;

		// if neighbor is existing river that is not this one, flow into it
		if (bottom != null && bottom.Rivers.Count == 0 && !bottom.Collidable)
			bottomValue = 0;
		if (top != null && top.Rivers.Count == 0 && !top.Collidable)
			topValue = 0;
		if (left != null && left.Rivers.Count == 0 && !left.Collidable)
			leftValue = 0;
		if (right != null && right.Rivers.Count == 0 && !right.Collidable)
			rightValue = 0;

		// override flow direction if a tile is significantly lower
		if (direction == Direction.Left)
			if (Mathf.Abs(rightValue - leftValue) < 0.1f)
				rightValue = int.MaxValue;
		if (direction == Direction.Right)
			if (Mathf.Abs(rightValue - leftValue) < 0.1f)
				leftValue = int.MaxValue;
		if (direction == Direction.Top)
			if (Mathf.Abs(topValue - bottomValue) < 0.1f)
				bottomValue = int.MaxValue;
		if (direction == Direction.Bottom)
			if (Mathf.Abs(topValue - bottomValue) < 0.1f)
				topValue = int.MaxValue;

		// find mininum
		float min = Mathf.Min(Mathf.Min(Mathf.Min(leftValue, rightValue), topValue), bottomValue);

		// if no minimum found - exit
		if (min == int.MaxValue)
			return;

		//Move to next neighbor
		if (min == leftValue)
		{
			if (left != null && left.Collidable)
			{
				if (river.CurrentDirection != Direction.Left)
				{
					river.TurnCount++;
					river.CurrentDirection = Direction.Left;
				}
				FindPathToWater(left, direction, ref river);
			}
		}
		else if (min == rightValue)
		{
			if (right != null && right.Collidable)
			{
				if (river.CurrentDirection != Direction.Right)
				{
					river.TurnCount++;
					river.CurrentDirection = Direction.Right;
				}
				FindPathToWater(right, direction, ref river);
			}
		}
		else if (min == bottomValue)
		{
			if (bottom != null && bottom.Collidable)
			{
				if (river.CurrentDirection != Direction.Bottom)
				{
					river.TurnCount++;
					river.CurrentDirection = Direction.Bottom;
				}
				FindPathToWater(bottom, direction, ref river);
			}
		}
		else if (min == topValue)
		{
			if (top != null && top.Collidable)
			{
				if (river.CurrentDirection != Direction.Top)
				{
					river.TurnCount++;
					river.CurrentDirection = Direction.Top;
				}
				FindPathToWater(top, direction, ref river);
			}
		}
	}


	public void FloodFill()
	{
		// Use a stack instead of recursion
		Stack<Tile> stack = new Stack<Tile>();

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{

				Tile t = tiles[x, y];

				//Tile already flood filled, skip
				if (t.FloodFilled) continue;

				// Land
				if (t.Collidable)
				{
					TileGroup group = new TileGroup();
					group.Type = TileGroupType.Land;
					stack.Push(t);

					while (stack.Count > 0)
					{
						FloodFill(stack.Pop(), ref group, ref stack);
					}

					if (group.Tiles.Count > 0)
						Lands.Add(group);
				}
				// Water
				else
				{
					TileGroup group = new TileGroup();
					group.Type = TileGroupType.Water;
					stack.Push(t);

					while (stack.Count > 0)
					{
						FloodFill(stack.Pop(), ref group, ref stack);
					}

					if (group.Tiles.Count > 0)
						Waters.Add(group);
				}
			}
		}
	}

	public void FloodFill(Tile tile, ref TileGroup tiles, ref Stack<Tile> stack)
	{
		// Validate
		if (tile == null)
			return;
		if (tile.FloodFilled)
			return;
		if (tiles.Type == TileGroupType.Land && !tile.Collidable)
			return;
		if (tiles.Type == TileGroupType.Water && tile.Collidable)
			return;

		// Add to TileGroup
		tiles.Tiles.Add(tile);
		tile.FloodFilled = true;

		// floodfill into neighbors
		Tile t = GetTop(tile);
		if (t != null && !t.FloodFilled && tile.Collidable == t.Collidable)
			stack.Push(t);
		t = GetBottom(tile);
		if (t != null && !t.FloodFilled && tile.Collidable == t.Collidable)
			stack.Push(t);
		t = GetLeft(tile);
		if (t != null && !t.FloodFilled && tile.Collidable == t.Collidable)
			stack.Push(t);
		t = GetRight(tile);
		if (t != null && !t.FloodFilled && tile.Collidable == t.Collidable)
			stack.Push(t);
	}

}

public class NoiseMaps
{
    public HeightMap heightMap;
    public HeatMap heatMap;
    public MoistureMap moistureMap;
    public Tile[,] tiles;

    public NoiseMaps(HeightMap heightMap, HeatMap heatMap, MoistureMap moistureMap, Tile[,] tiles)
    {
        this.heightMap = heightMap;
        this.heatMap = heatMap;
        this.moistureMap = moistureMap;
        this.tiles = tiles;
    }
}

public class NoiseMap
{
	public float[,] values;
	public float minValue;
	public float maxValue;
	public float avgValue;
}
public class HeightMap : NoiseMap
{
    public HeightMap(float[,] values, float minValue, float maxValue)
    {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
		this.avgValue = minValue + maxValue / 2;
    }
}

public class MoistureMap : NoiseMap
{


    public MoistureMap(float[,] values, float minValue, float maxValue)
    {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
		this.avgValue = minValue + maxValue / 2;
	}
}

public class HeatMap : NoiseMap
{

    public HeatMap(float[,] values, float minValue, float maxValue)
    {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
		this.avgValue = minValue + maxValue / 2;
	}
}