/*MIT License

Copyright(c) 2021 damienbod

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files(the "Software"), to deal
    in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
document.getElementById('signin').addEventListener('click', function () {
    handleSignInSubmit();
    event.preventDefault();
});

window.onload = function () {
    // handleSignInSubmit();
};

async function handleSignInSubmit(event) {
    //event.preventDefault();

    //let username = this.username.value;
    // passwordfield is omitted in demo
    // let password = this.password.value;
    // prepare form post data
    var formData = new FormData();
    //formData.append('username', username);
    // not done in demo
    // todo: validate username + password with server (has nothing to do with FIDO2/WebAuthn)

    // send to server for registering
    let makeAssertionOptions;
    try {
        var res = await fetch('/mfaassertionOptions', {
            method: 'POST', // or 'PUT'
            body: formData, // data can be `string` or {object}!
            headers: {
                'Accept': 'application/json',
                'RequestVerificationToken': document.getElementById('RequestVerificationToken').value
            }
        });

        makeAssertionOptions = await res.json();
    } catch (e) {
        showErrorAlert("Request to server failed", e);
    }

    //console.log("Assertion Options Object", makeAssertionOptions);

    // show options error to user
    if (makeAssertionOptions.status !== "ok") {
        console.log("Error creating assertion options");
        console.log(makeAssertionOptions.errorMessage);
        showErrorAlert(makeAssertionOptions.errorMessage);
        return;
    }

    // todo: switch this to coercebase64
    const challenge = makeAssertionOptions.challenge.replace(/-/g, "+").replace(/_/g, "/");
    makeAssertionOptions.challenge = Uint8Array.from(atob(challenge), c => c.charCodeAt(0));

    // fix escaping. Change this to coerce
    makeAssertionOptions.allowCredentials.forEach(function (listItem) {
        var fixedId = listItem.id.replace(/\_/g, "/").replace(/\-/g, "+");
        listItem.id = Uint8Array.from(atob(fixedId), c => c.charCodeAt(0));
    });

    //console.log("Assertion options", makeAssertionOptions);

    const fido2TapKeyToLogin = document.getElementById('fido2TapKeyToLogin').innerText;
    document.getElementById('fido2logindisplay').innerHTML += '<div><img src = "/images/securitykey.png" alt = "fido login" /></div><div><b>' + fido2TapKeyToLogin + '</b></div>';

    //Swal.fire({
    //    title: 'Logging In...',
    //    text: 'Tap your security key to login.',
    //    imageUrl: "/images/securitykey.min.svg",
    //    showCancelButton: true,
    //    showConfirmButton: false,
    //    focusConfirm: false,
    //    focusCancel: false
    //});

    // ask browser for credentials (browser will ask connected authenticators)
    let credential;
    try {
        credential = await navigator.credentials.get({ publicKey: makeAssertionOptions });
    } catch (err) {
        document.getElementById('fido2logindisplay').innerHTML = '';
        showErrorAlert(err.message ? err.message : err);
    }

    //document.getElementById('fido2logindisplay').innerHTML = '<p>Processing</p>';

    try {
        await verifyAssertionWithServer(credential);
    } catch (e) {
        document.getElementById('fido2logindisplay').innerHTML = '';
        const fido2CouldNotVerifyAssertion = document.getElementById('fido2CouldNotVerifyAssertion').innerText;
        showErrorAlert(fido2CouldNotVerifyAssertion, e);
    }
}

async function verifyAssertionWithServer(assertedCredential) {
    // Move data into Arrays incase it is super long
    let authData = new Uint8Array(assertedCredential.response.authenticatorData);
    let clientDataJSON = new Uint8Array(assertedCredential.response.clientDataJSON);
    let rawId = new Uint8Array(assertedCredential.rawId);
    let sig = new Uint8Array(assertedCredential.response.signature);
    const data = {
        id: assertedCredential.id,
        rawId: coerceToBase64Url(rawId),
        type: assertedCredential.type,
        extensions: assertedCredential.getClientExtensionResults(),
        response: {
            authenticatorData: coerceToBase64Url(authData),
            clientDataJson: coerceToBase64Url(clientDataJSON),
            signature: coerceToBase64Url(sig)
        }
    };

    let response;
    try {
        let res = await fetch("/mfamakeAssertion", {
            method: 'POST', // or 'PUT'
            body: JSON.stringify(data), // data can be `string` or {object}!
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.getElementById('RequestVerificationToken').value
            }
        });

        response = await res.json();
    } catch (e) {
        showErrorAlert("Request to server failed", e);
        throw e;
    }

    //console.log("Assertion Object", response);

    // show error
    if (response.status !== "ok") {
        console.log("Error doing assertion");
        console.log(response.errorMessage);
        document.getElementById('fido2logindisplay').innerHTML = '';
        showErrorAlert(response.errorMessage);
        return;
    }

    //document.getElementById('fido2logindisplay').innerHTML = '<p>Logged In!</p>';

    // show success message
    //await Swal.fire({
    //    title: 'Logged In!',
    //    text: 'You\'re logged in successfully.',
    //    //type: 'success',
    //    timer: 2000
    //});
    let fido2ReturnUrl = document.getElementById('fido2ReturnUrl').innerText;
    if (!fido2ReturnUrl) {
        fido2ReturnUrl = "/";
    }
    window.location.href = fido2ReturnUrl;
}