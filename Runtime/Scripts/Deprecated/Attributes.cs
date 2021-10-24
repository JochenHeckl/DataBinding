using System;
using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false )]
    public class BindingPathAttribute : PropertyAttribute
    {
    }

	[AttributeUsage( AttributeTargets.Field, AllowMultiple = false )]
	public class ComponentPropertyPathAttribute : PropertyAttribute
	{
	}

	[AttributeUsage( AttributeTargets.Field, AllowMultiple = false )]
	public class BindableComponentAttribute : PropertyAttribute
	{		
	}
}
