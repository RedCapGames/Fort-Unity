using System;
using System.Collections.Generic;
using Fort;
using Fort.Info;
using Fort.Serializer;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class FortTest : MonoBehaviour
{

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
        InfoResolver.Resolve<FortInfo>().ServerConnectionProvider.EditorConnection.Call<ServerPurchasableItem[]>("GetItems",null).Then(
            objects =>
            {
                Debug.Log(objects.Length);
            },error => Debug.Log("Error"));
    }

    

    // Update is called once per frame
    void Update()
    {
        
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
}
