# IDS4 Use

## Setup

- Multi Project Debug

![multi debug](/assets/multi%20debug.png)

- SSL
  1. ```cd "c:\Program Files (x86)\IIS Express"```
  1. ```IisExpressAdminCmd.exe setupsslUrl -url:https://localhost:44385/ -UseSelfSigned```
  1. ```IisExpressAdminCmd.exe setupsslUrl -url:https://localhost:44386/ -UseSelfSigned```
  1. Debug and trust all certificate trust prompts

## Running

### Credentials

See [Repositories folder](src/IDS4/Repositories)

- API: Swagger is configured with correct client credentials already

![authorize btn](/assets/authorize%20btn.png)
![authorize dlg](/assets/authorize%20dlg.png)

- MVC: User login is
  - User Name: scott
  - Password: Password123!

### Things to try

1. Try out the swagger api page and try calling the api with different combinations of scopes and not authorized.  Try runnimng the post api endpoint with the following scopes checked
  ![only read](/assets/only%20read.png)

1. In the mvc site, try clicking privacy or weather links logging in as scott
    1. Try unchecking some of the requested scopes and see what it does to the functionality.  You will hav eto logout and then log back in to get the consent again
    ![consent](/assets/consent.png)
    1. Click privacy link to see claims

See [AdminUI readme](/src/AdminUI/readme.md) and [skoruba/IdentityServer4.Admin](https://github.com/skoruba/IdentityServer4.Admin) for more info and setup.
