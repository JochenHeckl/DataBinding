using System;

namespace de.JochenHeckl.Unity.DataBinding
{
    public interface INotifyDataSourceChanged
	{
		event Action DataSourceChanged;
	}
}
