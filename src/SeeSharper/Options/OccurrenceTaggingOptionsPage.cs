using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;

namespace SeeSharper.Options
{
    public interface IOccurrenceTaggingOptions
    {
        event PropertyChangedEventHandler PropertyChanged;
        string[] DimPatterns { get; }
        string[] HighlightPatterns { get; }
        string DimPatternsString { get; set; }
        string HighlightPatternsString { get; set; }
        void Load();
        void Save();
    }

    public static class SiteExtensions
    {
        public static T GetService<T>(this IServiceProvider site, Type type = null)
        {
            return (T) site.GetService(type ?? typeof(T));
        }

    }

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

    [Export(typeof(IOccurrenceTaggingOptions))]
    public class OccurrenceTaggingOptions : IOccurrenceTaggingOptions
    {
        private static readonly string CollectionName = typeof(OccurrenceTaggingOptions).FullName;
        private readonly WritableSettingsStore _store;
        private string _dimPatternsString;
        private string _highlightPatternsString;

        public event PropertyChangedEventHandler PropertyChanged;
        public string[] DimPatterns => DimPatternsString?.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
        public string[] HighlightPatterns => HighlightPatternsString?.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

        public string DimPatternsString
        {
            get => _dimPatternsString;
            set
            {
                _dimPatternsString = value;
                OnPropertyChanged();
            }
        }

        public string HighlightPatternsString
        {
            get => _highlightPatternsString;
            set
            {
                _highlightPatternsString = value;
                OnPropertyChanged();
            }
        }

        [ImportingConstructor]
        public OccurrenceTaggingOptions(SVsServiceProvider serviceProvider)
        {
            var shellSettingsManager = new ShellSettingsManager(serviceProvider);
            _store = shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            if (!_store.CollectionExists(CollectionName))
            {
                _store.CreateCollection(CollectionName);
            }
            Load();
        }

        public void Load()
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this))
            {
                Load(property);
            }
        }

        private void Load(PropertyDescriptor property)
        {
            if (_store.PropertyExists(CollectionName, property.Name))
            {
                property.SetValue(this, _store.GetValue(CollectionName, property.Name));
            }
        }

        public void Save()
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this))
            {
                Save(property);
            }
        }

        private void Save(PropertyDescriptor property)
        {
            _store.SetValue(CollectionName, property.Name, property.GetValue(this));
        }

        private void OnPropertyChanged([CallerMemberName]string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

    public class OccurrenceTaggingOptionsPage : DialogPage
    {
        private IOccurrenceTaggingOptions _options;
        private IOccurrenceTaggingOptions Options => _options ??
            (_options = Site.GetService<IComponentModel>(typeof(SComponentModel)).DefaultExportProvider.GetExportedValue<IOccurrenceTaggingOptions>());

        private static int _nextId = 0;
        private readonly int _id = ++_nextId;

        public const string Category = "SeeSharper";
        public const string PageName = "Tagging";

        [Category(Category)]
        [DisplayName("Dim pattern")]
        [Description("Dim pattern")]
        public string DimPatternsString
        {
            get => Options?.DimPatternsString;
            set => Options.DimPatternsString = value;
        }

        [Category(Category)]
        [DisplayName("Highlight pattern")]
        [Description("Highlight pattern")]
        public string HighlightPatternsString
        {
            get => Options?.HighlightPatternsString;
            set => Options.HighlightPatternsString = value;
        }

        public override void LoadSettingsFromStorage()
        {
            Options.Load();
        }

        public override void SaveSettingsToStorage()
        {
            Options.Save();
        }
    }
}