using System;
using UnityEngine;
using Assets.Scripts.Data.Map;
using System.Collections.Generic;
using Assets.Scripts.Data.Material;


namespace Assets.Scripts.Control
{
    public class PlayerUtility : MonoBehaviour
    {
       /* void Update()
        {
            RemoveVoxels();
            
            //Debug.DrawRay(ray.origin, ray.direction * 10000, Color.yellow);
        }

        

        private void GetVoxelsInbetween()
        {
            if (_voxelPosList.Count < 2)
                return;
            var pos1 = _voxelPosList[0];
            var pos2 = _voxelPosList[1];
            var x1 = (int)pos1.x;
            var x2 = (int)pos2.x;
            var y1 = (int)pos1.y;
            var z1 = (int)pos1.z;
            var z2 = (int)pos2.z;

            //outofbounds catching
            if(y1 + _mouseScrollDelta > _maxMapHeight)
            {
                _mouseScrollDelta = _maxMapHeight - y1 - 1;
            }
            if(y1 + _mouseScrollDelta < 0)
            {
                _mouseScrollDelta = -y1 + 1;
            }

            if(_mouseScrollDelta >= 0)
            {
                if (x1 <= x2 && z1 <= z2)
                {
                    for (int x = x1; x <= x2; x++)
                    {
                        for(int y = y1; y <= pos1.y + _mouseScrollDelta; y++)
                        {
                            for (int z = z1; z <= z2; z++)
                            {
                                _voxelPosList.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
                else if (x1 >= x2 && z1 <= z2)
                {
                    for (int x = x2; x <= x1; x++)
                    {
                        for (int y = y1; y <= pos1.y + _mouseScrollDelta; y++)
                        {
                            for (int z = z1; z <= z2; z++)
                            {
                                _voxelPosList.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
                else if (x1 <= x2 && z1 >= z2)
                {
                    for (int x = x1; x <= x2; x++)
                    {
                        for(int y = y1; y <= pos1.y + _mouseScrollDelta; y++)
                        {
                            for (int z = z2; z <= z1; z++)
                            {
                                _voxelPosList.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
                else if (x1 >= x2 && z1 >= z2)
                {
                    for (int x = x2; x <= x1; x++)
                    {
                        for (int y = y1; y <= pos1.y + _mouseScrollDelta; y++)
                        {
                            for (int z = z2; z <= z1; z++)
                            {
                                _voxelPosList.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
            }
            if (_mouseScrollDelta < 0)
            {
                if (x1 <= x2 && z1 <= z2)
                {
                    for (int x = x1; x <= x2; x++)
                    {
                        for (int y = y1 + _mouseScrollDelta; y <= y1; y++)
                        {
                            for (int z = z1; z <= z2; z++)
                            {
                                _voxelPosList.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
                else if (x1 >= x2 && z1 <= z2)
                {
                    for (int x = x2; x <= x1; x++)
                    {
                        for (int y = y1 + _mouseScrollDelta; y <= y1; y++)
                        {
                            for (int z = z1; z <= z2; z++)
                            {
                                _voxelPosList.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
                else if (x1 <= x2 && z1 >= z2)
                {
                    for (int x = x1; x <= x2; x++)
                    {
                        for (int y = y1 + _mouseScrollDelta; y <= y1; y++)
                        {
                            for (int z = z2; z <= z1; z++)
                            {
                                _voxelPosList.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
                else if (x1 >= x2 && z1 >= z2)
                {
                    for (int x = x2; x <= x1; x++)
                    {
                        for (int y = y1 + _mouseScrollDelta; y <= y1; y++)
                        {
                            for (int z = z2; z <= z1; z++)
                            {
                                _voxelPosList.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
            }
        }


        //only use if hit already contains raycast information
        */

        
    }
}

