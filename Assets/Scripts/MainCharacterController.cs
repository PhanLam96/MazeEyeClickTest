using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MainCharacterController : MonoBehaviour
{
    [SerializeField] private MazeCell standingCell;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rg;
    [SerializeField] private Transform model;

    [Header("Movement")]
    public bool isCanMove;
    [SerializeField] private bool isAutoMove;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Vector3 moveDir;

    private bool isBusy;

    public void OnStartGame()
    {
        isCanMove = true;
    }
    public void OnEndGame()
    {
        isCanMove = false;
        StopCharacter();
    }

    void Update()
    {
        if (!isCanMove || isAutoMove) return;

        var inputHor = Input.GetAxis("Horizontal");
        var inputVer = Input.GetAxis("Vertical");

        moveDir = new Vector3(inputHor, 0, inputVer);
        moveDir.Normalize();

        if (moveDir != Vector3.zero)
            MoveCharacter(moveDir);
        else
            StopCharacter();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var mazeManager = GameManager.Instance.mazeManager;
            if (standingCell != null)
            {
                var listPath = mazeManager.listMazeGrid[mazeManager.currentLevel].GetExitPath(standingCell);
                var listPoint = new List<Vector3>();
                foreach (var cell in listPath)
                {
                    listPoint.Add(cell.transform.position);
                    if (cell.cellType == MazeCell.MazeCellGroundType.Normal)                    
                        cell.SetGround(MazeCell.MazeCellGroundType.Path);
                }

                if (listPoint.Count > 0)
                {
                    AutoMoveCharacter(listPoint);
                }
            }
        }
    }
    private void MoveCharacter(Vector3 moveDir)
    {
        rg.AddForce(moveDir * moveSpeed * Time.deltaTime);
        Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
        model.rotation = Quaternion.RotateTowards(model.rotation, toRotation, rotateSpeed * Time.deltaTime);
        animator.SetBool("isMoving", true);
        GameManager.Instance.soundManager.PlaySound(0);
    }
    private void StopCharacter()
    {
        rg.velocity = Vector3.zero;
        rg.angularVelocity = Vector3.zero;
        animator.SetBool("isMoving", false);
        GameManager.Instance.soundManager.StopSound();
    }
    private async void AutoMoveCharacter(List<Vector3> listPath)
    {
        moveSpeed *= 2;
        isAutoMove = true;
        foreach (var point in listPath)
        {
            await MoveToPointAsync(point);
        }
        moveSpeed /= 2;
        isAutoMove = false;
    }
    private async Task MoveToPointAsync(Vector3 target)
    {
        while (IsReachTarget(target) == false)
        {
            var moveDir = target - transform.position;
            MoveCharacter(moveDir);
            await Task.Delay(20);
        }
        StopCharacter();
    }
    private bool IsReachTarget(Vector3 target)
    {
        var distanceX = Mathf.Abs(target.x - transform.position.x);
        var distanceZ = Mathf.Abs(target.z - transform.position.z);
        return (distanceX < 0.1f && distanceZ < 0.1f); 
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isBusy) return;
        isBusy = true;
        if (other.gameObject.tag == "MazeTrigger")
        {
            if (other.gameObject.name == "End")
            {
                GameManager.Instance.EndGame();
                isCanMove = false;
            }
            else
            {
                int.TryParse(other.gameObject.name, out var level);
                GameManager.Instance.mazeManager.OnTriggerMaze(level);
            }
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "MazeCell")
        {
            GameManager.Instance.uiManager.AddStep();
            var cell = other.gameObject.GetComponent<MazeCell>();
            if (cell.cellType == MazeCell.MazeCellGroundType.Start)
            {
                StartCoroutine(Utils.IEDelayCall(() => cell.BuildWall("BOTTOM"), 1));
            }
            if (cell.cellType == MazeCell.MazeCellGroundType.End)
            {
                GameManager.Instance.mazeManager.EnableNextArea();
            }
        }
        StartCoroutine(Utils.IEDelayCall(()=> isBusy = false, 0.2f));
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "MazeCell")
        {
            standingCell = other.GetComponent<MazeCell>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "MazeCell")
        {
            standingCell = null;
        }
    }
}
