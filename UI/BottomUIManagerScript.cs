using UnityEngine;
using System.Collections;

/// <summary>UI - 하단 매뉴</summary>
public class BottomUIManagerScript : UIBaseScript
{
    public GameObject[] OnclickButton;

	void Start () {
        AddToUIManager(this);

        UIEventListener.Get(OnclickButton[0]).onClick += Onclick_Content1;
        UIEventListener.Get(OnclickButton[1]).onClick += Onclick_Content2;
        UIEventListener.Get(OnclickButton[2]).onClick += Onclick_Content3;
        UIEventListener.Get(OnclickButton[3]).onClick += Onclick_Content4;
        UIEventListener.Get(OnclickButton[4]).onClick += Onclick_Content5;
	}
	

    void Onclick_Content1(GameObject go){ UIManagerScript.Instance.UI_Lobby.MoveToContent(0); }
    void Onclick_Content2(GameObject go) { UIManagerScript.Instance.UI_Lobby.MoveToContent(1); }
    void Onclick_Content3(GameObject go) { UIManagerScript.Instance.UI_Lobby.MoveToContent(2); }
    void Onclick_Content4(GameObject go) { UIManagerScript.Instance.UI_Lobby.MoveToContent(3); }
    void Onclick_Content5(GameObject go) { UIManagerScript.Instance.UI_Lobby.MoveToContent(4); }
}
