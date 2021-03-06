using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using System;

public class Transaction
{
	public Guid iD;
	public TransactionType type;
	public string playerId;
	public string targetPlayerId;
	public HexCoords targetSettlement;
	public ResourceIdentifier resources;

	public Transaction()
	{
		iD = Guid.NewGuid();
	}


	public void Execute()
	{
		SettlementTile settlement = GameMaster.GameMap[targetSettlement.ToIndex()] as SettlementTile;
		ResourceTileInfo res;
		Player player = GameMaster.Players.First(p => p.Id == playerId);
		switch(type)
		{
			case TransactionType.Buy:
				res = settlement.ResourceCache.Keys.First(r => resources.Match(r));
				settlement.Buy(res, resources.count, player, false);
				break;
			case TransactionType.Sell:
				res = settlement.ResourceCache.Keys.First(r => resources.Match(r));
				player.Sell(res, resources.count, settlement, false);
				break;
			case TransactionType.Move:
				player.MoveTo(settlement, false);
				break;
		}
	}

	public string ToJSON() => JsonConvert.SerializeObject(this);

	public static Transaction FromJSON(string json) => JsonConvert.DeserializeObject<Transaction>(json);
}

public enum TransactionType
{
	Buy,
	Sell,
	Move,
	Trade,
}
