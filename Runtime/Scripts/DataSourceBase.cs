using System;

namespace de.JochenHeckl.Unity.DataBinding
{
    public class DataSourceBase<DataSourceType> : INotifyDataSourceChanged
		where DataSourceType : DataSourceBase<DataSourceType>
	{
		public event Action DataSourceChanged = delegate { };

		public void NotifyChanges( Action<DataSourceType> applyChanges )
		{
			applyChanges( (DataSourceType) this );
			DataSourceChanged();
		}
	}
}
