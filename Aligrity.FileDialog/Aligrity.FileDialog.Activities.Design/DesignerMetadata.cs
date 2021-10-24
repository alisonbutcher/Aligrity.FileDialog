using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using Aligrity.FileDialog.Activities.Design.Designers;
using Aligrity.FileDialog.Activities.Design.Properties;

namespace Aligrity.FileDialog.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(FileDialog), categoryAttribute);
            builder.AddCustomAttributes(typeof(FileDialog), new DesignerAttribute(typeof(FileDialogDesigner)));
            builder.AddCustomAttributes(typeof(FileDialog), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
