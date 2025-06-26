using Newtonsoft.Json;
using UnityEngine;

public static class JsonManager
{
    public static T LoadJsonFromTextAsset<T>(TextAsset data)
    {
        T loadedData = JsonConvert.DeserializeObject<T>(data.text);
        return loadedData;
    }
}
