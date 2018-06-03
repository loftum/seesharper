using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;

namespace SeeSharper.Options
{
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
}