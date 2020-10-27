using Mx.Util;

namespace Mx.Config
{
    /// <summary>数据管理</summary>
    public class DataManager : MonoSingleton<DataManager>
    {
        private DatabaseManager databaseManager;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            databaseManager = new DatabaseManager();
        }

        /// <summary>加载数据</summary>
        public void Load()
        {
            databaseManager.Load();
        }

        /// <summary>
        /// 获取数据（泛型）
        /// </summary>
        /// <returns></returns>
        public T GetDatabase<T>() where T : IDatabase, new()
        {
            return databaseManager.GetDatabase<T>();
        }
    }
}