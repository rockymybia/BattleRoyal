using UnityEngine;
using System.Collections;

public class UIBaseScript : MonoBehaviour {

    [SerializeField]
    public UI_UseType _UseType;

    [SerializeField]
    public UIType _UIType;

    public virtual void AddToUIManager(UIBaseScript _addui)
    {
        UIManagerScript.Instance.AddUI(this);
    }

    void FindUseType(UIBaseScript _UI)
    {
        if (_UI is TopAssetUIManagerScript ||
            _UI is LobbyUIManagerScript ||
            _UI is GameStartManagerScript ||
            _UI is BottomUIManagerScript)
        {
            _UseType = UI_UseType.OutGame;
        }
        else
            _UseType = UI_UseType.InGame;
    }

}
