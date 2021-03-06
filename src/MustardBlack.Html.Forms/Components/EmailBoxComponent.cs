using System.Globalization;
using MustardBlack.Html.Forms.Configuration;

namespace MustardBlack.Html.Forms.Components
{
    public class EmailBoxComponent<TViewModel, TProperty> : 
		TextBoxComponent<TViewModel, TProperty>, 
		IEmailBoxComponent<TProperty> 
    {
	    public EmailBoxComponent(ITermResolver termResolver, IValidationMessageRenderer validationMessageRenderer, CultureInfo culture) : base(termResolver, validationMessageRenderer, culture)
	    {
	    }

	    public override string ControlPrefix => "txt";

	    protected override string RenderComponent()
        {
            string fieldValue;

            if (this.defaultAsEmpty && Equals(this.value, default(TProperty)))
                fieldValue = null;
            else
                fieldValue = this.FormatValue(this.value);

            if (this.attemptedValue != null)
                fieldValue = this.attemptedValue;

			if (this.useLabelForPlaceholder)
				this.htmlAttributes.Add("placeholder", this.termResolver.ResolveTerm(this.Label, this.culture));
			else if (!string.IsNullOrEmpty(this.placeholder))
				this.htmlAttributes.Add("placeholder", this.termResolver.ResolveTerm(this.placeholder, this.culture));

	        if (this.ariaLabel)
				this.htmlAttributes.Add("aria-label", this.termResolver.ResolveTerm(string.IsNullOrEmpty(this.Label) ? this.placeholder : this.Label, this.culture));

			this.AddAriaDescribedBy();

            this.htmlAttributes.Add("type", "email");
            this.htmlAttributes.Add("value", fieldValue);
            var builder = new TagBuilder("input", this.htmlAttributes);
            return builder.ToString();
        }
    }
}