using System;

namespace JH.DataBinding
{
    public interface INotifyDataSourceChanged
	{
		event Action DataSourceChanged;
	}
}
