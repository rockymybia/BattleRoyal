using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;

public enum eResourceLoadType
{
    UnityEditor,
    UnityAndroid,
    UnityIOS,
}

public enum eAssetType
{
    Unit,
    Structure,
    Texture,
    Sound,
    Effect,
    UIAtlas,
    Map,
    UI
}

public class AssetLoadScript : Singleton<AssetLoadScript>
{    
    public const string AssetBundlesOutputPath = "/AssetBundles/";

    public eResourceLoadType ResourceLoadType = eResourceLoadType.UnityEditor;
    //public string AssetRootPath = string.Empty;

    public bool IsFullVersionBuild = false;
    static private Dictionary<string, Object> _cache = new Dictionary<string, Object>();

    IEnumerator Start()
    {        
        IsFullVersionBuild = true;

        if (IsFullVersionBuild)
        {
            yield break;
        }
        else
        {
            yield return StartCoroutine(Initialize());
        }

        //GameObject go = new GameObject();
        //StartCoroutine(InstantiateGameObjectAsync("structure.unity3d", "DefenseTower_1", go));
        //LoadAsset(eAssetType.Structure, "DefenseTower_1", go);
        // Load asset.
        //yield return StartCoroutine(InstantiateGameObjectAsync (assetBundleName, assetName) );        
    }
    
    protected IEnumerator Initialize()
    {
        // Don't destroy this gameObject as we depend on it to run the loading script.
        //DontDestroyOnLoad(gameObject);

        // With this code, when in-editor or using a development builds: Always use the AssetBundle Server
        // (This is very dependent on the production workflow of the project. 
        // 	Another approach would be to make this configurable in the standalone player.)
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        AssetBundleManager.SetDevelopmentAssetBundleServer();
#else
		// Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
		AssetBundleManager.SetSourceAssetBundleURL(Application.dataPath + "/");
		// Or customize the URL based on your deployment or configuration
		//AssetBundleManager.SetSourceAssetBundleURL("http://www.MyWebsite/MyAssetBundles");
#endif

        // Initialize AssetBundleManifest which loads the AssetBundleManifest object.
        var request = AssetBundleManager.Initialize();
        if (request != null)
            yield return StartCoroutine(request);
    }


    public IEnumerator InstantiateGameObjectAsync(eAssetType _assetType, string assetName, System.Action<GameObject> callback)
    {
        
        // This is simply to get the elapsed time for this phase of AssetLoading. // 어셋 로드 시작 시각을 세팅        
        float startTime = Time.realtimeSinceStartup;

        Object prefab;

        if (ResourceLoadType == eResourceLoadType.UnityEditor)
        {
            #region 리소스 폴더 로드 방식
            prefab = Get(_assetType, assetName);
            #endregion
        }
        else
        {
            #region 어셋번들 로드 방식
            string assetBundleName = _assetType.ToString().ToLower();
            // Load asset from assetBundle. //어셋을 비동기로 로드함
            AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject));
            if (request == null)
                yield break;
            yield return StartCoroutine(request);

            // Get the asset. //비동기 로드가 끝난후 어셋을 게임오브젝트 타입으로 로드함
            prefab = request.GetAsset<GameObject>();
            #endregion
        }
        
        GameObject go;
        if (prefab != null)
        {
            go = (GameObject)Instantiate(prefab); //생성
            callback(go);
        }           

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (prefab == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
        
    }


    //public IEnumerator LoadAsset(eAssetType _Type, string _AssetName)
    //{
    //    GameObject go;
    //    yield return StartCoroutine(InstantiateGameObjectAsync(_Type.ToString().ToLower(), _AssetName, _go => go =_go ));

    //}


    #region 리소스폴더 로드 방식
    public string GetResourcesLoadPath(eAssetType _Type, string _AssetName)
    {
        switch (_Type)
        {
            case eAssetType.Unit:
                return string.Format("Prefab/Unit/{0}", _AssetName);
            case eAssetType.Structure:
                return string.Format("Prefab/Structure/{0}", _AssetName);
            case eAssetType.Texture:
                return string.Format("Texture/{0}", _AssetName);
            case eAssetType.Sound:
                return string.Format("Prefab/Structure/{0}", _AssetName);
            case eAssetType.Effect:
                return string.Format("Effect/{0}", _AssetName);
            case eAssetType.UIAtlas:
                return string.Format("Prefab/Structure/{0}", _AssetName);
            case eAssetType.Map:
                return string.Format("Prefab/Map/{0}", _AssetName);
            case eAssetType.UI:
                return string.Format("Prefab/UI/{0}", _AssetName);
            default:
                return string.Format("Prefab/Structure/{0}", _AssetName);
        }        
    }


    public void Load(eAssetType _Type, string _AssetName)
    {
        string _Path = GetResourcesLoadPath(_Type, _AssetName);        
        Object t1 = Resources.Load(_Path);
        if (t1 != null)
        {
            _cache[t1.name] = t1;    
        }
        else
        {
            Debug.LogError(_AssetName + " 리소스를 찾지 못했습니다.");
        }
        
    }



    public Object Get(eAssetType _Type, string _AssetName)
    {        
        if (_cache.ContainsKey(_AssetName) == false)
        {
            Load(_Type, _AssetName);
        }
        return _cache[_AssetName];
    }

    public GameObject CreateAssetFrom_ResourcesFolder(eAssetType _Type, string _AssetName)
    {
        GameObject _prefab = (GameObject)Get(_Type, _AssetName);

        if (_prefab == null)
            return null;

        return GameUtil.CreateObjectToParent(_prefab, null);
    }
    #endregion

    public void Remove(params string[] arg)
    {
        for (int i = 0; i < arg.Length; i++)
        {
            string key = arg[i];
            _cache.Remove(key);
        }
    }

    //public void LoadAll(string folderPath)
    //{
    //    object[] t0 = Resources.LoadAll(folderPath);
    //    for (int i = 0; i < t0.Length; i++)
    //    {
    //        GameObject t1 = (GameObject)(t0[i]);
    //        t1.SetActive(false);
    //        _cache[t1.name] = t1;
    //    }
    //}


    //public GameObject Instance(eAssetType _Type, string _AssetName)
    //{
    //    if (_cache.ContainsKey(_AssetName) == false)
    //    {
    //        Load(_Type, _AssetName);
    //    }

    //    GameObject obj = (GameObject)GameObject.Instantiate(_cache[_AssetName]);
    //    obj.name = _AssetName;
    //    return obj;
    //}

    
    
}
