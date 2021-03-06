using System;
using System.Collections;
using System.Collections.Generic;
using NetworkManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISplashScreen : UIPanel
{
	public UILoginPanel loginPanel;
	public DateTime helloStart;

	private void Start()
	{
		if(!SpicyNetwork.Connect("spicy.luminousvector.com", 12344))
		{
			Debug.LogWarning("Unable to connect to server 1, trying server 2");
			if (!SpicyNetwork.Connect("spicy2.luminousvector.com", 9614))
			{
				Debug.LogWarning("Unable to Connect to Server 2! Switching to Offline mode");
				GameMaster.Offline = true;
			}
		}
		Debug.Log("Saying Hello");
		helloStart = DateTime.Now;
		if(SpicyNetwork.SayHello())
			Hello();
		else
			Debug.LogWarning("No Hello Response, Switching to offline mode.");
	}

	private void Hello()
	{
		var time = DateTime.Now - helloStart;
		GameMaster.Offline = false;
		Debug.Log($"Server Hello in {time.TotalMilliseconds}ms");
	}

	public void StartGame()
	{
		if(GameMaster.Offline)
		{
			SceneManager.LoadScene("main");
			return;
		}
		Hide();
		loginPanel.Show();
	}
}
