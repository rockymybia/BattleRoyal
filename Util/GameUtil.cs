using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameUtil : MonoBehaviour {

    public static GameObject CreateObjectToParent(Object go, Transform parent)
    {
        GameObject obj = Instantiate(go) as GameObject;
        obj.SetActive(true);
        obj.transform.parent = parent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localEulerAngles = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        
        return obj;
    }

    /// <summary>대상 오브젝트를 부모로 잡아서 위치이동 처리</summary>
    /// <param name="go">대상 오브젝트</param>
    /// <param name="parent">부모 오브젝트</param>
    public static void MoveToParent(GameObject go, Transform parent)
    {
        go.transform.parent = parent;
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localScale = Vector3.one;
    }

    public static void SetWorldPosition(GameObject _Base, GameObject _Target)
    { 
        _Target.transform.position = _Base.transform.position; 
    }
    
    public static void SetWorldPosition(GameObject go, Vector3 _wpos)
    {
        go.transform.position = _wpos;
    }

    /// <summary>오브젝트와 자식 오브젝트까지 레이어를 교체해줍니다.</summary>
    /// <param name="trans">부모 트랜스폼</param>
    /// <param name="name">레이어 이름</param>
    public static void ChangeLayersRecursively(Transform trans, string name)
    {
        trans.gameObject.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in trans)
        {
            ChangeLayersRecursively(child, name);
        }
    }

    /// <summary>루트에 게임 오브젝트 생성하기</summary>
    /// <param name="go"></param>
    /// <param name="_localpos">포지션</param>
    /// <param name="_localrotation">로테이션</param>    
    public static GameObject CreateObjectToRoot(GameObject go, Vector3 _localpos, Vector3 _localrotation, Vector3 _localscale)
    {
        GameObject obj = Instantiate(go) as GameObject;
        obj.SetActive(true);
        //obj.transform.parent = parent;
        obj.transform.localPosition = _localpos;
        obj.transform.localEulerAngles = _localrotation;
        obj.transform.localScale = _localscale;

        return obj;
    }

    /// <summary>게임 오브젝트 널체크를 진행한 후에 한 파괴처리</summary>
    /// <param name="destroyTarget">대상</param>    
    public static bool DestroyWithNullCheck(GameObject destroyTarget)
    {
        if (destroyTarget == null)
        {
            Debug.LogError("해당 오브젝트가 존재하지 않아 Destroy실패");
            return false;
        }

        Destroy(destroyTarget);
        return true;
    }

    public static List<T> Clone<T>(List<T> oldList)
    {
        var newList = new List<T>(oldList.Capacity);
        newList.AddRange(oldList);
        return newList;
    }

}
