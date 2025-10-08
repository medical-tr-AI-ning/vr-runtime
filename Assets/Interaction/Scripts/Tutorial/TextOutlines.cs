using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextOutlines : MonoBehaviour
{
    private Outline outline;
    private float thickness;
    [SerializeField] private float thickMin = 0;
    private float thickMax;
    private float dis;
    [SerializeField] private float disMin = 1;
    [SerializeField] private float disMax = 1.7f;
    private float alphaTarget;

    void Start()
    {
        outline = GetComponent<Outline>();
        thickMax = outline.effectDistance.x;
        alphaTarget = outline.effectColor.a;
    }
    void Update()
    {
        GetThickness();
        thickness = Mathf.Lerp(thickMin, thickMax, 1 - dis);
        outline.effectColor = new Color(outline.effectColor.r, outline.effectColor.g, outline.effectColor.b, Mathf.Lerp(0, alphaTarget, 1 - dis));
        outline.effectDistance = new Vector2(thickness,-thickness);
    }

    void GetThickness()
    {
        dis = Vector3.Distance(transform.position, Camera.main.transform.position);
        dis = Mathf.Clamp(dis, disMin, disMax);
        dis = dis - disMin;
        dis = dis / (disMax - disMin);
    }
}
