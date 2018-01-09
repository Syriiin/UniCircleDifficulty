using System;

using UniCircleTools;
using UniCircleTools.Beatmaps;

namespace UniCircleDifficulty.Skills.Reading
{
    /// <summary>
    /// Skill representing the difficulty of identifying rhythmic and visual patterns in notes
    /// </summary>
    class Reading : Skill<VisualPoint>
    {
        // TODO:
        //  - Density
        //      Base difficulty, nothing is hard on low density
        //      Simple streams are easy to read regardless of density
        //  - Overlap
        //      Overlapping notes that require movement away inbetween are hard
        //  - Pattern changes
        //      This is somewhat accounted for in overlap, but not the changing order
        //      Repeating overlapping notes with different order
        //  - Changes in timing without changes in distance
        //      Old mapping style

        // Reading doesn't really have lingering difficulty. If anything, it gets easier since you're getting used to the hard reading.
        protected override double ExertionDecayBase => 0;

        protected override double SkillMultiplier => 1;

        public override void ProcessHitObject(HitObject hitObject)
        {
            // Construct visual points from hitobject and call ProcessDifficultyPoint with them
            throw new NotImplementedException();
        }

        protected override void UpdateDifficultyPoints(VisualPoint visualPoint)
        {
            // Add diffPoint to currentDiffPoints
            _currentDiffPoints.Add(visualPoint);

            // Update pool
            throw new NotImplementedException();
        }

        protected override double CalculateEnergyExerted()
        {
            throw new NotImplementedException();
            // Perhaps, for density each note should have a (focal weight) that determins how much it contributes to density
            //  Stream notes have low focal weight

            // Object density, visual density (overlap), visual mods, quick reading bonus for fast circles

            // For the current hit object, it's reading difficulty is made up of many things:
            //  - The desity (based on weighted sum of focal points currently on the screen)
            //      But, individual notes of jumps have the same focal value as a single stream
            //  - If there is another note currently active near this note that is not the immidiate previous one
            //      If previous, it's just a stack, and that's not hard to read
            //  - If part of a repeated pattern (with different order)
            //      This will need more thought on how to detect
            //  - If there is a timing change, without a distance change

            // Difficulty of a note should take into account difficulty of all active notes (using dict or something)
        }

        public Reading(Mods mods) : base(mods) { }
    }
}
