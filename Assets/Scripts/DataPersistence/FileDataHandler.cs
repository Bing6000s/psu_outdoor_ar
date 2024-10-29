using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath  = "";           // holds the directory path
    private string dataFileName = "";           // holds file name

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;

    }

    public GameData Load()
    {
        //* NOTE - string concatenation won't work on every OS. The Path.Combine() method creates the path using the OS of the device 
        string fullPath = Path.Combine(dataDirPath,dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                //load the serialized data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath,FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();        //reads all data
                    }
                }

                //Deserialized data from JSON back into C# object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

            }
            catch (System.Exception e)
            {
                Debug.LogError("Error has occurred when trying to load data to file: " + fullPath + "\n" + e);
                
            }
        }
        return loadedData;
    }
    public void Save(GameData data)
    {
        // Creates full path
        //* NOTE - string concatenation won't work on every OS. The Path.Combine() method creates the path using the OS of the device 
        string fullPath = Path.Combine(dataDirPath,dataFileName);
        try
        {
            // Create the directory the file will be writing to. If it doesn't exist already
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serialize the C# game data object into JSON. true refers to if the data will be formatted
            string dataToStore = JsonUtility.ToJson(data,true);

            // Write the serialized data to the file
            //  using () blocks ensure that the file is closed after we are done read/writing
            using (FileStream stream = new FileStream(fullPath,FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error has occurred when trying to save data to file: " + fullPath + "\n" + e);
            
        }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> 4e7f91bb7aec1a6432c7b9d8c980e1214aa587b2
