using System.Collections.Generic;

using UniCircleTools;
using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Skills
{
    public interface ISkill
    {
        // This interface exists solely to be used in DifficultyCalculator because C# doesnt like
        //  having generics of generics (ie. List<Skill<DifficultyPoint>>)
        double Value { get; }
        void ProcessHitObjectSequence(IEnumerable<HitObject> hitObjects);
        void SetMods(Mods mods);
        void Reset();
    }
}
