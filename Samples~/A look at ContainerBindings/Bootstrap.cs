using System.Linq;

using de.JochenHeckl.Unity.DataBinding;
using de.JochenHeckl.Unity.DataBinding.Examples.ContainerBindings;

using UnityEngine;

using Random = UnityEngine.Random;

public class Bootstrap : MonoBehaviour
{
    public View partyViewTemplate;
    public Transform partyViewContainer;

    public Sprite theOneAndOnlyCharacterImage;

    private View partyView;

    void Start()
    {
        var partyViewModel = new PartyViewModel()
        {
            Characters = Enumerable.Range(1, 4).Select(x => MakeCharacterData(x)).ToArray()
        };

        partyView = GameObject.Instantiate<View>(partyViewTemplate, partyViewContainer);
        partyView.DataSource = partyViewModel;
    }

    private CharacterViewModel MakeCharacterData(int characterCreationIndex)
    {
        var charcterClasses = new string[] { "Mage", "Priest", "Rogue", "Warrior", };

        var characterLevel = Random.Range(1, 10);

        return new CharacterViewModel()
        {
            Name = $"Character 0{characterCreationIndex}",
            Class = charcterClasses.ElementAt(Random.Range(0, 3)),
            Level = characterLevel,
            LevelProgress = Random.Range(0f, 1f),
            Attributes = MakeAttributes(characterLevel),
            CharacterImage = theOneAndOnlyCharacterImage
        };
    }

    private AttributeViewModel[] MakeAttributes(int characterLevel)
    {
        var attributes = new string[]
        {
            "Agility",
            "Intelligence",
            "Resistence",
            "Sorcery",
            "Strength",
            "Will",
        };

        return attributes
            .Select(
                x =>
                    new AttributeViewModel()
                    {
                        Name = x,
                        Value = Random.Range(1, 6 + characterLevel / 6)
                    }
            )
            .ToArray();
    }
}
