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
        private NormalizedSnapshotSpanCollection _wordSpans;
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
            _wordSpans = new NormalizedSnapshotSpanCollection();
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
            var findDatas = _options.DimPatterns.Select(p => new FindData(p, snapshot, FindOptions.UseRegularExpressions, _textStructureNavigator));
            var results = findDatas.SelectMany(d => _textSearchService.FindAll(d));
            
            var newSpans = new NormalizedSnapshotSpanCollection(results);
            lock (_updateLock)
            {
                _wordSpans = newSpans;
                var temp = TagsChanged;
                temp?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_sourceBuffer.CurrentSnapshot, 0, _sourceBuffer.CurrentSnapshot.Length)));
            }
        }

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (_wordSpans == null)
            {
                return Enumerable.Empty<ITagSpan<IClassificationTag>>();
            }

            if (spans.Count == 0 || _wordSpans.Count == 0)
            {
                return Enumerable.Empty<ITagSpan<IClassificationTag>>();
            }

            var wordSpans = _wordSpans;
            if (spans[0].Snapshot != wordSpans[0].Snapshot)
            {
                wordSpans = new NormalizedSnapshotSpanCollection(wordSpans.Select(s => s.TranslateTo(spans[0].Snapshot, SpanTrackingMode.EdgeInclusive)));
            }

            var caretPoint = _view.Caret.Position.Point.GetPoint(_sourceBuffer, _view.Caret.Position.Affinity);
            var ret = NormalizedSnapshotSpanCollection.Overlap(spans, wordSpans)
                .Where(s => !caretPoint.HasValue || caretPoint.Value < s.Start || s.End < caretPoint.Value)
                .Select(s => new TagSpan<IClassificationTag>(s, new ClassificationTag(_classificationTypes[TagTypes.Dim])));
            return ret;
        }
    }
}