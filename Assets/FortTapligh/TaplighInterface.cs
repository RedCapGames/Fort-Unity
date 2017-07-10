using UnityEngine;
// using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

public enum ShowAdResult
{
    NO_INTERNET_ACSSES,
    BAD_TOKEN_USED,
    NO_AD_AVAILABLE,
    NO_AD_READY,
    INTERNAL_ERROR,
    AD_AVAILABLE,
    AD_VIEWED_COMPLETELY,
    AD_CLICKED,
    AD_IMAGE_CLOSED,
    AD_VIDEO_CLOSED_AFTER_FULL_VIEW,
    AD_VIDEO_CLOSED_ON_VIEW
}

public enum TokenResult
{
    TOKEN_NOT_FOUND,
    TOKEN_EXPIRED,
    NOT_USED,
    SUCCESS,
    INTERNAL_ERROR
}

class TaplighInterface : MonoBehaviour
{

#if !UNITY_EDITOR && UNITY_ANDROID
    AndroidJavaClass _taplighJavaInterface;
    AndroidJavaObject _currentActivity;
#endif

    static private TaplighInterface instance;
    static private string TaplighStr = "com.tapligh.unitysdk.TaplighUnity";

    static public int AdTypeImage = 1;
    static public int AdTypeVideo = 2;

    /*
    static private Action<int, string> _onCheckPrizeListener = null;
    public System.Action<int, string> OnCheckPrizeListener
    {
        get { return _onCheckPrizeListener; }
        set { _onCheckPrizeListener = value; }
    }
    */

    static private Action<ShowAdResult , string> _onShowAdListener = null;
    public System.Action<ShowAdResult ,string> OnShowAdListener
    {
        get { return _onShowAdListener; }
        set { _onShowAdListener = value; }
    }

    static private Action<ShowAdResult,string> _onShowInterstitialListener = null;
    public System.Action<ShowAdResult,string> OnShowInterstitialListener
    {
        get { return _onShowInterstitialListener; }
        set { _onShowInterstitialListener = value; }
    }


    static private Action<bool , ShowAdResult , string> _onIsAdAvailableListener = null;
    public System.Action<bool, ShowAdResult , string > OnIsAdAvailableListener
    {
        get { return _onIsAdAvailableListener; }
        set { _onIsAdAvailableListener = value; }
    }

    static private Action<TokenResult> _onTokenVerifyFinishedListener = null;
    public System.Action<TokenResult> OnTokenVerifyFinishedListener
    {
        get { return _onTokenVerifyFinishedListener; }
        set { _onTokenVerifyFinishedListener = value; }
    }

