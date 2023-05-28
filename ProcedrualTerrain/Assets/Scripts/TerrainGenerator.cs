using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    // GameObjects which get rotated
    public GameObject water;
    public GameObject terrain;
    public GameObject pivot;

    // GameObject for enviroment objects
    public GameObject house;

    Mesh mesh;
    public List<GameObject> objectsList;
    public AnimationCurve heightCurve;
    
    // Arrays to store values for the generated verticies (triangles are just 3 verticies)
    private int[] triangles;
    private Vector3[] vertices;

    // Tracks the colors put onto the mesh
    public Gradient enviromentGradient;
    private Color[] enviromentColors;
    
    // Clamping values for heights
    private float minTerrainheight;
    private float maxTerrainheight;

    private float lastNoiseHeight;

    // Size of the Terrain, set to 100 for consistency
    private int horizontalSize = 100;
    private int verticalSize = 100;

    // Values that get changed to manipulate how the terrain looks
    public int seed;

    [Range(25f, 75f)]
    public float scale;

    [Range(1.5f, 3)]
    public float lacunarity;

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


    public bool showTriangles = false;

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


    public void Start()
    {
        // Sets a refrence to the mesh that gets created
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Sets all the initial values
        initialAmplitude = amplitude;
        initialFrequency = frequency;
        initialPersistence = persistence;
        
        // Calls function to start the terrain generation
        CreateNewTerrain();
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateNewTerrain();
        }
    }
    public void CreateNewTerrain()
    {
        ResetTerrainValues();
        InitialiseMeshDimensions();
        SetTriangles();
        ColorMap();
        UpdateMeshValues();
        PopulateEnviroment();
    }

    // Resets the terrains variables so that a new one can be made
    public void ResetTerrainValues()
    {
        maxTerrainheight = 0;
        minTerrainheight = 0;

        initialAmplitude = amplitude;
        initialFrequency = frequency;
        initialPersistence = persistence;

        pivot.transform.eulerAngles = new Vector3(0, 0, 0);
        objectsList.Add(house);

        mesh.Clear();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Creates the verticies data and stores the positions as vector3s
    public void InitialiseMeshDimensions()
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
                // Set the height value of the vertices
                float noiseHeight = GenerateNoiseHeight(z, x, octaveOffsets);
                ClampTerrainHeights(noiseHeight);
                vertices[verticiesIndex] = new Vector3(x, noiseHeight, z);
                verticiesIndex++;
            }
        }
    }

    // Used to seed the random number generator and set the octaves offsets
    public Vector2[] GetOffsets()
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

        // Simple null check
        if(octaveOffsets != null)
        {
            return octaveOffsets;
        }
        else
        {
            return GetOffsets();
        }
        
    }

    public float GenerateNoiseHeight(int z, int x, Vector2[] octaveOffsets)
    {
        // Noise height is used to track incrementations while in the forloop of octaves
        float noiseHeight = 0;

        // Loop for as many chosen octaves 
        for (int i = 0; i < octaves; i++)
        {
            float mapZ = (z / scale) * frequency + octaveOffsets[i].y;
            float mapX = (x / scale) * frequency + octaveOffsets[i].x;

            // Doubling the perlinValue and adding 1 to it makes the generated terrain build out from the bottom more visually appealing
            float perlinValue = 2 * (Mathf.PerlinNoise(mapZ, mapX)) - 1;

            // Updates the noiseheight value to reflect the perlin noise
            noiseHeight += heightCurve.Evaluate(perlinValue) * amplitude;

            // Modifies the frequency and amplitude so it can be used in the next loop
            frequency *= lacunarity;
            amplitude *= persistence;
        }

        // Puts the amplitude back to its original value so that GenerateNoiseHeight can use them again
        amplitude = initialAmplitude;
        frequency = initialFrequency;
        persistence = initialPersistence;

        return noiseHeight;
    }

    // Simple fucntion that just clamps the heights for the gradient to use
    public void ClampTerrainHeights(float noiseHeight)
    {
        // Set min and max height of map for color gradient by clamping the values
        if (noiseHeight > maxTerrainheight)
        {
            maxTerrainheight = noiseHeight;
        }
        if (noiseHeight < minTerrainheight)
        {
            minTerrainheight = noiseHeight;
        }
            
    }

    // Creates the physical terrain by creating triangles that fill in the vertcies
    public void SetTriangles()
    {
        // Setting up variables to hold information about how many loops have been completed
        int tris = 0;
        int verticies = 0;

        // 3 Verticies makes up one triangle and two triangles make up a square
        int terrainAreaSize = horizontalSize * verticalSize;
        triangles = new int[terrainAreaSize * 6];

        // Nested for loop lerps through every verticie position on the terrain
        for (int z = 0; z < verticalSize; z++)
        {
            for (int x = 0; x < horizontalSize; x++)
            {
                // Visually shows the triangles created on the terrain or not depending on the booleon
                if (!showTriangles)
                {
                    // Two triangles are created which create a square 
                    triangles[tris + 0] = verticies;
                    triangles[tris + 1] = verticies + verticalSize + 1;
                    triangles[tris + 2] = verticies + 1;
                    triangles[tris + 3] = verticies + 1;
                    triangles[tris + 4] = verticies + horizontalSize + 1;
                    triangles[tris + 5] = verticies + horizontalSize + 2;

                    verticies++;
                    tris += 6;
                }
                else
                {
                    // Two triangles are related but one is moved so that it is blank and gives visualisation of how the triangle looks
                    triangles[tris + 0] = verticies;
                    triangles[tris + 1] = verticies + verticalSize + 1;
                    triangles[tris + 2] = verticies + verticalSize + 2;
                    triangles[tris + 3] = verticies;
                    triangles[tris + 4] = verticies + horizontalSize + 1;
                    triangles[tris + 5] = verticies + horizontalSize + 2;

                    verticies++;
                    tris += 6;
                }
            }
            verticies++;
        }
    }


    // Creates the Colormap which is essentially how the gradient pastes the colours onto the generated terrain based off their height
    public void ColorMap()
    {
        enviromentColors = new Color[vertices.Length];

        // Loop over vertices and apply a color from the depending on height (y axis value)
        for (int i = 0, z = 0; z < vertices.Length; z++)
        {
            // Gets the height of the vertcie being looped over
            float height = Mathf.InverseLerp(minTerrainheight, maxTerrainheight, vertices[i].y);

            // Sets the colour of this vertcie based off of how heigh the verticie is in relation to the maximum and minimum height
            enviromentColors[i] = enviromentGradient.Evaluate(height);

            // Increments loop
            i++;
        }
    }

    // Final step in mesh generation is to remake the mesh to represent all the newly made verticies value 
    public void UpdateMeshValues()
    {
        // Resets the mesh and sets the verticies to the triangles that were created in the prior function
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = enviromentColors;

        // Recalculates all the values to represent the updated vertcies
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        // Sets the scale of the gameobject to be the size of the bounds
        gameObject.transform.localScale = new Vector3(horizontalSize, 100, verticalSize);

        // Sets the phsycial mesh collider that is used for raycasting objects on
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }


    public void PopulateEnviroment()
    {
        // Destroys all previous spawned enviromental objects
        foreach (Transform child in enviromentParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            // Finds the actual psotion of the verticie in world
            Vector3 vertexWorldPosition = transform.TransformPoint(mesh.vertices[i]);
            float vertexHeight = vertexWorldPosition.y;

            int inCentralVerticies = -1;
            if(i > 1132)
            {
                inCentralVerticies = 0;
            }

            // Doesnt generate the enviromental objects if the height between two verticies is too steep
            if (System.Math.Abs(lastNoiseHeight - vertexWorldPosition.y) < 25)
            {
                // min height for object generation
                if (vertexHeight > 100)
                {
                    int randomValue = Random.Range(1, 5);
                    // Gives every vertex a random chance to spawn a enviromental object
                    if (randomValue == 1)
                    {
                        if(objectsList.Count != 0)
                        {
                            int randomObject = Random.Range(0, objectsList.Count + inCentralVerticies);
                            GameObject enviromentalObject = objectsList[randomObject];

                            var spawnAboveTerrainBy = vertexHeight * 2;
                            GameObject instantiatedEnviromentObject = Instantiate(enviromentalObject, new Vector3(mesh.vertices[i].x * horizontalSize, spawnAboveTerrainBy, mesh.vertices[i].z * verticalSize), Quaternion.identity);

                            // Parents the enviromental object to a gameobject that can later be cleared for new generations
                            instantiatedEnviromentObject.transform.SetParent(enviromentParent.transform, true);

                            if (enviromentalObject.GetComponent<Objects>().oneTimeSpawn == true)
                            {
                                objectsList.Remove(objectsList[randomObject]);
                            }
                        }
                        else
                        {
                            Debug.Log("No objects to spawn");
                        }
                    }
                }
            }
            lastNoiseHeight = vertexHeight;
        }
    }


}

