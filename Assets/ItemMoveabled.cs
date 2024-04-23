using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ItemMoveabled : MonoBehaviour
{
    [SerializeField] private Transform[] triggersResourcesHome;
    private bool _move;


    // Update is called once per frame
    void Update()
    {
        if (_move)
        {
            float _speed = 15;
            float dist = Vector2.Distance(transform.position, Input.mousePosition);
            transform.position = Vector2.MoveTowards(transform.position, Input.mousePosition,  (dist * _speed) * Time.deltaTime);

            if (Vector2.Distance(triggersResourcesHome[0].position, transform.position)<50)
            {
                transform.DOScale(Vector2.zero, 0.2f);
                _move = false;
                GameManager.Instance.SetResources(0, -1);
            }
            if (Vector2.Distance(triggersResourcesHome[1].position, transform.position)<50)
            {
                transform.DOScale(Vector2.zero, 0.2f);
                _move = false;
                GameManager.Instance.SetResources(-1, 0);
            }
        }

        if (Input.GetMouseButtonUp(0) && _move )
        {
            _move = false;
            transform.DOScale(Vector2.zero, 0.2f);
        }
    }

    public void Click()
    {
        if (_move)
        {
            return;
        }
        _move = true;
      Transform meClone =  Instantiate(gameObject).transform;
      meClone.parent = transform.parent;
      meClone.position = transform.position;
      meClone.localScale = transform.localScale;
     transform.SetSiblingIndex(transform.parent.childCount);
    }
    public void StopMove()
    {
        Debug.Log("stop");
        _move = false;
    }
}
