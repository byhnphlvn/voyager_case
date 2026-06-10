using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MovementScript : MonoBehaviour
{
    public LayerMask _canLayer;
    public List<SingleCan> _cansSelected= new List<SingleCan>();



    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit _rayHit;
            if(Physics.Raycast(camRay,out _rayHit, Mathf.Infinity, _canLayer))
            {
                if (_cansSelected.Count == 0)
                {
                    _cansSelected.Add(_rayHit.collider.GetComponent<SingleCan>());

                    _cansSelected[0].GetComponent<SingleCan>()._canRenderer.materials[4].SetFloat("_OutlineWidth", 1);
                    _cansSelected[0].GetComponent<SingleCan>().YoyoCan();


                }
                else
                {
                    if (!_cansSelected.Contains(_rayHit.collider.GetComponent<SingleCan>()))
                    {
                        List<SingleGrid> _checkPoses = new List<SingleGrid>();
                        _checkPoses.Add(_cansSelected[0]._gridParent);
                        _checkPoses.Add(_rayHit.collider.GetComponent<SingleCan>()._gridParent);
                        if (isNeighbor(_checkPoses))
                        {
                            _cansSelected.Add(_rayHit.collider.GetComponent<SingleCan>());
                            SwitchCans(_cansSelected);
                            _cansSelected.Clear();
                        }
                        else
                        {
                            ClearOutlines(_cansSelected);
                            _cansSelected.Clear();
                        }
                    }
                    else
                    {

                        ClearOutlines(_cansSelected);
                        _cansSelected.Clear();
                    }
                }
            }
        }
    }


    public bool isNeighbor(List<SingleGrid> _gridsToCheck)
    {
        Vector2Int firstPos = _gridsToCheck[0].gridPosition;
        Vector2Int secondPos = _gridsToCheck[1].gridPosition;


        if (firstPos.x == secondPos.x)
        {
            if (Mathf.Abs(firstPos.y - secondPos.y) == 1)
            {
                return true;
            }
        }

        if (firstPos.y == secondPos.y)
        {
            if (Mathf.Abs(firstPos.x - secondPos.x) == 1)
            {
                return true;
            }
        }
        return false;
    }
    public void ClearOutlines(List<SingleCan> _cansToClear)
    {
        for(int i = 0; i < _cansToClear.Count; i++)
        {
            _cansToClear[i].StopYoyo();
            _cansToClear[i].GetComponent<SingleCan>()._canRenderer.materials[4].SetFloat("_OutlineWidth", 0);
        }
    }
    public void SwitchCans(List<SingleCan> _cansToSwitch)
    {
        ClearOutlines(_cansToSwitch);

        SingleGrid _firstGrid = _cansToSwitch[0]._gridParent;
        SingleGrid _secondGrid= _cansToSwitch[1]._gridParent;

        SingleCan _firstCan = _cansToSwitch[0];
        SingleCan _secondCan= _cansToSwitch[1];

        _firstGrid.AnimatedSwitchCan(_secondCan,true);
        _secondGrid.AnimatedSwitchCan(_firstCan,false);
        
    }
}

