using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NotSelectingButton : Button
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        // 현재 게임 오브젝트를 선택 취소
        EventSystem.current.SetSelectedGameObject(null);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        // 현재 게임 오브젝트를 선택 취소
        EventSystem.current.SetSelectedGameObject(null);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        // 현재 게임 오브젝트를 선택 취소
        EventSystem.current.SetSelectedGameObject(null);
    }
}
