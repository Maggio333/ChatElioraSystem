using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatElioraSystem.Presentation.Services
{
    public interface IThemeService
    {
        //string Current { get; }
        //void Switch(string themeName);   // Light/Dark/… 
        void Apply(Application app);     // (re)wpięcie ResourceDictionaries
    }
}
