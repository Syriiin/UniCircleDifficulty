using System.Collections.Generic;

using UniCircleTools;
using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Skills
{
    // This interface exists solely to be used in DifficultyCalculator because C# doesnt like
    //  having generics of generics (ie. List<Skill<DifficultyPoint>>)
    public interface ISkill
    {
        double Value { get; }
        void ProcessHitObjectSequence(IEnumerable<HitObject> hitObjects);
        void SetMods(Mods mods);
        void Reset();
    }
}