    static public TaplighInterface Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("TaplighUnityObject");
                obj.AddComponent<TaplighInterface>();
                instance = obj.GetComponent<TaplighInterface>();
                // instance.InitializeTapligh();
            }
            return instance;
        }
    }


    void Awake()
    {
        Debug.Log("ADDWATCH CREATED - CURRENT AVTIVITY IS DONE");
        DontDestroyOnLoad(this.gameObject);
    }

    public void InitializeTapligh(string token)
    {
        Debug.Log("Object Created and JAva calsses are iniotiated " + TaplighStr);

#if !UNITY_EDITOR && UNITY_ANDROID
      //  AndroidJNI.AttachCurrentThread();

        _taplighJavaInterface = new AndroidJavaClass(TaplighStr);
        _taplighJavaInterface.CallStatic("initialize" , token );
        Debug.Log("INITIALIZEING IS DONE") ; 
#endif
        Debug.Log("END OF SET JAVA OBJECT");
    }

    public void ShowImageAd( bool skipable = true )
    {

#if !UNITY_EDITOR && UNITY_ANDROID

        if(_taplighJavaInterface != null)
                       _taplighJavaInterface.CallStatic( "showAd", this.gameObject.name,
                                             "OnShowAdFinishedJavaListener", 
                                              AdTypeImage , 
                                              skipable   );
        else 
            Debug.Log(" OBJECT IS NULL "); 
#endif
        Debug.Log("Show IMage Add");
    }

    public void ShowVideoAd(bool skipable = true)
    {
#if !UNITY_EDITOR && UNITY_ANDROID

        if(_taplighJavaInterface != null)
            _taplighJavaInterface.CallStatic( "showAd", this.gameObject.name,
                                             "OnShowAdFinishedJavaListener", 
                                              AdTypeVideo  , 
                                              skipable  );
        else 
            Debug.Log(" OBJECT IS NULL "); 
#endif
        Debug.Log("Show Video Add");

    }

    private void OnShowAdFinishedJavaListener(string adResponse)
    {
        List<string> results = GetResultArguments(adResponse); 
        ShowAdResult response = (ShowAdResult)(Int32.Parse(results[0]));
        if (_onShowAdListener != null)
            _onShowAdListener(response , results[1]); 
         
    }

    public bool ShowInterstitialAd(int adType = 1, bool skipable = true)
    {
        bool result = false;

        Debug.Log("SHOW INTER STATIAL. ");

#if !UNITY_EDITOR && UNITY_ANDROID
        if(_taplighJavaInterface != null ) 
           result =  _taplighJavaInterface.CallStatic<bool>(   "showInterstitialAd", this.gameObject.name,
                                                "OnShowInterstitialAdFinishedJavaListener", 
                                                adType , 
                                                skipable
                                           );
        else 
            Debug.Log(" OBJECT IS NULL ");
#endif

        Debug.Log(" INterstattiol retiuned : " + ( (result) ? "true" : "false " ) ); 
        return result; 
    }

    public void IsAdAvailable(int adType = 1)
    {
        Debug.Log("IS AD AVAILABLE. ");

#if !UNITY_EDITOR && UNITY_ANDROID
        if(_taplighJavaInterface != null ) 
             _taplighJavaInterface.CallStatic(   "isAdAvailable", this.gameObject.name,
                                                "OnIsAdAvailableJavaListener", 
                                                adType
                                                    );
        else 
            Debug.Log(" OBJECT IS NULL ");
#endif
    }


    private void OnShowInterstitialAdFinishedJavaListener(string adResponse)
    {
        List<string> results = GetResultArguments(adResponse);
        ShowAdResult response = (ShowAdResult)(Int32.Parse(results[0]));
        if (_onShowInterstitialListener != null)
            _onShowInterstitialListener(response , results[1]);
    }

    private void OnIsAdAvailableJavaListener(string adResponse)
    {
        List<string> results = GetResultArguments(adResponse);
        ShowAdResult response = (ShowAdResult)(Int32.Parse(results[0]));
        bool result = ( response == ShowAdResult.AD_AVAILABLE ) ? true : false; 

        if (_onIsAdAvailableListener != null)
            _onIsAdAvailableListener(result , response , results[1]);
    }


    public void VerifyToken(string token)
    {

        Debug.Log("VERIFY TOKEN ");

#if !UNITY_EDITOR && UNITY_ANDROID
        if(_taplighJavaInterface != null ) 
           _taplighJavaInterface.CallStatic(    "verifyToken", 
                                                this.gameObject.name,
                                                "OnTokenVerifyJavaListener", 
                                                token
                                           );
        else 
            Debug.Log(" OBJECT IS NULL ");
#endif
    }

    private void OnTokenVerifyJavaListener(string adResponse)
    {
        TokenResult response = (TokenResult)(Int32.Parse(adResponse));

        if ( _onTokenVerifyFinishedListener != null)
            _onTokenVerifyFinishedListener(response);
    }

    /*
    public void CheckPrize()
    {

#if !UNITY_EDITOR && UNITY_ANDROID
        if(_taplighJavaInterface != null)
            _taplighJavaInterface.CallStatic( "checkPrize" , this.gameObject.name,
                                             "OnCheckPrizeFinishedJavaListener"  );
        else 
            Debug.Log(" OBJECT IS NULL "); 
#endif
    }
    

    private void OnCheckPrizeFinishedJavaListener(string result)
    {
        int t = result.IndexOf(",");

        string serverResponseStr = result.Substring(0, t);
        string prize = result.Substring(t + 1, result.Length - t - 1);

        int serverResponse = Int32.Parse(serverResponseStr);

        Debug.Log( " The returnes Server response is  : " + serverResponse + "   . The prize is : " + prize ) ; 

        if (_onCheckPrizeListener != null)
        {
            _onCheckPrizeListener(serverResponse, prize);
        }

    }
    */


    public void SetAdShowInterval(int interval)
    {
#if !UNITY_EDITOR && UNITY_ANDROID

        if(_taplighJavaInterface != null)
            _taplighJavaInterface.CallStatic("setAdShowInterval", interval  );
        else
            Debug.Log(" OBJECT IS NULL "); 
#endif
        Debug.Log("SetAdShowInterval");
    }


    public void SetTestEnable(bool isTestEnable)
    {
#if !UNITY_EDITOR && UNITY_ANDROID

        if(_taplighJavaInterface != null)
            _taplighJavaInterface.CallStatic("setTestEnable", isTestEnable  );
        else
            Debug.Log(" OBJECT IS NULL "); 
#endif
        Debug.Log("SetTestEnable");
    }

    private List<string> GetResultArguments( string result )
    {
        List<string> arguments = new List<string>();

        int deviderIndex = result.IndexOf(';');
        arguments.Add( result.Substring( 0 , deviderIndex ) );

        for ( ; deviderIndex < result.Length ; deviderIndex++)
        {
            if (result[deviderIndex] != ';')
                break; 
        }

        arguments.Add( result.Substring( deviderIndex)); 

        return arguments; 
    }

}