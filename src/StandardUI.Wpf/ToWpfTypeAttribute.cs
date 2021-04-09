using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.StandardUI.Wpf
{
    /// <summary>
    /// Indicates the given type should be projected from a StandardUI Element to a Wpf control
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ToWpfTypeAttribute : Attribute
    {
        private string wpfTypeName;

        public ToWpfTypeAttribute(string wpfTypeName)
        {
            this.wpfTypeName = wpfTypeName;
        }

        public string WpfTypeName => wpfTypeName;
    }
}
