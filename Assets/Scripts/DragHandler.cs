using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class DragHandler : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static GameObject itemBeingDragged;
    Vector3 startPosition;
    Transform startParent;

    public static GameObject newParent;
    public static List<string> newInput = new List<string>();


    public void OnBeginDrag (PointerEventData eventData)
    {
        itemBeingDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemBeingDragged = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if(transform.parent == startParent)
        {
            transform.position = startPosition;
        }
        else
        {
            Debug.Log(transform.parent.name);
            //If it's dropped back to the input panel slots
            if(transform.parent.parent.name == "InputPanel")
            {
                newInput.Remove(startParent.name);
            }
            //if the object was already in the playboard
            else if(startParent.parent.name == "PlayBoardPanel")
            {
                //Replacing the old position of the item
                newInput[newInput.FindIndex(ind => ind.Equals(startParent.name))] = transform.parent.name;
            }
            // if it wasn't on playboard we get the new row and column
            else
            {
                newInput.Add(transform.parent.name);
            }
            //foreach (var item in newInput)
            //{
            //    Debug.Log(item);
            //}
            //if(newInput.Count >0)
            //    Debug.Log(newInput[0][0]);
        }
    }
}
