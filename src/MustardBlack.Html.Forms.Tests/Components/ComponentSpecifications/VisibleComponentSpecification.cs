using System;
using System.Globalization;
using MustardBlack.Html.Forms.Components;
using MustardBlack.Html.Forms.Configuration;

namespace MustardBlack.Html.Forms.Tests.Components.ComponentSpecifications
{
	public abstract class VisibleComponentSpecification<TComponent> : Specification where TComponent : IVisibleComponent
	{
        protected TComponent component;
		protected CultureInfo culture;
		protected ITermResolver termResolver;

		protected override void Given()
		{
			culture = new CultureInfo("en-GB");
			termResolver = this.Dependency<ITermResolver>();
			this.component = (TComponent)Activator.CreateInstance(typeof(TComponent), termResolver, culture);
			this.component.WithRenderingOrder(ComponentPart.Component);
		}

		protected abstract string expectedRendering { get; }

		[Then]
		public void TheCorrectMarkupShouldBeRendered()
		{
			component.ToString().ShouldEqual(expectedRendering);
		}
	}
}