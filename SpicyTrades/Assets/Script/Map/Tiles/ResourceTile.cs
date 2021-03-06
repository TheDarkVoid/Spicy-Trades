﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTile : Tile
{
	public new ResourceTileInfo tileInfo;
	public ResourceTile(ResourceTileInfo tileInfo, Transform parent, HexCoords hexCoords, float outerRadius) : base(tileInfo, parent, hexCoords, outerRadius)
	{
		this.tileInfo = tileInfo;
	}
}
