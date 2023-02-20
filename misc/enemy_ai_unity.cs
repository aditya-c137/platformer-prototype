using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class groundPathfinder: MonoBehaviour {
  public static groundPathfinder instance;
  public Tilemap GroundMap;
  public Graph currentGraph;
  public List < GroundPath > allGeneratedPaths = new List < GroundPath > ();
  public bool DrawGraph;
  public bool drawPath1;
  public List < Transform > StartPathGenerating = new List < Transform > ();
  void Awake() {
    instance = this;
    startAfterDelay();
  }
  void startAfterDelay() {
    //await System.Threading.Tasks.Task.Delay(4);
    SetGraph();
    foreach(Transform trans in StartPathGenerating) {
      addPath(trans);
    }
    foreach(GroundPath path in allGeneratedPaths) {
      UpdatePath(path);
    }
  }
  // Update is called once per frame
  void Update() {
    List < GroundPath > toDelete = new List < GroundPath > ();
    foreach(GroundPath path in allGeneratedPaths) {
      if (path.goal != null) {
        if (Vector2.Distance(path.goal.position, path.rememberPosition) > path.updateDistanceThreshold || path.instantUpdate) UpdatePath(path);
      }
      else toDelete.Add(path);
    }
    foreach(GroundPath pathToDelete in toDelete) {
      allGeneratedPaths.RemoveAt(allGeneratedPaths.IndexOf(pathToDelete));
    }
  }
  //function to updateAPath
  private void UpdatePath(GroundPath path) {
    path.rememberPosition = path.goal.position;
    Vector2Int currentNodePos = currentGraph.getNearestNode(path.goal.position);
    if (path.goalNodePos == currentNodePos && !path.instantUpdate) return;
    path.instantUpdate = false;
    path.goalNodePos = currentNodePos;
    path.waypoints.Clear();
    bool[, ] isNode_ = (bool[, ]) currentGraph.isNode.Clone();
    Vector2[, ] jumpPathGrid = new Vector2[currentGraph.XLenght, currentGraph.YLenght];
    List < openData > open = new List < openData > ();
    isNode_[currentNodePos.x - currentGraph.minPoint.x, currentNodePos.y - currentGraph.minPoint.y] = false;
    path.waypoints.Add(new GroundWaypoint(currentNodePos + new
    Vector2(0.5f, 0.5f), new Vector2(0, 0), new JumpWaypoint(false, 0f, 0f), currentNodePos));
    if (isNode_[currentNodePos.x + 1 - currentGraph.minPoint.x, currentNodePos.y - currentGraph.minPoint.y]) {
      open.Add(new openData(new Vector2Int(currentNodePos.x + 1 - currentGraph.minPoint.x, currentNodePos.y - currentGraph.minPoint.y), Vector2.left));
      isNode_[currentNodePos.x + 1 - currentGraph.minPoint.x, currentNodePos.y - currentGraph.minPoint.y] = false;
    }
    if (isNode_[currentNodePos.x - 1 - currentGraph.minPoint.x, currentNodePos.y - currentGraph.minPoint.y]) {
      open.Add(new openData(new Vector2Int(currentNodePos.x - 1 - currentGraph.minPoint.x, currentNodePos.y - currentGraph.minPoint.y), Vector2.right));
      isNode_[currentNodePos.x - 1 - currentGraph.minPoint.x, currentNodePos.y - currentGraph.minPoint.y] = false;
    }
    if (isNode_[currentNodePos.x - currentGraph.minPoint.x, currentNodePos.y - 1 - currentGraph.minPoint.y]) {
      open.Add(new openData(new Vector2Int(currentNodePos.x - currentGraph.minPoint.x, currentNodePos.y - 1 - currentGraph.minPoint.y), Vector2.up));
      isNode_[currentNodePos.x - currentGraph.minPoint.x, currentNodePos.y - 1 - currentGraph.minPoint.y] = false;
    }
    if (isNode_[currentNodePos.x - currentGraph.minPoint.x, currentNodePos.y + 1 - currentGraph.minPoint.y]) {
      open.Add(new openData(new Vector2Int(currentNodePos.x - currentGraph.minPoint.x, currentNodePos.y + 1 - currentGraph.minPoint.y), Vector2.down));
      isNode_[currentNodePos.x - currentGraph.minPoint.x, currentNodePos.y + 1 - currentGraph.minPoint.y] = false;
    }
    while (open.Count > 0) {
      Vector2Int currentPos = open[0].pos;
      if (isNode_[currentPos.x + 1, currentPos.y]) {
        open.Add(new openData(new Vector2Int(currentPos.x + 1, currentPos.y), Vector2.left));
        isNode_[currentPos.x + 1, currentPos.y] = false;
      }
      if (isNode_[currentPos.x - 1, currentPos.y]) {
        open.Add(new openData(new Vector2Int(currentPos.x - 1, currentPos.y), Vector2.right));
        isNode_[currentPos.x - 1, currentPos.y] = false;
      }
      if (isNode_[currentPos.x, currentPos.y - 1]) {
        open.Add(new openData(new Vector2Int(currentPos.x, currentPos.y - 1), Vector2.up));
        isNode_[currentPos.x, currentPos.y - 1] = false;
      }
      if (isNode_[currentPos.x, currentPos.y + 1]) {
        open.Add(new openData(new Vector2Int(currentPos.x, currentPos.y + 1), Vector2.down));
        isNode_[currentPos.x, currentPos.y + 1] = false;
      }
      path.waypoints.Add(new GroundWaypoint(new
      Vector2((float) currentPos.x + 0.5f + currentGraph.minPoint.x, (float) currentPos.y + 0.5f + currentGraph.minPoint.y), open[0].dir, new
      JumpWaypoint(!currentGraph.isNode[currentPos.x, currentPos.y - 1] && open[0].dir == Vector2.up, 0f, 0f), currentPos));
      jumpPathGrid[currentPos.x, currentPos.y] = open[0].dir;
      isNode_[currentPos.x, currentPos.y] = false;
      open.RemoveAt(0);
    }
    foreach(GroundWaypoint waypoint in path.waypoints) {
      if (waypoint.jump.isJump) {
        int hieght = 1;
        int right = 0;
        Vector2Int gridPos = waypoint.gridPosition + Vector2Int.up;
        for (int i = 50; i > 0; i--) {
          if (jumpPathGrid[gridPos.x, gridPos.y] == Vector2.up && currentGraph.isNode[gridPos.x, gridPos.y - 1]) {
            hieght++;
          }
          else if (jumpPathGrid[gridPos.x, gridPos.y] == Vector2.left && currentGraph.isNode[gridPos.x, gridPos.y - 1]) {
            right--;
          }
          else if (jumpPathGrid[gridPos.x, gridPos.y] == Vector2.right && currentGraph.isNode[gridPos.x, gridPos.y - 1]) {
            right++;
          }
          gridPos += new
          Vector2Int(Mathf.RoundToInt(jumpPathGrid[gridPos.x, gridPos.y].x), Mathf.RoundToInt(jumpPathGrid[gridPos.x, gridPos.y].y));
        }
        waypoint.jump.jumpHeight = hieght;
        waypoint.jump.rightMovement = right;
      }
    }
  }
  public StaticGroundPath getStaticPath(Vector2 pos) {
    StaticGroundPath setUpPath = new StaticGroundPath(pos);
    setUpPath.goal = pos;
    Vector2Int currentNodePos = currentGraph.getNearestNode(setUpPath.goal);
    setUpPath.goalNodePos = currentNodePos;
    setUpPath.waypoints.Clear();
    bool[, ] isNode_ = (bool[, ]) currentGraph.isNode.Clone();
    Vector2[, ] jumpPathGrid = new Vector2[currentGraph.XLenght, currentGraph.YLenght];
    List < openData > open = new List < openData > ();
    isNode_[currentNodePos.x - currentGraph.minPoint.x, currentNodePos.y - currentGraph.minPoint.y] = false;
    setUpPath.waypoints.Add(new GroundWaypoint(currentNodePos + new
    Vector2(0.5f, 0.5f), new Vector2(0, 0), new JumpWaypoint(false, 0f, 0f), currentNodePos));
    if (isNode_[currentNodePos.x + 1 - currentGraph.minPoint.x, currentNodePos.y - currentGraph.minPoint.y]) {
      open.Add(new openData(new Vector2Int(currentNodePos.x + 1 - currentGraph.minPoint.x, currentNodePos.y - currentGraph.minPoint.y), Vector2.left));
      isNode_[currentNodePos.x + 1 - currentGraph.minPoint.x, currentNodePos.y - currentGraph.minPoint.y] = false;
    }
    if (isNode_[currentNodePos.x - 1 - currentGraph.minPoint.x, currentNodePos.y - currentGraph.minPoint.y]) {
      open.Add(new openData(new Vector2Int(currentNodePos.x - 1 - currentGraph.minPoint.x, currentNodePos.y - currentGraph.minPoint.y), Vector2.right));
      isNode_[currentNodePos.x - 1 - currentGraph.minPoint.x, currentNodePos.y - currentGraph.minPoint.y] = false;
    }
    if (isNode_[currentNodePos.x - currentGraph.minPoint.x, currentNodePos.y - 1 - currentGraph.minPoint.y]) {
      open.Add(new openData(new Vector2Int(currentNodePos.x - currentGraph.minPoint.x, currentNodePos.y - 1 - currentGraph.minPoint.y), Vector2.up));
      isNode_[currentNodePos.x - currentGraph.minPoint.x, currentNodePos.y - 1 - currentGraph.minPoint.y] = false;
    }
    if (isNode_[currentNodePos.x - currentGraph.minPoint.x, currentNodePos.y + 1 - currentGraph.minPoint.y]) {
      open.Add(new openData(new Vector2Int(currentNodePos.x - currentGraph.minPoint.x, currentNodePos.y + 1 - currentGraph.minPoint.y), Vector2.down));
      isNode_[currentNodePos.x - currentGraph.minPoint.x, currentNodePos.y + 1 - currentGraph.minPoint.y] = false;
    }
    while (open.Count > 0) {
      Vector2Int currentPos = open[0].pos;
      if (isNode_[currentPos.x + 1, currentPos.y]) {
        open.Add(new openData(new Vector2Int(currentPos.x + 1, currentPos.y), Vector2.left));
        isNode_[currentPos.x + 1, currentPos.y] = false;
      }
      if (isNode_[currentPos.x - 1, currentPos.y]) {
        open.Add(new openData(new Vector2Int(currentPos.x - 1, currentPos.y), Vector2.right));
        isNode_[currentPos.x - 1, currentPos.y] = false;
      }
      if (isNode_[currentPos.x, currentPos.y - 1]) {
        open.Add(new openData(new Vector2Int(currentPos.x, currentPos.y - 1), Vector2.up));
        isNode_[currentPos.x, currentPos.y - 1] = false;
      }
      if (isNode_[currentPos.x, currentPos.y + 1]) {
        open.Add(new openData(new Vector2Int(currentPos.x, currentPos.y + 1), Vector2.down));
        isNode_[currentPos.x, currentPos.y + 1] = false;
      }
      setUpPath.waypoints.Add(new GroundWaypoint(new
      Vector2((float) currentPos.x + 0.5f + currentGraph.minPoint.x, (float) currentPos.y + 0.5f + currentGraph.minPoint.y), open[0].dir, new
      JumpWaypoint(!currentGraph.isNode[currentPos.x, currentPos.y - 1] && open[0].dir == Vector2.up, 0f, 0f), currentPos));
      jumpPathGrid[currentPos.x, currentPos.y] = open[0].dir;
      isNode_[currentPos.x, currentPos.y] = false;
      open.RemoveAt(0);
    }
    foreach(GroundWaypoint waypoint in setUpPath.waypoints) {
      if (waypoint.jump.isJump) {
        int hieght = 1;
        int right = 0;
        Vector2Int gridPos = waypoint.gridPosition + Vector2Int.up;
        for (int i = 50; i > 0; i--) {
          if (jumpPathGrid[gridPos.x, gridPos.y] == Vector2.up && currentGraph.isNode[gridPos.x, gridPos.y - 1]) {
            hieght++;
          }
          else if (jumpPathGrid[gridPos.x, gridPos.y] == Vector2.left && currentGraph.isNode[gridPos.x, gridPos.y - 1]) {
            right--;
          }
          else if (jumpPathGrid[gridPos.x, gridPos.y] == Vector2.right && currentGraph.isNode[gridPos.x, gridPos.y - 1]) {
            right++;
          }
          gridPos += new
          Vector2Int(Mathf.RoundToInt(jumpPathGrid[gridPos.x, gridPos.y].x), Mathf.RoundToInt(jumpPathGrid[gridPos.x, gridPos.y].y));
        }
        waypoint.jump.jumpHeight = hieght;
        waypoint.jump.rightMovement = right;
      }
    }
    setUpPath.pathGrid = (Vector2[, ]) jumpPathGrid.Clone();
    return setUpPath;
  }
  private class openData {
    public Vector2Int pos;
    public Vector2 dir;
    public openData(Vector2Int newPos, Vector2 newDirection) {
      pos = newPos;
      dir = newDirection;
    }
  }
  public GroundPath getGeneratedPath(Transform goalTransform) {
    foreach(GroundPath path in allGeneratedPaths) {
      if (path.goal == goalTransform) return path;
    }
    return null;
  }
  public void addPath(Transform goalTransform) {
    foreach(GroundPath path in allGeneratedPaths) {
      if (goalTransform == path.goal) return;
    }
    GroundPath newPath = new GroundPath(goalTransform, true);
    allGeneratedPaths.Add(newPath);
    UpdatePath(newPath);
  }
  public void SetGraph() {
    Graph setUpGraph = new Graph();
    setUpGraph.maxPoint = (Vector2Int) GroundMap.cellBounds.max;
    setUpGraph.minPoint = (Vector2Int) GroundMap.cellBounds.min;
    setUpGraph.XLenght = GroundMap.cellBounds.max.x - GroundMap.cellBounds.min.x;
    setUpGraph.YLenght = GroundMap.cellBounds.max.y - GroundMap.cellBounds.min.y;
    setUpGraph.isNode = new bool[setUpGraph.XLenght, setUpGraph.YLenght];
    foreach(Vector2Int pos in GroundMap.cellBounds.allPositionsWithin) {
      setUpGraph.isNode[pos.x - setUpGraph.minPoint.x, pos.y - setUpGraph.minPoint.y] = !GroundMap.HasTile((Vector3Int) pos) && GroundMap.HasTile((Vector3Int) pos - Vector3Int.up);
    }
    List < Vector2Int > setPositions = new List < Vector2Int > ();
    for (int x = 0; x < setUpGraph.XLenght; x++) {
      for (int y = 0; y < setUpGraph.YLenght; y++) {
        if (setUpGraph.isNode[x, y]) {
          if (!GroundMap.HasTile(new Vector3Int(x + setUpGraph.minPoint.x - 1, y + setUpGraph.minPoint.y, 0))) setPositions.Add(new Vector2Int(Mathf.Clamp(x - 1, 1, setUpGraph.XLenght - 1), y));
          if (!GroundMap.HasTile(new Vector3Int(x + setUpGraph.minPoint.x + 1, y + setUpGraph.minPoint.y, 0))) setPositions.Add(new Vector2Int(Mathf.Clamp(x + 1, 1, setUpGraph.XLenght - 1), y));
        }
      }
    }
    foreach(Vector2Int pos in setPositions) {
      setUpGraph.isNode[pos.x, pos.y] = true;
    }
    setPositions.Clear();
    for (int x = 0; x < setUpGraph.XLenght; x++) {
      for (int y = 0; y < setUpGraph.YLenght; y++) {
        if (setUpGraph.isNode[x, y]) {
          for (int i = 1; i <= 1001 && !GroundMap.HasTile(new
          Vector3Int(x + setUpGraph.minPoint.x, y + setUpGraph.minPoint.y - i, 0)); i++) {
            setPositions.Add(new Vector2Int(x, y - i));
          }
        }
      }
    }
    foreach(Vector2Int pos in setPositions) {
      setUpGraph.isNode[pos.x, pos.y] = true;
    }
    for (int x = 0; x < setUpGraph.XLenght; x++) {
      for (int y = 0; y < setUpGraph.YLenght; y++) {
        if (setUpGraph.isNode[x, y]) setUpGraph.allNodePositions.Add(new Vector2Int(x + setUpGraph.minPoint.x, y + setUpGraph.minPoint.y));
      }
    }
    currentGraph = setUpGraph;
    foreach(GroundPath path in allGeneratedPaths) {
      path.instantUpdate = true;
    }
  } [System.Serializable]
  public class Graph {
    public bool[, ] isNode;
    public List < Vector2Int > allNodePositions = new
    List < Vector2Int > ();
    public Vector2Int maxPoint;
    public Vector2Int minPoint;
    public int XLenght;
    public int YLenght;
    public Vector2Int getNearestNode(Vector2 pos) {
      float lowestDistance = 10000;
      int index = 0;
      for (int i = 0; i < allNodePositions.Count; i++) {
        float currentDis = Vector2.Distance(pos, (Vector2) allNodePositions[i] + new Vector2(0.5f, 0.5f));
        if (currentDis < lowestDistance) {
          index = i;
          lowestDistance = currentDis;
        }
      }
      return allNodePositions[index];
    }
  } [System.Serializable]
  public class GroundPath {
    public Transform goal;
    public Vector2 rememberPosition;
    public Vector2Int goalNodePos;
    public float updateDistanceThreshold = 0.3f;
    public List < GroundWaypoint > waypoints = new
    List < GroundWaypoint > ();
    public bool instantUpdate;
    public GroundWaypoint getNearestWaypoint(Vector2 pos) {
      float lowestDistance = 10000;
      int index = 0;
      for (int i = 0; i < waypoints.Count; i++) {
        float currentDis = Vector2.Distance(pos, waypoints[i].position);
        if (currentDis < lowestDistance) {
          index = i;
          lowestDistance = currentDis;
        }
      }
      return waypoints[index];
    }
    public GroundPath(Transform newGoal, bool newInstantUpdate) {
      goal = newGoal;
      rememberPosition = newGoal.position;
      instantUpdate = newInstantUpdate;
    }
  } [System.Serializable]
  public class StaticGroundPath {
    public Vector2 goal;
    public Vector2Int goalNodePos;
    public List < GroundWaypoint > waypoints = new
    List < GroundWaypoint > ();
    public Vector2[, ] pathGrid;
    public GroundWaypoint getNearestWaypoint(Vector2 pos) {
      float lowestDistance = 10000;
      int index = 0;
      for (int i = 0; i < waypoints.Count; i++) {
        float currentDis = Vector2.Distance(pos, waypoints[i].position);
        if (currentDis < lowestDistance) {
          index = i;
          lowestDistance = currentDis;
        }
      }
      return waypoints[index];
    }
    public StaticGroundPath(Vector2 newGoal) {
      goal = newGoal;
    }
  } [System.Serializable]
  public class GroundWaypoint {
    public Vector2 position;
    public Vector2 direction;
    public JumpWaypoint jump;
    public Vector2Int gridPosition;
    public GroundWaypoint(Vector2 newPos, Vector2 newDirection, JumpWaypoint newJump, Vector2Int newGridPosition) {
      position = newPos;
      direction = newDirection;
      jump = newJump;
      gridPosition = newGridPosition;
    }
  } [System.Serializable]
  public class JumpWaypoint {
    public bool isJump;
    public float jumpHeight;
    public float rightMovement;
    public JumpWaypoint(bool newIsJump, float newHeight, float
    newRightMovement) {
      isJump = newIsJump;
      jumpHeight = newHeight;
      rightMovement = newRightMovement;
    }
  }
  void OnDrawGizmos() {
    if (drawPath1 && allGeneratedPaths.Count > 0) foreach(GroundWaypoint waypoint in allGeneratedPaths[0].waypoints) {
      Gizmos.DrawSphere(waypoint.position + waypoint.direction * 0.34f, 0.1f);
      Gizmos.DrawLine(waypoint.position + waypoint.direction * 0.34f, waypoint.position - waypoint.direction * 0.34f);
    }
    if (!DrawGraph || currentGraph == null) return;
    for (int x = 0; x < currentGraph.isNode.GetLength(0); x++) {
      for (int y = 0; y < currentGraph.isNode.GetLength(1); y++) {
        if (currentGraph.isNode[x, y]) Gizmos.color = new Color(0, 1, 0, 0.3f);
        else Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireCube(new Vector3(x + currentGraph.minPoint.x + 0.5f, y + currentGraph.minPoint.y + 0.5f, 0), new Vector3(0.4f, 0.4f, 0));
      }
    }
  }
}
The latter needs a tile map. (the tilemap does not have to have a collider or texture, you can
just use blank tiles as positions where colliders of the ground are.Also the bounds of the
tilemap need to bigger than the area u want to cover with the graph, and all the area the
tilemap covers needs to be closed up by tiles and there should be a border around the whole
area out of tiles)—it is a bit messy and you should make your own version of it so it fits
your needs but the basics can be kept. ^ |assign this script to an empty object
The following is the component on the enemy that makes it move: using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class groundEnemyMovement: MonoBehaviour {
  public bool walk = true;
  [Header("Movement")]
  public bool chaise;
  public Transform Target;
  [Header("")]
  private groundPathfinder.GroundWaypoint currentWaypoint;
  public groundPathfinder.JumpWaypoint currentJump;
  public float walkingSpeed = 1300;
  public Transform pathTrackingPosition;
  public FRange speedFactor = new FRange(0.85f, 1.15f);
  [Header("")]
  public float jumpFactor = 24f;
  public int maxJumpHeight;
  public int jumpAmmount = 0;
  private int jumpsLeft;
  [Header("")]
  public float riseGrav = 3;
  public float fallGrav = 4;
  public LayerMask ground = 8;
  [Header("")]
  public bool isOnGround;
  public Transform groundCheckPosition;
  public float groundCheckRadius;
  public float minYVeloAirJump;
  [Header("")]
  public bool facingRight;
  public Vector2 rightScale;
  [Header("Attack")]
  public float attackRange;
  public bool inAttackRange;
  [Header("Patrol")][SerializeField] private FRange waitTimeRange;
  public float patrolSpeed = 600f;
  private groundPathfinder.StaticGroundPath currentPatrolPath;
  private bool awaitNewPatrolPath = false;
  [Header("vfx")]
  private bool rememberOnGround;
  private float lastTimeFiredParticals;
  public GameObject jumpEffect;
  [Header("slowness")]
  public effectManagerGeneral effectManagerGeneral;
  Rigidbody2D rb;
  [HideInInspector] public Vector2 direction = new Vector2(0, 0);
  void Awake() {
    rb = GetComponent < Rigidbody2D > ();
    rightScale = (Vector2) transform.localScale;
    facingRight = true;
    transform.localScale = new Vector3(rightScale.x, rightScale.y, 1f);
    walkingSpeed *= speedFactor.random();
    patrolSpeed *= speedFactor.random();
  }
  void Update() {
    isOnGround = Physics2D.OverlapCircle(groundCheckPosition.position, groundCheckRadius, ground);
    Vector2 trackingPos;
    if (pathTrackingPosition != null) trackingPos = (Vector2) pathTrackingPosition.position;
    else trackingPos = (Vector2) transform.position;
    if (!walk) return;
    if (chaise && Target != null) {
      inAttackRange = Vector2.Distance(rb.position, Target.position) <= attackRange;
      groundPathfinder.instance.addPath(Target);
      //waypoint
      currentWaypoint = groundPathfinder.instance.getGeneratedPath(Target).getNearestWaypoint(t
      rackingPos);
      if (currentWaypoint.jump.isJump) currentJump = currentWaypoint.jump;
      //directinon and force vector
      direction = currentWaypoint.direction;
      //is on Ground
      isOnGround = Physics2D.OverlapCircle(groundCheckPosition.position, groundCheckRadius, ground);
      if (isOnGround) jumpsLeft = jumpAmmount;
      //add force
      if (!inAttackRange) {
        if (direction.y > 0 && currentJump != null) {
          if ((float) currentJump.jumpHeight != 0) direction.x = (float) currentJump.rightMovement / (float) currentJump.jumpHeight;
          else direction.x = 0;
        }
        Vector2 walkVector = Vector2.right * direction.x * walkingSpeed * Time.deltaTime;
        if (effectManagerGeneral != null) walkVector *= (1 - effectManagerGeneral.slowness);
        rb.AddForce(walkVector);
        if (direction.y > 0
        /*&& currentWaypoint.jump.isJump*/
        && currentJump.jumpHeight <= maxJumpHeight) {
          if (isOnGround) {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpFactor
            /*
Mathf.Sqrt(currentJump.jumpHeight)*/
            , ForceMode2D.Impulse);
            if (jumpEffect != null) Instantiate(jumpEffect, transform.position, Quaternion.Euler(0, 0, 18.53f - 180 - direction.x * 35));
          }
          else if (jumpsLeft > 0 && rb.velocity.y <= minYVeloAirJump) {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpFactor
            /*
Mathf.Sqrt(currentJump.jumpHeight)*/
            , ForceMode2D.Impulse);
            jumpsLeft--;
            if (jumpEffect != null) Instantiate(jumpEffect, transform.position, Quaternion.Euler(0, 0, 18.53f - 180 - direction.x * 35));
          }
        }
      }
      else {
        if (Target.position.x > transform.position.x) direction = new Vector2(1, 0);
        else direction = new Vector2( - 1, 0);
      }
      //grav
      if (rb.velocity.y > 0) rb.gravityScale = riseGrav;
      else rb.gravityScale = fallGrav;
      //flip
      if (direction.x < -0.1f && facingRight) {
        facingRight = false;
        transform.localScale = new Vector3( - rightScale.x, rightScale.y, 1f);
      }
      else if (direction.x > 0.1f && !facingRight) {
        facingRight = true;
        transform.localScale = new Vector3(rightScale.x, rightScale.y, 1f);
      }
      direction = new Vector2(0, 0);
    }
    else {
      inAttackRange = false;
      if (currentPatrolPath != null) {
        if ((currentPatrolPath.getNearestWaypoint(trackingPos).jump.isJump && currentPatrolPath.getNearestWaypoint(trackingPos).jump.jumpHeight > maxJumpHeight) || currentPatrolPath.getNearestWaypoint(trackingPos).direction == new Vector2(0, 0) || currentPatrolPath == null) {
          if (!awaitNewPatrolPath) GetPatrolPath();
          //direction = new Vector2(0, 0);
        }
        else if (!awaitNewPatrolPath) {
          //waypoint
          currentWaypoint = currentPatrolPath.getNearestWaypoint(trackingPos);
          if (currentWaypoint.jump.isJump) currentJump = currentWaypoint.jump;
          //directinon and force vector
          direction = currentWaypoint.direction;
          //is on Ground
          isOnGround = Physics2D.OverlapCircle(groundCheckPosition.position, groundCheckRadius, ground);
          if (isOnGround) jumpsLeft = jumpAmmount;
          //add force
          if (direction.y > 0 && currentJump != null) {
            if ((float) currentJump.jumpHeight != 0) direction.x = (float) currentJump.rightMovement / (float) currentJump.jumpHeight;
            else direction.x = 0;
          }
          Vector2 walkVector = Vector2.right * direction.x * patrolSpeed * Time.deltaTime;
          if (effectManagerGeneral != null) walkVector *= (1 - effectManagerGeneral.slowness);
          rb.AddForce(walkVector);
          if (direction.y > 0
          /*&&
currentWaypoint.jump.isJump*/
          && currentJump.jumpHeight <= maxJumpHeight) {
            if (isOnGround) {
              rb.velocity = new Vector2(rb.velocity.x, 0);
              rb.AddForce(Vector2.up * jumpFactor
              /*
Mathf.Sqrt(currentJump.jumpHeight)*/
              , ForceMode2D.Impulse);
              if (jumpEffect != null) Instantiate(jumpEffect, transform.position, Quaternion.Euler(0, 0, 18.53f - 180 - direction.x * 35));
            }
            else if (jumpsLeft > 0 && rb.velocity.y <= minYVeloAirJump) {
              rb.velocity = new Vector2(rb.velocity.x, 0);
              rb.AddForce(Vector2.up * jumpFactor
              /*
Mathf.Sqrt(currentJump.jumpHeight)*/
              , ForceMode2D.Impulse);
              jumpsLeft--;
              if (jumpEffect != null) Instantiate(jumpEffect, transform.position, Quaternion.Euler(0, 0, 18.53f - 180 - direction.x * 35));
            }
          }
          //grav
          if (rb.velocity.y > 0) rb.gravityScale = riseGrav;
          else rb.gravityScale = fallGrav;
          //flip
          if (direction.x < -0.1f && facingRight) {
            facingRight = false;
            transform.localScale = new
            Vector3( - rightScale.x, rightScale.y, 1f);
          }
          else if (direction.x > 0.1f && !facingRight) {
            facingRight = true;
            transform.localScale = new
            Vector3(rightScale.x, rightScale.y, 1f);
          }
        }
      }
      else {
        currentPatrolPath = groundPathfinder.instance.getStaticPath(groundPathfinder.instance.curre
        ntGraph.allNodePositions[Random.Range(0, groundPathfinder.instance.currentGraph.allNodePositions.Count)] + new
        Vector2(0.5f, 0.5f));
      }
    }
    if (rememberOnGround != isOnGround) {
      rememberOnGround = isOnGround;
      lastTimeFiredParticals = Time.time;
      RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, ground);
      if (hit && isOnGround && jumpEffect != null) Instantiate(jumpEffect, hit.point, Quaternion.Euler(0, 0, 18.53f));
      else if (isOnGround && jumpEffect != null) Instantiate(jumpEffect, transform.position, Quaternion.Euler(0, 0, 18.53f));
    }
  }
  async void GetPatrolPath() {
    Vector2 trackingPos;
    if (pathTrackingPosition != null) trackingPos = (Vector2) pathTrackingPosition.position;
    else trackingPos = (Vector2) transform.position;
    awaitNewPatrolPath = true;
    await System.Threading.Tasks.Task.Delay(1000 * Mathf.RoundToInt(waitTimeRange.random()));
    if (this == null) return;
    currentPatrolPath = groundPathfinder.instance.getStaticPath(groundPathfinder.instance.curre
    ntGraph.allNodePositions[Random.Range(0, groundPathfinder.instance.currentGraph.allNodePositions.Count)] + new
    Vector2(0.5f, 0.5f));
    for (int i = 10; i > 0 && ((currentPatrolPath.getNearestWaypoint(trackingPos).jump.isJump && currentPatrolPath.getNearestWaypoint(trackingPos).jump.jumpHeight > maxJumpHeight) || currentPatrolPath.getNearestWaypoint(trackingPos).direction == new
    Vector2(0, 0) || groundPathfinder.instance.currentGraph.isNode[currentPatrolPath.goalNod
    ePos.x - groundPathfinder.instance.currentGraph.minPoint.x, currentPatrolPath.goalNodePos.y - 1 - groundPathfinder.instance.currentGraph.minPoint.y]); i--)
    currentPatrolPath = groundPathfinder.instance.getStaticPath(groundPathfinder.instance.curre
    ntGraph.allNodePositions[Random.Range(0, groundPathfinder.instance.currentGraph.allNodePositions.Count)] + new
    Vector2(0.5f, 0.5f));
    awaitNewPatrolPath = false;
  }
  void OnDrawGizmos() {
    Gizmos.DrawWireSphere(groundCheckPosition.position, groundCheckRadius);
    //foreach (groundPathfinder.GroundWaypoint waypoint in
    currentPatrolPath.waypoints)
    //{
    // Gizmos.color = new Color(1, 0, 0, 1);
    // Gizmos.DrawSphere(waypoint.position + waypoint.direction
    * 0.34f,
    0.1f);
    // Gizmos.DrawLine(waypoint.position + waypoint.direction *
    0.34f,
    waypoint.position - waypoint.direction * 0.34f);
    //}
  }
}