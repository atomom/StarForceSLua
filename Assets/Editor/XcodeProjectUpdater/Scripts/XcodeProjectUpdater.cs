//  XcodeProjectUpdater.cs
//  ProductName Test
//
//  Created by kikuchikan on 2015.07.29.

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using StarForce.Lua;

/// <summary>
/// Xcodeのプロジェクトを書き出す際に諸々の設定を自動で行うクラス
/// </summary>
public class XcodeProjectUpdater : MonoBehaviour
{

    //設定データのパス
    private const string SETTING_DATA_PATH = "Assets/Editor/XcodeProjectUpdater/Resources/XcodeProjectSetting.asset";

    //ビルド後に呼ばれる
    [PostProcessBuild(100)]
    private static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath)
    {

        //iOS以外にビルドしている場合は更新処理を行わないように
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        //Xcodeの設定データを読み込む
        XcodeProjectSetting setting = AssetDatabase.LoadAssetAtPath<XcodeProjectSetting>(SETTING_DATA_PATH);
        if (setting == null)
        {
            Debug.Log("Resources/" + SETTING_DATA_PATH + "がありません！");
            return;
        }
        //Xcodeプロジェクトの読み込み
        string pbxProjPath = PBXProject.GetPBXProjectPath(buildPath);
        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromString(File.ReadAllText(pbxProjPath));

        //ターゲットのID取得
        string targetGuid = pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName());

        //指定ディレクトリ内のファイルを全てコピーする
        if (!string.IsNullOrEmpty(setting.CopyDirectoryPath))
        {
            DirectoryProcessor.CopyAndAddBuildToXcode(pbxProject, targetGuid, setting.CopyDirectoryPath, buildPath, "");
        }

        //コンパイラフラグの設定
        foreach (XcodeProjectSetting.CompilerFlagsSet compilerFlagsSet in setting.CompilerFlagsSetList)
        {

            foreach (string targetPath in compilerFlagsSet.TargetPathList)
            {
                if (!pbxProject.ContainsFileByProjectPath(targetPath))
                {
                    Debug.Log(targetPath + "が無いのでコンパイラフラグが設定できませんでした");
                    continue;
                }

                string fileGuid = pbxProject.FindFileGuidByProjectPath(targetPath);
                List<string> flagsList = pbxProject.GetCompileFlagsForFile(targetGuid, fileGuid);

                flagsList.Add(compilerFlagsSet.Flags);
                pbxProject.SetCompileFlagsForFile(targetGuid, fileGuid, flagsList);
            }

        }

        //システムのフレームワークを追加
        foreach (string framework in setting.FrameworkList)
        {
            pbxProject.AddFrameworkToProject(targetGuid, framework, false);
        }

        foreach (string lib in setting.LibraryList)
        {
            string fileGuid = pbxProject.AddFile("usr/lib/" + lib, "Frameworks/" + lib, PBXSourceTree.Sdk);
            pbxProject.AddFileToBuild(targetGuid, fileGuid);
        }

        //Linker Flagの設定
        pbxProject.UpdateBuildProperty(targetGuid, XcodeProjectSetting.LINKER_FLAG_KEY, setting.LinkerFlagArray, null);

        //フレームワークがあるディレクトリへのパス設定
        pbxProject.UpdateBuildProperty(targetGuid, XcodeProjectSetting.FRAMEWORK_SEARCH_PATHS_KEY, setting.FrameworkSearchPathArray, null);

        //BitCodeの設定
        pbxProject.SetBuildProperty(targetGuid, XcodeProjectSetting.ENABLE_BITCODE_KEY, setting.EnableBitCode ? "YES" : "NO");

        //プロジェクトファイル書き出し
        File.WriteAllText(pbxProjPath, pbxProject.WriteToString());

        //URLスキームの設定
        List<string> urlSchemeList = new List<string>(setting.URLSchemeList);
        InfoPlistProcessor.SetURLSchemes(buildPath, setting.URLIdentifier, urlSchemeList);

        List<string> applicationQueriesSchemes = new List<string>(setting.ApplicationQueriesSchemes);
        InfoPlistProcessor.SetApplicationQueriesSchemes(buildPath, applicationQueriesSchemes);

        //デフォルトで設定されているスプラッシュ画像の設定を消す
        if (setting.NeedToDeleteLaunchiImagesKey)
        {
            InfoPlistProcessor.DeleteLaunchiImagesKey(buildPath);
        }

        //ATSの設定
        InfoPlistProcessor.SetATS(buildPath, setting.EnableATS);

        //ステータスバーの設定
        InfoPlistProcessor.SetStatusBar(buildPath, setting.EnableStatusBar);
        var code_path = buildPath + "/Classes/UnityAppController.h";
        string code_head = FileUtils.getInstance().getString(code_path);
        if(code_head.IndexOf("@property NSURL* mUrl;") == -1)
        {
            code_head = code_head.Replace("- (void)shouldAttachRenderDelegate;", "- (void)shouldAttachRenderDelegate;\n@property NSURL* mUrl;\n");
            FileUtils.getInstance().writeString(code_path, code_head);
        }
        code_path = buildPath + "/Classes/UnityAppController.mm";
        string code = FileUtils.getInstance().getString(code_path);
        if (code_head.IndexOf("_mUrl = url;") == -1)
        {
            code = code.Replace("AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);", "_mUrl = url;\nAppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);");
            FileUtils.getInstance().writeString(code_path, code);
        }

    }

}
