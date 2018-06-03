using System.ComponentModel;

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
}