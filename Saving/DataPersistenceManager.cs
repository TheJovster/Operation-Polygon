using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Configuration")]
    [SerializeField] private string fileName; 


    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null) 
        {
            Debug.LogError("Found multiple instances of DataPersistenceManager in the scene.");
        }
        Instance = this;
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame() 
    {
        this.gameData = new GameData();
    }

    public void LoadGame() 
    {
        //Load any saved data from a file using the data handler
        this.gameData = dataHandler.Load();

        //if no data is present - initialize new game
        if(this.gameData == null) 
        {
            Debug.Log("No Data to load - reveting to default values");
            NewGame();
            
        }
        //push the loaded data to all of the other scripts that need it
        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects) 
        {
            dataPersistenceObj.LoadData(gameData);
        }

    }

    public void SaveGame() 
    {
        //pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() 
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

}
