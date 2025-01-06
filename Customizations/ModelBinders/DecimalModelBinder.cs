using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

// Abbiamo creato questo tag helper personalizzato per ovviare al problema delle input
// numeriche con decimali, che con alcune culture delle lingue, per il doscorso
// del . e della , non carica e non salva i dati correttamente

// risistema il valore che torna dal tag helper personalizzato, per salvarlo nel DB
// se il tipo è decimal, la mia applicazione userà questo Model Binding e non quello standard

namespace MyCourse.Customizations.ModelBinders
{
    public class DecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decimalValue)) 
            {
                bindingContext.Result = ModelBindingResult.Success(decimalValue);
            }
            return Task.CompletedTask;
        }
    }
}