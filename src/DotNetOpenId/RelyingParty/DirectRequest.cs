﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Diagnostics;

namespace DotNetOpenId.RelyingParty {
	abstract class DirectRequest {
		protected DirectRequest(Uri provider, IDictionary<string, string> args) {
			Provider = provider;
			Args = args;
		}
		protected Uri Provider { get; private set; }
		protected IDictionary<string, string> Args { get; private set; }

		protected IDictionary<string, string> GetResponse() {
			byte[] body = ProtocolMessages.Http.GetBytes(Args);

			try {
				FetchResponse resp = Fetcher.Request(Provider, body);
				if ((int)resp.StatusCode >= 200 && (int)resp.StatusCode < 300) {
					return ProtocolMessages.KeyValueForm.GetDictionary(resp.ResponseStream);
				} else {
					if (TraceUtil.Switch.TraceError) {
						Trace.TraceError("Bad request code returned from remote server: {0}.", resp.StatusCode);
					}
					return null;
				}
			} catch (ArgumentException e) {
				throw new OpenIdException("Failure decoding Key-Value Form response from provider.", e);
			} catch (WebException e) {
				throw new OpenIdException("Failure while connecting to provider.", e);
			}
		}
	}
}
