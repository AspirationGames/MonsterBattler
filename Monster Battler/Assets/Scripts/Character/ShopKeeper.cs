using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    public IEnumerator Trade()
    {

        yield return ShopController.i.StartTrading(this);
    }
}
