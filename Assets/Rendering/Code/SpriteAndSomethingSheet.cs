using UnityEngine;

namespace SpriteShadows.Rendering.Lighting.Code
{
    public abstract class SpriteAndSomething<T>
    {
        public Sprite Sprite;
        public abstract T Something { get; set; }
    }
    
    public abstract class SpriteAndSomethingSheet<T> : MonoBehaviour
    {
        private int _lastUsedIdx;

        public T FindForSprite(Sprite sprite)
        {          
            int count = Count();
            if (count == 0)
                return default(T);
            
            int i = _lastUsedIdx;  
            do
            {
                var currentPair = GetElement(i);
                if (currentPair.Sprite == sprite)
                {
                    _lastUsedIdx = i;
                    return (T)currentPair.Something;
                }
            } while ((i = ++i % count) != _lastUsedIdx);
            
            return default(T);
        }

        public abstract void Add(Sprite sprite, T element);
        public abstract int Count();
        public abstract SpriteAndSomething<T> GetElement(int i);
    }
}