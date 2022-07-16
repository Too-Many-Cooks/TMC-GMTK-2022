using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DieManager : Singleton<DieManager>
{
    public Die[] dice;

    public Die FindDie(int sides)
    {
        return dice.First(d => d.Sides == sides);
    }
}
