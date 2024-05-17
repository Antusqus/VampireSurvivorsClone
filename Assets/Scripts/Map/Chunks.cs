using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunks : MonoBehaviour
{
    public int Size;
    public struct Terrain
    {
        private string type;

        public Terrain(string type, int x, int y)
        {
            this.type = type;

            // Terrain generator script goes here.

        }
    }


}
