using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simplex3DNoiseLookups : MonoBehaviour
{
    private const int NoisePeriod = 256;

    // This is a list of all integers between 0 and 255, scrambled in a random order
    private int[] IndexPermutations = { 151,160,137,91,90,15,
    131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
    190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
    88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
    77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
    102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
    135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
    5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
    223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
    129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
    251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
    49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
    138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180 };

    //Defines three gradients for each of the 4 vertices in every 3D simplex
    private Vector3Int[] Gradients = 
    {
        new Vector3Int(1,1,0), new Vector3Int(-1,1,0), new Vector3Int(1,-1,0), new Vector3Int(-1,-1,0),
        new Vector3Int(1,0,1), new Vector3Int(-1,0,1), new Vector3Int(1,0,-1), new Vector3Int(-1,0,-1),
        new Vector3Int(0,1,1), new Vector3Int(0,-1,1), new Vector3Int(0,1,-1),new Vector3Int(0,-1,-1) 
    };

    [SerializeField] private Material material;
    //private Vector4[] randomIndices2D = new Vector4[NoisePeriod * NoisePeriod];
    //private Vector3[] randomGradients = [NoisePeriod * 2];
    
    //public Vector4[] RandomIndices2D => RandomIndices2D;
    //public Vector3[] RandomGradients => randomGradients;

    void Awake()
    {
        initLookups();
    }

    void initLookups()
    {
        var hash2DTex = new Texture2D(NoisePeriod, NoisePeriod, TextureFormat.RGBA32, false);
        var hash2D = new Vector4[NoisePeriod * NoisePeriod];
        for (int j = 0; j < NoisePeriod; j++)
        {
            for (int i = 0; i < NoisePeriod; i++)
            {
                int a = (IndexPermutations[i] + j) % NoisePeriod;
                int b = (IndexPermutations[(i + 1) % NoisePeriod] + j) % NoisePeriod;
                int c = (a + 1) % NoisePeriod;
                int d = (b + 1) % NoisePeriod;
                hash2D[j * NoisePeriod + i] = new Vector4((float)a, (float)b, (float)c, (float)d);
                hash2DTex.SetPixel(i, j, new Color(a / 255f, b / 255f, c / 255f, d / 255f));
                hash2DTex.SetPixel(i, j, new Color(1,0,0,1)); ;
            }
        }
        //material.SetVectorArray("randomIndices2D", hash2D);
        hash2DTex.Apply();
        material.SetTexture("hash2DTex", hash2DTex);
        material.mainTexture = hash2DTex;

        //Gradients
        var randomGradients = new Vector4[NoisePeriod * 2];
        for (int j = 0; j < NoisePeriod; j++)
        {
            for (int i = 0; i < 12; i++)
            {
                int offset = IndexPermutations[j];
                randomGradients[offset].x = (float)Gradients[i].x;
                randomGradients[offset].y = (float)Gradients[i].y;
                randomGradients[offset].z = (float)Gradients[i].z;
            }
        }
        for (int i = NoisePeriod; i < NoisePeriod * 2; i++)
        {
            randomGradients[i] = randomGradients[i - NoisePeriod];
        }
        material.SetVectorArray("randomGradients", randomGradients);
    }
}
