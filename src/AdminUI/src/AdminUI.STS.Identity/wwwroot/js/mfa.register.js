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
document.getElementById('register').addEventListener('submit', handleRegisterSubmit);
$("[data-credid]").click(UpdateKey);

async function handleRegisterSubmit(event) {
    event.preventDefault();

    let username = this.username.value;
    //let displayName = this.displayName.value;
    // passwordfield is omitted in demo
    // let password = this.password.value;
    // possible values: none, direct, indirect
    let attestation_type = "direct";
    // possible values: <empty>, platform, cross-platform
    let authenticator_attachment = "cross-platform";
    // possible values: preferred, required, discouraged
    let user_verification = "preferred";
    // possible values: true,false
    let require_resident_key = false;
    // prepare form post data
    var data = new FormData();
    data.append('username', username);
    // data.append('displayName', displayName);
    data.append('attType', attestation_type);
    data.append('authType', authenticator_attachment);
    data.append('userVerification', user_verification);
    data.append('requireResidentKey', require_resident_key);

    // send to server for registering
    let makeCredentialOptions;
    try {
        makeCredentialOptions = await fetchMakeCredentialOptions(data);

    } catch (e) {
        console.error(e);
        let msg = "Something wen't really wrong";
        showErrorAlert(msg);
    }

    //console.log("Credential Options Object", makeCredentialOptions);

    if (makeCredentialOptions.status !== "ok") {
        console.log("Error creating credential options");
        console.log(makeCredentialOptions.errorMessage);
        showErrorAlert(makeCredentialOptions.errorMessage);
        return;
    }

    // Turn the challenge back into the accepted format of padded base64
    makeCredentialOptions.challenge = coerceToArrayBuffer(makeCredentialOptions.challenge);
    // Turn ID into a UInt8Array Buffer for some reason
    makeCredentialOptions.user.id = coerceToArrayBuffer(makeCredentialOptions.user.id);

    makeCredentialOptions.excludeCredentials = makeCredentialOptions.excludeCredentials.map((c) => {
        c.id = coerceToArrayBuffer(c.id);
        return c;
    });

    if (makeCredentialOptions.authenticatorSelection.authenticatorAttachment === null) makeCredentialOptions.authenticatorSelection.authenticatorAttachment = undefined;

    //console.log("Credential Options Formatted", makeCredentialOptions);

    const fido2TapYourSecurityKeyToFinishRegistration = document.getElementById('fido2TapYourSecurityKeyToFinishRegistration').innerText;
    document.getElementById('fido2mfadisplay').innerHTML += '<div><img src = "/images/securitykey.png" alt = "fido login" /></div><div><b>' + fido2TapYourSecurityKeyToFinishRegistration + '</b></div>';

    //Swal.fire({
    //    title: 'Registering...',
    //    text: 'Tap your security key to finish registration.',
    //    imageUrl: "/images/securitykey.min.svg",
    //    showCancelButton: true,
    //    showConfirmButton: false,
    //    focusConfirm: false,
    //    focusCancel: false
    //});

    //console.log("Creating PublicKeyCredential...");
    //console.log(navigator);
    //console.log(navigator.credentials);
    //console.log(makeCredentialOptions);
    let newCredential;
    try {
        newCredential = await navigator.credentials.create({
            publicKey: makeCredentialOptions
        });
    } catch (e) {
        const fido2RegistrationError = document.getElementById('fido2RegistrationError').innerText;
        console.error(fido2RegistrationError, e);
        document.getElementById('fido2mfadisplay').innerHTML = '';
        showErrorAlert(fido2RegistrationError, e);
    }

    console.log("PublicKeyCredential Created", newCredential);

    try {
        registerNewCredential(newCredential);

    } catch (e) {
        showErrorAlert(err.message ? err.message : err);
    }
}

async function fetchMakeCredentialOptions(formData) {
    let response = await fetch('Fido2/mfamakeCredentialOptions', {
        method: 'POST', // or 'PUT'
        body: formData, // data can be `string` or {object}!
        headers: {
            'Accept': 'application/json',
            'RequestVerificationToken': $("[name='__RequestVerificationToken']").val()
        }
    });

    let data = await response.json();

    return data;
}


