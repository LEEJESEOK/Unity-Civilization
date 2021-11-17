using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Test")]
    public bool test;
    public float testDelay = 1f;

    // Start is called before the first frame update
    void Start()
    {

        if (test)
        {
            StartCoroutine(TestCoroutine());
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator TestCoroutine()
    {
        int testAmount = 0;
        while (test)
        {
            yield return new WaitForSeconds(testDelay);

            UIManager.instance.TestResourcesUpdate(testAmount);
            ++testAmount;
        }
    }
}
