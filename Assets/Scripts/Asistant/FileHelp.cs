using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public static class FileHelp
{
    static string EncryptString(string original)
    {
        return original;
    }

    public static bool SaveStringFile(string content,string fullPath)
    {
        StreamWriter sw = File.CreateText(fullPath.ToString());
        sw.Write(content);
        sw.Close();

        return true;
    }

    public static string LoadStringFromFile(string fullPath)
    {
        string result = File.ReadAllText(fullPath.ToString());
        return result;
    }

    public static List<string> GetAllJsonFileNames(string fullPath)
    {
        List<string> result = new List<string>();
        DirectoryInfo folder = new DirectoryInfo(fullPath);

        foreach (FileInfo file in folder.GetFiles("*.json"))
            result.Add(file.Name);

        return result;
    }

    public static string GetFullPathByDataPathRelativePath(string path)
    {
        return string.Format(@"{0}/{1}", Application.dataPath, path);
    }

    public static bool FileExistInDataPathRelativePath(string path)
    {
        string fullPath = GetFullPathByDataPathRelativePath(path);
        return FileExist(fullPath);
    }

    public static bool FileExist(string fullPath)
    {
        return File.Exists(fullPath);
    }

    public static bool FolderExistInDataPathRelativePath(string path)
    {
        string fullPath = GetFullPathByDataPathRelativePath(path);
        return FolderExist(fullPath);
    }

    public static bool FolderExist(string fullPath)
    {
        return Directory.Exists(fullPath);
    }

    public static T LoadJsonFormatObjectByDataPathRelativePath<T>(string path)
    {
        string fullPath = GetFullPathByDataPathRelativePath(path);
        return JsonConvert.DeserializeObject<T>(LoadStringFromFile(fullPath));
    }

    public static T LoadJsonFormatObject<T>(string fullPath)
    {
        return JsonConvert.DeserializeObject<T>(LoadStringFromFile(fullPath));
    }
}
