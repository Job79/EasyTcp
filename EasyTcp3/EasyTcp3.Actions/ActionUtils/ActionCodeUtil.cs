namespace EasyTcp3.Actions.ActionUtils
{
    /// <summary>
    /// Functions used to convert a string to an action code (int)
    /// </summary>
    public static class ActionCodeUtil
    {
        /// <summary>
        /// Convert a string to an actionCode by using the djb2a hashing algorithm
        /// ! Collision are surely possible, but this shouldn't be a problem here (for example: haggadot & loathsomenesses)
        /// http://www.cse.yorku.ca/~oz/hash.html
        /// </summary>
        /// <param name="str">action id as string</param>
        /// <returns>action id as int</returns>
        public static int ToActionCode(this string str)
        {
            int hash = 5381;
            foreach (var t in str) hash = ((hash << 5) + hash) ^ (byte) t;
            return hash;
        } 
    }
}