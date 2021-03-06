using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using MustardBlack.Html.Forms.Components;
using MustardBlack.Html.Forms.Configuration;

namespace MustardBlack.Html.Forms
{
	public sealed class ComponentFactory<TViewModel> : IComponentFactory<TViewModel>
	{
		public IFormConfiguration Configuration { get; }
		public CultureInfo Culture { get; }
		public IComponentNameResolver NameResolver { get; set; }
		public IComponentIdResolver IdResolver { get; }
		public IErrorProvider ErrorProvider { get; }
		public IValidationMessageRenderer ValidationMessageRenderer { get; }

		public ComponentFactory(
			IFormConfiguration configuration,
			IComponentNameResolver nameResolver,
			IComponentIdResolver idResolver,
			IErrorProvider errorProvider, 
			IValidationMessageRenderer validationMessageRenderer,
			CultureInfo culture)
		{
			this.ErrorProvider = errorProvider;
			ValidationMessageRenderer = validationMessageRenderer;
			this.Configuration = configuration;
			this.Culture = culture;
			this.NameResolver = nameResolver;
			this.IdResolver = idResolver;
		}

		public IHiddenFieldComponent<TProperty> HiddenFieldFor<TProperty>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity, Func<TProperty, string> toStringFunc)
		{
			var hidden = new HiddenFieldComponent<TViewModel, TProperty>(toStringFunc);
			InitializeComponent(hidden, entity, property);
			return hidden;
		}

