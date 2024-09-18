using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Swipe { None, Top, Bottom, Left, TopLeft, BottomLeft, Right, TopRight, BottomRight }
public class SwipeController
{
    private Vector2 startPos, endPos;
    private LevelManager _levelManager;
    public void SetLevelManager(LevelManager levelManager)
    {
        _levelManager = levelManager;
    }

    private bool isSwiping = false;  // Cờ để kiểm tra trạng thái swipe

    public void OnUpdate()
    {
        if (_levelManager != null && !_levelManager.CheckOnShopState)
        {
            UIManager uiManageObj = GameObject.Find("UIManager").GetComponent<UIManager>();
            if (uiManageObj != null)
            {
                if (!uiManageObj.ShopUI.activeSelf)  // Kiểm tra nếu panel shop bị ẩn
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        isSwiping = true;  // Bắt đầu thao tác swipe
                        startPos = Input.mousePosition;
                        endPos = startPos;
                    }
                    else if (Input.GetMouseButtonUp(0) && isSwiping)
                    {
                        endPos = Input.mousePosition;
                        if (Vector2.Distance(endPos, startPos) > 0.1f)
                        {
                            SwipeDirection();
                        }
                        isSwiping = false;  // Kết thúc thao tác swipe
                    }
                }
                else
                {
                    isSwiping = false;  // Reset cờ nếu shop panel được mở
                }
            }
        }
    }

    // Hàm này cần được gọi khi mở hoặc đóng panel shop
    public void OnShopPanelToggle(bool isShopOpen)
    {
        isSwiping = false;  // Reset cờ bất cứ khi nào shop được mở hoặc đóng
    }


    private Swipe SwipeDirection()
    {
        Swipe direction = Swipe.None;
        Vector2 currentSwipe = endPos - startPos;

        float angle = Mathf.Atan2(currentSwipe.y, currentSwipe.x) * (180 / Mathf.PI);

        if(angle > 67.5f && angle < 112.5f)
        {
            direction = Swipe.Top;
        }else if (angle < -67.5f && angle > -112.5f)
        {
            direction = Swipe.Bottom;
        }else if (angle < -157.5f || angle > 157.5f)
        {
            direction = Swipe.Left;
        }else if (angle > -22.5f && angle < 22.5f)
        {
            direction = Swipe.Right;
        }else if (angle > 22.5f && angle < 67.5f)
        {
            direction = Swipe.TopRight;
        }else if (angle > 112.5f && angle < 157.5f)
        {
            direction = Swipe.TopLeft;
        }
        else if(angle < -22.5f && angle > -67.5f)
        {
            direction = Swipe.BottomRight;
        }else if (angle < -112.5f && angle > -157.5f)
        {
            direction = Swipe.BottomLeft;
        }

        if (direction != Swipe.None)
        {
            _levelManager.MoveBrush(direction);
            direction = Swipe.None;
        }


        return direction;
    }
}
