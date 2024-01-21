using System.Collections.Generic;

namespace de.JochenHeckl.Unity.DataBinding.Examples.ContainerBindings.Tutorial
{
    public class PartyViewModel : DataSourceBase<PartyViewModel>
    {
        public IEnumerable<CharacterViewModel> Characters { get; set; }
    }
}
