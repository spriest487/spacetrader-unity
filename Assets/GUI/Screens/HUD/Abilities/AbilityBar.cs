#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AbilityBar : MonoBehaviour
{
    [SerializeField]
    private AbilityButton buttonPrefab;
    
    [SerializeField]
    private Transform buttonHolder;

    private List<AbilityButton> buttons;

    private void RemoveExistingButtons()
    {
        foreach (Transform child in buttonHolder)
        {
            Destroy(child.gameObject);
        }
    }
    
    void Update()
    {
        if (buttons == null)
        {
            buttons = new List<AbilityButton>();
            RemoveExistingButtons();
        }

        var player = Universe.LocalPlayer;
        if (player)
        {
            var abilities = new List<Ability>();
            foreach (var ability in player.GetComponent<Ship>().Abilities)
            {
                if (ability != null)
                {
                    abilities.Add(ability);
                }
            }

            bool needsUpdate = false;

            //quick check
            int abilityCount = 0;
            foreach (var ability in abilities)
            {
                if (ability)
                {
                    ++abilityCount;
                }
            }
            if (abilityCount != buttons.Count)
            {
                needsUpdate = true;
            }

            if (!needsUpdate)
            {
                //slow check - do all buttons currently have corresponding abilities
                foreach (var ability in abilities)
                {
                    bool found = false;
                    foreach (var button in buttons)
                    {                        
                        if (button.Ability == ability)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        needsUpdate = true;
                        break;
                    }
                }
            }

            if (needsUpdate)
            {
                buttons.Clear();
                RemoveExistingButtons();

                foreach (var ability in abilities)
                {
                    var button = Instantiate(buttonPrefab);
                    button.transform.SetParent(buttonHolder, false);
                    button.Ability = ability;

                    buttons.Add(button);
                }
            }
        }
        else
        {
            buttons.Clear();
            RemoveExistingButtons();
        }
    }
}