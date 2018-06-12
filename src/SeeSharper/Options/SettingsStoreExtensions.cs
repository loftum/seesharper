using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Settings;

namespace SeeSharper.Options
{
    public static class SettingsStoreExtensions
    {
        public static object GetValue(this WritableSettingsStore store, string collectionPath, PropertyDescriptor property)
        {
            if (store.PropertyExists(collectionPath, property.Name))
            {
                switch (property.PropertyType)
                {
                    case Type t when t == typeof(string): return store.GetString(collectionPath, property.Name);
                    case Type t when t == typeof(int): return store.GetInt32(collectionPath, property.Name);
                    case Type t when t == typeof(long): return store.GetInt64(collectionPath, property.Name);
                    case Type t when t == typeof(bool): return store.GetBoolean(collectionPath, property.Name);
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
                case bool b: store.SetBoolean(collectionPath, propertyName, b);
                    break;
            }
        }
    }
}