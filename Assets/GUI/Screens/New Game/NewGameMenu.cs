#pragma warning disable 0649

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewGameMenu : MonoBehaviour
{
    [SerializeField]
    private string newGameScene;

    [Header("UI")]

    [SerializeField]
    private Image portrait;

    [SerializeField]
    private List<CareerOptionButton> careers;

    [SerializeField]
    private InputField nameInput;

    [Header("Selected Career")]

    [SerializeField]
    private Text careerDescription;

    [SerializeField]
    private Text shipDescription;

    [SerializeField]
    private Text equipmentDescription;

    [SerializeField]
    private CareerOptionButton selectedCareer;

    [SerializeField]
    private Text pilotSkillLabel;

    [SerializeField]
    private Text weaponsSkillLabel;

    [SerializeField]
    private Text mechSkillLabel;

    private GUIController guiController;

    private void Start()
    {
        guiController = GetComponentInParent<GUIController>();
    }

    private void OnEnable()
    {
        portrait.sprite = SpaceTraderConfig.CrewConfiguration.DefaultPortrait;
        SelectCareer(0);

        nameInput.text = "";
    }

    public void CyclePortrait(int diff)
    {
        var portraits = SpaceTraderConfig.CrewConfiguration.Portraits;
        Debug.Assert(portraits.Any());

        int selected = portraits.IndexOf(portrait.sprite);
        if (selected == -1)
        {
            selected = 0;
        }
        else
        {
            var portraitCount = portraits.Count;

            selected += diff;
            if (selected >= portraitCount)
            {
                selected = 0;
            }
            else if (selected < 0)
            {
                selected = portraitCount - 1;
            }
        }

        portrait.sprite = portraits[selected];
    }

    public void SelectCareer(int index)
    {
        Debug.Assert(index >= 0 && index < careers.Count);
        selectedCareer = careers[index];

        careerDescription.text = selectedCareer.Description;
        shipDescription.text = selectedCareer.ShipType.name;

        var cargo = selectedCareer.Loadout.CargoItems.ToList();

        equipmentDescription.text = string.Join("\n", selectedCareer.Loadout
            .FrontModules
            .Select(m => m.name)
            .Concat(selectedCareer.Loadout.CargoItems
                .Select(i => i.name))
            .ToArray());

        careers.ForEach(c => c.SetHighlight(c == selectedCareer));

        pilotSkillLabel.text = selectedCareer.PilotSkill.ToString();
        weaponsSkillLabel.text = selectedCareer.WeaponsSkill.ToString();
        mechSkillLabel.text = selectedCareer.MechanicalSkill.ToString();
    }

    public void Submit()
    {
        //run on global obj to persist through level change
        SpaceTraderConfig.Instance.StartCoroutine(LoadNextLevel(nameInput.text, portrait.sprite));
    }

    private IEnumerator LoadNextLevel(string pcName, Sprite pcPortrait)
    {
        yield return guiController.ShowLoadingOverlay();

        var world = SpaceTraderConfig.WorldMap;
        var area = world.GetArea(newGameScene);

        yield return SpaceTraderConfig.WorldMap.LoadArea(area);

        var ship = selectedCareer.ShipType.CreateShip(Vector3.zero, Quaternion.identity);
        var player = SpaceTraderConfig.LocalPlayer = ship.gameObject.AddComponent<PlayerShip>();
        player.AddMoney(1000);

        var pc = SpaceTraderConfig.CrewConfiguration.NewCharacter(pcName, pcPortrait);
        pc.Assign(ship, CrewAssignment.Captain);
        pc.PilotSkill = selectedCareer.PilotSkill;
        pc.MechanicalSkill = selectedCareer.MechanicalSkill;
        pc.WeaponsSkill = selectedCareer.WeaponsSkill;

        guiController.DismissLoadingOverlay();
        guiController.SwitchTo(ScreenID.None);
    }
}
