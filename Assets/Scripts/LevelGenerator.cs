using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class LevelGenerator : MonoBehaviour {
    public LevelData levelData;
    public List<string> levelTemplates;

    const int ROOM_SIZE = 10; //10*10
    const int ROOM_COUNT = 5; //5*5 = 25
    readonly Vector2 LEVEL_POS = new Vector2(0.0f, 0.0f);

    private void Create(ObjectData objDat, GameObject parent) {
        GameObject obj = new GameObject("Object");
        //set position
        obj.transform.position = new Vector3(objDat.pos.x + 1, objDat.pos.y + 1, objDat.pos.y); //upper walls should be drawn under lower walls
        //set sprite
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "General";
        switch(objDat.objID) {
            case 0: sr.sprite = levelData.wall;  break;
            case 1: sr.sprite = levelData.smallObstacle; break;
            case 2: sr.sprite = levelData.largeObstacle; break;
            case 3: sr.sprite = levelData.visionBlocker; break;
        }
        //set parent
        obj.transform.parent = parent.transform;
    }
    private GameObject CreateWall(Vector2 pos, GameObject parent) {
        GameObject wall = new GameObject("Wall");
        //set position
        wall.transform.position = new Vector3(pos.x, pos.y, pos.y); //upper walls should be drawn under lower walls
        //set sprite
        SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "General";
        sr.sprite = levelData.wall;
        //set parent
        wall.transform.parent = parent.transform;

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
        sr.sprite = floorSprite;
        sr.sortingLayerName = "Floor";
        floor.transform.position = pos;
        floor.transform.parent = parent.transform;
    }
    private void GenerateRoom(Vector2 pos, GameObject parent) {
        print("called");
        GameObject room = new GameObject("Room");
        //set floors
        for(int i = 1; i < ROOM_SIZE - 1; i++) {
            for(int j = 1; j < ROOM_SIZE - 1; j++) {
                SetFloor(new Vector2(i, j), room);
            }
        }

        //generate the room from a random template        
        int idx = Random.Range(0, levelTemplates.Count);
        byte[] levelCodes = File.ReadAllBytes(levelTemplates[idx]);
        foreach(byte levelCode in levelCodes) 
            Create(LevelCoder.Decode(levelCode), room);

        room.transform.parent = parent.transform;
        room.transform.position = pos;
    }

    void Start() {        
        GameObject level = new GameObject("Level");
        GameObject wallGrid = new GameObject("Wall Grid");
        wallGrid.transform.parent = level.transform;
        wallGrid.transform.position = LEVEL_POS;

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
