using System;
using System.Collections.Generic;
using Firaxis.Framework.Granny;
using System.Reflection;

namespace NexusBuddy.GrannyWrappers
{
    public unsafe class GrannyAnimationWrapper
    {
        private granny_animation* m_pkAnimation = (granny_animation*)0;
        private List<IGrannyTrackGroup> m_lstTrackGroups;
        private IGrannyAnimation wrappedAnimation;

        public GrannyAnimationWrapper(IGrannyAnimation inputAnimation)
        {
            wrappedAnimation = inputAnimation;
            Type myType = inputAnimation.GetType();
            FieldInfo fm_lstTrackGroups = myType.GetField("m_lstTrackGroups", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            m_lstTrackGroups = (List<IGrannyTrackGroup>)fm_lstTrackGroups.GetValue(inputAnimation);
        }

        public List<IGrannyTrackGroup> getTrackGroups()
        {
            return m_lstTrackGroups;
        }
    }
}
