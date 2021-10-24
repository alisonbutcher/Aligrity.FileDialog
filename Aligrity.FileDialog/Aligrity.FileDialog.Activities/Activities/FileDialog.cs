using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Aligrity.FileDialog.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Aligrity.FileDialog.Activities
{
    [LocalizedDisplayName(nameof(Resources.FileDialog_DisplayName))]
    [LocalizedDescription(nameof(Resources.FileDialog_Description))]
    public class FileDialog : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.Timeout_DisplayName))]
        [LocalizedDescription(nameof(Resources.Timeout_Description))]
        public InArgument<int> TimeoutMS { get; set; } = 60000;

        [LocalizedDisplayName(nameof(Resources.FileDialog_Title_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileDialog_Title_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Title { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileDialog_DialogType_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileDialog_DialogType_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public InArgument<int> DialogType { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileDialog_DefaultPath_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileDialog_DefaultPath_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> DefaultPath { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileDialog_FileTypeFilter_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileDialog_FileTypeFilter_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public InArgument<int> FileTypeFilter { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileDialog_OutputPath_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileDialog_OutputPath_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> OutputPath { get; set; }

        #endregion


        #region Constructors

        public FileDialog()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (DialogType == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(DialogType)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var timeout = TimeoutMS.Get(context);
            var title = Title.Get(context);
            var dialogType = DialogType.Get(context);
            var defaultPath = DefaultPath.Get(context);
            var fileTypeFilter = FileTypeFilter.Get(context);

            // Set a timeout on the execution
            var task = ExecuteWithTimeout(context, cancellationToken);
            if (await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)) != task) throw new TimeoutException(Resources.Timeout_Error);

            // Outputs
            return (ctx) => {
                OutputPath.Set(ctx, null);
            };
        }

        private async Task ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default)
        {
            ///////////////////////////
            // Add execution logic HERE
            ///////////////////////////
        }

        #endregion
    }
}

