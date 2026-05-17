//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//namespace CarRacing.Maps
//{
//    public class MapServices : MonoBehaviour
//    {
//        [Header("Prefabs")]
//        public GameObject roadSegmentPrefab;
//        public GameObject[] obstaclePrefabs;
//        public GameObject[] decorationPrefabs;


//        [Header("Map File")]
//        public string jsonFileName = "RaceMap01.json"; // inside StreamingAssets

//        private string FilePath => Path.Combine(Application.streamingAssetsPath, jsonFileName);

//        void Start()
//        {
//            LoadAndBuildMap();
//        }

//        public void LoadAndBuildMap()
//        {
//            if (!File.Exists(FilePath))
//            {
//                Debug.LogError($"Map JSON not found: {FilePath}");
//                return;
//            }

//            string json = File.ReadAllText(FilePath);
//            MapDataWrapper wrapper = JsonUtility.FromJson<MapDataWrapper>(json);

//            if (wrapper == null)
//            {
//                Debug.LogError("Failed to parse map JSON.");
//                return;
//            }

//            BuildPath(wrapper.paths);
//            BuildObstacles(wrapper.obstacles);
//            BuildDecorations(wrapper.decorations);
//        }

//        void BuildPath(MapPath[] paths)
//        {
//            foreach (var path in paths)
//            {
//                for (int i = 0; i < path.points.Length - 1; i++)
//                {
//                    Vector2 start = path.points[i];
//                    Vector2 end = path.points[i + 1];

//                    Vector2 midPoint = (start + end) / 2f;
//                    float distance = Vector2.Distance(start, end);
//                    float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;

//                    GameObject segment = Instantiate(roadSegmentPrefab, midPoint, Quaternion.Euler(0, 0, angle));
//                    segment.transform.localScale = new Vector3(distance, segment.transform.localScale.y, 1);
//                }
//            }
//        }

//        void BuildObstacles(MapObstacle[] obstacles)
//        {
//            foreach (var obs in obstacles)
//            {
//                GameObject prefab = FindByType(obs.obstacleType, obstaclePrefabs);
//                if (prefab != null)
//                    Instantiate(prefab, obs.position, Quaternion.identity);
//            }
//        }

//        void BuildDecorations(MapDecoration[] decorations)
//        {
//            foreach (var deco in decorations)
//            {
//                GameObject prefab = FindByType(deco.visualName, decorationPrefabs);
//                if (prefab != null)
//                    Instantiate(prefab, deco.position, Quaternion.identity);
//            }
//        }

//        GameObject FindByType(string typeName, GameObject[] prefabArray)
//        {
//            foreach (var prefab in prefabArray)
//            {
//                if (prefab.name.Equals(typeName, System.StringComparison.OrdinalIgnoreCase))
//                    return prefab;
//            }
//            return null;
//        }
//    }

//}