/* EasyTcp
 * Copyright (C) 2019  henkje (henkje@pm.me)
 * 
 * MIT license
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace EasyTcp.Server
{
    public class RefusedClient
    {
        /// <summary>
        /// Returns the IP of the refused client.
        /// </summary>
        public readonly string IP;

        /// <summary>
        /// Returns true if client is banned.
        /// Returns false if client is refused because of to much connections.
        /// </summary>
        public readonly bool IsBanned;

        public RefusedClient(string IP, bool IsBanned)
        {
            this.IP = IP;
            this.IsBanned = IsBanned;
        }
    }
}
