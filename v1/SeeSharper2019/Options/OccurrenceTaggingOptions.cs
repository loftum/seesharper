using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;

namespace SeeSharper2019.Options
{
    [Export(typeof(IOccurrenceTaggingOptions))]
    public class OccurrenceTaggingOptions : IOccurrenceTaggingOptions
    {
        private static readonly string CollectionName = typeof(OccurrenceTaggingOptions).FullName;
        private readonly WritableSettingsStore _store;
        private string _dimPatternsString = "";
        private string _emphasizePatternsString = "";

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _semanticColoringEnabled = true;

        public bool SemanticColoringEnabled
        {
            get => _semanticColoringEnabled;
            set
            {
                _semanticColoringEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _occurrenceTaggingEnabled = true;

        public bool OccurrenceTaggingEnabled
        {
            get => _occurrenceTaggingEnabled;
            set
            {
                _occurrenceTaggingEnabled = value;
                OnPropertyChanged();
            }
        }

        public string[] DimPatterns { get; private set; } = new string[0];

        public string DimPatternsString
        {
            get => _dimPatternsString;
            set
            {
                _dimPatternsString = value;
                DimPatterns = value?.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries) ??
                              new string[0];
                OnPropertyChanged();
            }
        }

        public string[] EmphasizePatterns { get; private set; } = new string[0];

        public string EmphasizePatternsString
        {
            get => _emphasizePatternsString;
            set
            {
                _emphasizePatternsString = value;
                EmphasizePatterns = value?.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries) ??
                                    new string[0];
                OnPropertyChanged();
            }
        }

        private static string[] Split(string value) => value?.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

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
                property.SetValue(this, _store.GetValue(CollectionName, property));
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

        private void OnPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}