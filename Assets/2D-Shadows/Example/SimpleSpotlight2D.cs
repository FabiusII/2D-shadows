using System.Collections.Generic;
using System.Linq;
using SpriteShadows.Scripts;

namespace SpriteShadows.Example
{
    public class SimpleSpotlight2D : Spotlight2D
    {
        public override List<LitRenderer> GetAllLitRenderers()
        {
            return FindObjectsOfType<LitRenderer>().ToList();
        }
    }
}