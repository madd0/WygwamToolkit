using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Wygwam.Windows.Security
{
    /// <summary>
    /// Store credential instance
    /// </summary>
    public class Credential
    {
        #region Properties

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        [SecurityProperty]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [SecurityProperty]
        public string Password { get; set; }

        #endregion
    }
}
