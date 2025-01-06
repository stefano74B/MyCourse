using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

// Abbiamo creato questo tag helper personalizzato per ovviare al problema delle input
// numeriche con decimali, che con alcune culture delle lingue, per il doscorso
// del . e della , non carica e non salva i dati correttamente

// da utilizzare nei tag input numerici con decimali con attributo asp-for

namespace MyCourse.Customizations.TagHelpers
{
    [HtmlTargetElement("input", Attributes = "asp-for")]
    public class InputNumberTagHelper : TagHelper
    {
        public override int Order => int.MaxValue;

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            bool isNumberInputType = output.Attributes.Any(attribute => "type".Equals(attribute.Name, StringComparison.InvariantCultureIgnoreCase) && "number".Equals(attribute.Value as string, StringComparison.InvariantCultureIgnoreCase));
            if (!isNumberInputType)
            {
                return;
            }
            if (For.ModelExplorer.ModelType != typeof(decimal))
            {
                return;
            }
            decimal value = (decimal) For.Model;
            string formattedValue = value.ToString("F2", CultureInfo.InvariantCulture);
            output.Attributes.SetAttribute("value", formattedValue);
		}
    }
}