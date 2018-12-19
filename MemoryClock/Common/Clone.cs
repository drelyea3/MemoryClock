using Newtonsoft.Json;

namespace Common
{
    public static class Clone
    {
        // Create a clone of an object by serializing/deserializing it.
        // This only works for classes that have been created to be 
        // serializable with JSON.
        public static T Create<T>(T source)
        {
            var json = JsonConvert.SerializeObject(source, Formatting.Indented);
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
    }
}
