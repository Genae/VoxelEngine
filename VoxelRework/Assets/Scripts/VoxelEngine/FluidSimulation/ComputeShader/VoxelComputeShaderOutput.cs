﻿using UnityEngine;
using System.Collections;

public class VoxelComputeShaderOutput : MonoBehaviour
{

    #region Compute Shader Fields and Properties

    /// <summary>
    /// The Compute shader we will use
    /// </summary>
    public ComputeShader computeShader;

    /// <summary>
    /// The total number of verticies to calculate.
    /// 10 * 10 * 10 block rendered in 10 * 10 * 10 threads in 1 * 1 * 1 groups
    /// </summary>
    int VertCount;
    
    /// <summary>
    /// This buffer will store the calculated data resulting from the Compute shader.
    /// </summary>
    public ComputeBuffer outputBuffer;
    public ComputeBuffer mapBuffer;

    public Shader PointShader;
    Material PointMaterial;
    
    public int cubeMultiplier = 5;

    /// <summary>
    /// A Reference to the CS Kernel we want to use.
    /// </summary>
    int CSKernel;

    #endregion

    /// <summary>
    /// Initialization, runs only once at the start
    /// </summary>
    void Start()
    {
        CSKernel = computeShader.FindKernel("CSMain");
        
        InitializeBuffers();
    }

    void InitializeBuffers()
    {
        VertCount = 10 * 10 * 10 * cubeMultiplier * cubeMultiplier * cubeMultiplier;

        // Set output buffer size.
        outputBuffer = new ComputeBuffer(VertCount, (sizeof(float) * 3) + (sizeof(int) * 6));
        mapBuffer = new ComputeBuffer(VertCount, sizeof(int));

        int width = 10 * cubeMultiplier;
        int height = 10 * cubeMultiplier;
        int depth = 10 * cubeMultiplier;

        int[] map = new int[VertCount];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    int idx = x + (y * 10 * cubeMultiplier) + (z * 10 * cubeMultiplier * 10 * cubeMultiplier);

                    if (x==y || z == y || (x + y + z < 10))
                        map[idx] = 1;
                    else
                        map[idx] = 0;
                }
            }
        }

        mapBuffer.SetData(map);

        computeShader.SetBuffer(CSKernel, "outputBuffer", outputBuffer);
        computeShader.SetBuffer(CSKernel, "mapBuffer", mapBuffer);

        computeShader.SetVector("group_size", new Vector3(cubeMultiplier, cubeMultiplier, cubeMultiplier));
        
        transform.position -= (Vector3.one * 10 * cubeMultiplier) *.5f;
    }
    
    public void Dispatch()
    {
        if (!SystemInfo.supportsComputeShaders)
        {
            Debug.LogWarning("Compute shaders not supported (not using DX11?)");
            return;
        }

        computeShader.Dispatch(CSKernel, cubeMultiplier, cubeMultiplier, cubeMultiplier);
    }

    private void OnDisable()
    {
        ReleaseBuffers();
    }

    void ReleaseBuffers()
    {
        outputBuffer.Release();
        mapBuffer.Release();
    }
}