﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/Resource")]
public class ResourceTileInfo : TileInfo
{
	public new readonly TileType tileType = TileType.Resource;
	public ResourceCategory category;
	public int requiredWorkers;
	public float basePrice;
	public float yeild;

	public string PrettyName
	{
		get
		{
			return "<color=#" + ColorUtility.ToHtmlStringRGB(color) + name + "/<color>";
		}
	}

}

public enum ResourceCategory
{
	Material,
	Food,
	Luxury,
	Fuel,
	Stategic
}
