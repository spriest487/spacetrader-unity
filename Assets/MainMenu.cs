using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	const int MENU_WIDTH = 300;
	const int MENU_HEIGHT = 500;

	private delegate void MenuAction();

	private readonly IDictionary<string, MenuAction> buttons;

	public void OnGUI()
	{
		var left = Screen.width / 2 - MENU_WIDTH / 2;
		var top = Screen.height / 2 - MENU_HEIGHT / 2;

		GUI.Box(new Rect(left, top, MENU_WIDTH, MENU_HEIGHT), left.ToString());
	}
}
