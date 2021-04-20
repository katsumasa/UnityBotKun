using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class MouseTest : MonoBehaviour,IPointerEnterHandler, IPointerClickHandler
{
    Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
    }

    private void OnMouseUp()
    {
        Debug.Log("OnMouseUp");
    }


    private void OnMouseOver()
    {
        Debug.Log("OnMouseOver");
        renderer.material.color = new Color(Mathf.Sin(Time.deltaTime), Mathf.Sin(Time.deltaTime), Mathf.Sin(Time.deltaTime));
    }


    private void OnMouseDrag()
    {
        Debug.Log("OnMouseDrag");        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
    }
}
