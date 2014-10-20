using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using MuonLab.Web.Xhtml.Components;
using MuonLab.Web.Xhtml.Components.Implementations;
using MuonLab.Web.Xhtml.Configuration;
using MuonLab.Web.Xhtml.Properties;

namespace MuonLab.Web.Xhtml
{
	public sealed class ComponentFactory<TViewModel> : IComponentFactory<TViewModel>
	{
		readonly IFormConfiguration configuration;
		public CultureInfo Culture { get; private set; }
		public IComponentNameResolver NameResolver { get; set; }
		public IComponentIdResolver IdResolver { get; private set; }
		public ITermResolver TermResolver { get; private set; }
		public IErrorProvider ErrorProvider { get; private set; }

		public ComponentFactory(
			IFormConfiguration configuration,
			IComponentNameResolver nameResolver,
			IComponentIdResolver idResolver,
			ITermResolver termResolver,
			IErrorProvider errorProvider, CultureInfo culture)
		{
			this.ErrorProvider = errorProvider;
			this.configuration = configuration;
			this.Culture = culture;
			this.NameResolver = nameResolver;
			this.IdResolver = idResolver;
			this.TermResolver = termResolver;
		}

		public IHiddenFieldComponent<TProperty> HiddenFieldFor<TProperty>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity, Func<TProperty, string> toStringFunc)
		{
			var hidden = new HiddenFieldComponent<TViewModel, TProperty>(toStringFunc);
			InitializeComponent(hidden, entity, property);
			return hidden;
		}

		public ITextBoxComponent<TProperty> TextBoxFor<TProperty>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity)
		{
			var textBox = new TextBoxComponent<TViewModel, TProperty>(this.TermResolver, this.Culture);
			InitializeComponent(textBox, entity, property);
			return textBox;
		}

		public IEmailBoxComponent<TProperty> EmailBoxFor<TProperty>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity)
		{
			var textBox = new EmailBoxComponent<TViewModel, TProperty>(this.TermResolver, this.Culture);
			InitializeComponent(textBox, entity, property);
			return textBox;
		}

		public IPasswordBoxComponent PasswordBoxFor(Expression<Func<TViewModel, string>> property, TViewModel entity)
		{
			var passwordBox = new PasswordBoxComponent<TViewModel>(this.TermResolver, this.Culture);
			InitializeComponent(passwordBox, entity, property);
			return passwordBox;
		}

		public ITextAreaComponent<TProperty> TextAreaFor<TProperty>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity)
		{
			var textAreaComponent = new TextAreaComponent<TViewModel, TProperty>(this.TermResolver, this.Culture);

			InitializeComponent(textAreaComponent, entity, property);

			return textAreaComponent;
		}

		public IDropDownComponent<TProperty> DropDownFor<TProperty, TData>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity, IEnumerable<TData> items, Func<TProperty, string> propertyValueFunc, Func<TData, string> itemValueFunc, Func<TData, string> itemTextFunc, Func<TData, object> itemHtmlAttributes)
		{
			var dropDown = new DropDownComponent<TViewModel, TProperty, TData>(this.TermResolver, this.Culture, items, propertyValueFunc, itemValueFunc, itemTextFunc, itemHtmlAttributes);

			InitializeComponent(dropDown, entity, property);

			return dropDown;
		}

		public ICheckBoxListComponent CheckBoxListFor<TProperty, TData>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity, IEnumerable<TData> items, Func<TData, string> itemValueFunc, Func<TData, string> itemTextFunc, Func<TProperty, TData, bool> itemIsValue)
		{
			var checkBoxComponent = new CheckBoxListComponent<TViewModel, TProperty, TData>(this.TermResolver, this.Culture, items, itemValueFunc, itemTextFunc, itemIsValue);

			InitializeComponent(checkBoxComponent, entity, property);

			return checkBoxComponent;
		}

		public ICheckBoxComponent<bool> CheckBoxFor(Expression<Func<TViewModel, bool>> property, TViewModel entity)
		{
			var checkBoxComponent = new CheckBoxForBoolComponent<TViewModel>(this.TermResolver, this.Culture);

			InitializeComponent(checkBoxComponent, entity, property);

			return checkBoxComponent;
		}
		
		public ICheckBoxComponent<IEnumerable<TInner>> CheckBoxFor<TInner>(Expression<Func<TViewModel, IEnumerable<TInner>>> property, TViewModel entity, TInner value)
		{
			var checkBoxComponent = new CheckBoxForEnumerableComponent<TViewModel, TInner>(this.TermResolver, this.Culture, value);

			InitializeComponent(checkBoxComponent, entity, property);

			return checkBoxComponent;
		}

		public IRadioButtonListComponent RadioButtonListFor<TProperty, TData>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity, IEnumerable<TData> items, Func<TData, string> itemValueFunc, Func<TData, string> itemTextFunc)
		{
			var radioButtonList = new RadioButtonListComponent<TViewModel, TProperty, TData>(this.TermResolver, this.Culture, items, itemValueFunc, itemTextFunc);

			InitializeComponent(radioButtonList, entity, property);

			return radioButtonList;
		}

		public IFileUploadComponent FileUploadFor<TProperty>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity)
		{
			var fileUploadComponent = new FileUploadComponent<TViewModel, TProperty>(this.TermResolver, this.Culture);

			InitializeComponent(fileUploadComponent, entity, property);

			return fileUploadComponent;
		}

		public string ValidationMessageFor<TProperty>(Expression<Func<TViewModel, TProperty>> property, TViewModel entity)
		{
			var name = this.NameResolver.ResolveName(property);
			var state = this.ErrorProvider.GetStateFor(name);
			var errors = this.ErrorProvider.GetErrorsFor(name);

			return new ValidationMessageRenderer().Render(state, ValidationMarkerMode.Always, errors);
		}

		public string ValidationMessageFor(string name, TViewModel entity)
		{
			var state = this.ErrorProvider.GetStateFor(name);
			var errors = this.ErrorProvider.GetErrorsFor(name);

			return new ValidationMessageRenderer().Render(state, ValidationMarkerMode.Always, errors);
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
			if (this.configuration != null)
				this.configuration.Initialize(component);
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
			component.WithLabel(this.TermResolver.ResolveLabel(property, this.Culture)).WithoutLabel();

			// run the config on the component
			if (this.configuration != null)
				this.configuration.Initialize(component);

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