using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteShadows.Scripts
{
    [Serializable]
    public class SpriteAndMesh : SpriteAndSomething<Mesh>
    {
        [SerializeField] private Mesh mesh;

        public override Mesh Something
        {
            get { return mesh; }
            set { mesh = value; }
        }
    }

    public class SpriteAndMeshSheet : SpriteAndSomethingSheet<Mesh>
    {
        [SerializeField] private List<SpriteAndMesh> meshes = new List<SpriteAndMesh>();

        public override void Add(Sprite sprite, Mesh element)
        {
            var pair = new SpriteAndMesh()
            {
                Sprite = sprite,
                Something = element
            };
            meshes.Add(pair);
        }

        public override int Count()
        {
            return meshes.Count;
        }

        public override SpriteAndSomething<Mesh> GetElement(int i)
        {
            return meshes[i];
        }

        public void Clear()
        {
            meshes.Clear();
        }
    }
}