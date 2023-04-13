using Checkers.Core;
using System;
using UnityEngine;

namespace Checkers
{
    [Serializable]
    public struct BoardElementPrefab<T> where T : MonoBehaviour
    {
        public ColorType Color;

        public T Prefab;
    }
}
