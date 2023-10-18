using UnityEngine;
using UnityEngine.Pool;

namespace Prototype
{
    public abstract class BasePool<TSingleton, PoolT> : Singleton<TSingleton> where TSingleton : MonoBehaviour where PoolT : UnityEngine.Object
    {
        IObjectPool<PoolT> m_Pool;

        public int maxPoolSize = 10;
        public bool collectionChecks = true;
        public IObjectPool<PoolT> Pool
        {
            get
            {
                if (m_Pool == null)
                {

                    m_Pool = new ObjectPool<PoolT>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);

                }
                return m_Pool;
            }
        }

        protected abstract void OnDestroyPoolObject(PoolT source);

        protected abstract void OnReturnedToPool(PoolT source);

        protected abstract void OnTakeFromPool(PoolT source);

        protected abstract PoolT CreatePooledItem();
    }
}

