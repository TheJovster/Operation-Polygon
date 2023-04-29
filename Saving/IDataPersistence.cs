using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IDataPersistence
{
    void LoadData(GameData data);
    void SaveData(ref GameData data);

}
