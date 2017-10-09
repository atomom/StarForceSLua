using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

public class Export
{
    private static string csvPath = "";
    //-------------------------------------------csv----------------------------------------------------//
    static void changeLuaToUTF8(string luaPath)
    {
        DirectoryInfo dir = new DirectoryInfo(luaPath);
        foreach (FileInfo file in dir.GetFiles("*.lua.*", SearchOption.AllDirectories))
        {
            if (file.Extension != ".meta")
            {
                string path = file.FullName.Replace('\\', '/');
                //Debug.Log(path);
                string content = File.ReadAllText(path, Encoding.GetEncoding("gb2312"));
                content = content.Replace("\r\n", "\n");
                //Debug.Log(content);
                //UTF8Encoding的参数一定要为false，表示不省略BOM，即Byte Order Mark，也即字节流标记，它是用来让应用程序识别所用的编码的
                path = Application.dataPath.Replace("Assets", "lua/configs/") + file.Name;
                //Debug.Log(path);
                File.WriteAllText(path, content, new UTF8Encoding(false));
                //using (var sw = new StreamWriter(path, false, new UTF8Encoding(false)))
                //{
                //    sw.Write(content);

                //    sw.Close();
                    
                //}
                
                Debug.Log("Encode file::>>" + path + " OK!");
            }
        }

        Debug.Log("转换完成");
    }

    [MenuItem("Tools/配置表更新")]
    static void updateCsv()
    {
        //转换为Utf8编码，存入临时文件夹
        string tmpDir = Application.dataPath.Replace("Assets", "lua/configs");
        if (Directory.Exists(tmpDir))
            Directory.Delete(tmpDir, true);
        Directory.CreateDirectory(tmpDir);
        csvPath = Application.dataPath.Replace("Assets", "other/table/real");
        DirectoryInfo dir = new DirectoryInfo(csvPath);
        foreach (FileInfo file in dir.GetFiles("*.csv", SearchOption.AllDirectories))
        {
            using (StreamReader sr = new StreamReader(file.FullName, Encoding.Default, false))
            {
                byte[] bytes = Encoding.Default.GetBytes(sr.ReadToEnd());
                sr.Close();
                byte[] data = Encoding.Convert(Encoding.Default, Encoding.UTF8, bytes);
                string str = Encoding.UTF8.GetString(data);

                string writePath = tmpDir + file.Name;
                using (StreamWriter sw = new StreamWriter(writePath, false, Encoding.UTF8))//以新的编码格式写入
                {
                    sw.Write(str);
                    sw.Close();
                }
                Debug.Log("write to " + writePath);
            }
        }

        string pathlua = Application.dataPath.Replace("Assets", "other/table/lua");
        if (Directory.Exists(pathlua))
            Directory.Delete(pathlua, true);
        Directory.CreateDirectory(pathlua);

        //转为lua表
        Debug.Log("start to pack to lua ！！！");
        string path = Application.dataPath.Replace("Assets", "other/table/PackTool.bat");
        Debug.Log(path);
        System.Diagnostics.Process proc = System.Diagnostics.Process.Start(path);
        proc.WaitForExit();
        //删除临时文件
        //Directory.Delete(tmpDir, true);
        

        changeLuaToUTF8(pathlua);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    //-------------------------------------------csv----------------------------------------------------//
}
