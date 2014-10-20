using System;
using System.Globalization;
using MuonLab.Testing;
using MuonLab.Web.Xhtml.Components;
using MuonLab.Web.Xhtml.Configuration;

namespace MuonLab.Web.Xhtml.Tests.Components.FormattableComponentSpecifications
{
    public abstract class FormattableComponentTSpecification<TComponent, TProperty> : Specification where TComponent : IFormattableComponent<TProperty>
    {
        protected IFormattableComponent<TProperty> component;

        protected override void Given()
        {
			this.component = (TComponent)Activator.CreateInstance(typeof(TComponent), Dependency<ITermResolver>(), new CultureInfo("en-GB"));
			if(this.component is IVisibleComponent)
				((IVisibleComponent)component).WithRenderingOrder(ComponentPart.Component);
        }

        protected abstract string expectedRendering { get; }
    }
}