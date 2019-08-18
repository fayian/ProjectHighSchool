using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using File = ProjectHighSchool.File;

public class LevelGenerator : MonoBehaviour {
    public LevelData levelData;
    public List<string> levelTemplates;

    const int ROOM_SIZE = 10; //10*10
    const int ROOM_COUNT = 5; //5*5 = 25
    readonly Vector2 LEVEL_POS = new Vector2(0.0f, 0.0f);

    private GameObject[] prefabs = new GameObject[3];

    private void Create(ObjectData objDat, GameObject parent) {
        objDat.pos += new Vector2(1.0f, 1.0f); //offset because of the walls

        GameObject obj = null;
        switch (objDat.objID) {
            case 0:
                obj = CreateWall(objDat.pos, parent);
            return; //already using CreateWall function to finish other stuff
            case 1:
                obj = CreateWall(objDat.pos, parent);
                obj.GetComponent<SpriteRenderer>().sprite = levelData.smallObstacle;
            return; //already using CreateWall function to finish other stuff
            case 2:
                obj = Instantiate(prefabs[0]);
                obj.GetComponent<SpriteRenderer>().sprite = levelData.largeObstacle;                
                break;
            case 3: obj = Instantiate(prefabs[2]);
                obj.GetComponent<SpriteRenderer>().sprite = levelData.visionBlocker;                
            break;
        }

        obj.transform.parent = parent.transform;
        obj.transform.localPosition = new Vector2(objDat.pos.x, objDat.pos.y);
        obj.transform.position += new Vector3(0.0f, 0.0f, obj.transform.position.y); //upper objects should be drawn earlier
        if (objDat.objID == 3) obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -1.0f); //vision blockers are in floor layer, so they so be drawn after normal floors
        if (objDat.objID == 2) { //set the levelMapForMob of big obstacles
            int x = (int)obj.transform.position.x; int y = (int)obj.transform.position.y;
            Global.levelMapForMob[x][y] = true;
            Global.levelMapForMob[x + 1][y] = true;
            Global.levelMapForMob[x][y + 1] = true;
            Global.levelMapForMob[x + 1][y + 1] = true;
        }
    }
    private GameObject CreateWall(Vector2 pos, GameObject parent) {
        GameObject wall = Instantiate(prefabs[1]);
        //transform
        wall.transform.parent = parent.transform;
        wall.transform.localPosition = new Vector2(pos.x, pos.y);
        wall.transform.position += new Vector3(0.0f, 0.0f, wall.transform.position.y); //upper objects should be drawn earlier
        //set sprite
        wall.GetComponent<SpriteRenderer>().sprite = levelData.wall;        
        //add the wall to the levelMapForMob
        Global.levelMapForMob[(int)(wall.transform.position.x)][(int)(wall.transform.position.y)] = true;

        return wall;
    }
    private void CreateWalls(GameObject parent) {
        //create horizontal walls
        for(int i = 0; i <= ROOM_COUNT * (ROOM_SIZE - 1); i += ROOM_SIZE - 1) {
            for (int j = 0; j <= ROOM_COUNT * (ROOM_SIZE - 1); j++) {

                //(                    not the first or last row                       ) && (         in the middle        )
                if (i != 0 && i != ROOM_COUNT * (ROOM_SIZE - 1) && (j % 9 == 4 || j % 9 == 5)) //create passages
                    SetFloor(new Vector2(j, i), parent);
                else CreateWall(new Vector2(j, i), parent);
            }
        }
        //create vertical walls
        for(int i = 0; i <= ROOM_COUNT * (ROOM_SIZE - 1); i += ROOM_SIZE - 1) {
            for (int j = 0; j <= ROOM_COUNT * (ROOM_SIZE - 1); j++) {
                if (j % (ROOM_SIZE - 1) == 0) continue; // skip when there is already a wall

                if (i != 0 && i != ROOM_COUNT * (ROOM_SIZE - 1) && (j % 9 == 4 || j % 9 == 5))  //create passages
                    SetFloor(new Vector2(i, j), parent);
                else CreateWall(new Vector2(i, j), parent);
            }
        }
    }
    private void SetFloor(Vector2 pos, GameObject parent) {     

        //have a 2/15 chance to use special sprite on this floor
        Sprite floorSprite;
        int floorKind = Random.Range(0, 15);
        if (floorKind == 0) floorSprite = levelData.specialFloor0;
        else if (floorKind == 1) floorSprite = levelData.specialFloor1;
        else floorSprite = levelData.floor;
        //create floor        
        GameObject floor = new GameObject("Floor");
        SpriteRenderer sr = floor.AddComponent<SpriteRenderer>();
        floor.transform.parent = parent.transform;
        floor.transform.localPosition = pos;        
        sr.sprite = floorSprite;
        sr.sortingLayerName = "Floor";
    }
    private void GenerateRoom(Vector2 pos, GameObject parent) {
        GameObject room = new GameObject("Room");

        //transform
        room.transform.parent = parent.transform;
        room.transform.localPosition = pos;

        //set floors
        for (int i = 1; i < ROOM_SIZE - 1; i++) {
            for(int j = 1; j < ROOM_SIZE - 1; j++) {
                SetFloor(new Vector2(i, j), room);
            }
        }

        //generate the room from a random template        
        int idx = Random.Range(0, levelTemplates.Count);
        byte[] levelCodes = File.ReadAllBytes(levelTemplates[idx]);
        foreach(byte levelCode in levelCodes) 
            Create(LevelCoder.Decode(levelCode), room);        
    }

    void Start() {
        //Initialize
        prefabs[0] = Resources.Load("Prefabs/LargeObstacle") as GameObject;
        prefabs[1] = Resources.Load("Prefabs/SmallObstacle") as GameObject;
        prefabs[2] = Resources.Load("Prefabs/VisionBlocker") as GameObject;

        Global.levelMapForMob.Clear();
        Global.levelMapForMob.Capacity = ROOM_COUNT * ROOM_SIZE;
        for (int i = 0; i < ROOM_COUNT * ROOM_SIZE; i++) {
            List<bool> temp = new List<bool>(ROOM_COUNT * ROOM_SIZE);
            for (int j = 0; j < ROOM_COUNT * ROOM_SIZE; j++) {
                temp.Add(false);
            }
            Global.levelMapForMob.Add(temp);
        }
        //Create level
        GameObject level = new GameObject("Level");
        GameObject wallGrid = new GameObject("Wall Grid");
        level.transform.position = LEVEL_POS;
        wallGrid.transform.parent = level.transform;

        CreateWalls(wallGrid);

        for (int i = 0; i < ROOM_COUNT; i++)
            for (int j = 0; j < ROOM_COUNT; j++)
                GenerateRoom(new Vector2(i * (ROOM_SIZE - 1), j * (ROOM_SIZE - 1)), level);
    }
    private struct ObjectData {
        public Vector2 pos;
        public short objID;
    }
    private static class LevelCoder {
        //level code: 00  000 000   <= a byte
        //                     id     x      y
        //id:
        //  wall:0   small obstacle:1   large obstacle:2   vision blocker:3

        public static ObjectData Decode(byte code) {
            ObjectData rtn = new ObjectData {
                pos = new Vector2((code & 0b00_111_000) / 0b1000, code & 0b00_000_111),
                objID = (short)((code & 0b11_000_000) / 0b1000000)
            };
            return rtn;
        }
        public static byte Encode(ObjectData data) {
            return (byte)(data.pos.x + data.pos.y * 010 + data.objID * 0100);
        }
    }

    //TODO:
    //private static class MazeGeneration {
    //}
}
