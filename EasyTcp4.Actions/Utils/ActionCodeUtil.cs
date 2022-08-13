namespace EasyTcp4.Actions.Utils
{
    public static class ActionCodeUtil
    {
        /// <summary>
        /// Convert string to actionCode with the djb2a algorithm
        /// The djb2a algorithm can be found here: http://www.cse.yorku.ca/~oz/hash.html 
        /// </summary>
        /// <param name="str">action string</param>
        /// <returns>hash of action string (action code)</returns>
        public static int ToActionCode(this string str)
        {
            int hash = 5381;
            foreach (var t in str) hash = ((hash << 5) + hash) ^ (byte)t;
            return hash;
        }

        /// <summary>
        /// Determines whether specified action string is equal to an action code
        /// </summary>
        public static bool IsEqualToAction(this int actionCode, string str)
            => actionCode == str.ToActionCode();

        /// <summary>
        /// Determines whether a message holds a specic action code
        /// </summary>
        /// <param name="message">message with actionCode</param>
        /// <param name="str">action string</param>
        public static bool IsAction(this Message message, string str)
            => message.GetActionCode().IsEqualToAction(str);
    }
}
