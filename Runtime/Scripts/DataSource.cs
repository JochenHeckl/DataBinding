using System;

namespace de.JochenHeckl.Unity.DataBinding
{
    public class DataSource<DataSourceType>
		where DataSourceType : DataSource<DataSourceType>
	{
		public event Action DataSourceChanged = delegate { };

		public void NotifyChanges( Action<DataSourceType> applyChanges )
		{
			applyChanges( (DataSourceType) this );
			DataSourceChanged();
		}
	}
}
