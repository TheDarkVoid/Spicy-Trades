using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SettlementEvent
{
	public string name;
	public string description;
	[Range(0, 100)]
	public int Chance;

	public List<ResourceIdentifier> resourceDemands = new List<ResourceIdentifier>();
	public int duration;
	public int cooldown;
	public SettlementType location;

	public EventCompletion completionEffect;

}
