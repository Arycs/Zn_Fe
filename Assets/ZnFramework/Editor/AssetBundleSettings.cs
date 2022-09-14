using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using LitJson;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using ZnFramework;

[CreateAssetMenu(menuName = "EditorAssets/AsstBundleSettings")]
public class AssetBundleSettings : ScriptableObject
{
    public enum CusBuildTarget
    {
        Windows,
        Android,
        IOS
    }

    #region TempPath OutPath

    private string TempPath => Application.dataPath + "/../" + AssetBundleSavePath + "/" + ResourceVersion + "_Temp/" +
                               CurrBuildTarget;

    private string OutPath => TempPath.Replace("_Temp", "");

    #endregion

    [LabelText("资源版本号"), PropertySpace(2), HorizontalGroup("Common", LabelWidth = 70), VerticalGroup("Common/Left")]
    public string ResourceVersion = "1.0.1";

    [LabelText("目标平台"), PropertyOrder(10), VerticalGroup("Common/Left")]
    public CusBuildTarget CurrBuildTarget;

    [LabelText("参数"), PropertySpace(10), VerticalGroup("Common/Left")]
    public BuildAssetBundleOptions Options;

    [LabelText("更新版本号"), VerticalGroup("Common/Right"), Button(ButtonSizes.Medium)]
    public void UpdateResourceVersion()
    {
        var version = ResourceVersion;
        var arr = version.Split('.');
        int.TryParse(arr[2], out var shortVersion);
        version = $"{arr[0]}.{arr[1]}.{arr[2]}";
        ResourceVersion = version;
    }

    [LabelText("清空资源包"), PropertySpace(5), VerticalGroup("Common/Right"), Button(ButtonSizes.Medium)]
    public void ClearAssetBundle()
    {
        if (Directory.Exists(Application.streamingAssetsPath + "/AssetBundles/"))
        {
            Directory.Delete(Application.streamingAssetsPath + "/AssetBundles/", true);
        }

        Debug.Log("清理完毕");
        AssetDatabase.Refresh();
    }

