using Microsoft.VisualStudio.Settings;

namespace SeeSharper.Options
{
    public static class SettingsStoreExtensions
    {
        public static object GetValue(this WritableSettingsStore store, string collectionPath, string propertyName)
        {
            if (store.PropertyExists(collectionPath, propertyName))
            {
                switch (store.GetPropertyType(collectionPath, propertyName))
                {
                    case SettingsType.Int32: return store.GetInt32(collectionPath, propertyName);
                    case SettingsType.Int64: return store.GetInt64(collectionPath, propertyName);
                    case SettingsType.String: return store.GetString(collectionPath, propertyName);
                    case SettingsType.Binary:
                        using (var stream = store.GetMemoryStream(collectionPath, propertyName))
                        {
                            var buffer = new byte[stream.Length];
                            stream.Read(buffer, 0, buffer.Length);
                            return buffer;
                        }
                }
            }
            return null;
        }

        public static void SetValue(this WritableSettingsStore store, string collectionPath, string propertyName, object value)
        {
            switch (value)
            {
                case null: store.DeleteProperty(collectionPath, propertyName);
                    break;
                case string s: store.SetString(collectionPath, propertyName, s);
                    break;
                case long l: store.SetInt64(collectionPath, propertyName, l);
                    break;
                case int i: store.SetInt32(collectionPath, propertyName, i);
                    break;
            }
        }
    }
}