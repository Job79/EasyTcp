using System;

namespace EasyTcp3.Actions
{
    /// <summary>
    /// Attribute type for EasyTcpActions, only methods with this attribute will get loaded as actions
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EasyTcpAction : Attribute
    {
        /// <summary>
        /// Code of this action,
        /// error will be thrown if not unique
        /// </summary>
        public int ActionCode { get; set; }

        public EasyTcpAction(int actionCode)  => ActionCode = actionCode;

        
        //TODO
        /*
        public EasyTcpAction(string actionCode) => ActionCode = Hash(actionCode);

        /// <summary>
        /// jdba2 hashing function
        /// http://www.cse.yorku.ca/~oz/hash.html
        /// https://softwareengineering.stackexchange.com/questions/49550/which-hashing-algorithm-is-best-for-uniqueness-and-speed
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private int Hash(string str)
        {
            int hash = 5381;
            foreach (var t in str) hash = ((hash << 5) + hash) ^ (byte)t;
            return hash;
        }*/
    }
}