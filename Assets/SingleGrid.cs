using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SingleGrid : MonoBehaviour
{
    public Transform _canPosition;
    public SingleCan _canInside;

    public bool empty;


    public Vector2Int gridPosition;
    public void StartGetCan(SingleCan _canToGet)
    {
        _canToGet.GetComponent<SingleCan>()._canRenderer.materials[4].SetFloat("_OutlineWidth", 0);
        empty = false;
        _canInside = _canToGet;
        _canToGet._gridParent = this;
        _canToGet.transform.parent = transform;
        _canToGet.transform.localPosition = _canPosition.localPosition;
        _canToGet.transform.localEulerAngles = _canPosition.localEulerAngles;
        _canToGet.transform.localScale = _canPosition.localScale;
    }

    public void AfterGetCan(SingleCan _canToGet)
    {
        _canToGet.GetComponent<SingleCan>()._canRenderer.materials[4].SetFloat("_OutlineWidth", 0);
        empty = false;
        _canInside = _canToGet;
        _canToGet._gridParent = this;
        _canToGet.transform.parent = transform;
        _canToGet.transform.localPosition = _canPosition.localPosition;
        _canToGet.transform.localEulerAngles = _canPosition.localEulerAngles;
        _canToGet.transform.localScale = Vector3.zero;
        _canToGet.transform.DOScale(_canPosition.transform.localScale * 1.1f, .2f);
        _canToGet.transform.DOScale(_canPosition.transform.localScale, .1f).SetDelay(.2f);
    }

    public void SlideCan(SingleCan _canToGet)
    {
        Debug.Log(_canToGet.GetComponent<SingleCan>());
        Debug.Log(_canToGet.GetComponent<SingleCan>()._canRenderer);
        _canToGet.GetComponent<SingleCan>()._canRenderer.materials[4].SetFloat("_OutlineWidth", 0);
        empty = false;
        _canInside = _canToGet;
        _canToGet._gridParent = this;
        _canToGet.transform.parent = transform;

        _canToGet.transform.DOLocalMoveZ(_canPosition.localPosition.z, .2f).SetEase(Ease.OutQuad);

        _canToGet.transform.DOLocalMoveX(_canPosition.localPosition.x, .2f).SetEase(Ease.Linear);
        _canToGet.transform.DOLocalMoveY(_canPosition.localPosition.y, .2f).SetEase(Ease.Linear).OnComplete(delegate {
        });

    }

    public void AnimatedSwitchCan(SingleCan _canToGet,bool firstOne)
    {
        _canToGet.GetComponent<SingleCan>()._canRenderer.materials[4].SetFloat("_OutlineWidth", 0);
        empty = false;
        _canInside = _canToGet;
        _canToGet._gridParent = this;
        _canToGet.transform.parent = transform;
        bool differentY = false;
        if (_canToGet.transform.localPosition.y - _canPosition.localPosition.y != 0)
        {
            differentY = true;
        }
        if (firstOne)
        {
            _canToGet.transform.DOLocalMoveZ(_canPosition.localPosition.z - 2, .2f).SetEase(Ease.OutQuad);

            _canToGet.transform.DOLocalMoveZ(_canPosition.localPosition.z, .1f).SetEase(Ease.InQuad).SetDelay(.2f);

            _canToGet.transform.DOLocalMoveX(_canPosition.localPosition.x, .3f).SetEase(Ease.Linear);
            _canToGet.transform.DOLocalMoveY(_canPosition.localPosition.y, .3f).SetEase(Ease.Linear).OnComplete(delegate {

                _canToGet.PunchScale();
            });
            if (!differentY)
            {
                _canToGet._canRenderer.transform.DOLocalRotate(new Vector3(0, 360, 0), .3f, RotateMode.LocalAxisAdd);
            }
        }
        else
        {
            _canToGet.transform.DOLocalMoveZ(_canPosition.localPosition.z - 6, .2f).SetEase(Ease.OutQuad);

            _canToGet.transform.DOLocalMoveZ(_canPosition.localPosition.z, .1f).SetEase(Ease.InQuad).SetDelay(.2f);

            _canToGet.transform.DOLocalMoveX(_canPosition.localPosition.x, .3f).SetEase(Ease.Linear);
            _canToGet.transform.DOLocalMoveY(_canPosition.localPosition.y, .3f).SetEase(Ease.Linear);
            _canToGet._canRenderer.transform.DOLocalRotate(new Vector3(0, -360, 0), .3f, RotateMode.LocalAxisAdd).OnComplete(delegate {
                _canToGet.PunchScale();
                
            });
            TileManager.instance.CheckBottomTiles();
        }
    }

    

}   