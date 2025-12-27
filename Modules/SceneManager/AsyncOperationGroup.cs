using System.Collections.Generic;
using UnityEngine;

namespace CookieUtils.Extras.SceneManager
{
    internal class AsyncOperationGroup
    {
        private readonly List<AsyncOperation> _operations = new();

        public void Add(AsyncOperation operation)
        {
            _operations.Add(operation);
        }

        public float GetProgress()
        {
            float average = 0;
            foreach (AsyncOperation operation in _operations)
            {
                average += operation.progress;
            }

            average /= _operations.Count;

            return average;
        }
    }
}
