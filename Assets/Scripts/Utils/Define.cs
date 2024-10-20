using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum WorldObject
    {
        MyPlayer,
        Player,
        Monster,
    }

    public enum Layer
    {
        Ground = 6,
        User = 7,
        Player = 8,

    }

    public enum MouseEvent
    {
        Press,
        Click,
        PointerDown,
        PointerUp,
        Drag,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum SkillMode
    {
        None,
        Toggle,
        Targeting,
        NoneTargeting_Fixed,
        NoneTargeting_Free,
    }

    public struct MoveEvent
    {
        public float startTime;
        public float time;

        public float velX;
        public float velY;
        public float velZ;
    }
}
