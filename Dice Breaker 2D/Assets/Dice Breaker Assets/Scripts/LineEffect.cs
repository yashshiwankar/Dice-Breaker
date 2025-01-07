using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineEffect : MonoBehaviour
{
    [SerializeField] private GameObject dot;
    private GameObject[] dotArray;
    [SerializeField] int dotAmt = 7;
    float dotGap;
    void Start()
    {
        dotGap = 1 / dotAmt;
        SpawnDots();
    }
    void SpawnDots()
    {
        dotArray = new GameObject[dotAmt];
        for (int i = 0; i < dotAmt; i++) {
            GameObject temp = Instantiate(dot, transform.position, Quaternion.identity, transform);
            temp.SetActive(false);
            dotArray[i] = temp;
        }
    }
    public void DrawLineEffect(Vector3 startPos, Vector3 dir)
    {
        for(int i = 0; i < dotAmt; i++)
        {
            Vector3 targetPos = Vector2.Lerp(startPos, dir, i * dotGap);
            dotArray[i].transform.position = targetPos;
            dotArray[i].SetActive(true);
        }
        transform.up = dir.normalized;
    }

}
