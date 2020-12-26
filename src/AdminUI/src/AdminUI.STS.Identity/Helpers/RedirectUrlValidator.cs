using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Validation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminUI.STS.Identity.Helpers
{
    public class RedirectUrlValidator : IRedirectUriValidator
    {
        /// <summary>
        /// Checks if a given URI string is in a collection of strings (using ordinal ignore case comparison)
        /// </summary>
        /// <param name="uris">The uris.</param>
        /// <param name="requestedUri">The requested URI.</param>
        /// <returns></returns>
        protected bool StringCollectionContainsString(IEnumerable<string> uris, string requestedUri)
        {
            if (uris.IsNullOrEmpty()) return false;
            var parsed = uris.Select(u =>
            {
                Uri p;
                if (Uri.TryCreate(u, UriKind.Absolute, out p))
                {
                    if (p.IsLoopback)// if loopback ignore port
                    {
                        var f = p.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port, UriFormat.UriEscaped);
                        return uris.Contains(f, StringComparer.OrdinalIgnoreCase);//strict check ignoring port

                    }
                    else
                        return uris.Contains(requestedUri, StringComparer.OrdinalIgnoreCase);//strict check 
                }
                else
                    return false;
            });
            return parsed.Any(v => true);
        }
        /// <summary>
        /// Determines whether a redirect URI is valid for a client.
        /// </summary>
        /// <param name="requestedUri">The requested URI.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        ///   <c>true</c> is the URI is valid; <c>false</c> otherwise.
        /// </returns>
        public virtual Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            return Task.FromResult(StringCollectionContainsString(client.RedirectUris, requestedUri));
        }

        /// <summary>
        /// Determines whether a post logout URI is valid for a client.
        /// </summary>
        /// <param name="requestedUri">The requested URI.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        ///   <c>true</c> is the URI is valid; <c>false</c> otherwise.
        /// </returns>
        public virtual Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            return Task.FromResult(StringCollectionContainsString(client.PostLogoutRedirectUris, requestedUri));
        }
    }
}
