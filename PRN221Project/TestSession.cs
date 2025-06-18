using Newtonsoft.Json;

namespace PRN221Project
{
    public static class TestSession
    {
        //set session
        public static void SetObjectsession(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        //get session
        public static T GetObjectsession<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }

}
