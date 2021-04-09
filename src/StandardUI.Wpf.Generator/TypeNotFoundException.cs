using System;
using System.Collections.Generic;
using System.Text;

namespace DinoCat.Wpf.Generator
{
    class TypeNotFoundException : Exception
    {
        public TypeNotFoundException(string qualifiedTypeName) : base($"Could not find {qualifiedTypeName}") =>
            QualifiedTypeName = qualifiedTypeName;

        public string QualifiedTypeName { get; }
    }
}
