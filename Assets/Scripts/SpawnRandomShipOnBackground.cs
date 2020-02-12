using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomShipOnBackground : MonoBehaviour
{
    public GameObject[] Ships;
    public Vector2 RangeSpawn;
    public float speed;
    private bool reset = true;
    public GameObject ShipBackgroundContainer;
    
    private void OnDisable()
    {
        Intro.OnIntroClickContinueHandled -= Hide;
    }

    void Start()
    {
        ChooseNext();
        Intro.OnIntroClickContinueHandled += Hide;
    }

    void Hide()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    public void ChooseNext()
    {
        StartCoroutine(DelayMakeShipsAppear());
    }

    IEnumerator DelayMakeShipsAppear()
    {
        foreach (GameObject item in Ships)
        {
            item.SetActive(false);
        }

        yield return new WaitForSeconds(1.0f);

        int randomShipToAppear = Random.Range(2, 4);

        for (int i = 0; i < randomShipToAppear; i++)
        {
            int randomNum = UnityEngine.Random.Range(0, Ships.Length);
            GameObject ship = Ships[randomNum];

            while (ship.activeSelf)
            {
                randomNum = UnityEngine.Random.Range(0, Ships.Length);
                ship = Ships[randomNum];
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(1, 4));

            float prev = 0;

            if (prev == 0)
            {
                prev = Random.Range(RangeSpawn.x, RangeSpawn.y);
            }
            else
            {
                float tempXPos = Random.Range(RangeSpawn.x, RangeSpawn.y);
                while (prev == tempXPos)
                {
                    tempXPos = Random.Range(RangeSpawn.x, RangeSpawn.y);
                    yield return null;
                }
            }

            Ships[randomNum].SetActive(true);
            Ships[randomNum].transform.position = new Vector3(prev, -50, 150);

        }

        reset = false;
    }

    void Update()
    {
        if (Input.touchCount > 0 || Input.anyKey)
        {

        }

        MoveShips();
    }

    public void MoveShips()
    {
        float step = speed * Time.deltaTime; // calculate distance to move
        for (int i = 0; i < Ships.Length; i++)
        {
            GameObject ship = Ships[i];
            if (ship.activeSelf)
            {
                ship.transform.Translate(transform.forward * step);

                if (ship.transform.position.z < -30)
                {
                    Debug.Log(ship.transform.position.z);
                    ship.SetActive(false);
                }
            }
        }
        int activeShips = 0;
        for (int i = 0; i < Ships.Length; i++)
        {
            GameObject ship = Ships[i];

            if (ship.activeSelf)
            {
                activeShips = 0;
            }
            else
            {
                activeShips++;
            }
        }

        if (!reset && activeShips == Ships.Length)
        {
            reset = true;
            ChooseNext();
        }
    }
}