		public ITextBoxComponent<TProperty> TextBoxFor<TProperty>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity)
		{
			var textBox = new TextBoxComponent<TViewModel, TProperty>(this.Configuration.TermResolver, this.ValidationMessageRenderer, this.Culture);
			InitializeComponent(textBox, entity, property);
			return textBox;
		}

		public IEmailBoxComponent<TProperty> EmailBoxFor<TProperty>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity)
		{
			var textBox = new EmailBoxComponent<TViewModel, TProperty>(this.Configuration.TermResolver, this.ValidationMessageRenderer, this.Culture);
			InitializeComponent(textBox, entity, property);
			return textBox;
		}

        public INumberBoxComponent<TProperty> NumberBoxFor<TProperty>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity)
        {
			var numberBox = new NumberBoxComponent<TViewModel, TProperty>(this.Configuration.TermResolver, this.ValidationMessageRenderer, this.Culture);
            InitializeComponent(numberBox, entity, property);
            return numberBox;
		}

        public IPasswordBoxComponent PasswordBoxFor(Expression<Func<TViewModel, string>> property, TViewModel entity)
		{
			var passwordBox = new PasswordBoxComponent<TViewModel>(this.Configuration.TermResolver, this.ValidationMessageRenderer, this.Culture);
			InitializeComponent(passwordBox, entity, property);
			return passwordBox;
		}

		public ITextAreaComponent<TProperty> TextAreaFor<TProperty>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity)
		{
			var textAreaComponent = new TextAreaComponent<TViewModel, TProperty>(this.Configuration.TermResolver, this.ValidationMessageRenderer, this.Culture);

			InitializeComponent(textAreaComponent, entity, property);

			return textAreaComponent;
		}

		public IListBoxComponent<IEnumerable<TPropertyInner>> ListBoxFor<TPropertyInner, TData>(Expression<Func<TViewModel, IEnumerable<TPropertyInner>>> property, TViewModel entity, IEnumerable<TData> items, Func<TPropertyInner, string> propValueFunc, Func<TData, TPropertyInner> itemValueFunc, Func<TData, string> itemTextFunc, Func<TData, object> itemHtmlAttributes)
		{
			var dropDown = new ListBoxComponent<TViewModel, TPropertyInner, TData>(this.Configuration.TermResolver, this.ValidationMessageRenderer, this.Culture, items, itemValueFunc, propValueFunc, itemTextFunc, itemHtmlAttributes);

			InitializeComponent(dropDown, entity, property);

			return dropDown;
		}
		public IDropDownComponent<TProperty> DropDownFor<TProperty, TData>(Expression<Func<TViewModel, TProperty>> property,
			TViewModel entity, IEnumerable<TData> items, Func<TProperty, string> propValueFunc, Func<TData, TProperty> itemValueFunc, Func<TData, string> itemTextFunc,
			Func<TData, object> itemHtmlAttributes)
		{
			var dropDown = new DropDownComponent<TViewModel, TProperty, TData>(this.Configuration.TermResolver, this.ValidationMessageRenderer, this.Culture, items, propValueFunc, itemValueFunc, itemTextFunc, itemHtmlAttributes);

			InitializeComponent(dropDown, entity, property);

			return dropDown;
		}

		public ICheckBoxListComponent CheckBoxListFor<TProperty, TData>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity, IEnumerable<TData> items, Func<TData, string> itemValueFunc, Func<TData, string> itemTextFunc, Func<TProperty, TData, bool> itemIsValue)
		{
			var checkBoxComponent = new CheckBoxListComponent<TViewModel, TProperty, TData>(this.Configuration.TermResolver, this.ValidationMessageRenderer, this.Culture, items, itemValueFunc, itemTextFunc, itemIsValue);

			InitializeComponent(checkBoxComponent, entity, property);

			return checkBoxComponent;
		}

		public ICheckBoxComponent<bool> CheckBoxFor(Expression<Func<TViewModel, bool>> property, TViewModel entity)
		{
			var checkBoxComponent = new CheckBoxForBoolComponent<TViewModel>(this.Configuration.TermResolver, this.ValidationMessageRenderer, this.Culture);

			InitializeComponent(checkBoxComponent, entity, property);

			return checkBoxComponent;
		}
		
		public ICheckBoxComponent<IEnumerable<TInner>> CheckBoxFor<TInner>(Expression<Func<TViewModel, IEnumerable<TInner>>> property, TViewModel entity, TInner value)
		{
			var checkBoxComponent = new CheckBoxForEnumerableComponent<TViewModel, TInner>(this.Configuration.TermResolver, this.ValidationMessageRenderer, this.Culture, value);

			InitializeComponent(checkBoxComponent, entity, property);

			return checkBoxComponent;
		}

		public IRadioButtonListComponent RadioButtonListFor<TProperty, TData>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity, IEnumerable<TData> items, Func<TData, string> itemValueFunc, Func<TData, string> itemTextFunc)
		{
			var radioButtonList = new RadioButtonListComponent<TViewModel, TProperty, TData>(this.Configuration.TermResolver, this.ValidationMessageRenderer, this.Culture, items, itemValueFunc, itemTextFunc);

			InitializeComponent(radioButtonList, entity, property);

			return radioButtonList;
		}

		public IFileUploadComponent FileUploadFor<TProperty>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity)
		{
			var fileUploadComponent = new FileUploadComponent<TViewModel, TProperty>(this.Configuration.TermResolver, this.ValidationMessageRenderer, this.Culture);

			InitializeComponent(fileUploadComponent, entity, property);

			return fileUploadComponent;
		}

		public string ValidationMessageFor<TProperty>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity)
		{
			var name = this.NameResolver.ResolveName(property);
			var state = this.ErrorProvider.GetStateFor(name);
			var errors = this.ErrorProvider.GetErrorsFor(name);

			return this.ValidationMessageRenderer.Render(state, ValidationMarkerMode.Always, errors, null);
		}

		public string ValidationMessageFor(string name, TViewModel entity)
		{
			var state = this.ErrorProvider.GetStateFor(name);
			var errors = this.ErrorProvider.GetErrorsFor(name);

			return this.ValidationMessageRenderer.Render(state, ValidationMarkerMode.Always, errors, null);
		}

		public void InitializeComponent<TComponentViewModel, TProperty>(Component<TComponentViewModel, TProperty> component, TComponentViewModel viewModel, Expression<Func<TComponentViewModel, TProperty>> property)
		{
			// Set the Name
			component.WithName(this.NameResolver.ResolveName(property));

			// Set the Value
			if (!ReferenceEquals(viewModel, null))
			{
				try
				{
					component.WithValue(property.Compile().Invoke(viewModel));
				}
				catch (NullReferenceException e)
				{
					throw new ComponentRenderingException("Could not set component value, some part of the property chain is null: " + property, e);
				}
			}

			// Set the Id
			component.WithId(this.IdResolver.ResolveId(property, component.ControlPrefix));

			// run the config on the component
			this.Configuration.Initialize(component);
		}

		public void InitializeComponent<TComponentViewModel, TProperty>(VisibleComponent<TComponentViewModel, TProperty> component, TComponentViewModel viewModel, Expression<Func<TComponentViewModel, TProperty>> property)
		{
			// Set the Name
			component.WithName(this.NameResolver.ResolveName(property));

			// Set the Value
			if (!ReferenceEquals(viewModel, null))
			{
				try
				{
					component.WithValue(property.Compile().Invoke(viewModel));
				}
				catch (NullReferenceException e)
				{
					throw new ComponentRenderingException("Could not set component value, some part of the property chain is null: " + property, e);
				}
			}

			// Set the Id
			component.WithId(this.IdResolver.ResolveId(property, component.ControlPrefix));

			// set the default label, then hide it as it should be hidden by default.
			component.WithLabel(this.Configuration.TermResolver.ResolveTerm(property)).WithoutLabel();

			// run the config on the component
			this.Configuration.Initialize(component);

			component.OnPrepareForRender += component_OnPrepareForRender;
		}

		void component_OnPrepareForRender(object sender, EventArgs e)
		{
			var component = sender as IVisibleComponent;

			var state = this.ErrorProvider.GetStateFor(component.Name);
			if (state != ComponentState.Unvalidated)
			{
				var attemptedValue = this.ErrorProvider.GetAttemptedValueFor(component.Name);
				if (attemptedValue != null)
					component.WithAttemptedValue(attemptedValue);
			}

			component.WithState(state, this.ErrorProvider.GetErrorsFor(component.Name));
		}
	}
}