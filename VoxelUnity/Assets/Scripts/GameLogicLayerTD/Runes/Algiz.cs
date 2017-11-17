using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogicLayerTD.Runes;
using UnityEngine;

public class Algiz : Rune {

    void Update()
    {
        transform.parent.GetComponent<MeshRenderer>().material.color = GetUpgradeRunes().Count > 0 ? Color.blue : Color.white;
    }

    public List<UpgradeRune> GetUpgradeRunes()
    {
        var upgrades = FindObjectsOfType<UpgradeRune>().ToList();
        if (upgrades.Count > 0)
            upgrades = upgrades.Where(u => transform.Equals(u.Tower)).ToList();
        return upgrades;
    }
}