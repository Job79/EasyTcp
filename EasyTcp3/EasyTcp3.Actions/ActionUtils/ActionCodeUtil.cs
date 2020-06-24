namespace EasyTcp3.Actions.ActionUtils
{
    /// <summary>
    /// Functions used to convert a string to an action code (int)
    /// </summary>
    // http://www.cse.yorku.ca/~oz/hash.html
    public static class ActionCodeUtil
    {
        /// <summary>
        /// Convert string to actionCode by using the djb2a hashing algorithm
        /// </summary>
        /// <param name="str">action string</param>
        /// <returns>action string as int</returns>
        public static int ToActionCode(this string str)
        {
            int hash = 5381;
            foreach (var t in str) hash = ((hash << 5) + hash) ^ (byte) t;
            return hash;
        }

        /// <summary>
        /// Determines whether specified string is equal to actionCode
        /// </summary>
        /// <param name="actionCode">action code</param>
        /// <param name="str">action string</param>
        /// <returns></returns>
        public static bool IsEqualToAction(this int actionCode, string str)
            => actionCode == str.ToActionCode();

        /// <summary>
        /// Determines whether specified string is equal to actionMessage.ActionCode
        /// </summary>
        /// <param name="actionMessage">action message</param>
        /// <param name="str">action string</param>
        /// <returns></returns>
        public static bool IsAction(this ActionMessage actionMessage, string str)
            => actionMessage.ActionCode.IsEqualToAction(str);
    }
}