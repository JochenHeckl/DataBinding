using System;

namespace IC.DataBinding
{
    public interface INotifyDataSourceChanged
	{
		event Action DataSourceChanged;
	}
}
