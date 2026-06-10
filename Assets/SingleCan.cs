using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
[System.Serializable]
public class ColorMaterials
{
    public List<Material> _mats = new List<Material>();
}
public class SingleCan : MonoBehaviour
{
    public List<int> matNumbers;
    public int colorNumber;

    public List<ColorMaterials> _colorMaterials;
    public SingleGrid _gridParent;


    public Renderer _canRenderer;


    public int bufferZoneNumber;


    public Vector3 _rendererDefaultScale;
    private void Awake()
    {
        _rendererDefaultScale = _canRenderer.transform.localScale;
    }
    public void SetCan(int canNumber)
    {
        colorNumber = canNumber;
        SetMaterials();
    }

    public void PunchScale()
    {
        _canRenderer.transform.DOComplete();
        _canRenderer.transform.DOPunchScale(Vector3.one*5,.2f).OnComplete(delegate {

            _canRenderer.transform.localScale = _rendererDefaultScale;
	    });
    }
    public void YoyoCan()
    {
        //_canRenderer.transform.DOScale(_rendererDefaultScale*1.1f,.2f).SetLoops(-1,LoopType.Yoyo);
    }
    public void StopYoyo()
    {
        //_canRenderer.transform.DOKill();
    }
    public void SetMaterials()
    {
        int matPlaceNumber = 0;
        Material[] mats = new Material[_canRenderer.GetComponent<Renderer>().materials.Length];
        for (int i = 0; i < _canRenderer.GetComponent<Renderer>().materials.Length; i++)
        {
            if (matNumbers.Contains(i))
            {
                mats[i] = _colorMaterials[colorNumber]._mats[matPlaceNumber];
                matPlaceNumber++;
            }
            else
            {
                mats[i] = _canRenderer.GetComponent<Renderer>().materials[i];
            }
        }
        _canRenderer.GetComponent<Renderer>().materials = mats;
    }
}
