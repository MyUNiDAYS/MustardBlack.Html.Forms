using System;
using System.Collections.Generic;
using System.Globalization;
using MustardBlack.Html.Forms.Configuration;

namespace MustardBlack.Html.Forms.Components
{
	public class DropDownComponent<TViewModel, TData> :
		VisibleComponent<TViewModel, IEnumerable<TData>>,
		IDropDownComponent<TData>
	{
		readonly IEnumerable<TData> items;
		readonly Func<TData, string> itemValueFunc;
		readonly Func<TData, string> itemTextFunc;
		readonly Func<TData, object> itemHtmlAttributes;
		protected bool showNullOption;
		protected string nullOptionText;
		protected TData nullOptionValue;
		protected bool nullOptionValueSet;

		public DropDownComponent(ITermResolver termResolver, CultureInfo culture, IEnumerable<TData> items, 
			Func<TData, string> itemValueFunc, Func<TData, string> itemTextFunc, Func<TData, object> itemHtmlAttributes)
			: base(termResolver, culture)
		{
			if (items == null)
				throw new ArgumentNullException("items", "DropDown's data items enumerable cannot be null");

			this.items = items;
			this.itemValueFunc = itemValueFunc;
			this.itemTextFunc = itemTextFunc;
			this.itemHtmlAttributes = itemHtmlAttributes;
		}

		public override string ControlPrefix
		{
			get { return "ddl"; }
		}

		/// <summary>
		/// Adds a Null option with the default null option text.
		/// </summary>
		/// <returns></returns>
		public virtual IDropDownComponent WithNullOption()
		{
			return this.WithNullOption(this.termResolver.ResolveTerm("Choose", this.culture));
		}

		/// <summary>
		/// Adds a Null option with and sets the null option text.
		/// </summary>
		/// <param name="nullOptionText">The null option text.</param>
		/// <returns></returns>
		public virtual IDropDownComponent WithNullOption(string nullOptionText)
		{
			this.showNullOption = true;
			this.nullOptionText = nullOptionText;
			return this;
		}

		/// <summary>
		/// Adds a Null option with and sets the null option text.
		/// </summary>
		/// <param name="nullOptionText">The null option text.</param>
		/// <returns></returns>
		public virtual IDropDownComponent WithNullOption(string nullOptionText, TData nullOptionValue)
		{
			this.showNullOption = true;
			this.nullOptionText = nullOptionText;
			this.nullOptionValue = nullOptionValue;
			this.nullOptionValueSet = true;
			return this;
		}

		/// <summary>
		/// Removes a previously set null option
		/// </summary>
		/// <returns></returns>
		public virtual IDropDownComponent WithoutNullOption()
		{
			this.showNullOption = false;
			this.nullOptionValueSet = false;
			return this;
		}

		protected override string RenderComponent()
		{
			var builder = new TagBuilder("select", this.htmlAttributes);

			if (this.ariaLabel)
				this.htmlAttributes.Add("aria-label", this.termResolver.ResolveTerm(this.Label, this.culture));

			this.AddAriaDescribedBy();

			if (this.showNullOption)
			{
				var nullValue = this.nullOptionValueSet ? itemValueFunc(this.nullOptionValue) : string.Empty;
				var nullOptionBuilder = new TagBuilder("option", new Dictionary<string, object>
				{
					{"value", nullValue},
					{"data-null-value", "true"}
				});
				nullOptionBuilder.SetInnerText(this.nullOptionText);
				builder.InnerHtml = nullOptionBuilder.ToString();
			}

			foreach (var item in items)
			{
				var optionAttributes = this.itemHtmlAttributes != null ? this.itemHtmlAttributes(item) : null;

				var optionBuilder = new TagBuilder("option", optionAttributes);

				optionBuilder.HtmlAttributes.Add("value", this.itemValueFunc.Invoke(item));

				if (!ReferenceEquals(this.value, null) && Equals(this.value, item))
					optionBuilder.HtmlAttributes.Add("selected", new HtmlProperty());

				optionBuilder.SetInnerText(this.itemTextFunc.Invoke(item));

				builder.InnerHtml += optionBuilder.ToString();
			}

			return builder.ToString();
		}
	}
}