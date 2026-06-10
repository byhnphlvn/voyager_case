using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class WaitZone : MonoBehaviour
{
    public static WaitZone instance;
    public List<int> boxesToCome;
    public int currentBoxNumber;


    public GameObject _boxPrefab;

    public List<Transform> _boxPositions;

    public List<SingleBox> _currentBoxes;


    public List<int> fullData;


    public List<SingleBufferSquare> _bufferSquares;

    public List<int> bufferZoneFullData;



    public List<SingleCan> _cansInBufferZone;
    private void Awake()
    {
        instance = this;
        SetStartBoxes();
    }

    public void GetCan(SingleCan _canToGet)
    {
        for (int i = 0; i < _currentBoxes.Count; i++)
        {
            Debug.Log("BoxNum" + i);
            if (_currentBoxes[i].colorNum == _canToGet.colorNumber)
            {
                Debug.Log("BoxNum" + i + "_SameColored");
                if (_currentBoxes[i].fullData.Contains(0))
                {
                    Debug.Log("BoxNum" + i + "_HasEmptyPosition");
                    _currentBoxes[i].GetCan(_canToGet);
                    return;
                }
            }
        }
        if (smallestNumber() != 99)
        {
            int smallestNum = smallestNumber();
            bufferZoneFullData[smallestNum] = 1;

            _cansInBufferZone.Add(_canToGet);
            _canToGet.transform.parent = _bufferSquares[smallestNum].transform;
            _canToGet.transform.DOLocalRotate(_bufferSquares[smallestNum]._canPositionInside.localEulerAngles, .2f);
            _canToGet.transform.DOScale(_bufferSquares[smallestNum]._canPositionInside.localScale, .2f);
            _canToGet.transform.DOJump(_bufferSquares[smallestNum]._canPositionInside.position + new Vector3(0, 8, 0), 5, 1, .3f).OnComplete(delegate {
                _canToGet.transform.DOLocalMove(_bufferSquares[smallestNum]._canPositionInside.localPosition, .2f).SetEase(Ease.Linear);
            }).SetEase(Ease.Linear);
        }
    }
    public int smallestNumber()
    {
        for (int i = 0; i < bufferZoneFullData.Count; i++)
        {
            if (bufferZoneFullData[i] == 0)
            {
                return i;
            }

        }
        return 99;
    }

    public void CheckNewBox()
    {
        List<SingleCan> _cansToRemoveFromBufferZone = new List<SingleCan>();
        for(int i = 0; i < _cansInBufferZone.Count; i++)
        {
            foreach(SingleBox _box in _currentBoxes)
            {
                if (_box.colorNum == _cansInBufferZone[i].colorNumber)
                {
                    if (_box.smallestNumber() != 99)
                    {
                        _box.GetCan(_cansInBufferZone[i]);
                        bufferZoneFullData[_cansInBufferZone[i].bufferZoneNumber] = 0;
                        _cansToRemoveFromBufferZone.Add(_cansInBufferZone[i]);
                    }
                }
            }
        }
        for(int i = 0; i < _cansToRemoveFromBufferZone.Count; i++)
        {
            _cansInBufferZone.Remove(_cansToRemoveFromBufferZone[i]);
        }
    }
   
    public void BoxRemoved(SingleBox _boxToRemove)
    {
        _currentBoxes.Remove(_boxToRemove);
        fullData[_boxToRemove.boxPosNumber] = 0;
        if (currentBoxNumber < boxesToCome.Count - 1)
        {
            NewBox(_boxToRemove.boxPosNumber);
        }

    }
    public void NewBox(int posNumber)
    {
        GameObject _box = Instantiate(_boxPrefab);
        _box.GetComponent<SingleBox>().boxPosNumber = posNumber;
        _currentBoxes.Add(_box.GetComponent<SingleBox>());
        _box.transform.position = _boxPositions[posNumber].transform.position - new Vector3(10,-2,0);
        _box.transform.DOMove(_boxPositions[posNumber].transform.position + new Vector3(0, 3, 0), .2f);
        _box.transform.DOMove(_boxPositions[posNumber].transform.position + new Vector3(0, 0, 0), .1f).SetDelay(.2f).OnComplete(delegate {
            CheckNewBox();
        });
        _box.transform.eulerAngles = _boxPositions[posNumber].transform.eulerAngles;
        _box.transform.localScale = _boxPositions[posNumber].transform.localScale;
        _box.GetComponent<SingleBox>().SetBoxes(boxesToCome[currentBoxNumber]);
        currentBoxNumber++;
        
    }
    public void SetStartBoxes()
    {
        for (int i = 0; i < _boxPositions.Count; i++)
        {
            GameObject _box = Instantiate(_boxPrefab);
            _box.GetComponent<SingleBox>().boxPosNumber = i;
            _currentBoxes.Add(_box.GetComponent<SingleBox>());
            _box.transform.position = _boxPositions[i].transform.position;
            _box.transform.eulerAngles = _boxPositions[i].transform.eulerAngles;
            _box.transform.localScale = _boxPositions[i].transform.localScale;
            _box.GetComponent<SingleBox>().SetBoxes(boxesToCome[i]);
            currentBoxNumber++;
        }
    }
}
