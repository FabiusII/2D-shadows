using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteShadows.Rendering.Lighting.Code
{
    [Serializable]
    public class SpriteAndCollider : SpriteAndSomething<PolygonCollider2D>
    {
        [SerializeField] private PolygonCollider2D collider;

        public override PolygonCollider2D Something
        {
            get { return collider; }
            set { collider = value; }
        }
    }

    public class SpriteAndColliderSheet : SpriteAndSomethingSheet<PolygonCollider2D>
    {
        [SerializeField] private List<SpriteAndCollider> colliders;

        public override void Add(Sprite sprite, PolygonCollider2D element)
        {
            var pair = new SpriteAndCollider
            {
                Sprite = sprite,
                Something = element
            };
            colliders.Add(pair);
        }

        public override int Count()
        {
            return colliders.Count;
        }

        public override SpriteAndSomething<PolygonCollider2D> GetElement(int i)
        {
            return colliders[i];
        }
    }
}