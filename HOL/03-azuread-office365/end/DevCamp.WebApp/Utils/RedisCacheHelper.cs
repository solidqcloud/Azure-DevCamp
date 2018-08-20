using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Configuration;

namespace DevCamp.WebApp.Utils
{
	public class RedisCacheHelper
	{
		private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
		{
			return ConnectionMultiplexer.Connect(Settings.REDISCACHE_CONNECTIONSTRING);
		});

		static ConnectionMultiplexer CacheConnection
		{
			get
			{
				return lazyConnection.Value;
			}
		}

		public static string GetDataFromCache(string CacheKey)
		{
			string cachedData = string.Empty;
			IDatabase cache = CacheConnection.GetDatabase();

			cachedData = cache.StringGet(CacheKey);
			return cachedData;
		}

		public static bool UseCachedDataSet(string CacheKey, out string CachedData)
		{
			bool retVal = false;
			CachedData = string.Empty;
			IDatabase cache = CacheConnection.GetDatabase();
			if (cache.Multiplexer.IsConnected)
			{
				if (cache.KeyExists(CacheKey))
				{
					CachedData = GetDataFromCache(CacheKey);
					retVal = true;
				}
			}
			return retVal;
		}

		public static void AddtoCache(string CacheKey, object ObjectToCache, int CacheExpiration = 60)
		{
			IDatabase cache = CacheConnection.GetDatabase();
			cache.StringSet(CacheKey, JsonConvert.SerializeObject(ObjectToCache), TimeSpan.FromSeconds(CacheExpiration));
		}

		public static void ClearCache(string CacheKey)
		{
			IDatabase cache = CacheConnection.GetDatabase();
			cache.KeyDelete(CacheKey);
		}
	}
}