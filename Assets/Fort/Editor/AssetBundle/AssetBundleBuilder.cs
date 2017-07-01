using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using Assets.FortBacktory;
using Fort.Info;
using Fort.ServerConnection;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Fort.AssetBundle
{
    public static class AssetBundleBuilder
    {
        private static long GetBundleFileSize(string bundleName)
        {
            string outputPath = Path.Combine(EditorAssetBundleUtility.AssetBundlesOutputPath, EditorAssetBundleUtility.GetPlatformName());
            FileInfo fileInfo = new FileInfo(Path.Combine(outputPath,bundleName));
            return fileInfo.Length;
        }
        private static string ResolveAssetBundlePath(string bundleName, string hash)
        {
            return string.Format("/AssetBundles/{0}/{1}/{2}.bundle", EditorAssetBundleUtility.GetPlatformName(), bundleName,hash);
        }
        public static Promise SyncAssetBundles()
        {
            //BacktoryCloudUrl.Url = "http://localhost:8086";
            Deferred deferred = new Deferred();
            string outputPath = Path.Combine(EditorAssetBundleUtility.AssetBundlesOutputPath, EditorAssetBundleUtility.GetPlatformName());
            AssetBundleManifest assetBundleManifest = Build();
            EditorUtility.DisplayProgressBar("Get server asset bundles", "Get server asset bundles", 0);
            InfoResolver.Resolve<FortInfo>()
                .ServerConnectionProvider.EditorConnection.Call<ServerAssetBundle[]>("GetAssetBundles", new {Platform= EditorAssetBundleUtility.GetPlatformName() }).Then(
                    serverBundles =>
                    {
                        List<AssetNameVersion> addedAssets = new List<AssetNameVersion>();
                        List<ServerAssetBundle> finalAssetBundles = new List<ServerAssetBundle>(serverBundles);
                        Dictionary<string,ServerAssetBundleVersion> finalMap = new Dictionary<string, ServerAssetBundleVersion>();
                        EditorUtility.ClearProgressBar();
                        foreach (
                            var source in
                                assetBundleManifest.GetAllAssetBundles()
                                    .Select(s => new { Name = s, Hash = assetBundleManifest.GetAssetBundleHash(s) })
                                    .Where(
                                        arg1 =>
                                            !serverBundles.Any(
                                                bundle =>
                                                    bundle.Name == arg1.Name &&
                                                    bundle.Versions.Any(version => version.Hash == arg1.Hash.ToString())))
                            )
                        {
                            AssetNameVersion assetNameVersion = new AssetNameVersion
                            {
                                Name = source.Name,
                                Version = new ServerAssetBundleVersion
                                {
                                    Hash = source.Hash.ToString(),
                                    Path = ResolveAssetBundlePath(source.Name, source.Hash.ToString()),
                                    Size = GetBundleFileSize(source.Name)
                                }
                            };
                            ServerAssetBundle serverAssetBundle = finalAssetBundles.FirstOrDefault(bundle => bundle.Name == source.Name);
                            if (serverAssetBundle == null)
                            {
                                
                                serverAssetBundle = new ServerAssetBundle
                                {
                                    Name = source.Name,
                                    Versions = new[]
                                    {
                                        assetNameVersion.Version
                                    }
                                };
                                finalAssetBundles.Add(serverAssetBundle);
                            }
                            else
                            {
                                serverAssetBundle.Versions =
                                    serverAssetBundle.Versions.Concat(new[]
                                    {
                                        assetNameVersion.Version
                                    }).ToArray();
                            }
                            addedAssets.Add(assetNameVersion);
                        }
                        foreach (string bundleName in assetBundleManifest.GetAllAssetBundles())
                        {
                            string hash = assetBundleManifest.GetAssetBundleHash(bundleName).ToString();
                            ServerAssetBundleVersion serverAssetBundleVersion = finalAssetBundles.Where(bundle => bundle.Name == bundleName).SelectMany(bundle => bundle.Versions).First(version => version.Hash == hash);
                            finalMap.Add(bundleName,serverAssetBundleVersion);
                        }
                        EditorUtility.DisplayProgressBar("Uploading Asset Bundles", "Uploading Asset Bundles", 0);
                        InfoResolver.Resolve<FortInfo>()
                            .ServerConnectionProvider.EditorConnection.SendFilesToStorage(
                                addedAssets.Select(version => new StorageFile
                                {
                                    FileName = Path.GetFileName(version.Version.Path),
                                    Path = Path.GetDirectoryName(version.Version.Path)+"/",
                                    Stream = File.OpenRead(Path.Combine(outputPath, version.Name))
                                }).ToArray(), f =>
                                {
                                    EditorUtility.DisplayProgressBar("Uploading Asset Bundles", "Uploading Asset Bundles", f);
                                }).Then(files =>
                                {
                                    for (int i = 0; i < addedAssets.Count; i++)
                                    {
                                        addedAssets[i].Version.Path = files[i];
                                    }
                                    EditorUtility.DisplayProgressBar("Updating server Asset bundle list", "Updating server Asset bundle list", 0);
                                    InfoResolver.Resolve<FortInfo>()
                                        .ServerConnectionProvider.EditorConnection
                                        .Call<UpdateServerAchievementResoponse>("UpdateAssetBundles",
                                            new
                                            {
                                                AssetBundles = finalAssetBundles.ToArray(),
                                                Platform = EditorAssetBundleUtility.GetPlatformName()
                                            })
                                        .Then(resoponse =>
                                        {
                                            EditorUtility.ClearProgressBar();
                                            string fullDirectory = string.Format("/Assets/Fort/Resources/BundleConfig/{0}",
                                                EditorAssetBundleUtility.GetPlatformName());
                                            string inAssetPath = string.Format("/Fort/Resources/BundleConfig/{0}",
                                                EditorAssetBundleUtility.GetPlatformName());
                                            if (!Directory.Exists(fullDirectory))
                                                AssetDatabaseHelper.CreateFolderRecursive(fullDirectory);
                                            string jsonFile = Application.dataPath + inAssetPath + "/AssetBundleBundleInfo.json";
                                            using (StreamWriter writer = new StreamWriter(jsonFile))
                                            {
                                                writer.Write(JsonConvert.SerializeObject(finalMap));
                                            }
                                            //AssetDatabase.ImportAsset(fullDirectory+"/AssetBundleBundleInfo.json");
                                            AssetDatabase.Refresh();
                                            deferred.Resolve();
                                        }, error =>
                                        {
                                            
                                            EditorUtility.ClearProgressBar();
                                            deferred.Reject();
                                            throw new Exception("Cannot update server asset bundle list");

                                        });
                                }, () =>
                                {
                                    EditorUtility.ClearProgressBar();
                                    deferred.Reject();
                                    throw new Exception("Cannot Upload asset bundles to storage");
                                });
                    }, error =>
                    {
                        
                        EditorUtility.ClearProgressBar();
                        deferred.Reject();
                        throw new Exception("Cannot resolve asset bundles from server");
                    });
            return deferred.Promise();
        }

        public static AssetBundleManifest Build()
        {
            string outputPath = Path.Combine(EditorAssetBundleUtility.AssetBundlesOutputPath, EditorAssetBundleUtility.GetPlatformName());
            if (Directory.Exists(outputPath))
                Directory.Delete(outputPath, true);
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            return BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }

        class ServerAssetBundle
        {
            public string Name { get; set; }
            public ServerAssetBundleVersion[] Versions { get; set; }

        }



        class AssetNameVersion
        {
            public string Name { get; set; }
            public ServerAssetBundleVersion Version { get; set; }
        }

        class UpdateServerAchievementResoponse
        {
            
        }
    }
}
