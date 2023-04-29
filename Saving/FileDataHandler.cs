using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Rendering.Universal;

public class FileDataHandler 
{
    private string dataDirectoryPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirectorypath, string dataFileName) 
    {
        this.dataDirectoryPath = dataDirectorypath;
        this.dataFileName = dataFileName;
    }

    public GameData Load() 
    {
        //use Path.Combine to make sure that the saving can be done regardless of which OS it's being saved on.
        string path = Path.Combine(dataDirectoryPath, dataFileName);
        GameData loadedData = null;
        if (File.Exists(path)) 
        {
            try 
            {
                string dataToLoad = "";
                using(FileStream stream = new FileStream(path, FileMode.Open)) 
                {
                    using(StreamReader reader = new StreamReader(stream)) 
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                //deserialize data
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                Debug.Log(path);
            }
            catch(Exception e) 
            {
                Debug.LogError("Error occured when trying to load data from file: " + path + "\n" + e);
            }

        }
        return loadedData;
    }

    public void Save(GameData data) 
    {
        string path = Path.Combine(dataDirectoryPath, dataFileName); //
        try
        {
            //create directory path if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            //serialize the C# game data object into a Json string
            string dataToStore = JsonUtility.ToJson(data, true);

            //write to file
            using(FileStream stream = new FileStream(path, FileMode.Create)) 
            {
                using(StreamWriter writer = new StreamWriter(stream)) 
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + path + "\n" + e);
        }
    }
}
