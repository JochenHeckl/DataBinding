using System.Collections.Generic;

namespace JH.DataBinding.Examples.ContainerBindings.Tutorial
{
    public class PartyViewModel : DataSourceBase<PartyViewModel>
    {
        public IEnumerable<CharacterViewModel> Characters { get; set; }
    }
}
