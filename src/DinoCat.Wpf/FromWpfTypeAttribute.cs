using System;

namespace DinoCat.Wpf
{
    /// <summary>
    /// Indicates the given type should be projected from Wpf into DinoCat
    /// </summary>
    /// <remarks>
    /// For FromWpfTypeAttribute to be processed DinoCat.Wpf.Generator must be referenced
    /// from your assembly.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple =false, Inherited = false)]
    public class FromWpfTypeAttribute : Attribute
    {
        private string dinoTypeName;

        public FromWpfTypeAttribute(string dinoTypeName)
        {
            this.dinoTypeName = dinoTypeName;
        }

        public string DinoTypeName => dinoTypeName;
    }
}
