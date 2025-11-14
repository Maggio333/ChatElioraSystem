using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatElioraSystem.Presentation.Services
{
    public sealed class ThemeService : IThemeService
    {
        // ZMIEŃ "YourApp" na *nazwę Twojego assembly* (Project/Assembly name)
        private static readonly Uri ColorsUri =
            new("pack://application:,,,/ChatElioraSystem;component/Presentation/Themes/Colors.xaml", UriKind.Absolute);
        private static readonly Uri TogglesUri =
            new("pack://application:,,,/ChatElioraSystem;component/Presentation/Styles/ToggleButtonStyles.xaml", UriKind.Absolute);

        private readonly List<ResourceDictionary> _loaded = new();

        public void Apply(Application app)
        {
            // wyczyść poprzednie nasze słowniki (jeśli były)
            foreach (var rd in _loaded)
                app.Resources.MergedDictionaries.Remove(rd);
            _loaded.Clear();

            // KOLEJNOŚĆ: najpierw kolory, potem style (bo style odwołują się do kolorów)
            foreach (var uri in new[] { ColorsUri, TogglesUri })
            {
                var rd = new ResourceDictionary { Source = uri };
                app.Resources.MergedDictionaries.Add(rd);
                _loaded.Add(rd);
            }
        }
    }
    //public sealed class ThemeService : IThemeService
    //{
    //    private readonly Dictionary<string, Uri[]> _themes = new()
    //    {
    //        ["Light"] = new[]
    //        {
    //        new Uri("pack://application:,,,/ChatElioraSystem;component/Styles/ToggleButtonStyles.xaml"),
    //        new Uri("pack://application:,,,/ChatElioraSystem;component/Themes/Colors.xaml"),

    //    },
    //        ["Dark"] = new[]
    //        {
    //        new Uri("pack://application:,,,/YourApp;component/Themes/Base.xaml"),
    //        new Uri("pack://application:,,,/YourApp;component/Themes/Colors.Dark.xaml"),
    //        new Uri("pack://application:,,,/YourApp;component/Themes/Controls.xaml"),
    //    },
    //    };

    //    private readonly List<ResourceDictionary> _loaded = new();
    //    public string Current { get; private set; } = "Light";

    //    public void Apply(Application app)
    //    {
    //        foreach (var d in _loaded) app.Resources.MergedDictionaries.Remove(d);
    //        _loaded.Clear();

    //        foreach (var uri in _themes[Current])
    //        {
    //            var rd = new ResourceDictionary { Source = uri };
    //            _loaded.Add(rd);
    //            app.Resources.MergedDictionaries.Add(rd);
    //        }
    //    }

    //    public void Switch(string themeName)
    //    {
    //        if (!_themes.ContainsKey(themeName) || themeName == Current) return;
    //        Current = themeName;
    //        Apply(Application.Current);
    //    }
    //}

}
