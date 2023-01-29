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
        StartCoroutine(
            SendRequest(
            Path.Combine(
                assetPath, "myAsset")));
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

    private IEnumerator SendRequest(string url)
    {
    using (UnityWebRequest request = UnityWebRequest.Get(url))
    {
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
        // handle failure
        }
        else
        {
        try
        {
            // entire file is returned via downloadHandler
            string fileContents = request.downloadHandler.text;
            // or
            //byte[] fileContents = request.downloadHandler.data;

            // do whatever you need to do with the file contents
            isAssetRead = true;
        }
        catch (Exception x)
        {
            // handle failure
        }
        }
    }
    }
    
}
