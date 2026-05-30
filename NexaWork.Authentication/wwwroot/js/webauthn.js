function coerceToArrayBuffer(thing) {
    if (typeof thing === "string") {
        var base64 = thing.replace(/-/g, '+').replace(/_/g, '/');
        var padLen = (4 - (base64.length % 4)) % 4;
        base64 += "=".repeat(padLen);
        var binary = window.atob(base64);
        var bytes = new Uint8Array(binary.length);
        for (var i = 0; i < binary.length; i++) {
            bytes[i] = binary.charCodeAt(i);
        }
        return bytes.buffer;
    }
    return thing;
}

function coerceToBase64Url(thing) {
    if (thing instanceof ArrayBuffer) {
        thing = new Uint8Array(thing);
    }
    if (thing instanceof Uint8Array) {
        var str = "";
        for (var i = 0; i < thing.length; i++) {
            str += String.fromCharCode(thing[i]);
        }
        var base64 = window.btoa(str);
        return base64.replace(/\+/g, '-').replace(/\//g, '_').replace(/=/g, '');
    }
    return thing;
}

async function registerPasskey() {
    try {
        const response = await fetch('/api/WebAuthn/makeCredentialOptions', { method: 'POST' });
        const options = await response.json();

        if (options.status === "error") {
            alert("Error: " + options.errorMessage);
            return;
        }

        options.challenge = coerceToArrayBuffer(options.challenge);
        options.user.id = coerceToArrayBuffer(options.user.id);
        if (options.excludeCredentials) {
            options.excludeCredentials.forEach(c => {
                c.id = coerceToArrayBuffer(c.id);
            });
        }

        const cred = await navigator.credentials.create({ publicKey: options });

        const makeCredResponse = {
            id: cred.id,
            rawId: coerceToBase64Url(cred.rawId),
            type: cred.type,
            extensions: cred.getClientExtensionResults(),
            response: {
                attestationObject: coerceToBase64Url(cred.response.attestationObject),
                clientDataJSON: coerceToBase64Url(cred.response.clientDataJSON),
                transports: cred.response.getTransports ? cred.response.getTransports() : []
            }
        };

        const finalize = await fetch('/api/WebAuthn/makeCredential', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(makeCredResponse)
        });

        if (!finalize.ok) {
            const err = await finalize.text();
            alert("Registration API failed: " + finalize.status + "\n" + err);
            return;
        }

        const finalResult = await finalize.json();
        if (finalResult.status === "error") {
            alert("Error registering passkey: " + finalResult.errorMessage);
        } else {
            alert("Passkey registered successfully!");
            window.location.reload();
        }
    } catch (e) {
        alert("Registration failed: " + e.message);
    }
}

async function loginWithPasskey(username, returnUrl) {
    try {
        const requestBody = { username: username || "" };

        const response = await fetch('/api/WebAuthn/assertionOptions', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(requestBody)
        });
        const options = await response.json();

        if (options.status === "error") {
            alert("Error: " + options.errorMessage);
            return;
        }

        options.challenge = coerceToArrayBuffer(options.challenge);
        if (options.allowCredentials) {
            options.allowCredentials.forEach(c => {
                c.id = coerceToArrayBuffer(c.id);
            });
        }

        const assertion = await navigator.credentials.get({ publicKey: options });

        const assertionResponse = {
            id: assertion.id,
            rawId: coerceToBase64Url(assertion.rawId),
            type: assertion.type,
            extensions: assertion.getClientExtensionResults(),
            response: {
                authenticatorData: coerceToBase64Url(assertion.response.authenticatorData),
                clientDataJSON: coerceToBase64Url(assertion.response.clientDataJSON),
                signature: coerceToBase64Url(assertion.response.signature),
                userHandle: assertion.response.userHandle ? coerceToBase64Url(assertion.response.userHandle) : null
            }
        };

        const finalize = await fetch('/api/WebAuthn/makeAssertion', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(assertionResponse)
        });

        if (!finalize.ok) {
            const err = await finalize.text();
            alert("Login API failed: " + finalize.status + "\n" + err);
            return;
        }

        const finalResult = await finalize.json();
        if (finalResult.status === "ok") {
            // We successfully validated FIDO2.
            // Let the server MVC controller finish the login (e.g. issuing the persistent cookie, bypassing 2FA, etc)
            const form = document.createElement("form");
            form.method = "POST";
            form.action = "/Account/LoginCallbackWithPasskey";
            
            const returnUrlInput = document.createElement("input");
            returnUrlInput.type = "hidden";
            returnUrlInput.name = "returnUrl";
            returnUrlInput.value = returnUrl || "";
            form.appendChild(returnUrlInput);

            document.body.appendChild(form);
            form.submit();
        } else {
            alert("Login failed: " + finalResult.errorMessage);
        }
    } catch (e) {
        alert("Passkey login failed: " + e.message);
    }
}
