using System;
using System.Collections;
using System.Collections.Generic;
using NetworkManager;
using Newtonsoft.Json;
using UnityEngine;

public class GameMaster
{
	public const float TickRate = 1f/100f;
	public static bool Offline { get; set; } = true;
	public static event Action GameReady
	{
		add
		{
			Instance._gameReady += value;
		}
		remove
		{
			Instance._gameReady -= value;
		}
	}
	public static GameRegistry Registry
	{
		get
		{
			return Instance._registry;
		}
		set
		{
			Instance._registry = value;
		}
}

	public static GameMaster Instance
	{
		get
		{
			return _instance ?? (_instance = new GameMaster());
		}
	}
	public static Map GameMap
	{
		get
		{
			return Instance._gameMap;
		}
		set
		{
			Instance._gameMap = value;
		}
	}
	public static Player Player
	{
		get
		{
			return GameMap.CurrentPlayer;
		}
	}

	public static int CurrentTick { get; set; }

	public static Dictionary<SettlementTile, TradeKnowledge> PriceKnowledge
	{
		get
		{
			return Instance._tradeKnowledge;
		}
	}

	public static CameraPan CameraPan { get; set; }

	public static List<Player> Players
	{
		get
		{
			return GameMap.Players;
		}
	}

	public static MapGenerator Generator { get; set; }
	private static GameMaster _instance;
	private Dictionary<SettlementTile, TradeKnowledge> _tradeKnowledge;
	private event Action _gameReady;
	private Map _gameMap;
	private GameRegistry _registry;

	private List<Guid> _processedTransactrions = new List<Guid>();
	
	public static void CachePrices(SettlementTile settlement)
	{
		if (Instance._tradeKnowledge == null)
			Instance._tradeKnowledge = new Dictionary<SettlementTile, TradeKnowledge>();
		var tradeKnowledge = Instance._tradeKnowledge;

		var resourceCache = settlement.ResourceCache;
		if (resourceCache == null)
			return;
		if(tradeKnowledge.ContainsKey(settlement))
		{
			AddToCache(tradeKnowledge[settlement], resourceCache);

		}else
		{
			var valueCache = new TradeKnowledge();
			tradeKnowledge.Add(settlement, valueCache);
			AddToCache(valueCache, resourceCache);
		}
	}

	private static void AddToCache(TradeKnowledge knowledgeCache, Dictionary<ResourceTileInfo, float[]> resourceCache)
	{
		knowledgeCache.AquisitionTick = CurrentTick;
		foreach (var res in resourceCache.Keys)
		{
			if (knowledgeCache.Cache.ContainsKey(res))
				knowledgeCache.Cache[res] = resourceCache[res][1];
			else
				knowledgeCache.Cache.Add(res, resourceCache[res][1]);
		}
	}

	public static void SendTransaction(Transaction transaction)
	{
		if(!Offline)
		{
			Debug.Log("Send");
			SpicyNetwork.Sync(transaction.ToJSON());
		}
	}

	public static void OnTransactionRecieve(Transaction transaction)
	{
		if(transaction.playerId != Player.Id)
		{
			if(Instance._processedTransactrions.Contains(transaction.iD))
			{
				Instance._processedTransactrions.Add(transaction.iD);
				transaction.Execute();
			}
		}
	}

	public static Tile GetTile(int x, int y, int z)
	{
		return GameMap[x, y, z];
	}

	public static void TouchTile(Tile tile)
	{
		if (tile.GetType() == typeof(SettlementTile))
		{
			UIManager.ShowPricePanel((tile as SettlementTile).Center);
		}
#if DEBUG
		var n = tile.GetNeighbors();
		foreach (Tile t in n)
			if (t != null)
				Debug.DrawLine(tile.WolrdPos, t.WolrdPos, Color.black, 3);
#endif
	}

	public static void Ready()
	{
		Instance._gameReady.Invoke();
		SpicyNetwork.SyncData += (s, a) =>
		{
			OnTransactionRecieve(Transaction.FromJSON(a.SData));
			Debug.Log($"Data: {a.SData}");
		};
	}
}
