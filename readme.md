# IDS4 Use

## Badges
[![Build status](https://ci.appveyor.com/api/projects/status/sus87d52unb41vwl/branch/master?svg=true)](https://ci.appveyor.com/project/vbjay/ids4-sample/branch/master)

## Setup

- Multi Project Debug

![multi debug](/assets/multi%20debug.png)

- SSL
  1. ```cd "c:\Program Files (x86)\IIS Express"```
  1. ```IisExpressAdminCmd.exe setupsslUrl -url:https://localhost:44385/ -UseSelfSigned```
  1. ```IisExpressAdminCmd.exe setupsslUrl -url:https://localhost:44386/ -UseSelfSigned```
  1. ```IisExpressAdminCmd.exe setupsslUrl -url:https://localhost:44310/ -UseSelfSigned```
  1. ```IisExpressAdminCmd.exe setupsslUrl -url:https://localhost:44302/ -UseSelfSigned```
  1. ```IisExpressAdminCmd.exe setupsslUrl -url:https://localhost:44303/ -UseSelfSigned```
  1. Debug and trust all certificate trust prompts

- Client UI
  - Run the following cmd to build the client ui.  See this [script](src/AdminUI/build-client.ps1) and the gulp scipts found at [AdminUI](src/AdminUI/src/AdminUI.Admin/gulpfile.js) and [Identity server](src/AdminUI/src/AdminUI.STS.Identity/gulpfile.js).

  ```ps
  PowerShell.exe -ExecutionPolicy Bypass -File src/AdminUI/build-client.ps1
  ```

## Running

You may need to run twice.  The database will be created for you but if it doesn't exist, it may have a hiccup.  Just run again.

### Credentials

See [Admin Seed Data](src/AdminUI/src/AdminUI.Admin/identityserverdata.json).  All the clients, resources, scopes wil have been created for you.  You can delete them in the ui and next time your run, they will be created again.  See [Identity Seed Data](src/AdminUI/src/AdminUI.Admin/identitydata.json) to see the admin user login.  It is recomended you register another user.  You can assign that user the Administrator role to keep admin clean.

- API: Swagger is configured with correct client credentials already

![authorize btn](/assets/authorize%20btn.png)
![authorize dlg](/assets/authorize%20dlg.png)

### Things to try

1. First thing, register a user so you don't use the admin user to do things.
1. Try out the swagger api page and try calling the api with different combinations of scopes and not authorized.  Try runnimng the post api endpoint with the following scopes checked
  ![only read](/assets/only%20read.png)

1. In the mvc site, try clicking privacy or weather links logging in as a registered user.
    1. Try unchecking some of the requested scopes and see what it does to the functionality.  You will have to logout and then log back in to get the consent again
    ![consent](/assets/consent.png)
    1. Click privacy link to see claims
1. Useful powershell script to generate secrets

  ```ps
    $sec = ""
    $cnt = 1
    For ($i = 0; $i -le $cnt; $i++) {
          $id = [guid]::NewGuid()
              $sec += $id.ToString("D")
              if ( $i + 1 -lt $cnt) {
                $sec += "-"    
              }
          }
      $sec


  ```

## Room For Improvement

- Try making the vb client call the weather api.
- Add to the mvc weather page ui and functionality to call the POST endpoint to the weather api and see if you can display the results.
- See what happens when you only allow read scope and try calling the POST endpoint.  Try adding error handling or even ui updating to prevent the user from even trying
- In consent screen uncheck access to scopes and see what exceptions it causes.  Figure out how to handle those situations
- Actually use refresh tokens when token has expired.

See [AdminUI readme](/src/AdminUI/readme.md) and [skoruba/IdentityServer4.Admin](https://github.com/skoruba/IdentityServer4.Admin) for more info and setup.
