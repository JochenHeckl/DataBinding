using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace de.JochenHeckl.Unity.DataBinding
{
    public interface IDataSource
    {
        event Action ViewModelChanged;
        void NotifyViewModelChanged();
    }
}
