using System.ComponentModel;

namespace SeeSharper.Options
{
    public interface IOccurrenceTaggingOptions
    {
        event PropertyChangedEventHandler PropertyChanged;
        string[] DimPatterns { get; }
        string[] EmphasizePatterns { get; }
        string DimPatternsString { get; set; }
        string EmphasizePatternsString { get; set; }
        bool SemanticColoringEnabled { get; set; }
        bool OccurrenceTaggingEnabled { get; set; }
        void Load();
        void Save();
    }
}