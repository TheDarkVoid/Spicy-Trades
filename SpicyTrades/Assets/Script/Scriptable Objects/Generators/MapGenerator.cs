﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapGenerator : ScriptableObject
{
	public Vector2 Size = new Vector2(20, 20);
	public TileMapper tileMapper;
	public float OuterRadius = 0.577f;
	public FeatureGenerator[] featureGenerators;
	[HideInInspector]
	public bool Regen;

	public float InnerRadius
	{
		get
		{
			return OuterRadius * Mathf.Sqrt(3) / 2;
		}
	}
	public abstract Tile Generate(int x, int y, Transform parent = null);

	public Vector3 GetPosition(int x, int y)
	{
		return new Vector3
		{
			y = y * (InnerRadius * 1.5f),
			x = (x + y * .5f - y / 2) * (InnerRadius * 2f),
		};
	}
	public Tile CreateTile(int x, int y, Transform parent)
	{
		return CreateTile(tileMapper.GetTile(0), x, y, parent);
	}

	public Tile CreateTile(TileInfo tileInfo , int x, int y, Transform parent)
	{
		var tile = new Tile(tileInfo, parent, HexCoords.FromOffsetCoords(x,y), OuterRadius);
		return tile;
		//var g = Instantiate(t, GetPosition(x, y), Quaternion.identity, parent);
		//return g.GetComponent<Tile>().SetPos(x, y);
	}

	public void GenerateFeatures(Map map)
	{
		if (featureGenerators == null)
			return;
		foreach (var fg in featureGenerators)
		{
			if (fg != null)
			{
				Debug.Log("Running Feature Generator: " + fg.GetType().Name);
				fg.Generate(map);
			}
		}
	}

	public virtual Map GenerateMap(Transform parent = null)
	{
		Map map = new Map((int)Size.y, (int)Size.x);
		for (int y = 0, i = 0; y < map.Height; y++)
		{
			for (int x = 0; x < map.Width; x++)
			{
				map[i++] = Generate(x, y, parent);
			}
		}
		return map;
	}
}
