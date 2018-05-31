using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using SeeSharper.SyntaxColoring.Tags;

namespace SeeSharper.OccurrenceColoring
{
    internal class OccurrenceTagger : ITagger<IClassificationTag>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private readonly ITextBuffer _sourceBuffer;
        private readonly ITextSearchService _textSearchService;
        private readonly ITextStructureNavigator _textStructureNavigator;
        private NormalizedSnapshotSpanCollection _wordSpans;
        private readonly IDictionary<string, IClassificationType> _classificationTypes = new Dictionary<string, IClassificationType>();
        private readonly object _updateLock = new object();

        public OccurrenceTagger(ITextView view, ITextBuffer sourceBuffer, ITextSearchService textSearchService,
            ITextStructureNavigator textStructureNavigator, IClassificationTypeRegistryService registry)
        {
            _sourceBuffer = sourceBuffer;
            _textSearchService = textSearchService;
            _textStructureNavigator = textStructureNavigator;
            _wordSpans = new NormalizedSnapshotSpanCollection();
            _classificationTypes[TagTypes.Dim] = registry.GetClassificationType(TagTypes.Dim);
            _classificationTypes[TagTypes.Highlight] = registry.GetClassificationType(TagTypes.Highlight);
            view.LayoutChanged += LayoutChanged;
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
            var data = new FindData("Hest\\d{3}", snapshot, FindOptions.UseRegularExpressions, _textStructureNavigator);
            var result = _textSearchService.FindAll(data);
            var newSpans = new NormalizedSnapshotSpanCollection(result);
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
                wordSpans = new NormalizedSnapshotSpanCollection(wordSpans.Select(s => s.TranslateTo(spans[0].Snapshot, SpanTrackingMode.EdgeExclusive)));
            }

            return NormalizedSnapshotSpanCollection.Overlap(spans, wordSpans)
                .Select(s => s.Snapshot.GetLineFromPosition(s.Start.Position))
                .Select(l => new TagSpan<IClassificationTag>(l.Extent, new ClassificationTag(_classificationTypes[TagTypes.Dim])));
        }
    }
}