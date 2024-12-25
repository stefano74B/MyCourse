using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MyCourse.Customizations.TagHelpers
{
    // se non si vogliono rispettare i nomi uguali in questo modo indico come si chiama il tag da utilizzare
    // [HtmlTargetElement("stars")]
    public class RatingTagHelper : TagHelper
    {

        // si può usare una property con lo stesso nome passato nel tag helpers
        public double Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // nel caso non si rispettino i nomi, si può indicare come si deve chiamare il tag in output
            // output.TagMane = "span";

            // da usare se i nomi delle proprietà sono diversi, altrimenti usare la proprietà scritta sopra
            // double value = (double) context.AllAttributes["value"].Value;

            for(int i = 1; i <= 5; i++)
            {
                if (Value >= i)
                {
                    output.Content.AppendHtml("<i class=\"fas fa-star\"></i>");
                }
                else if (Value > i - 1)
                {
                    output.Content.AppendHtml("<i class=\"fas fa-star-half-alt\"></i>");
                }
                else
                {
                    output.Content.AppendHtml("<i class=\"far fa-star\"></i>");
                }
            }
        }

    }
}