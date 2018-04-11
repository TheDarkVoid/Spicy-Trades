using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class UISettlementPanel : UIPanel
{
	public TextMeshProUGUI titleText;
	public RectTransform contentBase;
	public GameObject resourceItemPanel;
	public TextMeshProUGUI infoText;
	public Button buyButton;
	public TMP_InputField buyCountInput;

	private ResourceTileInfo _selected;

	public void Show(SettlementTile settlement)
	{
		Show();
		if(GameMaster.CameraPan != null)
			GameMaster.CameraPan.isPaused = true;
		titleText.text = settlement.Name;
		infoText.text = "Select an Item";
		var rCache = settlement.ResourceCache;
		if (rCache == null)
			return;
		int i = 0;
		if(_selected != null)
		{
			DisplayInfo(_selected, rCache[_selected], settlement);
		}
		foreach (var res in rCache.Keys)
		{
			var li = Instantiate(resourceItemPanel, contentBase).GetComponent<UIResourceListItem>();
			li.nameText.text = res.PrettyName;
			li.priceText.text = new Coin((res.basePrice * rCache[res][1])).ToString();
			li.iconImage.sprite = res.icon;
			var rt = li.GetComponent<RectTransform>();
			rt.anchoredPosition = new Vector2(0, i++ * -100);
			var btn = li.GetComponent<Button>();
			var btnClick = new Button.ButtonClickedEvent();
			btnClick.AddListener(() =>
			{
				DisplayInfo(res, rCache[res], settlement);
				_selected = res;
			});
			btn.onClick = btnClick;
		}
		contentBase.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rCache.Count * 50);

	}

	private void DisplayInfo(ResourceTileInfo res, float[] values, SettlementTile settlement)
	{
		buyButton.interactable = true;
		var sb = new StringBuilder();
		sb.AppendLine(res.PrettyName);
		sb.AppendLine("Base: " + new Coin(res.basePrice));
		sb.AppendLine("Cost: " + new Coin((res.basePrice * values[1])).ToString(" "));
		sb.AppendLine("Value: " + (values[1] * 100) + "%");
		sb.AppendLine("Supply: " + values[0]);
		infoText.text = sb.ToString();
		buyButton.onClick = new Button.ButtonClickedEvent();
		buyButton.onClick.AddListener(() =>
		{
			Debug.Log("Buy " + res.name);
			settlement.Buy(res, int.Parse(buyCountInput.text));
#if DEBUG
			GameMaster.Player.LogItems();
#endif
			DisplayInfo(res, values, settlement);
		});
	}

	public override void Hide()
	{
		if(GameMaster.CameraPan != null)
			GameMaster.CameraPan.isPaused =	false;
		base.Hide();
		buyButton.interactable = false;
		DestroyChildren(contentBase);
	}
}
