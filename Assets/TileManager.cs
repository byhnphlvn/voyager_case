using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class CanRow
{
    public List<int> cansToCome = new List<int>();
    public int currentNumber;
}

public class TileManager : MonoBehaviour
{
    public Vector2Int gridSize;
    public Vector3 gridOffset;

    public SingleGrid[,] grid;



    public List<CanRow> _canRows=new List<CanRow>();



    public GameObject _canPrefab;
    public GameObject _gridPrefab;

    public Transform _startGridPosition;



    public List<Transform> _canDropPositions;


    public AnimationCurve fallCurve;
    public static TileManager instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        SetGrid();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            CheckBottomTiles();
        }
    }


    public void CheckBottomTiles()
    {
        Debug.Log("Checked");
        bool allSameColor = true;
        int startColorNumber = grid[0, 0]._canInside.colorNumber;
        List<SingleCan> _sameColoredCans = new List<SingleCan>();
        for (int i = 0; i < gridSize.x; i++)
        {
            if(grid[i, 0]._canInside.colorNumber == startColorNumber)
            {
                Debug.Log("SameColored");
                _sameColoredCans.Add(grid[i, 0]._canInside);
            }
            else
            {
                Debug.Log("NotSameColored");
                allSameColor = false;
            }
        }

        if (allSameColor)
        {
            Debug.Log("AllSameColor");
            StartCoroutine(MakeCansFall(_sameColoredCans));
        }
    }
    public IEnumerator MakeCansFall(List<SingleCan> cansToDrop)
    {
        for (int i = 0; i < cansToDrop.Count; i++)
        {
            cansToDrop[i].GetComponent<Collider>().enabled = false;
        }
            yield return new WaitForSeconds(.3f);
        for (int i = 0; i < cansToDrop.Count; i++)
        {
            cansToDrop[i]._gridParent.empty = true;
            Debug.Log("CanDropped");
            SingleCan _canToDrop = cansToDrop[i];
            _canToDrop.transform.parent = null;
            bool firstOne = false;
            if (i == 0)
            {
                firstOne = true;
            }
            
            _canToDrop.transform.DOMove(_canDropPositions[i].position, .3f*Random.Range(.8f,1.2f)).SetEase(fallCurve).OnComplete(delegate {
                
                int isMinus = 0;
                int randomInt = Random.Range(0,2);
                if (randomInt == 0)
                {
                    isMinus = 1;
                }
                else
                {
                    isMinus = -1;
                }
                //_canToDrop.transform.DOPunchRotation(new Vector3(30, 30, 30) * Random.Range(1, 1.2f)*isMinus, .2f);
                _canToDrop.transform.DOShakeRotation(.3f, 20, 10, 90);
                _canToDrop.transform.DOPunchScale(Vector3.one * .2f, .2f);
                _canToDrop.transform.DOJump(_canToDrop.transform.position, 2*Random.Range(1,1.3f), 1, .3f).OnComplete(delegate {

                    WaitZone.instance.GetCan(_canToDrop);
                });
                Debug.Log("MoveFinished");

            });
            
            MoveCans(_canToDrop._gridParent.gridPosition.x);
        }
    }
    public void AddCanToLine(int xNumber)
    {
        if (_canRows[xNumber].currentNumber < _canRows[xNumber].cansToCome.Count - 1)
        {
            GameObject _can = Instantiate(_canPrefab);
            //_can.GetComponent<SingleCan>().SetCan(_canRows[xNumber].cansToCome[_canRows[xNumber].currentNumber]);
            _can.GetComponent<SingleCan>().SetCan(Random.Range(0, 6));
            grid[xNumber, gridSize.y-1].AfterGetCan(_can.GetComponent<SingleCan>());
            _canRows[xNumber].currentNumber++;
        }
    }
    public void MoveCans(int xNumber)
    {
        Debug.Log("CansMoving");
        for (int i=0;i<gridSize.y; i++)
        {
            Debug.Log(i + "INumber_" + xNumber+"GridSize_"+gridSize.y);
            if(!grid[xNumber, i].empty)
            {
                if (i != 0)
                {
                    Debug.Log(xNumber + "_" + (i) + "Empty");

                    MoveCan(grid[xNumber, i]._canInside, grid[xNumber, i - 1]);
                    grid[xNumber, i]._canInside = null;
                    grid[xNumber, i].empty = true;
                }
            }
        }
        AddCanToLine(xNumber);
    }
    public void MoveCan(SingleCan _canToSlide,SingleGrid _gridToGet)
    {
        _gridToGet.SlideCan(_canToSlide);
    }
    public void SetGrid()
    {
        grid = new SingleGrid[gridSize.x, gridSize.y];

        for(int x = 0; x  < gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                GameObject _gridPiece = Instantiate(_gridPrefab);
                grid[x, y] = _gridPiece.GetComponent<SingleGrid>();
                grid[x, y].gridPosition = new Vector2Int(x, y);
                _gridPiece.transform.position = _startGridPosition.position + new Vector3(x * gridOffset.x, y * gridOffset.y, y * gridOffset.z);
                GameObject _can = Instantiate(_canPrefab);
                _canRows[x].currentNumber++;
                //_can.GetComponent<SingleCan>().SetCan(_canRows[x].cansToCome[y]);
                _can.GetComponent<SingleCan>().SetCan(Random.Range(0, 6));
                grid[x, y].StartGetCan(_can.GetComponent<SingleCan>());
            }
        }
    }
}
