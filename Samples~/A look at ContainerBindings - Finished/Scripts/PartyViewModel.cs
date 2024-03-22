using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH.DataBinding.Examples.ContainerBindings
{
    public class PartyViewModel : DataSourceBase<PartyViewModel>
    {
        public IEnumerable<CharacterViewModel> Characters { get; set; }
    }
}
