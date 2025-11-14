using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace ChatElioraSystem.Presentation.Xaml
{
    [MarkupExtensionReturnType(typeof(object))]
    public class Resolve<T> : MarkupExtension where T : class
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return null!;
            if (App.Host is null)
                throw new InvalidOperationException("App.Host nie został jeszcze zainicjalizowany.");
            return App.Host.Services.GetRequiredService<T>();
        }
    }



}
