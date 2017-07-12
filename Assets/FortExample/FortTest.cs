using System;
using System.Collections.Generic;
using System.IO;
using Fort;
using Fort.Info;
using Fort.Info.GameItem;
using Fort.Info.GameLevel;
using Fort.Serializer;
using Fort.ServerConnection;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class FortTest : MonoBehaviour
{
    //private WeakReference _reference;

    // Use this for initialization
    void Start()
    {
        /*	    ServiceLocator.Resolve<ISettingService>().ResolveServerSettings<TestGameServerSetting>().Then(settings =>
                {
                });*/
        /*        ServiceLocator.Resolve<IStoreService>().ResolvePackages().Then(packages =>
                {

                });*/
        /*        using (FileStream stream = File.Create(@"d:\1.bin"))
                {
                    Serializer serializer = new Serializer();
                    Test test = new Test
                    {
                        List = new List<string> { "Arash", "Mostafa" },
                        TokenType = SerializationTokenType.List,
                        Strings = new[] { "Arash" },
                        Dictionary = new Dictionary<int, string> { { 12, "Arash" } },
                        Type = typeof(Test),
                        Value = null
                    };
                    test.Test1 = test;
                    test.Ghaz = new Ghaz();
                    test.Ghaz.Tests = new[] {test};
                    serializer.Serialize(stream, test);
                }
                using (FileStream stream = File.OpenRead(@"d:\1.bin"))
                {
                    Serializer serializer = new Serializer();
                    object deserialize = serializer.Deserialize(stream);
                    Test test = (Test)deserialize;
                    if (ReferenceEquals(test, test.Ghaz.Tests[0]))
                    {
                        object data = test.Value;
                        Debug.Log(data);
                    }
                }*/
        //gameObject.AddComponent<TestLibrary.Class1>();
        /*        int compareTo = 1.CompareTo(2);
                Test test = new Test
                {
                    List = new List<string> { "Arash", "Mostafa" },
                    TokenType = SerializationTokenType.List,
                    Strings = new[] { "Arash" },
                    Dictionary = new Dictionary<int, string> { { 12, "Arash" } },
                    Type = typeof(Test),
                    Value = null
                };
                test.Test1 = test;*/
        //Test resolveData = ServiceLocator.Resolve<IStorageService>().ResolveData<Test>();
        //ServiceLocator.Resolve<IStorageService>().UpdateData(test);
        //ServiceLocator.Resolve<IStorageService>().ResolveData<Test>();
        //ServiceLocator.Resolve<IStorageService>().UpdateData(new Test());
        /*        Debug.Log(typeof(object).ContainsGenericParameters);
                Debug.Log(typeof(List<>).ContainsGenericParameters);
                Debug.Log(typeof(List<int>).ContainsGenericParameters);*/
        //BacktoryUserConnection backtoryUserConnection = new BacktoryUserConnection();
        /*        backtoryUserConnection.Register("Morad1", "Pashmak").Then(() =>
                {
                    Debug.Log("Success");
                }, status =>
                {
                    switch (status)
                    {
                        case RegisterationErrorResultStatus.CannotConnectToServer:
                            Debug.Log("Error in registeration");
                            break;
                        case RegisterationErrorResultStatus.UsernameIsInUse:
                            Debug.Log("User in use");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("status", status, null);
                    }
                });*/
        //backtoryUserConnection.Call<int>("GetUserData",null);
        /*        backtoryUserConnection.Relogin().Then(() =>
                {
                    Debug.Log("Success");
                },() => Debug.Log("Failed"));*/
        //StartCoroutine(Call());
        /*        InfoResolver.Resolve<FortInfo>()
                    .ServerConnectionProvider.EditorConnection.SendFilesToStorage(new[] {new StorageFile
                    {
                        FileName = "1.data",
                        Path = "/Test/",
                        Stream = File.OpenRead(@"E:\projects\Fort\Fort-Unity\AssetBundles\Windows\testassetbundle")
                    }},
                        f =>
                        {
                            Debug.Log(f);
                        }).Then(strings => Debug.Log(strings[0]),() => Debug.Log("Error"));*/
        /*        InfoResolver.Resolve<FortInfo>().ServerConnectionProvider.UserConnection.LoadFromStorage(new Uri("http://storage.backtory.com/FortAssetBundle/Ghaz2.txt"),
                    f =>
                    {
                        Debug.Log(f);
                    } ).Then(s => Debug.Log(s),() => Debug.LogError("Error"));*/
        /*        Balance balance = new Balance();
                balance["Coin"] = 12;
                ServiceLocator.Resolve<IUserManagementService>().AddScoreAndBalance(0, balance);*/
        //_reference = new WeakReference(new WeakAction().Action);
        /*        ServiceLocator.Resolve<ISceneLoaderService>()
                    .Load(new SceneLoadParameters(InfoResolver.Resolve<FortInfo>().GameLevel.LoaderScene.Value.SceneName));*/
        //ServiceLocator.Resolve<IUserManagementService>().AddScoreAndBalance(100,10);
        //int coin = ServiceLocator.Resolve<IUserManagementService>().Balance;
/*        TaplighInterface.Instance.SetTestEnable(true);
        ServiceLocator.Resolve<IAdvertisementService>().ShowVideo(null,false,false).Then(() =>
        {
            Debug.Log("Video succeded");
        }, failed =>
        {
            Debug.Log("Video failed");
        });*/
/*        ServiceLocator.Resolve<IStoreService>().ResolvePackages().Then(infos =>
        {
            ServiceLocator.Resolve<IStoreService>().PurchasePackage(infos[0]).Then(() => Debug.Log("success"),result => Debug.Log(result.ToString()));
        });*/
        
    }

    

    // Update is called once per frame
    void Update()
    {
/*        IStorageService storageService = ServiceLocator.Resolve<IStorageService>();
        storageService.UpdateData(new SavedData {Value = 12});
        Debug.Log(storageService.ResolveData<SavedData>().Value);*/
    }

    /*    class TestGameServerSetting : GameServerSetting
        {
            public bool Active { get; set; }
        }*/

        public class InternalTest
        {
            internal string Value { get; set; } 
        }
    [Serializable]
    public class Test
    {
        public int? Value { get; set; }
        public Test Test1 { get; set; }
        public List<string> List { get; set; }
        public Ghaz Ghaz { get; set; }
        public string[] Strings { get; set; }
        public Dictionary<int,string> Dictionary { get; set; }
        public Type Type { get; set; }
        public SerializationTokenType TokenType { get; set; }
    }

    public class Ghaz
    {
        public Test[] Tests { get; set; }
    }
/*    public class TestGameCategory : GameLevelCategory
    {
        [GameItemFilter(typeof(Text))]
        public GameItemInfo Image { get; set; }
    }*/
/*    public class WeakAction
    {
        public Action Action { get; set; }

        public WeakAction()
        {
            Action = () =>
            {
                
            };
            object target = Action.Target;
        }

        private static void Paskmak()
        {
            throw new NotImplementedException();
        }
    }*/
    public class TestGameLevelInfo:GameLevelInfo
    {
        [CustomExport]
        public int LevelScore { get; set; }

        public GameItemInfo Data { get; set; }
        public AnimationCurve AnimationCurve { get; set; }
    }

    public class SavedData
    {
        public int Value { get; set; }
    }
}
