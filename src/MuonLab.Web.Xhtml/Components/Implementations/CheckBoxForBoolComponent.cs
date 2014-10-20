using System.Collections.Generic;
using System.Globalization;
using MuonLab.Web.Xhtml.Configuration;

namespace MuonLab.Web.Xhtml.Components.Implementations
{
	public class CheckBoxForBoolComponent<TViewModel> :
		VisibleComponent<TViewModel, bool>,
		ICheckBoxComponent<bool>
    {
		public CheckBoxForBoolComponent(ITermResolver termResolver, CultureInfo culture) : base(termResolver, culture)
		{
		}

		public override string ControlPrefix
        {
            get { return "chk"; }
        }

    	protected override string RenderComponent()
        {
            var checkbox = new TagBuilder("input", this.htmlAttributes);
            checkbox.HtmlAttributes.Add("type", "checkbox");
            checkbox.HtmlAttributes.Add("value", "TRUE");

            if(this.value)
                checkbox.HtmlAttributes.Add("checked", "checked");

			if (this.htmlAttributes.ContainsKey("disabled"))
				return checkbox.ToString();

            var dictionary = new Dictionary<string, object>
                                 {
                                     {"name", this.Name}, 
                                     {"type", "hidden"}, 
                                     {"value", "FALSE"}
                                 };
            var hidden = new TagBuilder("input", dictionary);

            return checkbox.ToString() + hidden.ToString();
        }
    }
}