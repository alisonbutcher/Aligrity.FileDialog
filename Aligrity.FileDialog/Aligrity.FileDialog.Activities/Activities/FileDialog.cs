using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Aligrity.FileDialog.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using Aligrity.FileDialog.Enums;
using Microsoft.WindowsAPICodePack.Dialogs;

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
        public FileDialogs DialogType { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileDialog_DefaultPath_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileDialog_DefaultPath_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> DefaultPath { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileDialog_FileTypeFilter_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileDialog_FileTypeFilter_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public FileTypeFilters FileTypeFilter { get; set; }

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


            // Set a timeout on the execution
            var task = ExecuteWithTimeout(context, cancellationToken);
            if (await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)) != task) throw new TimeoutException(Resources.Timeout_Error);

            // Outputs
            return (ctx) => {
                OutputPath.Set(ctx, task.Result);
            };
        }

        private async Task<string> ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default)
        {
            var title = Title.Get(context);
            var defaultPath = DefaultPath.Get(context);
            var output = string.Empty;
            var flt = new CommonFileDialogFilter();
            dynamic dialog = new CommonOpenFileDialog();

            // Selection of File Dialog type (leave default Open / Save)
            if (DialogType == FileDialogs.Save)
            {
                dialog = new CommonSaveFileDialog();
            }

            // Dialog params
            if (title != string.Empty) dialog.Title = title;
            if (defaultPath != string.Empty)
            {
                dialog.InitialDirectory = defaultPath;
            }

            // Dialog File Filter Types
            switch (this.FileTypeFilter)
            {
                case FileTypeFilters.Excel:
                    flt.Extensions.Add("xlsx");
                    flt.Extensions.Add("xlsm");
                    flt.Extensions.Add("xls");
                    flt.DisplayName = "Excel";
                    break;
                case FileTypeFilters.Word:
                    flt.Extensions.Add("docx");
                    flt.Extensions.Add("doc");
                    flt.DisplayName = "Word";
                    break;
                case FileTypeFilters.PowerPoint:
                    flt.Extensions.Add("pptx");
                    flt.Extensions.Add("ppt");
                    flt.DisplayName = "Powerpoint";
                    break;
                case FileTypeFilters.CSV:
                    flt.Extensions.Add("csv");
                    flt.DisplayName = "Comma Separated Variable";
                    break;
                case FileTypeFilters.Text:
                    flt.Extensions.Add("txt");
                    flt.DisplayName = "Text";
                    break;
                default:
                    flt.Extensions.Add("*");
                    flt.DisplayName = "All Files";
                    break;
            }

            dialog.Filters.Add(flt);

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                output = dialog.FileName;
            }

            return await Task.FromResult(output);
        }

        #endregion
    }
}

