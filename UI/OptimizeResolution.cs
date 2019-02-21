using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class OptimizeResolution : MonoBehaviour {

    public float target_width = 1280f;
    public float target_height = 720f;

    float TargetAspect;
    float RelativeAspect;

	// Use this for initialization
	void Start () {                
        Optimize();
	}
	
	//해상도 최적화 체크 및 카메라 사이즈 조정    
    //기본 16:9 비율로 비교함 1280*720  (갤럭시S2 HD, 갤럭시S3 , 갤럭시노트2)    
    public void Optimize()
    {        
        TargetAspect = (float)target_width / target_height;
        RelativeAspect = GetComponent<Camera>().aspect;
        GetComponent<Camera>().backgroundColor = new Color(0.0f, 0.0f, 0.0f);
        
        
        //if (TargetAspect != RelativeAspect)
    //    {
    //        camera.orthographicSize = TargetAspect / RelativeAspect;
    //        camera.backgroundColor = new Color(255.0f, 255.0f, 255.0f);
    //    }

        if (GetComponent<Camera>().orthographic)
        {
            if (TargetAspect != RelativeAspect)
            {
                GetComponent<Camera>().orthographicSize = TargetAspect / RelativeAspect;
                GetComponent<Camera>().backgroundColor = new Color(255.0f, 255.0f, 255.0f);
            }
        }
        else {
            GetComponent<Camera>().fieldOfView *= TargetAspect / RelativeAspect;
        }
    }

    [ContextMenu("Execute")]
    public void DirectOptimize()
    {
        TargetAspect = (float)target_width / target_height;
        RelativeAspect = GetComponent<Camera>().aspect;

        Debug.Log("TargetAspect : " + TargetAspect + " / " + RelativeAspect);

        //camera.orthographicSize = TargetAspect / RelativeAspect;
        //camera.backgroundColor = new Color(255.0f, 255.0f, 255.0f);
        //Camera.main.fieldOfView *= TargetAspect;


        if (GetComponent<Camera>().orthographic)
        {
            if (TargetAspect != RelativeAspect)
            {
                GetComponent<Camera>().orthographicSize = TargetAspect / RelativeAspect;
                GetComponent<Camera>().backgroundColor = new Color(255.0f, 255.0f, 255.0f);
            }
        }
        else
        {
            Camera.main.fieldOfView *= TargetAspect;
        }
        
    }

}
