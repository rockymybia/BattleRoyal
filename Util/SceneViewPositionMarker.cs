using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneViewPositionMarker : MonoBehaviour {

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        Handles.Label(transform.position, transform.name);
    }
#endif
                                                     }