async function fetchMakeCredentialOptions(formData) {
    let response = await fetch('Fido2/mfamakeCredentialOptions', {
        method: 'POST', // or 'PUT'
        body: formData, // data can be `string` or {object}!
        headers: {
            'Accept': 'application/json',
            'RequestVerificationToken': $("[name='__RequestVerificationToken']").val()
        }
    });

    let data = await response.json();

    return data;
}

// This should be used to verify the auth data with the server
async function registerNewCredential(newCredential) {
    // Move data into Arrays incase it is super long
    let attestationObject = new Uint8Array(newCredential.response.attestationObject);
    let clientDataJSON = new Uint8Array(newCredential.response.clientDataJSON);
    let rawId = new Uint8Array(newCredential.rawId);

    const cred = {
        id: newCredential.id,
        rawId: coerceToBase64Url(rawId),
        type: newCredential.type,
        extensions: newCredential.getClientExtensionResults(),
        response: {
            AttestationObject: coerceToBase64Url(attestationObject),
            clientDataJson: coerceToBase64Url(clientDataJSON)
        }
    };

    let response;
    try {
        response = await registerCredentialWithServer(cred);
    } catch (e) {
        showErrorAlert(e);
    }

    //console.log("Credential Object", response);

    // show error
    if (response.status !== "ok") {
        console.log("Error creating credential");
        console.log(response.errorMessage);
        showErrorAlert(response.errorMessage);
        return;
    }



    // show success 
    swalWithBootstrapButtons.fire({
        title: 'Registration Successful!',
        html: 'You&lsquo;ve registered successfully.  It is highly recomended for you to have recovery codes generated. ',
        // type: 'success',
        timer: 6000
    }).then(value => {
        return UpdateDescription(response.result.credentialId);
    }).then((result) => {
        window.location.reload();
    });





    // possible values: true,false
    // window.location.href = "/Identity/Account/Manage/GenerateRecoveryCodes";
}


async function registerCredentialWithServer(formData) {
    let response = await fetch('Fido2/mfamakeCredential', {
        method: 'POST', // or 'PUT'
        body: JSON.stringify(formData), // data can be `string` or {object}!
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'RequestVerificationToken': $("[name='__RequestVerificationToken']").val()
        }
    });

    let data = await response.json();

    return data;
}



async function UpdateKey(e) {
    //$...data removes = from value

    var cred = $(this).data('credid');


    //// show success 
    //Swal.fire({
    //    title: 'You clicked',
    //    text: cred,
    //    // type: 'success',
    //    timer: 3000
    //}).then(result => {

    //    // show success 
    //    return Swal.fire({
    //        title: 'Round trip',
    //        text: coerceToBase64Url(coerceToArrayBuffer(cred)),
    //        // type: 'success',
    //        timer: 3000
    //    }).then(result2 => {

    return UpdateDescription(cred).then(result => {
        window.location.reload();

    });
    //    });
    //});
}
async function UpdateDescription(cred) {
    return await swalWithBootstrapButtons.fire({
        title: 'Update Description',
        input: 'text',
        inputPlaceholder: 'Enter a description for this key',
        inputAttributes: {
            autocapitalize: 'off',

        },
        showCancelButton: true,
        confirmButtonText: 'Update Description',
        showLoaderOnConfirm: true,
        preConfirm: function (txt) {
            let frm = {
                description: txt,
                credId: cred
            };
            return $.ajax({
                type: 'POST',
                contentType: "application/json",
                url: '/Manage/UpdateSecurityKeyDescription',
                data: JSON.stringify(frm),
                dataType: 'json',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': $("[name='__RequestVerificationToken']").val()
                }
            })

                //fetch('/Manage/UpdateSecurityKeyDescription', {
                //    method: 'POST', // or 'PUT'
                //    body: JSON.stringify(frm), // data can be `string` or {object}!

                //})
                .then((dresponse, status) => {
                    if (status != 'success') {
                        throw new Error(dresponse.statusText)
                    }
                    return dresponse;
                })
                .catch(error => {
                    Swal.showValidationMessage(
                        `Request failed: ${error}`
                    )
                })
        },
        allowOutsideClick: () => !Swal.isLoading()
    })

}