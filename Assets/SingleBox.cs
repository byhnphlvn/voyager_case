using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SingleBox : MonoBehaviour
{
    public List<Transform> boxCovers = new List<Transform>();
    public List<Transform> boxCoversClosedPositions = new List<Transform>();

    public AnimationCurve _closeCurve;
    public AnimationCurve _moveOutCurve;
    public AnimationCurve _moveUpCurve;

    public float closeTime;
    public float closeWaitTime;


    public List<Renderer> _colorRenderers;
    public List<Material> _colorMaterials;
    public int colorNum;

    public List<Transform> _canPositions;
    public List<Transform> _canAfterPoses;

    public List<int> fullData;
    public List<SingleCan> _cansInside;

    public int boxPosNumber;
    public void SetBoxes(int colorNumber)
    {
        for (int i = 0; i < _colorRenderers.Count; i++)
        {
            _colorRenderers[i].material = _colorMaterials[colorNumber];
        }
        colorNum = colorNumber;
    }
    public void GetCan(SingleCan _canToGet)
    {
        if (smallestNumber() != 99)
        {
            _cansInside.Add(_canToGet);
            Debug.Log("GotCan");
            int smallestNum = smallestNumber();
            fullData[smallestNum] = 1;
            _canToGet.transform.parent = _canPositions[smallestNum].transform.parent;
            _canToGet.transform.DOLocalRotate(_canPositions[smallestNum].localEulerAngles, .2f);
            _canToGet.transform.DOScale(_canPositions[smallestNum].localScale, .2f);
            _canToGet.transform.DOJump(_canPositions[smallestNum].position + new Vector3(0, 8, 0), 5, 1, .3f).OnComplete(delegate {
                _canToGet.transform.DOLocalMove(_canPositions[smallestNum].localPosition, .2f).SetEase(Ease.Linear);

            }).SetEase(Ease.Linear);
            if (smallestNumber() == 99)
            {
                
                StartCoroutine(CloseBox());
            }
        }
    }
    public int smallestNumber()
    {
        for (int i = 0; i < fullData.Count; i++)
        {
            if (fullData[i] == 0)
            {
                return i;
            }

        }
        return 99;
    }
    public IEnumerator CloseLids()
    {
        
        for(int i = 0; i < boxCovers.Count; i++)
        {
            bool lastOne = false;
            if (i == boxCovers.Count - 1)
            {
                lastOne = true;
            }
            boxCovers[i].transform.DOLocalRotate(boxCoversClosedPositions[i].transform.localEulerAngles, closeTime,RotateMode.FastBeyond360).SetEase(_closeCurve).OnComplete(delegate {
                if (lastOne)
                {
                    transform.DOPunchScale(new Vector3(0, .15f, .15f), .2f);
                }
	        });
            yield return new WaitForSeconds(closeWaitTime);
        }
    }
    public IEnumerator CloseBox()
    {
        yield return new WaitForSeconds(.5f);
        
        for (int i = 0; i < _cansInside.Count; i++)
        {
            _cansInside[i].transform.DOLocalMove(_canAfterPoses[i].transform.localPosition,.2f);
        }
        StartCoroutine(CloseLids());
        
        transform.DOMoveY(transform.position.y + 5, .2f).SetEase(_moveUpCurve);
        
        
        yield return new WaitForSeconds(.2f);
        WaitZone.instance.BoxRemoved(this);
        transform.DOMoveX(transform.position.x + 30, .5f).SetEase(_moveOutCurve);
        //transform.DORotate(new Vector3(-15, transform.eulerAngles.y, transform.eulerAngles.z), .2f);
        transform.DORotate(new Vector3(10, transform.eulerAngles.y, transform.eulerAngles.z), .1f).SetDelay(0f);

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            //StartCoroutine(CloseLids());
            StartCoroutine(CloseBox());
        }
    }
}