using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;
    private int MESH_SCALE = 100;
    public GameObject[] objects;
    public AnimationCurve heightCurve;
    private Vector3[] vertices;
    private int[] triangles;

    private Color[] colors;
    public Gradient gradient;

    private float minTerrainheight;
    private float maxTerrainheight;

    private float lastNoiseHeight;

    private int horizontalSize = 100;
    private int verticalSize = 100;

    public int seed;

    [Range(25f, 75f)]
    public float scale;

    [Range(1.5f, 3)]
    public float lacunarity;

    // Octaves are for how many times the perlin noise is applied
    [Range(1f, 6f)]
    public int octaves;

    [Range(10, 35)]
    public float amplitude;
    private float initialAmplitude;

    [Range(0, 2.5f)]
    public float frequency = 1;
    private float initialFrequency;

    [Range(0, 0.75f)]
    public float persistence = 0.5f;
    private float initialPersistence;



    // A gameobject to parent all spawned in enviromental elements onto
    public GameObject enviromentParent;

    // Singleton instance
    public static TerrainGenerator instance;
    public void Awake()
    {
        // Initialise Singleton
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        instance = this;
    }


    void Start()
    {
        initialAmplitude = amplitude;
        initialFrequency = frequency;
        initialPersistence = persistence;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateNewMap();
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateNewMap();
        }
    }
    public void CreateNewMap()
    {
        ResetTerrainValues();
        CreateMeshShape();
        CreateTriangles();
        ColorMap();
        UpdateMesh();
    }

    // Resets the terrains variables so that a new one can be made
    public void ResetTerrainValues()
    {
        maxTerrainheight = 0;
        minTerrainheight = 0;

        initialAmplitude = amplitude;
        initialFrequency = frequency;
        initialPersistence = persistence;

        mesh.Clear();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Creates the verticies data and stores the positions as vector3s
    private void CreateMeshShape()
    {
        // Creates seed
        Vector2[] octaveOffsets = GetOffsets();

        // Create an array to store verticies data 
        vertices = new Vector3[(verticalSize + 1) * (horizontalSize + 1)];

        // Loops through every position on the size of the soon to be generated terrain and assigns it a vertice
        int verticiesIndex = 0;
        for (int z = 0; z <= verticalSize; z++)
        {
            for (int x = 0; x <= horizontalSize; x++)
            {
                // Set height of vertices
                float noiseHeight = GenerateNoiseHeight(z, x, octaveOffsets);
                SetMinMaxHeights(noiseHeight);
                vertices[verticiesIndex] = new Vector3(x, noiseHeight, z);
                verticiesIndex++;
            }
        }
    }

    // Used to seed the random number generator and set the octaves offsets
    private Vector2[] GetOffsets()
    {
        // Seeds the random so that all generation will always come out the same given the same values
        System.Random rand = new System.Random(seed);

        // Creates a array that holds the octave information (Dictates how smooth the terrain will turn out)
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            // Values of these offsets will always be the same given the seed
            float offsetX = rand.Next(-100000, 100000);
            float offsetY = rand.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
        return octaveOffsets;
    }

    private float GenerateNoiseHeight(int z, int x, Vector2[] octaveOffsets)
    {
        // I moved amplitude and frequency from here to one of the inspector variables
        //float amplitude = 20;
        //float frequency = 1;
        //float persistence = 0.5f;
        float noiseHeight = 0;

        // Loop for as many chosen octaves 
        for (int i = 0; i < octaves; i++)
        {
            float mapZ = (z / scale) * frequency + octaveOffsets[i].y;
            float mapX = (x / scale) * frequency + octaveOffsets[i].x;

            // Doubling the perlinValue and adding 1 to it makes the generated terrain build out from the bottom more visually appealing
            float perlinValue = 2 * (Mathf.PerlinNoise(mapZ, mapX)) - 1;


            noiseHeight += heightCurve.Evaluate(perlinValue) * amplitude;
            frequency *= lacunarity;
            amplitude *= persistence;
        }

        // Puts the amplitude back to its original value so that GenerateNoiseHeight can use them again
        amplitude = initialAmplitude;
        frequency = initialFrequency;
        persistence = initialPersistence;

        return noiseHeight;
    }

    private void SetMinMaxHeights(float noiseHeight)
    {
        // Set min and max height of map for color gradient
        if (noiseHeight > maxTerrainheight)
            maxTerrainheight = noiseHeight;
        if (noiseHeight < minTerrainheight)
            minTerrainheight = noiseHeight;
    }


    private void CreateTriangles()
    {
        // Need 6 vertices to create a square (2 triangles)
        triangles = new int[horizontalSize * verticalSize * 6];

        int vert = 0;
        int tris = 0;
        // Go to next row
        for (int z = 0; z < horizontalSize; z++)
        {
            // fill row
            for (int x = 0; x < horizontalSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + horizontalSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + horizontalSize + 1;
                triangles[tris + 5] = vert + horizontalSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    private void ColorMap()
    {
        colors = new Color[vertices.Length];

        // Loop over vertices and apply a color from the depending on height (y axis value)
        for (int i = 0, z = 0; z < vertices.Length; z++)
        {
            float height = Mathf.InverseLerp(minTerrainheight, maxTerrainheight, vertices[i].y);
            colors[i] = gradient.Evaluate(height);
            i++;
        }
    }

    private void PopulateEnviroment()
    {
        // Destroys all previous spawned enviromental objects
        foreach (Transform child in enviromentParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        Debug.Log(vertices.Length);
        for (int i = 0; i < vertices.Length; i++)
        {
            // find actual position of vertices in the game
            Vector3 worldPt = transform.TransformPoint(mesh.vertices[i]);
            var noiseHeight = worldPt.y;

            // Stop generation if height difference between 2 vertices is too steep
            if (System.Math.Abs(lastNoiseHeight - worldPt.y) < 25)
            {
                // min height for object generation
                if (noiseHeight > 100)
                {
                    // Chance to generate
                    if (Random.Range(1, 5) == 1)
                    {
                        if(objects.Length != 0)
                        {
                            GameObject enviromentalObject = objects[Random.Range(0, objects.Length)];
                            var spawnAboveTerrainBy = noiseHeight * 2;
                            GameObject instantiatedEnviromentObject = Instantiate(enviromentalObject, new Vector3(mesh.vertices[i].x * MESH_SCALE, spawnAboveTerrainBy, mesh.vertices[i].z * MESH_SCALE), Quaternion.identity);

                            // Parents the enviromental object to a gameobject that can later be cleared for new generations
                            instantiatedEnviromentObject.transform.SetParent(enviromentParent.transform, true);
                        }
                        else
                        {
                            Debug.Log("No objects to spawn");
                        }
                    }
                }
            }
            lastNoiseHeight = noiseHeight;
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        GetComponent<MeshCollider>().sharedMesh = mesh;
        gameObject.transform.localScale = new Vector3(MESH_SCALE, MESH_SCALE, MESH_SCALE);

        PopulateEnviroment();
    }

}

