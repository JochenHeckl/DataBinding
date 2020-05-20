using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding
{
	public abstract class ViewModelBase : IDataSource
	{
		public event Action ViewModelChanged = delegate {};

		public void NotifyViewModelChanged()
		{
			ViewModelChanged();
		}
	}
}