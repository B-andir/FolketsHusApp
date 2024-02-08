using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FolketsHusApp;

public static class PreferencesStore {

    /// <summary>
    /// Store an element using any kind of key (if it doesnt exist)
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void Set(string key, object? value) {
        string keyvalue = JsonConvert.SerializeObject(value);
        if (keyvalue != null && !string.IsNullOrEmpty(keyvalue)) {
            Preferences.Set(key, keyvalue);
        }
    }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.

    /// <summary>
    /// Get an element using a certain key, with type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T Get<T>(string key) {

        T unpackedValue = default;
        string keyvalue = Preferences.Get(key, string.Empty);

        if (keyvalue != null && !string.IsNullOrEmpty(keyvalue)) {
            unpackedValue = JsonConvert.DeserializeObject<T>(keyvalue);
        }

        return unpackedValue;
    }

    public static JToken GetJToken(string key) {
        JToken unpackedValue = default;
        string keyvalue = Preferences.Get(key, string.Empty);

        if (keyvalue != null && !string.IsNullOrEmpty(keyvalue)) {
            unpackedValue = JToken.Parse(keyvalue);
        }

        return unpackedValue;

    }

#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8603 // Possible null reference return.

    /// <summary>
    /// Delete an element with a certain key
    /// </summary>
    /// <param name="key"></param>
    public static void Delete(string key) {
        Preferences.Remove(key);
    }


    /// <summary>
    /// Check if an element with a certain key exists
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool Exists(string key) {
        return Preferences.ContainsKey(key);
    }


    /// <summary>
    /// ATTENTION: Clears the whole Preferences-Store
    /// </summary>
    public static void ClearAll() {
        Preferences.Clear();
    }
}
