using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class initialLoad : MonoBehaviour
{

    private bool isAssetRead;


    // Start is called before the first frame update
    void Start ()
    {
    // if webGL, this will be something like "http://..."
    string assetPath = Application.streamingAssetsPath;
    Debug.Log("Asset path: " + assetPath);

    bool isWebGl = assetPath.Contains("://") || 
                    assetPath.Contains(":///");

    try
    {
        if (isWebGl)
        {
        StartCoroutine(DownLoadAsset(StreamingAssets));
        }
        else{
            isAssetRead = true;
        }
    }
    catch
    {
        // handle failure
    }
    }

    // Update is called once per frame
    void Update ()
    {
    // check to see if asset has been successfully read yet
    if (isAssetRead)
    {
        // once asset is successfully read, 
        // load the next screen (e.g. main menu or gameplay)
        SceneManager.LoadScene("Menu");
    }

    // need to consider what happens if 
    // asset fails to be read for some reason
    }

    IEnumerator DownLoadAsset(assetName)
    {
        UnityWebRequest unityWebRequest = UnityWebRequestAssetBundle.GetAssetBundle(Application.streamingAssetsPath);
        yield return unityWebRequest.Send();
        Debug.Log("responseCode:" + unityWebRequest.responseCode);
        if (unityWebRequest.isDone)
        {
            ab = (unityWebRequest.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
            Instantiate(ab.LoadAsset(assetName) as GameObject, Vector3.zero, new Quaternion(0, 0, 0, 0));
        }
    }
    
}
