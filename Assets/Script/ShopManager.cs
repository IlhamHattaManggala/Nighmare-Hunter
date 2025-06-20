using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject ShopBg;
    // Start is called before the first frame update
    public void openShop()
    {
        Time.timeScale = 0;
        ShopBg.SetActive(true);
    }
    public void closeShop()
    {
        ShopBg.SetActive(false);
        Time.timeScale = 1;
    }
}
