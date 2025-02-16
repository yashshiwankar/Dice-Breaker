using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineEffect : MonoBehaviour
{
    [SerializeField] private GameObject rect;
    private LineRenderer lineRenderer;
    [SerializeField] private Vector3 lineStartPos = new Vector3(0f, -3f, 0f); // default vals
    [SerializeField] private Vector3 lineEndPos = new Vector3(0f, -1f, 0f);
    
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();        
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, lineStartPos);
        lineRenderer.SetPosition(1, lineEndPos);
        rect.SetActive(false);
    }
    public void DrawLineEffect(Vector3 dir)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, Vector3.Lerp(lineStartPos, dir, dir.magnitude));
    }
    public void DrawRect()
    {
        rect.SetActive(true);
        rect.transform.localScale = Vector3.one;
    }
    public void DisableLineEffect()
    {
        lineRenderer.enabled = false;
        rect.transform.localScale = Vector3.zero;
        rect.SetActive(false);
    }
    private void OnDisable()
    {
        lineRenderer.SetPosition(0, lineStartPos);
        lineRenderer.SetPosition(1, lineEndPos);
    }
}
