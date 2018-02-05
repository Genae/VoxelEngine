using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Healthbar : MonoBehaviour
{
    private float _maxHealth;
    private float _currentHealth;
    private float _widthScale = 1;

    private Transform _plane;
    private Transform _anchor;

    void Update()
    {
        _anchor.transform.forward = -Camera.main.transform.forward;
    }

    public void Init(float maxH, float curH)
    {
        _maxHealth = maxH;
        _currentHealth = curH;
        InitBar();
    }

    public void UpdateCurrentHealth(float curH)
    {
        _currentHealth = curH;
        _widthScale = _currentHealth / _maxHealth;
        _plane.localScale = new Vector3(0.2f * _widthScale, 1, 0.05f);
        if(_widthScale < 0.5f && _widthScale > 0.25f) _plane.GetComponent<Renderer>().material.color = new Color(255,140,0);
        if (_widthScale < 0.25f) _plane.GetComponent<Renderer>().material.color = Color.red;
    }

    private void InitBar()
    {
        _anchor = new GameObject("Anchor").transform;
        _anchor.parent = transform;
        _anchor.Rotate(-90,0,0);
        _anchor.localPosition = Vector3.zero;
        _anchor.transform.localPosition = Vector3.up * 4 * transform.localScale.y;

        _plane = GameObject.CreatePrimitive((PrimitiveType.Plane)).transform;
        _plane.parent = _anchor;
        _plane.name = "Healthbar";
        _plane.GetComponent<Renderer>().material.color = Color.green;
        _plane.localPosition = Vector3.zero;
        _plane.localScale = new Vector3(0.2f * _widthScale, 1, 0.05f);
        _plane.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        _plane.GetComponent<MeshRenderer>().receiveShadows = false;
    }
}
