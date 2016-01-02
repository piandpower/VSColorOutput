using System;
using System.ComponentModel.Composition;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using VSColorOutput.State;

#pragma warning disable 649

namespace VSColorOutput.Output.ColorClassifier
{
    [ContentType("output")]
    [Export(typeof(IClassifierProvider))]
    public class OutputClassifierProvider : IClassifierProvider
    {
        [Import] internal SVsServiceProvider ServiceProvider;
        [Import] internal IClassificationTypeRegistryService ClassificationRegistry;
        [Import] internal IClassificationFormatMapService ClassificationFormatMapService;

        private static BuildEvents.BuildEvents _buildEvents;
        private static OutputClassifier _outputClassifier;

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            try
            {
                if (_buildEvents == null)
                {
                    Interlocked.CompareExchange(
                        ref _buildEvents, 
                        new BuildEvents.BuildEvents(), 
                        null);

                    _buildEvents.Initialize(ServiceProvider);
                }

                if (_outputClassifier == null)
                {
                    Interlocked.CompareExchange(
                        ref _outputClassifier,
                        new OutputClassifier(), 
                        null);

                    _outputClassifier.Initialize(ClassificationRegistry, ClassificationFormatMapService);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.ToString());
                throw;
            }
            return _outputClassifier;
        }
    }
}