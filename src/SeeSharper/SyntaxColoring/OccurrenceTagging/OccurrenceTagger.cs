using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using SeeSharper.Options;
using SeeSharper.SyntaxColoring.Tags;

namespace SeeSharper.SyntaxColoring.OccurrenceTagging
{
    internal class OccurrenceTagger : ITagger<IClassificationTag>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private readonly ITextView _view;
        private readonly ITextBuffer _sourceBuffer;
        private readonly ITextSearchService _textSearchService;
        private readonly ITextStructureNavigator _textStructureNavigator;
        private NormalizedSnapshotSpanCollection _dimSpans;
        private NormalizedSnapshotSpanCollection _highlightSpans;
        private readonly IDictionary<string, IClassificationType> _classificationTypes = new Dictionary<string, IClassificationType>();
        
        private readonly IOccurrenceTaggingOptions _options;
        private readonly object _updateLock = new object();

        public OccurrenceTagger(ITextView view,
            ITextBuffer sourceBuffer,
            ITextSearchService textSearchService,
            ITextStructureNavigator textStructureNavigator,
            IClassificationTypeRegistryService registry,
            IOccurrenceTaggingOptions options)
        {
            _view = view;
            _sourceBuffer = sourceBuffer;
            _textSearchService = textSearchService;
            _textStructureNavigator = textStructureNavigator;
            _options = options;
            _dimSpans = new NormalizedSnapshotSpanCollection();
            _highlightSpans = new NormalizedSnapshotSpanCollection();
            _classificationTypes[TagTypes.Dim] = registry.GetClassificationType(TagTypes.Dim);
            _classificationTypes[TagTypes.Highlight] = registry.GetClassificationType(TagTypes.Highlight);
            view.LayoutChanged += LayoutChanged;
            view.Caret.PositionChanged += OnPositionChanged;
            _options.PropertyChanged += OnOptionsChanged;
            Update(_sourceBuffer.CurrentSnapshot);
        }

        ~OccurrenceTagger()
        {
            _view.Caret.PositionChanged -= OnPositionChanged;
            _options.PropertyChanged -= OnOptionsChanged;
        }

        private void OnOptionsChanged(object sender, PropertyChangedEventArgs e)
        {
            Update(_sourceBuffer.CurrentSnapshot);
        }

        private void OnPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            TagsChanged?.Invoke(this,
                new SnapshotSpanEventArgs(new SnapshotSpan(_sourceBuffer.CurrentSnapshot, 0,
                    _sourceBuffer.CurrentSnapshot.Length)));
        }

        private void LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (e.OldSnapshot == e.NewSnapshot)
            {
                return;
            }
            Update(e.NewSnapshot);
        }

        private void Update(ITextSnapshot snapshot)
        {
            var dimSpans = Find(_options.DimPatterns, snapshot);
            var highlightSpans = Find(_options.HighlightPatterns, snapshot);
            lock (_updateLock)
            {
                _dimSpans = dimSpans;
                _highlightSpans = highlightSpans;
                var temp = TagsChanged;
                temp?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_sourceBuffer.CurrentSnapshot, 0, _sourceBuffer.CurrentSnapshot.Length)));
            }
        }

        private NormalizedSnapshotSpanCollection Find(IEnumerable<string> patterns, ITextSnapshot snapshot)
        {
            var findDatas = patterns.Select(p => new FindData(p, snapshot, FindOptions.UseRegularExpressions | FindOptions.Multiline, _textStructureNavigator));
            var results = findDatas.SelectMany(d => _textSearchService.FindAll(d));
            var spans = new NormalizedSnapshotSpanCollection(results);
            return spans;
        }

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return GetTags(spans, _dimSpans, TagTypes.Dim).Concat(GetTags(spans, _highlightSpans, TagTypes.Highlight));
        }

        private IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans, NormalizedSnapshotSpanCollection wordSpans, string classificationType)
        {
            if (wordSpans == null)
            {
                return Enumerable.Empty<ITagSpan<IClassificationTag>>();
            }

            if (spans.Count == 0 || wordSpans.Count == 0)
            {
                return Enumerable.Empty<ITagSpan<IClassificationTag>>();
            }
            if (spans[0].Snapshot != wordSpans[0].Snapshot)
            {
                wordSpans = new NormalizedSnapshotSpanCollection(wordSpans.Select(s => s.TranslateTo(spans[0].Snapshot, SpanTrackingMode.EdgeInclusive)));
            }
            var caretPoint = _view.Caret.Position.Point.GetPoint(_sourceBuffer, _view.Caret.Position.Affinity);
            var ret = NormalizedSnapshotSpanCollection.Overlap(spans, wordSpans)
                .Where(s => !caretPoint.HasValue || caretPoint.Value < s.Start || s.End < caretPoint.Value)
                .Select(s => new TagSpan<IClassificationTag>(s, new ClassificationTag(_classificationTypes[classificationType])));
            return ret;
        }
    }
}