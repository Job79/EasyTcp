/* HenkTcp
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

using System.Net.Sockets;
using System.Collections.Generic;

namespace HenkTcp.Server
{
    internal class ClientObject
    {
        public TcpClient TcpClient;

        /// <summary>
        /// Buffer is used for receiving data.
        /// </summary>
        public byte[] Buffer;

        /// <summary>
        /// DataBuffer will be used when client sends a to big message for the buffer.
        /// </summary>
        public List<byte> DataBuffer;
    }
}