    private void CopyAssetBundleToStreamingAsset()
    {
        ClearAssetBundle();
        var sourceAssetBundlePath = AssetBundleSavePath + "/" + ResourceVersion + "/Windows/";
        var targetAssetBundlePath = Application.streamingAssetsPath + "/AssetBundles/";
        CopyFolder(sourceAssetBundlePath, targetAssetBundlePath);
        EditorUtility.DisplayDialog("", "拷贝完毕", "确定");
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Todo 应该放到辅助工具类 拷贝文件
    /// </summary>
    /// <param name="srcPath"></param>
    /// <param name="tarPath"></param>
    private void CopyFolder(string srcPath, string tarPath)
    {
        if (!Directory.Exists(srcPath))
        {
            Debug.Log("CopyFolder is finish.");
            return;
        }
        if (!Directory.Exists(tarPath))
        {
            Directory.CreateDirectory(tarPath);
        }
        //获得源文件下所有文件
        List<string> files = new List<string>(Directory.GetFiles(srcPath));
        files.ForEach(f =>
        {
            string destFile = Path.Combine(tarPath, Path.GetFileName(f));
            File.Copy(f, destFile, true); //覆盖模式
        });

        //获得源文件下所有目录文件
        List<string> folders = new List<string>(Directory.GetDirectories(srcPath));
        folders.ForEach(f =>
        {
            string destDir = Path.Combine(tarPath, Path.GetFileName(f));
            CopyFolder(f, destDir); //递归实现子文件夹拷贝
        });
    }

    /// <summary>
    /// 要收集的资源
    /// </summary>
    private List<AssetBundleBuild> NeedBuilds = new List<AssetBundleBuild>();

    [LabelText("打包"), PropertySpace(5), VerticalGroup("Common/Right"), Button(ButtonSizes.Medium)]
    public void BuildAssetBundle()
    {
        NeedBuilds.Clear();
        foreach (var data in Datas)
        {
            var lenPath = data.Path.Length;
            for (var i = 0; i < lenPath; i++)
            {
                var path = data.Path[i];
                BuildAssetBundleForPath(path, data.Overall);
            }
        }

        if (Directory.Exists(TempPath))
        {
            Directory.Delete(TempPath, true);
        }

        Directory.CreateDirectory(TempPath);

        if (NeedBuilds.Count == 0)
        {
            Debug.Log("未找到需要打包内容");
            return;
        }

        Debug.Log("NeedBuilds Count = " + NeedBuilds.Count);

        BuildPipeline.BuildAssetBundles(TempPath, NeedBuilds.ToArray(), Options, GetBuildTarget());

        Debug.Log("临时资源包打包完毕");

        CopyFile(TempPath);

        Debug.Log("拷贝到输出目录完毕");

        AssetBundleEncrypt();
        Debug.Log("资源包加密完成");

        CreateDependenciesFile();
        Debug.Log("生成依赖关系文件完毕");

        CreateVersionFile();
        Debug.Log("生成版本文件完毕");

        var res =  EditorUtility.DisplayDialog("AssetBundle打包完成","打包完成,是否拷贝资源到StreamingAsset","Ok","NO");
        if (res)
        {
            CopyAssetBundleToStreamingAsset();
        }
    }

    #region BuildAssetBundleForPath 根据路径打包资源

    /// <summary>
    ///  根据路径打包资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="dataOverall"></param>
    private void BuildAssetBundleForPath(string path, bool dataOverall)
    {
        var fullPath = Application.dataPath + "/" + path;
        var directory = new DirectoryInfo(fullPath);
        var arrFiles = directory.GetFiles("*", SearchOption.AllDirectories);
        if (dataOverall)
        {
            var build = new AssetBundleBuild
            {
                assetBundleName = path + ".ab", assetBundleVariant = "y", assetNames = GetValidateFiles(arrFiles)
            };
            NeedBuilds.Add(build);
        }
        else
        {
            var arr = GetValidateFiles(arrFiles);
            foreach (var a in arr)
            {
                var build = new AssetBundleBuild
                {
                    assetBundleName = a.Substring(0, a.LastIndexOf(".", StringComparison.Ordinal))
                        .Replace("Assets/", "") + ".ab",
                    assetBundleVariant = "y",
                    assetNames = new[] {a}
                };
                NeedBuilds.Add(build);
            }
        }
    }

    #endregion

    #region GetValidateFiles 检查文件合法性

    /// <summary>
    /// 检查文件合法性
    /// </summary>
    /// <param name="arrFiles"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private string[] GetValidateFiles(FileInfo[] arrFiles)
    {
        var lst = new List<string>();
        var len = arrFiles.Length;
        for (var i = 0; i < len; i++)
        {
            var file = arrFiles[i];
            if (!file.Extension.Equals(".meta", StringComparison.CurrentCultureIgnoreCase))
            {
                lst.Add("Assets" + file.FullName.Replace("\\", "/").Replace(Application.dataPath, ""));
            }
        }

        return lst.ToArray();
    }

    #endregion

    #region GetBuildTarget 获取目标平台

    /// <summary>
    /// 获取目标平台
    /// </summary>
    /// <returns></returns>
    private BuildTarget GetBuildTarget()
    {
        switch (CurrBuildTarget)
        {
            case CusBuildTarget.Android:
                return BuildTarget.Android;
            case CusBuildTarget.IOS:
                return BuildTarget.iOS;
            default:
                return BuildTarget.StandaloneWindows;
        }
    }

    #endregion

    #region CopyFile 拷贝文件到正式目录

    /// <summary>
    /// 拷贝文件到正式目录
    /// </summary>
    /// <param name="oldPath"></param>
    private void CopyFile(string oldPath)
    {
        if (Directory.Exists(OutPath))
        {
            Directory.Delete(OutPath, true);
        }

        IOUtil.CopyDirectory(oldPath, OutPath);
        var directory = new DirectoryInfo(OutPath);
        var arrFiles = directory.GetFiles("*.y", SearchOption.AllDirectories);
        var len = arrFiles.Length;
        for (var i = 0; i < len; i++)
        {
            var file = arrFiles[i];
            File.Move(file.FullName, file.FullName.Replace(".ab.y", ".assetbundle"));
        }
    }

    #endregion

    #region AssetBundleEncrypt 资源包加密

    /// <summary>
    /// 资源包加密
    /// </summary>
    private void AssetBundleEncrypt()
    {
        var len = Datas.Length;
        for (var i = 0; i < len; i++)
        {
            var assetBundleData = Datas[i];
            if (!assetBundleData.IsEncrypt) continue;
            foreach (var p in assetBundleData.Path)
            {
                var path = OutPath + "/" + p;
                if (assetBundleData.Overall)
                {
                    path += ".assetbundle";
                    AssetBundleEncryptFile(path);
                }
                else
                {
                    AssetBundleEncryptFolder(path);
                }
            }
        }
    }

    private void AssetBundleEncryptFolder(string folderPath, bool isDelete = false)
    {
        var directory = new DirectoryInfo(folderPath);
        var arrFiles = directory.GetFiles("*", SearchOption.AllDirectories);
        foreach (var arrFile in arrFiles)
        {
            AssetBundleEncryptFile(arrFile.FullName, isDelete);
        }
    }

    private void AssetBundleEncryptFile(string filePath, bool isDelete = false)
    {
        byte[] buffer = null;
        if (!File.Exists(filePath))
        {
            return;
        }

        using (var fs = new FileStream(filePath, FileMode.Open))
        {
            buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
        }

        buffer = SecurityUtil.Xor(buffer);
        using (var fs = new FileStream(filePath, FileMode.Open))
        {
            fs.Write(buffer, 0, buffer.Length);
            fs.Flush();
        }
    }

    #endregion

    #region CreateDependenciesFile 生成依赖文件

    /// <summary>
    /// 生成依赖文件
    /// </summary>
    private void CreateDependenciesFile()
    {
        CollectDepend();
        var tempList = new List<AssetEntity>();
        var len = Datas.Length;
        for (var i = 0; i < len; i++)
        {
            var assetBundleData = Datas[i];
            foreach (var t in assetBundleData.Path)
            {
                var path = Application.dataPath + "/" + t;
                CollectFileInfo(tempList, path);
            }
        }

        var assetList = new List<AssetEntity>();
        len = tempList.Count;
        for (var i = 0; i < len; i++)
        {
            var entity = tempList[i];
            var newEntity = new AssetEntity
            {
                Category = entity.Category,
                AssetName = entity.AssetFullName.Substring(entity.AssetFullName.LastIndexOf('/') + 1),
                AssetFullName = entity.AssetFullName,
                AssetBundleName = entity.AssetBundleName
            };
            newEntity.AssetName = newEntity.AssetName.Substring(0, newEntity.AssetName.LastIndexOf('.'));
            

            // 资源依赖项
            newEntity.DependsAssetList = new List<AssetDependEntity>();
            var arr = AssetDatabase.GetDependencies(entity.AssetFullName, true);
            foreach (var a in arr)
            {
                if (a.Equals(newEntity.AssetFullName) || !GetIsAsset(tempList, a)) continue;
                var assetDepends = new AssetDependEntity()
                {
                    Category = GetAssetCategory(a),
                    AssetFullName = a,
                };
                assetDepends.AssetBundleName =
                    (GetAssetBundleName(assetDepends.AssetFullName) + ".assetbundle").ToLower();
                newEntity.DependsAssetList.Add(assetDepends);
            }
            // 资源引用项
            newEntity.RefrenceAssetList = new List<AssetReferenceEntity>();
            var arr2 =  GetReferenceAsset(entity.AssetFullName);
            if (arr2 != null)
            {
                foreach (var a in arr2)
                {
                    if (a.Equals(newEntity.AssetFullName) || !GetIsAsset(tempList,a)) continue;
                    var assetReference = new AssetReferenceEntity()
                    {
                        Category = GetAssetCategory(a),
                        AssetFullName = a,
                    };
                    assetReference.AssetBundleName = (GetAssetBundleName(assetReference.AssetFullName) + ".assetbundle").ToLower();
                    newEntity.RefrenceAssetList.Add(assetReference);
                }
            }
            
            
            assetList.Add(newEntity);
        }

        var targetPath = OutPath;
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        var strJsonFilePath = targetPath + "AssetInfo.json";
        IOUtil.CreateTextFile(strJsonFilePath, JsonMapper.ToJson(assetList));
        Debug.LogError("生成AssetInfo.json 完毕");

        var ms = new ZnMemoryStream();
        len = assetList.Count;
        ms.WriteInt(len);
        for (var i = 0; i < len; i++)
        {
            var entity = assetList[i];
            ms.WriteByte((byte) entity.Category);
            ms.WriteUTF8String(entity.AssetFullName);
            ms.WriteUTF8String(entity.AssetBundleName);

            if (entity.DependsAssetList != null)
            {
                var depLen = entity.DependsAssetList.Count;
                ms.WriteInt(depLen);
                for (var j = 0; j < depLen; j++)
                {
                    var assetDepends = entity.DependsAssetList[j];
                    ms.WriteByte((byte) assetDepends.Category);
                    ms.WriteUTF8String(assetDepends.AssetFullName);
                }
            }
            else
            {
                ms.WriteInt(0);
            }
        }

        var filePath = targetPath + "/AssetInfo.bytes";
        var buffer = ms.ToArray();
        buffer = ZlibUtil.CompressBytes(buffer);
        var fs = new FileStream(filePath, FileMode.Create);
        fs.Write(buffer, 0, buffer.Length);
        fs.Close();
        fs.Dispose();
        Debug.LogError("生成AssetBundleInfo.bytes 完成");
    }

    #endregion

    #region CollectFileInfo 收集文件信息

    /// <summary>
    /// 收集文件信息
    /// </summary>
    /// <param name="tempLst"></param>
    /// <param name="folderPath"></param>
    private void CollectFileInfo(List<AssetEntity> tempLst, string folderPath)
    {
        var directory = new DirectoryInfo(folderPath);
        var arrFiles = directory.GetFiles("*", SearchOption.AllDirectories);
        foreach (var file in arrFiles)
        {
            if (file.Extension == ".meta")
            {
                continue;
            }

            if (file.FullName.IndexOf(".idea", StringComparison.Ordinal) != -1)
            {
                continue;
            }

            var filePath = file.FullName;
            var index = filePath.IndexOf("Assets\\", StringComparison.CurrentCultureIgnoreCase);
            var newPath = filePath.Substring(index);

            var entity = new AssetEntity
            {
                AssetFullName = newPath.Replace("\\", "/"),
                Category = GetAssetCategory(newPath.Replace(file.Name, ""))
            };
            entity.AssetBundleName = (GetAssetBundleName(entity.AssetFullName) + ".assetbundle").ToLower();
            tempLst.Add(entity);
        }
    }

    #endregion

    #region GetAssetCategory 获取资源分类

    /// <summary>
    /// 获取资源分类
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private AssetCategory GetAssetCategory(string filePath)
    {
        AssetCategory category = AssetCategory.Audio;

        if (filePath.IndexOf("Audio", StringComparison.Ordinal) != -1)
        {
            category = AssetCategory.Audio;
        }
        else if (filePath.IndexOf("CusShaders", StringComparison.Ordinal) != -1)
        {
            category = AssetCategory.CusShaders;
        }
        else if (filePath.IndexOf("DataTable", StringComparison.Ordinal) != -1)
        {
            category = AssetCategory.DataTable;
        }
        else if (filePath.IndexOf("Effect", StringComparison.Ordinal) != -1)
        {
            category = AssetCategory.EffectSources;
        }
        else if (filePath.IndexOf("Scenes", StringComparison.Ordinal) != -1)
        {
            category = AssetCategory.Scenes;
        }
        else if (filePath.IndexOf("UIFont", StringComparison.Ordinal) != -1)
        {
            category = AssetCategory.UIFont;
        }
        else if (filePath.IndexOf("UIPrefab", StringComparison.Ordinal) != -1)
        {
            category = AssetCategory.UIPrefab;
        }
        else if (filePath.IndexOf("UIRes", StringComparison.Ordinal) != -1)
        {
            category = AssetCategory.UIRes;
        }
        else if (filePath.IndexOf("xLuaLogic", StringComparison.Ordinal) != -1)
        {
            category = AssetCategory.XLuaLogic;
        }

        return category;
    }

    #endregion

    #region GetAssetBundleName 获取AssetBundle包名字

    /// <summary>
    /// 根据路径获取所属AssetBundle名字
    /// </summary>
    /// <param name="assetFullName"></param>
    /// <returns></returns>
    private string GetAssetBundleName(string assetFullName)
    {
        var len = Datas.Length;
        //循环设置文件夹包括子文件里边的项
        for (var i = 0; i < len; i++)
        {
            var assetBundleData = Datas[i];
            foreach (var p in assetBundleData.Path)
            {
                if (assetFullName.IndexOf(p, StringComparison.CurrentCultureIgnoreCase) <= -1) continue;
                return assetBundleData.Overall
                    ? p.ToLower()
                    : assetFullName.Substring(0, assetFullName.LastIndexOf('.')).ToLower().Replace("assets/", "");
            }
        }

        return null;
    }

    #endregion

    #region GetIsAsset 判断某个资源是否存在于资源列表

    /// <summary>
    /// 判断某个资源是否存在于资源列表
    /// </summary>
    /// <param name="tempLst"></param>
    /// <param name="assetFullName"></param>
    /// <returns></returns>
    private bool GetIsAsset(List<AssetEntity> tempLst, string assetFullName)
    {
        var len = tempLst.Count;
        for (var i = 0; i < len; i++)
        {
            var entity = tempLst[i];
            if (entity.AssetFullName.Equals(assetFullName, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region CreateVersionFile 创建版本文件

    /// <summary>
    /// 创建版本文件
    /// </summary>
    private void CreateVersionFile()
    {
        var path = OutPath;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var strVersionFilePath = path + "/VersionFile.txt";
        IOUtil.DeleteFile(strVersionFilePath);
        var sbContent = new StringBuilder();
        var directory = new DirectoryInfo(path);
        var arrFiles = directory.GetFiles("*", SearchOption.AllDirectories);
        sbContent.AppendLine(ResourceVersion);
        foreach (var file in arrFiles)
        {
            if (file.Extension == ".manifest")
            {
                continue;
            }

            var fullName = file.FullName;
            var name = fullName.Substring(fullName.IndexOf(CurrBuildTarget.ToString(), StringComparison.Ordinal) +
                                          CurrBuildTarget.ToString().Length + 1);
            var md5 = EncryptUtil.GetFileMD5(fullName);
            if (md5 == null)
            {
                continue;
            }

            var size = file.Length.ToString();
            var isFirstData = false;
            var isEncrypt = false;
            var isBreak = false;
            foreach (var d in Datas)
            {
                foreach (var p in d.Path)
                {
                    name = name.Replace("\\", "/");
                    if (name.IndexOf(p, StringComparison.CurrentCultureIgnoreCase) == -1) continue;
                    isFirstData = d.IsFirstData;
                    isEncrypt = d.IsEncrypt;
                    isBreak = true;
                    break;
                }

                if (isBreak)
                {
                    break;
                }
            }

            var strLine = $"{name}|{md5}|{size}|{(isFirstData ? 1 : 0)}|{(isEncrypt ? 1 : 0)}";
            sbContent.AppendLine(strLine);
        }

        IOUtil.CreateTextFile(strVersionFilePath, sbContent.ToString());
        var ms = new ZnMemoryStream();
        var str = sbContent.ToString().Trim();
        var arr = str.Split('\n');
        var len = arr.Length;
        ms.WriteInt(len);
        for (var i = 0; i < len; i++)
        {
            if (i == 0)
            {
                ms.WriteUTF8String(arr[i]);
            }
            else
            {
                var arrInner = arr[i].Split('|');
                ms.WriteUTF8String(arrInner[0]);
                ms.WriteUTF8String(arrInner[1]);
                ms.WriteULong(ulong.Parse(arrInner[2]));
                ms.WriteByte(byte.Parse(arrInner[3]));
                ms.WriteByte(byte.Parse(arrInner[4]));
            }
        }

        var filePath = path + "/VersionFile.bytes";
        var buffer = ms.ToArray();
        buffer = ZlibUtil.CompressBytes(buffer);
        var fs = new FileStream(filePath, FileMode.Create);
        fs.Write(buffer, 0, buffer.Length);
        fs.Close();
        fs.Dispose();
        Debug.Log("创建版本文件成功");
    }

    #endregion

    [LabelText("资源包保存路径"), FolderPath] public string AssetBundleSavePath;

    [LabelText("勾选进行编辑")] public bool IsCanEditor;

    [EnableIf("IsCanEditor"), BoxGroup("AssetBundleSettings")]
    public AssetBundleData[] Datas;


    [Serializable]
    public class AssetBundleData
    {
        [LabelText("名称")] public string Name;
        [LabelText("文件夹是否为一个资源")] public bool Overall;
        [LabelText("是否为初始资源")] public bool IsFirstData;
        [LabelText("是否加密")] public bool IsEncrypt;
        [FolderPath(ParentFolder = "Assets")] public string[] Path;
    }

    #region 相关数据的缓存操作, 用来判断被引用关系

    private Dictionary<string, List<string>> ReferenceCacheDic = new Dictionary<string, List<string>>();

    private Dictionary<string, List<string>> DependCacheDic = new Dictionary<string, List<string>>();

    /// <summary>
    /// 收集要打包资源的依赖信息
    /// </summary>
    private void CollectDepend()
    {
        var count = 0;
        var guids = AssetDatabase.FindAssets("", new[] {"Assets/Download"});
        foreach (var guid in guids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var dependencies = AssetDatabase.GetDependencies(assetPath, false);
            if (!DependCacheDic.TryGetValue(assetPath, out var lst1))
            {
                DependCacheDic.Add(assetPath, dependencies.ToList());
            }

            foreach (var filePath in dependencies)
            {
                if (!ReferenceCacheDic.TryGetValue(filePath, out var lst2))
                {
                    lst2 = new List<string>();
                    ReferenceCacheDic[filePath] = lst2;
                }

                lst2.Add(assetPath);
            }

            count++;
            EditorUtility.DisplayProgressBar("Search Dependencies", "Dependencies", (count * 1.0f / guids.Length));
        }

        EditorUtility.ClearProgressBar();
    }
    
    // 判断文件是否被依赖
    private List<string> GetReferenceAsset(string filePath)
    {
        return ReferenceCacheDic.TryGetValue(filePath, out var list) ? list : null;
    }
    #endregion
}