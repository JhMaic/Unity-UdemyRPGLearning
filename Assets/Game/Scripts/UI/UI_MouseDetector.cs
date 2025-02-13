using System.Collections.Generic;
using CustomInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIMouseDetector : MonoBehaviour
{
    // 指定需要检测的 Canvas，Canvas 上必须挂有 GraphicRaycaster 组件
    [SerializeField] [SelfFill(true)] private Canvas targetCanvas;
    [SerializeField] [SelfFill(true)] private GraphicRaycaster raycaster;

    [SerializeField] [ForceFill] private EventSystem eventSystem;


    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && TryGetMouseHit(out var itemSlot))
            itemSlot.InvokeMouseDown();
    }

    private bool TryGetMouseHit(out UI_ItemSlot itemSlot)
    {
        itemSlot = null;

        // 创建 PointerEventData，并设置鼠标当前位置
        var pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        // 用于存储射线检测结果的列表
        var results = new List<RaycastResult>();

        // 发射射线检测 UI 元素
        raycaster.Raycast(pointerData, results);

        // 遍历检测结果
        foreach (var result in results)
        {
            itemSlot = result.gameObject.GetComponent<UI_ItemSlot>();
            if (itemSlot is not null)
                return true;
        }

        return false;
    }
}